using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiteSpawner : MonoBehaviour
{
    public GameObject mites;
    [SerializeField]
    int length = 10;
    Vector3 position;
    Quaternion rotation;
    float boxRadius;
    private void Start() {
        boxRadius = mites.GetComponent<MiteAI>().boxRadius;
        Component aiScript = mites.GetComponent<MiteAI>();

        for (int i = 0; i < length; i++)
        {
            position = new Vector3(Random.Range(-boxRadius, boxRadius), Random.Range(-boxRadius, boxRadius), Random.Range(-boxRadius, boxRadius));
            rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
            Instantiate(mites, position, rotation);
        }
        GameObject[] mitesToUpdate = GameObject.FindGameObjectsWithTag("Boid");
        foreach (GameObject bitch in mitesToUpdate)
        {
            bitch.GetComponent<MiteAI>().updateBoidsList();
        }
    }
//    private void OnDrawGizmos() {
//         for (int i = 0; i < length; i++)
//         {
//             position = new Vector3(Random.Range(-boxRadius, boxRadius), Random.Range(-boxRadius, boxRadius), Random.Range(-boxRadius, boxRadius));
//             rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
//             Gizmos.color = Color.cyan;
//             Gizmos.DrawSphere(position, .1f);
//         }
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireCube(new Vector3(0, 0, 0), new Vector3(boxRadius * 2, boxRadius * 2, boxRadius * 2));
//     }
}
