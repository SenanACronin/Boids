using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    void Update()
    {
        GameObject[] boids = GameObject.FindGameObjectsWithTag("Boid");
        Vector3 pos = Vector3.zero;
        foreach (GameObject boid in boids)
        {
            pos += boid.transform.position;
        }
        pos /= boids.Length;
        transform.LookAt(pos);
    }
}
