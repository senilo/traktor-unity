using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



public class WagonController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public Transform connectedTo;
    int numBalls = 0;
    private float interp(float x, float x0, float x1, float y0, float y1)
    {
        return y0 + (y1 - y0) * (x - x0) / (x1 - x0);
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void FixedUpdate()
    {

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.front)
            {
                Vector3 center = 0.5f * (axleInfo.leftWheel.transform.position + axleInfo.rightWheel.transform.position);
                Vector3 direction = transform.worldToLocalMatrix*(connectedTo.position - center);
                float steering = Mathf.Clamp(Mathf.Rad2Deg * Mathf.Atan2( direction.x, direction.z ), -30, 30);

                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }
    /*
    public bool addBall(GameObject ball)
    {
        if (numBalls == 14) return false;

        if (numBalls < 8)
        {
            int row = numBalls / 2;
            int col = numBalls % 2;

            ball.transform.parent = this.transform;
            ball.transform.localPosition = new Vector3(-wagonWidth / 4 + col * wagonWidth / 2, ballHeight, (float)(wagonLength * (0.5 - 1.0 / 8 - 0.25 * row)));
            ball.transform.rotation = transform.rotation;
            ball.GetComponent<BoxCollider>().isTrigger = false;
        }
        else
        {
            int row = (numBalls - 8) / 2;
            int col = (numBalls - 8) % 2;
            ball.transform.parent = this.transform;
            ball.transform.localPosition = new Vector3(-wagonWidth / 4 + col * wagonWidth / 2, ballHeight2, (float)(wagonLength * (0.5 - 1.0 / 4 - 0.25 * row)));
            ball.transform.rotation = transform.rotation;
            ball.GetComponent<BoxCollider>().isTrigger = false;

        }
        balls.Add(ball);
        numBalls++;
        return true;
    }
    */
}