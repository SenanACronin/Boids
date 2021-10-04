using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiteAI : MonoBehaviour
{

    public float boxRadius;
    public LayerMask obstacles;
    [SerializeField]
    float sphereRadius;
    [SerializeField]
    float viewDistance = .1f;
    [SerializeField]
    int viewAngle = 80;
    [SerializeField]
    float speed = 1f;
    [SerializeField]
    float avoidancePow = .01f;
    [SerializeField]
    float alignmentPow = .01f;
    [SerializeField]
    float targetFollowPow;
    [SerializeField]
    float obstacleAvoidancePow = .5f;
    [SerializeField]
    float cohesionPow = .1f;
    Vector3 averageBoidPositions;
    GameObject closestTarget;
    GameObject controller;
    List<GameObject> listOfBoids = new List<GameObject>();
    List<GameObject> visableBoids = new List<GameObject>();
    List<GameObject> boidTargets = new List<GameObject>();
    List<Vector3> visionVectors;
    Rigidbody rb;


    private void Awake() {
        controller = GameObject.FindGameObjectWithTag("GameController");
        if (controller != null )
        {
            updateBoidsList();
        }
        
        rb = GetComponent<Rigidbody>();

        closestTarget = this.gameObject;

        visionVectors = gameObject.GetComponent<FibbanacciPoints>().GeneratePointsOnSphere(400, -1, viewAngle);
        
        // if (Time.time > 2)
        // {
        //     controller.GetComponent<MiteSpawner>().UpdateMites();
        // }
    }
    private void FixedUpdate() {
        // clear the list of visable boids each frame
        visableBoids.Clear();
        
        // make a list of all boids that are visable
        foreach (GameObject otherBoid in listOfBoids)
        {
            //if the distance between this boid and tho other is less than the viewDistance check the angle
            if (Vector3.Magnitude(otherBoid.transform.position - transform.position) <= viewDistance)
            {
                // if the angle(measured in degrees, from 0 to 180) is less than viewAngle, add it to the list
                if (Vector3.Angle(transform.forward, otherBoid.transform.position - transform.position) <= viewAngle)
                {
                    visableBoids.Add(otherBoid);
                }
            }
        }

        //* for each visible boid, apply the three rules
        // AVOIDANCE
        foreach (GameObject boid in visableBoids)
        {
            Vector3 lookingAway = transform.position - boid.transform.position;
            float distToBoid =  (1f/ Vector3.Distance(this.transform.position, boid.transform.position));

            Quaternion targetRotation = Quaternion.FromToRotation(transform.forward, lookingAway) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, distToBoid * avoidancePow);
        }

        // ALLIGNMENT
        foreach (GameObject boid in visableBoids){
            Quaternion targetRotation = boid.transform.rotation;
            float distToBoid = (1f / Vector3.Distance(this.transform.position, boid.transform.position));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, distToBoid* alignmentPow);
        }

        // COHESION
        // calculate the average pos of visBoids
        averageBoidPositions = Vector3.zero;
        if (visableBoids.Count != 0){
            foreach(GameObject boid in visableBoids){
                averageBoidPositions += boid.transform.position;
            }
            averageBoidPositions /= visableBoids.Count;

            // Point toward the avg pos 
            Vector3 vToPos = averageBoidPositions - transform.position;
            Quaternion targetRotation = Quaternion.FromToRotation(transform.forward, vToPos) * transform.rotation;
            float strengthToCoh = (Vector3.Magnitude(vToPos)*2) + 1;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, strengthToCoh * cohesionPow);
        }

        // TARGET FOLLOWING
        boidTargets.Clear();
        foreach (GameObject target in GameObject.FindGameObjectsWithTag("BoidTarget"))
        {
            boidTargets.Add(target);
            if (closestTarget == this.gameObject || Vector3.Distance(transform.position, target.transform.position) < Vector3.Distance(transform.position, closestTarget.transform.position))
            {
                closestTarget = target;
            }
        }
        Vector3 vectorToTarget = closestTarget.transform.position - transform.position;
        Quaternion rotationToTarget = Quaternion.FromToRotation(transform.forward, vectorToTarget) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationToTarget, targetFollowPow);

        //TODO
        // add obstacle avoidance 
        Vector3 bestDir = this.transform.position + FindUnobstructedDirection(visionVectors, sphereRadius);
        
        Quaternion r = Quaternion.FromToRotation(transform.forward, vectorToTarget) * transform.rotation;
        //transform.rotation = Quaternion.Lerp(transform.rotation, r, obstacleAvoidancePow);
        

        // move forward
        rb.AddForce(transform.forward * Time.deltaTime * speed);
        
        // // Teleport boid to stay in bounding box
        // if (transform.position.x > boxRadius+.5f || transform.position.x < -boxRadius - .5f)
        // {
        //     float traX = -transform.position.x;
        //     transform.position = new Vector3(traX, transform.position.y, transform.position.z);
        //     // clamp the position to be within the box radius
        //     transform.position = new Vector3(Mathf.Clamp(transform.position.x, -boxRadius, boxRadius), Mathf.Clamp(transform.position.y, -boxRadius - .5f, boxRadius + .5f), Mathf.Clamp(transform.position.z, -boxRadius - .5f, boxRadius + .5f));
        // }
        // else if (transform.position.z > boxRadius+.5f || transform.position.z < -boxRadius - .5f)
        // {
        //     float traZ = -transform.position.z;
        //     transform.position = new Vector3(transform.position.x, transform.position.y, traZ);
        //     // clamp the position to be within the box radius
        //     transform.position = new Vector3(Mathf.Clamp(transform.position.x, -boxRadius, boxRadius), Mathf.Clamp(transform.position.y, -boxRadius - .5f, boxRadius + .5f), Mathf.Clamp(transform.position.z, -boxRadius - .5f, boxRadius + .5f));
        // }
        // else if (transform.position.y > boxRadius + .5f || transform.position.y <-boxRadius - .5f)
        // {
        //     float traY = -transform.position.y;
        //     transform.position = new Vector3(transform.position.x, traY, transform.position.z);
        //     // clamp the position to be within the box radius
        //     transform.position = new Vector3(Mathf.Clamp(transform.position.x, -boxRadius, boxRadius), Mathf.Clamp(transform.position.y, -boxRadius - .5f, boxRadius + .5f), Mathf.Clamp(transform.position.z, -boxRadius - .5f, boxRadius + .5f));
        // }

    }

    private Vector3 FindUnobstructedDirection(List<Vector3> rays, float sphereRadius)
    {
        Vector3 bestDir = transform.forward;
        float furthestDist = 0;
        RaycastHit hit;

        foreach (Vector3 v in rays)
        {
            Vector3 dir = transform.TransformDirection(v);
            if (Physics.SphereCast(transform.position, sphereRadius, dir, out hit, viewDistance, obstacles))
            {
                if (hit.distance > furthestDist)
                {
                    bestDir = dir;
                    furthestDist = hit.distance;
                }
            }
            else
            {
                return dir;
            }
        }
        return bestDir;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,transform.position + averageBoidPositions - transform.position);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(averageBoidPositions, .2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(this.transform.position, closestTarget.transform.position);
        Gizmos.DrawLine(this.transform.position, this.transform.position + FindUnobstructedDirection(visionVectors, sphereRadius));
        foreach (GameObject bio in visableBoids)
        {
            Gizmos.DrawWireSphere(bio.transform.position, .05f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, bio.transform.position);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position+  transform.position - bio.transform.position);
        }
        // Gizmos.color = Color.gray;
        // foreach (GameObject bio in listOfBoids)
        // {
        //     Gizmos.DrawLine(transform.position, bio.transform.position);
        // }
    }
    
        
    
    /// <summary>
    /// Updates the listOfBoids variable on the available object
    /// </summary>
    /// <param name="boid"> The boid that is calling this method</param>
    /// <returns> A list of all other boids </returns>
        public void updateBoidsList () {
        GameObject[] arrayBoids = GameObject.FindGameObjectsWithTag("Boid");
        listOfBoids.Clear();
        foreach (GameObject thisBoid in arrayBoids)
        {
            if(thisBoid != this.gameObject){
                listOfBoids.Add(thisBoid);
            }
        }
    }
}

