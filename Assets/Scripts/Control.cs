using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThanFramework;

public class Control : FunctionLib
{
    LineRenderer drawLine;

    bool triggerHeld = false;

    GameObject startCircle;
    SpriteRenderer startRend;
    LineRenderer startLineRend;
    GameObject endCircle;
    SpriteRenderer endRend;

    GameObject circleBounds;
    LineRenderer circleBoundsRend;

    public float minRatio = .05f;
    public float maxRatio = .25f;
    private float minRadius = 50;
    private float maxRadius = 300;

    Vector2 savePoint;
    Vector2 relEndPos;
    Vector2 normalizedEndPos;
    Vector2 endPos;
    float dist;

    public float drawLineAlpha = .05f;
    public float startLineAlpha = .1f;

    Gradient circleBoundsGradient;
    GradientColorKey[] cb_colorKey;
    GradientAlphaKey[] cb_alphaKey;
    private float circleBoundsAlpha;
    public float circleBoundsMaxAlpha = .4f;

    public float launchMin = 20;
    public float launchMax = 50;

    public static bool isPlotting = false;

    public static Vector2 estVel;

    Canvas parentCanvas;

    CircleRenderer startCircleRend;
    CircleRenderer circleBoundsCircle;

    float circleBoundsRenderThreshold = .666f;
    float circleBoundsRenderThresholdRadius;

    private void Start()
    {
        Definitions.DefinitionStart();

        //Some rendering initialization
        drawLine = GetComponent<LineRenderer>();

        startCircle = GetChild(this.gameObject, "StartCircle");
        endCircle = GetChild(this.gameObject, "EndCircle");
        circleBounds = GetChild(this.gameObject, "CircleBounds");

        startRend = startCircle.GetComponent<SpriteRenderer>();
        startLineRend = startCircle.GetComponent<LineRenderer>();
        endRend = endCircle.GetComponent<SpriteRenderer>();
        circleBoundsRend = circleBounds.GetComponent<LineRenderer>();

        Color drawLineColor = drawLine.startColor;
        Color startLineColor = startLineRend.startColor;
        //Color circleBoundsColor = circleBoundsRend.startColor;

        drawLineColor.a = drawLineAlpha;
        startLineColor.a = startLineAlpha;
        //circleBoundsColor.a = circleBoundsAlpha;

        circleBoundsGradient = new Gradient();

        cb_colorKey = new GradientColorKey[2];
        cb_colorKey[0].color = cb_colorKey[1].color = Color.white;
        cb_colorKey[0].time = 0;
        cb_colorKey[1].time = 1;

        cb_alphaKey = new GradientAlphaKey[5];
        cb_alphaKey[0].alpha = cb_alphaKey[1].alpha = cb_alphaKey[3].alpha = cb_alphaKey[4].alpha = 0;
        cb_alphaKey[2].alpha = circleBoundsMaxAlpha;
        cb_alphaKey[0].time = 0;
        cb_alphaKey[1].time = .43f;
        cb_alphaKey[2].time = .5f;
        cb_alphaKey[3].time = .57f;
        cb_alphaKey[4].time = 1;

        circleBoundsGradient.SetKeys(cb_colorKey, cb_alphaKey);
        circleBoundsRend.colorGradient = circleBoundsGradient;

        drawLine.startColor = drawLine.endColor = drawLineColor;
        startLineRend.startColor = startLineRend.endColor = startLineColor;

        parentCanvas = transform.parent.GetComponent<Canvas>();

        minRadius = Definitions.mainCamera.pixelHeight * minRatio;
        maxRadius = Definitions.mainCamera.pixelHeight * maxRatio;

        startCircleRend = startCircle.GetComponent<CircleRenderer>();
        circleBoundsCircle = circleBounds.GetComponent<CircleRenderer>();

        //get the size to for rendering the touch min and max radiuses for the player
        startCircleRend._horizRadius = startCircleRend._vertRadius = (minRadius / Definitions.visiblePixelsPerUnitRatio) * (1 / startCircle.transform.localScale.x) * (1 / transform.localScale.x) * (1 / parentCanvas.transform.localScale.x);
        circleBoundsCircle._horizRadius = circleBoundsCircle._vertRadius = (maxRadius / Definitions.visiblePixelsPerUnitRatio) * (1 / circleBounds.transform.localScale.x) * (1 / transform.localScale.x) * (1 / parentCanvas.transform.localScale.x);
        startCircleRend.CreatePoints();
        circleBoundsCircle.CreatePoints();

        circleBoundsRenderThresholdRadius = maxRadius * circleBoundsRenderThreshold;
    }

