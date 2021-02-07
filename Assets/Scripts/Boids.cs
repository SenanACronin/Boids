using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    private Rigidbody rb;
    public float Speed = 1f;
    public GameObject[] ListBoids;
    public float minDistanceToBoid = 2.5f;
    private Transform placeholder;
    private Transform currentTra;
    public int box = 5;
    Vector3 aimVector;

    void Start() 
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        ListBoids = GameObject.FindGameObjectsWithTag("Boid");
        rb.AddForce(transform.forward);
    }


    /// <summary>
    /// Returns a vector from this boid to the center of mass of other boids
    /// </summary>
    /// <param name="List"> A list of all boid gameobjects in the scene</param>
    /// <param name="ThisBoid"> The Boid Gameobject that this method is being called on </param>
    /// <returns></returns>
    Vector3 Convergance(GameObject[] List, GameObject ThisBoid)
    {
        Vector3 perCenter = new Vector3(0, 0, 0);

        foreach (GameObject CurrentBoid in List)
        {
            if (ThisBoid != CurrentBoid)
            {
                perCenter += CurrentBoid.transform.position;
            }
        }
        perCenter = perCenter / (List.Length - 1);

        perCenter -= this.transform.position;

        return perCenter;
    }


    /// <summary>
    /// Returns a vector that is an addition of vectors away from all boids within minDistanceToBoid
    /// </summary>
    /// <param name="List"> A list of all boids in the scene</param>
    /// <param name="ThisBoid"> The boid that this function is being called on </param>
    /// <returns></returns>
    Vector3 Avoidance(GameObject[] List, GameObject ThisBoid)
    {
        Vector3 avoidBoids = new Vector3(0,0,0);
        foreach (GameObject byrd in ListBoids)
        {
            if (Vector3.Distance(this.transform.position, byrd.transform.position) < minDistanceToBoid && byrd != ThisBoid)
            {
                avoidBoids += (this.transform.position + (Vector3.Normalize(this.transform.position - byrd.transform.position) / 5 * (1 / Vector3.Distance(this.transform.position, byrd.transform.position))));
            }
        }
        if (avoidBoids != new Vector3(0, 0, 0))
        {
            return avoidBoids;
        }
        else
        {
            return ThisBoid.transform.position;
        }

    }

    /// <summary>
    /// return a vector that is the average velocity of all other boids in the scene
    /// </summary>
    /// <param name="List"> A list of all boids in the scene </param>
    /// <param name="ThisBoid"> The boid this is being called on </param>
    /// <returns></returns>
    Vector3 Alignment(GameObject[] List, GameObject ThisBoid)
    {
        Vector3 avgVelocity = new Vector3(0, 0, 0);
        foreach (GameObject Boid in List)
        {
            if (Boid != ThisBoid)
            {
                avgVelocity += Boid.GetComponent<Rigidbody>().velocity;
            }
        }
        
        return avgVelocity / List.Length;
    }

    private void FixedUpdate() 
    {
        Vector3 v1;
        v1 = Convergance(ListBoids, gameObject) + Avoidance(ListBoids, gameObject) - transform.position + Alignment(ListBoids, gameObject);

        Quaternion rotation = Quaternion.LookRotation(v1);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Mathf.Clamp(v1.magnitude/100, .01f, 2f));
        rb.AddForce(Speed * transform.forward);
        Speed = Mathf.Clamp( Mathf.Lerp(Speed, Alignment(ListBoids, gameObject).magnitude, .1f), .5f, 1.5f);
    }



    private void OnDrawGizmosSelected() 
    {
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawLine(transform.position, aimVector);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.rb.velocity);

        
        Gizmos.color = Color.magenta;
        foreach (GameObject byrd in ListBoids)
        {
            if (Vector3.Distance(this.transform.position, byrd.transform.position) < minDistanceToBoid)
            {
                Gizmos.DrawLine(this.transform.position,  this.transform.position + (Vector3.Normalize (this.transform.position - byrd.transform.position)/5 * (1/Vector3.Distance(this.transform.position, byrd.transform.position)))  );
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Avoidance(ListBoids, gameObject));

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Convergance(ListBoids, gameObject));
        Gizmos.DrawWireSphere(transform.position + Convergance(ListBoids, this.gameObject), 0.5f);

        // Gizmos.color = Color.black;
        // foreach (GameObject byrd in ListBoids)
        // {
        //     Gizmos.DrawLine(this.transform.position, byrd.transform.position);
        //     Gizmos.DrawWireSphere(byrd.transform.position, Vector3.Distance(this.transform.position, byrd.transform.position));
        // }

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position + Convergance(ListBoids, this.gameObject) ,transform.position + Convergance(ListBoids, this.gameObject) + Alignment(ListBoids, this.gameObject));


    }
    
   
       
}
