using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smolMiteAI : MonoBehaviour
{
    List<GameObject> listOfBoids = new List<GameObject>();
    [SerializeField]
    private float viewDistance;
    [SerializeField]
    private float viewAngle;
    List<GameObject> visableBoids = new List<GameObject>();
    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameController").Length == 0)
        {
            updateBoidsList();
        }
    }
    private void FixedUpdate() {
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



   }
    public void updateBoidsList()
    {
        GameObject[] arrayBoids = GameObject.FindGameObjectsWithTag("Boid");
        listOfBoids.Clear();
        foreach (GameObject thisBoid in arrayBoids)
        {
            if (thisBoid != this.gameObject)
            {
                listOfBoids.Add(thisBoid);
            }
        }
}
}