    // Update is called once per frame
    void Update()
    {
        InputHandler.ControlUpdate();

        //if we are holding the mouse down, activate UI
        startRend.enabled = startLineRend.enabled = endRend.enabled = drawLine.enabled = isPlotting = InputHandler.grabTrigger;

        circleBoundsRend.enabled = false; //circle bounds are always false unless specified later

        if (InputHandler.grabTrigger) //if mouse is held
        {
            if (!triggerHeld) //if this is the start of our action, place the starting point to aim from
            {
                triggerHeld = true;
                savePoint = InputHandler.mouseScreenPos;
            }

            //Calculate the end point of our aim
            dist = Vector2.Distance(savePoint, InputHandler.mouseScreenPos);// InputHandler.mousePos);

            if (dist > minRadius) //as long as we are aiming
            {
                //gets the direction of where we're aiming
                normalizedEndPos = new Vector2(InputHandler.mouseScreenPos.x - savePoint.x, InputHandler.mouseScreenPos.y - savePoint.y).normalized;

                //adds our distance (power)
                if (dist > maxRadius)
                    dist =  maxRadius;
                relEndPos = normalizedEndPos * dist;

                //calculates the final endposition
                endPos = savePoint + relEndPos;


                if (dist >= circleBoundsRenderThresholdRadius)
                {
                    circleBoundsRend.enabled = true;
                    float angle = Mathf.Atan2(relEndPos.y, relEndPos.x) * Mathf.Rad2Deg;
                    circleBounds.transform.localRotation = Quaternion.Euler(0, 0, angle + 90);

                    circleBoundsAlpha = Mathf.Lerp(0, circleBoundsMaxAlpha, (dist - circleBoundsRenderThresholdRadius) / (maxRadius - circleBoundsRenderThresholdRadius));
                    cb_alphaKey[2].alpha = circleBoundsAlpha;
                    circleBoundsGradient.SetKeys(cb_colorKey, cb_alphaKey);
                    circleBoundsRend.colorGradient = circleBoundsGradient;
                    Debug.Log((dist - circleBoundsRenderThresholdRadius) / (maxRadius - circleBoundsRenderThresholdRadius));
                    //Debug.Log((dist - maxRadius * circleBoundsRenderThreshold) / maxRadius);
                }

                //draw end position
                Vector2 drawEndPos = Definitions.mainCamera.ScreenToWorldPoint(endPos);
                endCircle.transform.position = drawEndPos;
                drawLine.SetPosition(1, drawEndPos);

            }
            else //if we are within aimpoint, cancel the ui
            {
                isPlotting = false;
                endRend.enabled = false;
                circleBoundsRend.enabled = false;
                drawLine.enabled = false;
            }

            //draw start position
            Vector2 drawSavePoint = Definitions.mainCamera.ScreenToWorldPoint(savePoint);
            startCircle.transform.position = circleBounds.transform.position = drawSavePoint;
            drawLine.SetPosition(0, drawSavePoint);

            if (isPlotting)
            {
                estVel = GetVelocity();
            }
            else
            {
                estVel = Vector2.zero;
            }
        }
        else if (triggerHeld) //launch if action is triggered by release
        {
            triggerHeld = false;

            if (dist > minRadius)
            {
                SetLaunch();
            }
        }
    }

    void SetLaunch() //Most of the heavy lifting for the goop is done here, this is because it makes it less resource intensive to have more goop on screen
    {
        if (Definitions.Goops.Count > 0)
        {
            //get an accurate power level from the range available
            Vector2 launchVel = GetVelocity();

            foreach (Goop goop in Definitions.Goops)
            {
                goop.Launch(launchVel);
            }
        }
    }

    Vector2 GetVelocity()
    {
        float range = maxRadius - minRadius;
        float endRange = dist - minRadius;
        float launchMod = endRange / range;

        float goopBoost = (launchMax - launchMin) * launchMod;

        return normalizedEndPos * (launchMin + goopBoost);
    }
}
