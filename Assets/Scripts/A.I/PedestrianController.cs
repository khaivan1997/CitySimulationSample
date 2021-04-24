using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]

public class PedestrianController : MonoBehaviour
{
    //data
    public Vector3 destination;
    public string[] unavoidableObstacles = { "Vehicle" };
    public string[] avoidableObstacles = { "Pedestrian" };
    public float stuckfixPeriod;
   

    [Header("info")]
    [ReadOnly]
    public float obstacleCheckRadius;
    [ReadOnly]
    public Vector3[] nodes;

    //components
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public Rigidbody rigidBody;
    //trymoving path with physics
    [HideInInspector]
    public NavMeshPath path;
    [ReadOnly]
    public int subpathIndex;
    [HideInInspector]
    public float minimumVelocityMagnitude ;
    [HideInInspector]
    public float stopDistance ;

    //fix hard stuck
    [HideInInspector]
    public Vector3 previousPos;
    [HideInInspector]
    public float checkStuckCooldown;

    // Start is called before the first frame update
    public virtual void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(transform.position);

        rigidBody = GetComponent<Rigidbody>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        path = new NavMeshPath();
        agent.CalculatePath(destination, path);
        //foreach (Vector3 v in path.corners)
          //  Debug.Log(this.gameObject.tag+": "+v);
        subpathIndex = 0;
        minimumVelocityMagnitude = 0.6f;
        stopDistance = 0.2f;
        obstacleCheckRadius = GetComponent<BoxCollider>().size.x*1.2f;
        nodes = path.corners;
        previousPos = transform.position;
        checkStuckCooldown = stuckfixPeriod;
        isBraking = false;
    }


    // Update is called once per frame
    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        agent.Warp(transform.position);
        if (path == null || subpathIndex >= path.corners.Length)
        {
            if (Vector3.Distance(destination, transform.position) <= this.stopDistance)
                stopMoving();
            else
                tryMoveToPosition(destination);
            return;
        }
        if(Vector3.Distance(path.corners[subpathIndex], transform.position) <= this.stopDistance)
        {
            //transform.position = path.corners[subpathIndex];
            subpathIndex++;
        }
        if(subpathIndex < path.corners.Length)
        {
            if (!tryMoveToPosition(path.corners[subpathIndex]))
                    subpathIndex++ ;
        }
    }
    public bool tryMoveToPosition (Vector3 destination)
    {
        if ( havingUnavoidableObstacle())
        {
            stopMoving();
            return true;
        }

        

     
        //force.y = 0;
        tryAvoidObstacle();
        //Debug.Log("Velobefore:"+ force);
        //Debug.Log(gameObject.tag+" Veloafter:" + (destination - transform.position).normalized);
        return move(destination);   
        
    }

    [ReadOnly]
    public bool isBraking;
    [ReadOnly]
    public float avoidMultiplier;
    [ReadOnly]
    public GameObject toAvoid;
    public void tryAvoidObstacle()
    {
        float maxDistance = agent.radius;
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxDistance, LayerMask.GetMask(avoidableObstacles))
            .OrderBy(collider => (collider.transform.position - transform.position).magnitude).ToArray() ;
        bool isAvoid = false;
        numCollider = colliders.Length;
        avoidMultiplier = 0;
        foreach (Collider collider in colliders)
        {
            if (collider.transform == this.transform)
                continue;
            
            if (isObjectinFront(collider.transform.position) ) 
            {
                Vector3 opponentForward = collider.transform.forward;
                toAvoid = collider.gameObject;
                if (Vector3.Angle(transform.forward, opponentForward) <90f){
                    isBraking = true;
                    return;
                }
                isAvoid = true;
                if (isObjectontheRight(collider.transform.position))
                    avoidMultiplier -= 1f;
                else avoidMultiplier += 1f;
                break;
            }
        }
        if (!isAvoid)
            toAvoid = null;
        isBraking = false;
    }
    [ReadOnly]
    public int numCollider;

    public bool havingUnavoidableObstacle()
    {
        float maxDistance = agent.radius;
        Collider[] colliders = Physics.OverlapSphere(transform.position, agent.radius * 2, LayerMask.GetMask(unavoidableObstacles));


        foreach (Collider collider in colliders)
        {
            if (isObjectinFront(collider.transform.position,10f))
                return true;
        }
        return false;
    }

    public void OnDrawGizmos()
    {
        float maxDistance = agent.radius * 2; ;
        Gizmos.DrawSphere(transform.position, obstacleCheckRadius);
    }
    public virtual void stopMoving()
    {
        rigidBody.velocity = Vector3.zero;
        //Debug.Log("Stop:" + subpathIndex+ " havingob:" + havingUnavoidableObstacle());
        
    }
    public virtual bool move(Vector3 desination)
    {
        //Debug.Log(gameObject.tag + " apply velo:"+velocity);
        Vector3 velocity = desination-transform.position;
        if (velocity.magnitude > minimumVelocityMagnitude)
        {
            velocity.y = 0;
            velocity = (velocity * agent.speed).normalized;
        }
        
        velocity = velocity * agent.speed;
        velocity.y = 0;
        //rigidBody.AddForce(transform.up * velocity.y);
        rigidBody.velocity = velocity;
        rigidBody.transform.rotation = Quaternion.LookRotation(rigidBody.velocity, transform.up);

        if (isBraking)
        {
           rigidBody.velocity = rigidBody.velocity /2f;
           return true;
        }
        rigidBody.velocity = rigidBody.velocity + rigidBody.transform.right * avoidMultiplier;
        velocity = desination - transform.position;
        if (velocity.magnitude <= minimumVelocityMagnitude )
            return false;
        return true;
    }

    public bool isObjectinFront(Vector3 other, float viewPort = 30f)
    {
        Vector3 distanceVector = other - transform.position;
        //Debug.Log("mag:"+distanceVector.magnitude);
        float result0 = Vector3.Dot(transform.forward, distanceVector);
        if ( result0 > 0 || Mathf.Approximately(result0, 0))
        {
            float result = Vector3.Angle(transform.forward, distanceVector);
            if (Mathf.Approximately(result, viewPort))
                return true;
            return result <= viewPort ;
        }
        return false;
    }


    public bool isObjectontheRight(Vector3 other)
    {
        Vector3 distanceVector = other - transform.position;
        //Debug.Log("RightVector:" + transform.right + " " + other + " and relative:"+ (transform.InverseTransformPoint(other).x > 0));
        float result = transform.InverseTransformPoint(other).x;
        if (Mathf.Approximately(result, 0))
            return true;
        return result > 0;

    }

    public void StuckFix()
    {
        if(checkStuckCooldown == 0)
        {

        }
    }
}
