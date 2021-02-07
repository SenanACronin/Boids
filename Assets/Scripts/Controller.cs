using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller : MonoBehaviour
{
    
    private void FixedUpdate() 
    {
        Quaternion currentRotation = this.transform.rotation;
        Quaternion q1;
        q1 = Quaternion.FromToRotation(this.transform.forward, new Vector3(0, 5, 0));
        q1 = Quaternion.Lerp(currentRotation, q1, .01f);
        this.transform.rotation = q1;
        
    }

}
