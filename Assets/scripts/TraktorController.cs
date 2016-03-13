using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



public class TraktorController : MonoBehaviour {
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public float minSteeringAngle = 5;
    public float steeringSpeed1 = 20;
    public float steeringSpeed2 = 50;
    public GameObject tp;
    Rigidbody rb;
    public Text velText;
    public float maxSpeed1 = 30;
    public float maxSpeed2 = 40;
    public float reverseMaxSpeed1 = 10;
    public float reverseMaxSpeed2 = 15;
    public AudioSource tut;
    public AudioSource traktor1, traktor2;

    float currentSpeed = 0;
    float engineRPM = 0;

    private float interp(float x, float x0, float x1, float y0, float y1)
    {
        return y0 + (y1 - y0)*(x - x0)/(x1 - x0);
    }

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = tp.transform.localPosition;
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
        currentSpeed = Vector3.Dot(rb.velocity, transform.forward) * 3.6f;

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
                motor = Input.GetAxis("Vertical")*maxMotorTorque - maxMotorTorque * (-currentSpeed - reverseMaxSpeed1) / (reverseMaxSpeed1 - reverseMaxSpeed2);
            }
        } else if (Input.GetAxis("Vertical") > 0) { 
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

        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        if (currentSpeed > steeringSpeed1)
        {
            steering = Input.GetAxis("Horizontal") * interp(currentSpeed, steeringSpeed1, steeringSpeed2, maxSteeringAngle, minSteeringAngle);
        }
        

        foreach (AxleInfo axleInfo in axleInfos) 
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
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
    public void Update()
    {
        velText.text =(Mathf.Round( (float)(Vector3.Dot(rb.velocity, transform.forward) * 3.6)) ).ToString() + " km/h";
        if (Input.GetButtonDown("Jump"))
        {
            tut.Play();
        }

        if (currentSpeed > 1)
        {
            if (!traktor1.isPlaying)
            {
                traktor1.Play();
            }
            traktor2.Stop();
            traktor1.pitch = 1.0f + 1f * Mathf.Abs(currentSpeed) / 40.0f;
        }
        else
        {
            if (!traktor2.isPlaying)
            {
                traktor2.Play();
              
            }
            traktor1.Stop();
        }

    }
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.ToString());
        if (collision.gameObject.CompareTag("ball"))
        {
            collision.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
    public bool front;
}