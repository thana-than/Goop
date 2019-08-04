using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ThanFramework;
using static ThanFramework.Definitions;
using static ThanFramework.InputHandler;

public class FunctionLib : MonoBehaviour
{
    public float plotRatio = .5f;
    public Vector2 cameraMargin = new Vector2(.03f, .03f);

    public bool IsBelowScreen()
    {
        return IsBelowScreen(cameraMargin);
    }

    public bool IsBelowScreen(Vector2 margin)
    {
        Vector2 screenPoint = Definitions.mainCamera.WorldToViewportPoint(transform.position);

        if (screenPoint.y<(0 - margin.y)) //if object is offscreen below
        {
            return true;
        }

        return false;
    }

    #region Rendering

    /*
    public void RenderDottedLine()
    {
        RenderDottedLine(gameObject.GetComponent<LineRenderer>());
    }
    public void RenderDottedLine(LineRenderer line)
    {

    }
    */

    #endregion

    #region Physics

    public void RenderPlot(LineRenderer lr1, LineRenderer lr2, Vector2 pos, Rigidbody2D rb, Vector2 vel, int skipCount = 5)
    {
        RenderPlot(lr1, lr2, pos, rb.gravityScale, rb.drag, vel, skipCount);
    }

    public void RenderPlot(LineRenderer lr1, LineRenderer lr2, Vector2 pos, float rbGrav, float rbDrag, Vector2 vel, int skipCount = 5)
    {
        lr1.enabled = true;

        Vector2 screenPoint = Definitions.mainCamera.WorldToViewportPoint(pos);

        Vector2[] points = Plot(rbGrav, rbDrag, transform.position, vel * plotRatio, lr1.positionCount * skipCount);
        //Render main line
        for (int i = 0, q = 0; i < lr1.positionCount; i++, q = q + skipCount)
        {
            lr1.SetPosition(i, points[q]);
        }


        Vector2 lastPoint = Definitions.mainCamera.WorldToViewportPoint(points[(lr1.positionCount * skipCount) - 1]);

        if (lastPoint.x < (0 - cameraMargin.x)) // if line is offscreen to the left
        {
            lr2.enabled = true;

            Vector2[] points2 = Plot(rbGrav, rbDrag, Definitions.mainCamera.ViewportToWorldPoint(new Vector2(screenPoint.x + 1, screenPoint.y)), vel * plotRatio, lr2.positionCount * skipCount);

            for (int i = 0, q = 0; i < lr2.positionCount; i++, q = q + skipCount)
            {
                lr2.SetPosition(i, points2[q]);
            }
        }
        else if (lastPoint.x > (1 + cameraMargin.x)) //if offscreen to the right
        {
            lr2.enabled = true;

            Vector2[] points2 = Plot(rbGrav, rbDrag, Definitions.mainCamera.ViewportToWorldPoint(new Vector2(screenPoint.x - 1, screenPoint.y)), vel * plotRatio, lr2.positionCount * skipCount);

            for (int i = 0, q = 0; i < lr2.positionCount; i++, q = q + skipCount)
            {
                lr2.SetPosition(i, points2[q]);
            }
        }
        else //if we aren't offscreen, theres no reason to render a second line
        {
            lr2.enabled = false;
        }
    }

    public static Vector2[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps)
    {
        return Plot(rigidbody.gravityScale, rigidbody.drag, pos, velocity, steps);
    }

    public static Vector2[] Plot(float rbGrav, float rbDrag, Vector2 pos, Vector2 velocity, int steps)
    {
        Vector2[] results = new Vector2[steps];

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = Physics2D.gravity * rbGrav * timestep * timestep;
        float drag = 1f - timestep * rbDrag;
        Vector2 moveStep = velocity * timestep;

        for (int i = 0; i < steps; ++i)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
        }

