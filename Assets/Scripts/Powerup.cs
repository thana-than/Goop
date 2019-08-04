using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ThanFramework;

public class Powerup : FunctionLib
{

    Goop interactiveGoop;

    bool cooldown = false;
    SpriteRenderer spr;
    ParticleSystem particles;
    Color startingColor;

    public Color fadeColor = Color.grey;
    public float cooldownTimer = 1;

    public Vector2 bumpForce = new Vector2(0, 15);
    public float boostForce = 60f;
    Vector2 boostVelocity;

    public bool hit_bump = true;
    public bool hit_reactivateLaunch = true;
    public bool hit_boostInDirection = false;
    public bool hit_normalizeBoost = false;
    public bool hit_clone = false;

    LineRenderer plotRenderer;
    LineRenderer plotRenderer2;
    public GameObject plotRend;
    GameObject plotRend2;


    public float normalizeBoostForce = 30f;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        particles = GetComponent<ParticleSystem>();

        startingColor = spr.color;

        if (hit_boostInDirection)
        {
            plotRend = Instantiate(plotRend, this.transform);
            plotRenderer = plotRend.GetComponent<LineRenderer>();
            plotRend2 = Instantiate(plotRend, this.transform);
            plotRenderer2 = plotRend2.GetComponent<LineRenderer>();

            Color c1, c2;
            c1 = c2 = startingColor;
            c1.a = .15f;
            c2.a = 0;

            plotRenderer.startColor = c1;
            plotRenderer2.startColor = c1;
            plotRenderer.endColor = c2;
            plotRenderer2.endColor = c2;
        }

        //var main = particles.main;
        //main.startColor = startingColor;
    }

    private void Update()
    {
        if (hit_boostInDirection)
        {
            boostVelocity = -this.transform.up * boostForce;
            RenderPlot(plotRenderer, plotRenderer2, transform.position, Definitions.modelGrav, Definitions.modelDrag, -boostVelocity);
        }

        if (IsBelowScreen(new Vector2 (0,.33f)))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent == null)
            return;

        if (!cooldown && collision.transform.parent.gameObject.CompareTag("Player"))//&& collision.transform.parent.GetComponent<Goop>() != null)
        {
            interactiveGoop = collision.transform.parent.GetComponent<Goop>();

            if (hit_reactivateLaunch && !hit_boostInDirection)
                interactiveGoop.canLaunch = true;

            if (hit_bump)
                Bump();

            if (hit_boostInDirection)
            {
                if (hit_reactivateLaunch)
                    interactiveGoop.Launch(boostVelocity, .25f);
                else
                    interactiveGoop.Launch(boostVelocity, true);
            }

            if (hit_normalizeBoost)
            {
                interactiveGoop.rb.velocity = interactiveGoop.rb.velocity.normalized * normalizeBoostForce;
            }

            if (hit_clone)
            {
                GameObject clone = Instantiate(interactiveGoop.gameObject, interactiveGoop.transform.position, Quaternion.identity);
                Vector2 cloneVelocity = interactiveGoop.rb.velocity;
                cloneVelocity.x = -cloneVelocity.x;
                clone.GetComponent<Rigidbody2D>().velocity = cloneVelocity;
                clone.GetComponent<Goop>().canLaunch = hit_reactivateLaunch;
            }

            StartCoroutine(Cooldown(cooldownTimer));
        }
    }

    void Bump()
    {
        interactiveGoop.rb.velocity = interactiveGoop.rb.velocity / 2;
        interactiveGoop.rb.AddForce(bumpForce, ForceMode2D.Impulse);
    }

    private IEnumerator Cooldown(float waitfor)
    {
        cooldown = true;
        particles.Clear();
        particles.Pause();
        spr.color = fadeColor;
        yield return new WaitForSeconds(waitfor);
        cooldown = false;
        particles.Play();
        spr.color = startingColor;
    }
}
