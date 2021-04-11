using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiteAI : MonoBehaviour
{
    
    public float boxRadius;
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
    float cohesionPow = .1f;
    Vector3 positions;
    GameObject closestTarget;
    List<GameObject> listOfBoids = new List<GameObject>();
    List<GameObject> visableBoids = new List<GameObject>();
    List<GameObject> boidTargets = new List<GameObject>();
    Rigidbody rb;

    private void Awake() {
        if (GameObject.FindGameObjectsWithTag("GameController").Length == 0 ){
            updateBoidsList();
        }
        rb = GetComponent<Rigidbody>();
        closestTarget = this.gameObject;
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
            float distToBoid =  (5/ Vector3.Distance(this.transform.position, boid.transform.position));

            Quaternion targetRotation = Quaternion.FromToRotation(transform.forward, lookingAway) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, distToBoid * avoidancePow);
        }

        // ALLIGNMENT
        foreach (GameObject boid in visableBoids){
            Quaternion targetRotation = boid.transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, alignmentPow);
        }

        // COHESION
        // calculate the average pos of visBoids
        positions = Vector3.zero;
        if (visableBoids.Count != 0){
            foreach(GameObject boid in visableBoids){
            positions += boid.transform.position;
            }
            positions /= visableBoids.Count;

            // Point toward the avg pos 
            Vector3 vToPos = positions - transform.position;
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
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(0,0,0), new Vector3(boxRadius * 2,boxRadius * 2,boxRadius * 2));
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,transform.position + positions - transform.position);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(positions, .5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(this.transform.position, closestTarget.transform.position);
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
    /// Generates an array of Vector3 points in space that lie on sphere, with spacing defined by the golden ratio
    /// </summary>
    /// <param name="numPoints"></param>
    /// <returns></returns>
    // TODO make this work at all
        private Vector3[] GeneratePointsFibonacci(int numPoints){
            Vector3[] points = new Vector3[numPoints];
            double phi = System.Math.PI * (3 - System.Math.Sqrt(5));  //golden angle in radians

            for (int i = 0; i < numPoints; i++)
            {
                float y = 1 - (i / numPoints - 1) * 2;
                float radiuis = (float)System.Math.Sqrt(1 - y * y);

                double theta = phi * i;

                float x = (float)System.Math.Cos(theta) * radiuis;
                float z = (float)System.Math.Sin(theta) * radiuis;

                points[i] = new Vector3(x, y, z);
            }
            return points;
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

