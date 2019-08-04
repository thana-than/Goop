using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ThanFramework;

public class CameraControl : MonoBehaviour
{

    Camera cam;

    public float cameraSpeed = .01f;
    public float cameraMove = 0;
    public float t = 0;

    Transform highestGoop;

    public bool isCameraCooldown = false;
    public float cameraCooldownTime = 1;

    public float topYMargin = 7.5f;

    float startingZPos;

    public bool heightTest = false;
    public float testHeight;
    public float rangeToActivateCooldown = 6f;

    // Start is called before the first frame update
    void Start()
    {
        startingZPos = transform.position.z;
        cam = GetComponent<Camera>();

        //float cross = (cam.pixelHeight * Definitions.visibleUnitWidth / cam.pixelWidth) / 2;
    }


    // Update is called once per frame
    private void Update()
    {
        cam.orthographicSize = (float)cam.pixelHeight * Definitions.visibleUnitWidth / cam.pixelWidth / 2;
    }

    void FixedUpdate()
    {
        if (Definitions.Goops.Count > 0)
        {
            //Standard camera scroll
            if (!isCameraCooldown)
            {
                cameraMove = Mathf.Lerp(0, cameraSpeed, t);

                transform.Translate(new Vector2(0, cameraMove));

                if (t < 1)
                    t = t + .02f;
            }
            else
            {
                t = 0;
            }

            //Track the highest goop y position


            float highestY = Definitions.Goops[0].transform.position.y;
            foreach (Goop goop in Definitions.Goops)
            {
                if (goop.transform.position.y > highestY)
                {
                    highestY = goop.transform.position.y;
                }
            }

            //if the highest goop y goes near top bounds, follow it
            if (highestY > (transform.position.y + topYMargin))
            {
                if (!heightTest)
                {
                    heightTest = true;
                    testHeight = transform.position.y + topYMargin;
                }

                transform.position = new Vector3(0, highestY - topYMargin, startingZPos);
            }
            else if (heightTest)
            {
                heightTest = false;
                //start a cooldown for the smooth camera scrool to start again
                if ((transform.position.y + topYMargin) - testHeight > rangeToActivateCooldown)
                    StartCoroutine(CameraCooldown(cameraCooldownTime));
            }
        }

    }

    private IEnumerator CameraCooldown(float waitfor)
    {
        isCameraCooldown = true;
        yield return new WaitForSeconds(waitfor);
        isCameraCooldown = false;
    }
}
