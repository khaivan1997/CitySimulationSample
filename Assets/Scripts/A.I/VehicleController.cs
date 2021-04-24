using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : PedestrianController
{
    public List<AxleInfo> axleInfos;
    public float maxSteeringAngle;
    public float maxSpeed;

    [HideInInspector]
    public float maxstopTorque;
    

    public override void OnEnable()
    {
        base.OnEnable();
        
        this.minimumVelocityMagnitude = 4f;
        maxstopTorque = agent.speed;
    }

    public void Start()
    {
        foreach (AxleInfo axle in axleInfos)
            axle.init();
    }
    public override bool move(Vector3 destination)
    {
        Vector3 velocity = destination - transform.position;
        float steering;
        float motor = agent.speed;
        if (avoidMultiplier != 0)
        {
            steering = maxSteeringAngle * avoidMultiplier;
            motor = motor / 2f;
        }
        else
        {
            Vector3 local = transform.InverseTransformPoint(destination);
            steering = Mathf.Abs(local.x /local.magnitude);
            steering = isObjectontheRight(destination) ? steering : -steering;
            steering = maxSteeringAngle * steering;
        }
            
        
       //velocity.magnitude > agent.speed ? agent.speed : velocity.magnitude;
        //motor = Mathf.Abs(steering)>maxSteeringAngle/3 ? motor/1.5f :motor;// velocity.magnitude > maxMotorTorque ? maxMotorTorque : velocity.magnitude;
        //Debug.Log(" destination: "+ path.corners[subpathIndex]+" direction:" + velocity + " at:" + motor + " steerAngle: " + steering);
        moveCar(motor, steering);
        if (isBraking)
        {
            deceleration(maxstopTorque / 2f);
            return true;
        }
        if (velocity.magnitude <= minimumVelocityMagnitude && subpathIndex < path.corners.Length-1)
        {
            deceleration(maxstopTorque);
            return false;
        }
        deceleration(0);
        return true;
    }
    public override void stopMoving()
    {
        Debug.Log("stop");
        deceleration(maxstopTorque*1.5f);
    }

    protected void deceleration(float stopTorgue)
    {
        foreach (AxleInfo axle in axleInfos)
        {
            axle.leftWheelCollider.brakeTorque = stopTorgue;
            axle.rightWheelCollider.brakeTorque = stopTorgue;
        }
    }

    public void OnDisable()
    {
        
    }


    public void ApplyLocalPositionToVisuals(AxleInfo axleInfo)
    {
        Vector3 position;
        Quaternion rotation;
        axleInfo.leftWheelCollider.GetWorldPose(out position, out rotation);
        Transform leftWheelMesh = axleInfo.leftWheelCollider.transform.GetChild(0);
        leftWheelMesh.transform.position = position;
        leftWheelMesh.transform.rotation =  axleInfo.leftMeshDefaultRotation* rotation;

        axleInfo.rightWheelCollider.GetWorldPose(out position, out rotation);
        Transform rightWheelMesh = axleInfo.rightWheelCollider.transform.GetChild(0);
        rightWheelMesh.transform.position = position;
        rightWheelMesh.transform.rotation = axleInfo.rightMeshDefaultRotation* rotation;
    }

    public void moveCar(float motor, float steering)
    {
        float angleTurnRate = Time.fixedDeltaTime * 30f;
        foreach (AxleInfo axle in axleInfos)
        {
            if (axle.steering)
            {
                float currentSteering = axle.leftWheelCollider.steerAngle;
                axle.leftWheelCollider.steerAngle = Mathf.Lerp(currentSteering, steering, angleTurnRate);
                axle.rightWheelCollider.steerAngle = Mathf.Lerp(currentSteering, steering, angleTurnRate);
            }
            if (axle.motor)
            {
                float currentSpeed = 2 * Mathf.PI * axle.leftWheelCollider.radius * axle.leftWheelCollider.rpm * 60f / 3600;
                if (currentSpeed < maxSpeed)
                {
                    axle.leftWheelCollider.motorTorque = motor;
                    axle.rightWheelCollider.motorTorque = motor;
                }
            }
            ApplyLocalPositionToVisuals(axle);
        }
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheelCollider;
    public WheelCollider rightWheelCollider;
    public bool motor;
    public bool steering;

    [HideInInspector]
    public Quaternion leftMeshDefaultRotation;
    [HideInInspector]
    public Quaternion rightMeshDefaultRotation;
    public void init()
    {
        leftMeshDefaultRotation = leftWheelCollider.transform.GetChild(0).localRotation;
        rightMeshDefaultRotation = rightWheelCollider.transform.GetChild(0).localRotation;
    }
}
