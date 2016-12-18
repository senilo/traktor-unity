using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;



public class WagonController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque;
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public Transform connectedTo;
    public Transform link;
    float wagonWidth = 2.5f, wagonLength = 6f;
    float ballHeight = 0.3f, ballHeight2 = 1.55f;
    List<GameObject> balls;
    bool firstUpdate = true;

    public float maxSpeed1 = 30;
    public float maxSpeed2 = 40;
    public float reverseMaxSpeed1 = 10;
    public float reverseMaxSpeed2 = 15;

    private float interp(float x, float x0, float x1, float y0, float y1)
    {
        return y0 + (y1 - y0) * (x - x0) / (x1 - x0);
    }

    public void Start()
    {
        GetComponent<Rigidbody>().sleepThreshold = 0.0f;
        balls = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Drop"))
        {
            DropBall();
        }
        if (GetComponent<Rigidbody>().IsSleeping())
        {
            Debug.Log("Wagon sover!");
        }
        foreach(var axle in axleInfos)
        {
            if (axle.leftWheel.attachedRigidbody.IsSleeping())
            {
                Debug.Log("front: " + axle.steering + ", left sover");

            }
            if (axle.rightWheel.attachedRigidbody.IsSleeping())
            {
                Debug.Log("front: " + axle.steering + ", right sover");

            }
        }
        if (link.GetComponent<Rigidbody>().IsSleeping())
        {
            Debug.Log("link sover!");
        }
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
        var currentSpeed = Vector3.Dot(GetComponent<Rigidbody>().velocity, transform.forward) * 3.6f;

        float motor = 0;
        float brake = 0;
        if (Input.GetAxis("Vertical") < 0)
        {
            if (currentSpeed > 0)
            {
                motor = 0;
                brake = maxMotorTorque;
            }
            else if (-currentSpeed < reverseMaxSpeed1)
            {
                motor = maxMotorTorque * Input.GetAxis("Vertical");
            }
            else if (-currentSpeed < reverseMaxSpeed2)
            {
                motor = Input.GetAxis("Vertical") * maxMotorTorque - maxMotorTorque * (-currentSpeed - reverseMaxSpeed1) / (reverseMaxSpeed1 - reverseMaxSpeed2);
            }
        }
        else if (Input.GetAxis("Vertical") > 0)
        {
            if (currentSpeed < 0)
            {
                motor = 0;
                brake = maxMotorTorque;
            }
            else if (currentSpeed < maxSpeed1)
            {
                motor = maxMotorTorque * Input.GetAxis("Vertical");
            }
            else if (currentSpeed < maxSpeed2)
            {
                motor = Input.GetAxis("Vertical") * maxMotorTorque - maxMotorTorque * (currentSpeed - maxSpeed1) / (maxSpeed1 - maxSpeed2);
                motor = Input.GetAxis("Vertical") * Mathf.Lerp(maxMotorTorque, 0, (currentSpeed - maxSpeed1) / (maxSpeed1 - maxSpeed2));
            }
        }
        else
        {
            motor = 0;
        }


        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                // Compute steering angle for front wheels, point at connectedTo
                Vector3 center = 0.5f * (axleInfo.leftWheel.transform.position + axleInfo.rightWheel.transform.position);
                Vector3 direction = transform.worldToLocalMatrix*(connectedTo.position - center);
                float steering = Mathf.Clamp(Mathf.Rad2Deg * Mathf.Atan2( direction.x, direction.z ), -30, 30);

                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            
            if(axleInfo.motor == true)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            axleInfo.leftWheel.brakeTorque = brake;
            axleInfo.rightWheel.brakeTorque = brake;

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }
    public bool addBall(GameObject ball)
    {
        int numBalls = balls.Count;
        if (numBalls == 14) return false;

        if (numBalls < 8)
        {
            int row = numBalls / 2;
            int col = numBalls % 2;

            ball.transform.parent = this.transform;
            ball.transform.localPosition = new Vector3(-wagonWidth / 4 + col * wagonWidth / 2, ballHeight, (float)(wagonLength * (0.5 - 1.0 / 8 - 0.25 * row)));
            ball.transform.rotation = transform.rotation;
            ball.GetComponent<Rigidbody>().isKinematic = true;
            ball.GetComponentInChildren<MeshCollider>().enabled = false;
            //ball.GetComponent<BoxCollider>().isTrigger = false;
        }
        else
        {
            int row = (numBalls - 8) / 2;
            int col = (numBalls - 8) % 2;
            ball.transform.parent = this.transform;
            ball.transform.localPosition = new Vector3(-wagonWidth / 4 + col * wagonWidth / 2, ballHeight2, (float)(wagonLength * (0.5 - 1.0 / 4 - 0.25 * row)));
            ball.transform.rotation = transform.rotation;
            ball.GetComponent<Rigidbody>().isKinematic = true;
            ball.GetComponentInChildren<MeshCollider>().enabled = false;
            //ball.GetComponent<BoxCollider>().isTrigger = false;

        }
        balls.Add(ball);
        numBalls++;
        return true;
    }

    public void DropBall()
    {
        if (balls.Count == 0) return;

        else
        {
            var b = balls[balls.Count - 1];
            b.transform.parent = null;
            b.transform.Translate(Vector3.up * 2, Space.Self);
            b.GetComponent<Rigidbody>().isKinematic = false;
            b.GetComponentInChildren<MeshCollider>().enabled = true;
            balls.Remove(b);
        }

    }
}