        return results;
    }

    #endregion

    #region Child Handling

    public GameObject[] GetChildren()
    {
        return GetChildren(this.gameObject);
    }

    static public GameObject[] GetChildren(GameObject objectParent)
    {
        if (objectParent.transform.childCount > 0) //if the object has children
        {
            GameObject[] children = new GameObject[objectParent.transform.childCount];
            for (int i = 0; i < children.Length; i++) //place each child in array
            {
                children[i] = objectParent.transform.GetChild(i).gameObject;
            }
            return children;
        }
        else
        {
            return null;
        }
    }

    static public GameObject GetChild(GameObject objectParent, int childLayer) //Find child by layer
    {
        if (objectParent.transform.childCount > 0) //if the object has children
        {
            int children = objectParent.transform.childCount;
            for (int i = 0; i < children; ++i) //search to see if there are any children that are within the specified layer
            {
                if (objectParent.transform.GetChild(i).gameObject.layer == childLayer)
                {
                    return objectParent.transform.GetChild(i).gameObject;
                }
            }
            return null;
        }
        else
        {
            return null;
        }
    }

    static public GameObject GetChild(GameObject objectParent, string name, bool searchByTag) //Find child by name
    {
        if (objectParent.transform.childCount > 0) //if the object has children
        {
            int children = objectParent.transform.childCount;
            for (int i = 0; i < children; ++i) //search to see if there are any children that are within the specified layer
            {
                if (searchByTag)
                {
                    if (objectParent.transform.GetChild(i).gameObject.tag == name)
                        return objectParent.transform.GetChild(i).gameObject;
                }
                else if (objectParent.transform.GetChild(i).gameObject.name == name)
                {
                    return objectParent.transform.GetChild(i).gameObject;
                }
            }
            return null;
        }
        else
        {
            return null;
        }
    }

    static public GameObject GetChild(GameObject objectParent, string name) //Find child by name
    {
        return GetChild(objectParent, name, false);
    }

    static public GameObject GetChild(GameObject objectParent, string name, bool searchByTag, bool createIfFalse)
    {
        GameObject child = GetChild(objectParent, name, searchByTag);
        if (!child)
        {
            child = new GameObject();
            child.transform.SetParent(objectParent.transform);
            if (searchByTag)
                child.tag = name;
            else child.name = name;
        }

        return child;
    }

    #endregion

    #region Math Operations

    protected Vector2 Abs(Vector2 value)
    {
        return new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y));
    }

    protected Vector3 Abs(Vector3 value)
    {
        return new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
    }

    protected int GetSignIfValue(float value)
    {
        int v = 0;

        if (value > 0)
            v = 1;
        else if (value < 0)
            v = -1;

        return v;
    }

    protected Vector2Int GetSignIfValue(Vector2 value)
    {
        return new Vector2Int(GetSignIfValue(value.x), GetSignIfValue(value.y));
    }

    protected Vector3Int GetSignIfValue(Vector3 value)
    {
        return new Vector3Int(GetSignIfValue(value.x), GetSignIfValue(value.y), GetSignIfValue(value.z));
    }

    public static Vector2 RandomDirection(int plane, float distance) // Picks a random direction based on given plane and distance
    {
        Vector2 dir = new Vector2(0, 0);

        if (plane == DirectionPlane.fourWay) //For the four way plane, we only want to move either horizontal or vertical. We quickly decide what to change our plane to at random (before we actually go to our switch case).
        {
            plane = UnityEngine.Random.Range((int)0, (int)2);
            //Debug.Log("fourWayRand " + plane);
        }

        switch (plane)
        {
            case DirectionPlane.hor:
                dir = new Vector2(RandOperand() * distance, 0); //Distance multiplied by the operand modifier (either + or -)
                break;

            case DirectionPlane.vert:
                dir = new Vector2(0, RandOperand() * distance);
                break;

            case DirectionPlane.eightWay: //for eight way movement, we can move across both the horizontal and vertical planes

                int randX = UnityEngine.Random.Range((int)0, (int)2); //Are we going to move on the horizontal plane?
                int randY = UnityEngine.Random.Range((int)0, (int)2); //Are we going to move on the vertical plane?

                if (randX == 0 && randY == 0) //Make sure the value isn't Vector2.zero
                {
                    if (UnityEngine.Random.Range((int)0, (int)2) == 1)
                        randX = 1;
                    else
                        randY = 1;
                }

                //Debug.Log("randX " + randX + " | randY " + randY);

                dir = new Vector2(RandOperand() * randX * distance, RandOperand() * randY * distance);

                if (randX == 1 && randY == 1) // if moving diagonally, we shorten the distance to seem more proportionate
                    dir = dir * axisProportion;

                break;
        }

        //Debug.Log("dir " + dir);
        return dir;
    }

    public static Vector2 RandomDirection(int plane)
    {
        return RandomDirection(plane, 1);
    }

    public static int RandOperand() //Returns either +1 or -1
    {
        int rand = UnityEngine.Random.Range((int)0, (int)2);
        //Debug.Log("Operand Value " + rand);

        if (rand == 0)
            return -1;
        else
            return 1;
    }

    public static float FindDegree(int x, int y) //does what it says, returns a degree from a point (relative to zero)
    {
        float value = (float)((Mathf.Atan2(x, y) / Mathf.PI) * 180f);
        if (value < 0) value += 360f;

        return value;
    }

    public static float FindDegree(float x, float y) //does the same as above but for two float values
    {
        float value = (float)((Mathf.Atan2(x, y) / Mathf.PI) * 180f);
        if (value < 0) value += 360f;

        return value;
    }

    public static float FindDegree(Vector2 pos) //does the same as above but for a Vector2 (probably the most used)
    {
        float value = (float)((Mathf.Atan2(pos.x, pos.y) / Mathf.PI) * 180f);
        if (value < 0) value += 360f;

        return value;
    }

    public static float FixAngle(float angle) //Makes sure the given number is within 0-360
    {
        while (angle > 360)
        {
            angle = angle - 360;
        }

        while (angle < 0)
        {
            angle = angle + 360;
        }

        return angle;
    }

    public static float FixAngle(int angle)
    {
        while (angle > 360)
        {
            angle = angle - 360;
        }

        while (angle < 0)
        {
            angle = angle + 360;
        }

        return angle;
    }

    public static Vector2 GetRelativePosition(Transform origin, Vector3 position)
    {
        Vector3 distance = position - origin.position;
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
        relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
        relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);

        return relativePosition;
    }

    public static Vector2 GetRelativePosition(Vector3 origin, Vector3 position)
    {
        Vector3 relativePosition = position - origin;

        return relativePosition;
    }

    #endregion
}
