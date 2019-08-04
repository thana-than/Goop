using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThanFramework;

public class Goop : FunctionLib
{
    public Rigidbody2D rb;
    public int collisions;
    public bool canLaunch;

    public float groundCheckDist = .3f;
    public float groundCheckSeparations = .15f;

    public float groundCooldownTime = .25f;
    bool isGroundCooldown = false;

    public bool onScreen;

    float startingZPos;

    LineRenderer plotRenderer;
    LineRenderer plotRenderer2;


    ParticleSystem particles_reactivateLaunch;
    ParticleSystem particles_launch;

    bool canLaunchTriggerCheck = false;
    bool canLaunchTrigger = false;

    public Color lineColor = Color.white;

    private static float dottedLineMatRatio = 0;

    // Start is called before the first frame update
    void Start()
    {
        startingZPos = transform.position.z;

        rb = GetComponent<Rigidbody2D>();
        plotRenderer = GetComponent<LineRenderer>();
        plotRenderer2 = GetChild(this.gameObject, "Render").GetComponent<LineRenderer>();

        particles_reactivateLaunch = GetComponent<ParticleSystem>();
        particles_launch = GetChild(this.gameObject, "Render").GetComponent<ParticleSystem>();

        //Color m_color = Color.Lerp(new Color(lineColor.r, lineColor.g, lineColor.b, 0.5f), new Color(0f, 0f, 0f, 0f), fadeOutSpeed);

        Color endColor = lineColor;
        endColor.a = 0;
        plotRenderer.startColor = lineColor;
        plotRenderer.endColor = endColor;
        plotRenderer2.startColor = lineColor;
        plotRenderer2.endColor = endColor;

        //plotRenderer.textureMode = LineTextureMode.Tile;
        //plotRenderer2.textureMode = LineTextureMode.Tile;

        Definitions.Goops.Add(this); //Adds this goop to the list of existing goops

        if (dottedLineMatRatio == 0)
        {
            Vector2 dottedLineMatSize = plotRenderer.material.GetTextureScale("_MainTex");
            dottedLineMatRatio =  dottedLineMatSize.x / dottedLineMatSize.y / 2;
        }

    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = new Vector2(5, 5);
        }
        */

        Vector3[] linePositions = new Vector3[plotRenderer.positionCount];
        plotRenderer.GetPositions(linePositions);
        float lineDist = 0;
        for (int i = 0; i < plotRenderer.positionCount - 2; i++)
        {
            lineDist += Vector2.Distance(linePositions[i], linePositions[i+1]);
        }

        //float dottedDistance = Vector2.Distance(plotRenderer.GetPosition(0), plotRenderer.GetPosition(1)) * plotRenderer.positionCount / Control.dottedLineDistance;
        float dottedDistance = lineDist / dottedLineMatRatio;
        plotRenderer.material.SetTextureScale("_MainTex", new Vector2 (dottedDistance, 1));
        plotRenderer2.material.SetTextureScale("_MainTex", new Vector2(dottedDistance, 1));

        if (!isGroundCooldown && GroundCheck() && collisions > 0) //if we are touching the ground, and our cooldown is depleted, we can launch
        {
            canLaunch = true;
        }

        Vector2 screenPoint = Definitions.mainCamera.WorldToViewportPoint(transform.position);
        //onScreen = screenPoint.x > (0 - cameraMargin.x) && screenPoint.x < (1 + cameraMargin.x) && screenPoint.y > (0 - cameraMargin.y) && screenPoint.y < (1 + cameraMargin.y);


        Vector3 telePosition;
        if (screenPoint.x < (0 - cameraMargin.x)) //if object offscreen to the left
        {
            telePosition = Definitions.mainCamera.ViewportToWorldPoint(new Vector2(.99f + cameraMargin.x, screenPoint.y));
            telePosition.z = startingZPos;

            transform.position = telePosition;
        }
        else if (screenPoint.x > (1 + cameraMargin.x)) //if object is offscreen to the right
        {
            telePosition = Definitions.mainCamera.ViewportToWorldPoint(new Vector2(.01f - cameraMargin.x, screenPoint.y));
            telePosition.z = startingZPos;

            transform.position = telePosition;
        }

        if (IsBelowScreen())
        {
            Destroy(this.gameObject);
        }

        if (Control.isPlotting && canLaunch)
        {
            RenderPlot(plotRenderer, plotRenderer2, transform.position, rb, -Control.estVel);
        }
        else
        {
            plotRenderer.enabled = false;
            plotRenderer2.enabled = false;
        }


        if (canLaunch)
        {
            if (!canLaunchTriggerCheck)
            {
                canLaunchTriggerCheck = true;
                canLaunchTrigger = true;
            }
            else
            {
                canLaunchTrigger = false;
            }
        }
        else
        {
            canLaunchTriggerCheck = false;
            canLaunchTrigger = false;
        }

        if (canLaunchTrigger)
        {
            particles_reactivateLaunch.Play();
        }
    }

    bool GroundCheck()
    {
        if (collisions < 1)
            return false;

        int layerMask = (LayerMask.GetMask("Ground"));

        Debug.DrawRay(transform.position, Vector2.down, Color.blue, groundCheckDist);
        Debug.DrawRay(new Vector2(transform.position.x + groundCheckSeparations, transform.position.y), Vector2.down, Color.blue, groundCheckDist);
        Debug.DrawRay(new Vector2(transform.position.x - groundCheckSeparations, transform.position.y), Vector2.down, Color.blue, groundCheckDist);

        //Checks the ground at different points
        RaycastHit2D[] groundCheck = new RaycastHit2D[3];
        groundCheck[0] = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDist, layerMask);
        groundCheck[1] = Physics2D.Raycast(new Vector2(transform.position.x + groundCheckSeparations, transform.position.y), Vector2.down, groundCheckDist, layerMask);
        groundCheck[2] = Physics2D.Raycast(new Vector2(transform.position.x - groundCheckSeparations, transform.position.y), Vector2.down, groundCheckDist, layerMask);

        //Have we touched the ground at all?
        foreach (RaycastHit2D check in groundCheck)
        {
            if (check.collider != null)
            {
                return true;
            }
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisions++;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collisions--;

        if (collisions < 0)
            collisions = 0;
    }

    public void Launch(Vector2 dir, float relaunchEnableWarmup, bool ignoreCanLaunch = true)
    {
        Launch(dir, true, ignoreCanLaunch);

        StartCoroutine(reLaunchWarmup(relaunchEnableWarmup));
    }

    public void Launch (Vector2 dir, bool disableRelaunch = true, bool ignoreCanLaunch = false) //Launch object, see Control script for full function
    {
        if (canLaunch || ignoreCanLaunch)
        {
            if(disableRelaunch)
            {
                canLaunch = false;
            }
            StartCoroutine(GroundCooldown(groundCooldownTime));

            particles_launch.Play();

            rb.velocity = Vector2.zero;
            //Launch inverse of the given direction
            rb.AddForce(-dir, ForceMode2D.Impulse);
        }
    }

    private void OnDestroy()
    {
        Definitions.Goops.Remove(this);
    }

    private IEnumerator GroundCooldown(float waitfor)
    {
        isGroundCooldown = true;
        yield return new WaitForSeconds(waitfor);
        isGroundCooldown = false;
    }

    private IEnumerator reLaunchWarmup(float waitfor)
    {
        yield return new WaitForSeconds(waitfor);
        canLaunch = true;
    }
}
