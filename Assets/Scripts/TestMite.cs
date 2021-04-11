using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMite : MonoBehaviour
{
    Vector3 pos = new Vector3(1, 2, 3);
    Vector3 vToPos;
    private void FixedUpdate() {
        float xPos = 2 * Mathf.Sin(Time.time);
        transform.position = new Vector3(xPos, transform.position.y, transform.position.z);

        vToPos = pos - transform.position;
        // Quaternion targetRotation = Quaternion.FromToRotation(transform.forward, vToPos) * transform.rotation;
        // transform.rotation = Quaternion.Lerp(transform.rotation,targetRotation, .1f);
        Vector3 lookingAway = transform.position - pos;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.forward, lookingAway) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1f);

    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(pos, .05f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + vToPos );
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position+ transform.forward);
        Gizmos.DrawLine(transform.position, transform.position+ transform.position - pos);
    }
}
