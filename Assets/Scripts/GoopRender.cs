using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThanFramework;

public class GoopRender : MonoBehaviour
{
    SpriteRenderer rend;
    Goop parGoop;
    Rigidbody2D rb;

    public float stretchMax = 1.5f;
    public float magnitudeMod = .1f;

    public Color startingColor;
    public Color fadedColor;

    public Vector2 Stretch //does a bit of math to get our stretch factor, we stretch the x axis and squish the y to an equal amount
    {
        get
        {
            float x_stretch = AbsClamp(1 + rb.velocity.magnitude * magnitudeMod, 1, stretchMax);
            float y_squish = 1 - Mathf.Abs(x_stretch - 1);

            return new Vector2(x_stretch, y_squish);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rb = transform.parent.GetComponent<Rigidbody2D>();
        parGoop = transform.parent.GetComponent<Goop>();
    }

    // Update is called once per frame
    void Update()
    {
        SetColor();

        //Rotates our sprite towards the angle of momentum
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0,0, angle);

        //Stretches the sprite along the pseudo x-axis (sprite rotation does the rest of the work)
        transform.localScale = Stretch;

    }

    void SetColor()
    {
        if (parGoop.canLaunch)
        {
            rend.color = startingColor;
        }
        else
        {
            rend.color = fadedColor;
        }
    }


    float AbsClamp(float f, float cMin = 0, float cMax = 1)
    {
        return Mathf.Clamp(Mathf.Abs(f), cMin, cMax);
    }
}