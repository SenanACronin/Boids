using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FibbanacciPoints : MonoBehaviour
{
    [SerializeField]
    bool isSphere = true;
    [SerializeField]
    bool visualiseVectors = false;
    [SerializeField]
    float pow = 1;
    [SerializeField]
    [Range(0f, 2f)]
    float turnFraction = 1.6180339f;
    [SerializeField]
    int numberOfPoints = 200;
    public float viewAngle;
    private void OnDrawGizmosSelected() {
        if(!isSphere)
        {
            Gizmos.color = Color.cyan;
            foreach (Vector3 point in GeneratePointsOnDisk(numberOfPoints, turnFraction, pow))
            {
                Gizmos.DrawSphere(point, 1f);
            }
        }
        else
        {
            if (!visualiseVectors)
            {
                Gizmos.color = Color.green;
                foreach (Vector3 point in GeneratePointsOnSphere(numberOfPoints, turnFraction, pow))
                {
                    Gizmos.DrawSphere(point *10, 1f);
                }
            }
            else
            {
                Gizmos.color = Color.green;
                foreach (Vector3 point in GeneratePointsOnSphere(numberOfPoints, turnFraction, pow))
                {
                    if(Vector3.Angle(Vector3.forward, point) > viewAngle)
                    {
                        Gizmos.color = Color.red;
                    }
                    Gizmos.DrawLine(Vector3.zero, point);
                }
            }
        }
    }

    private Vector3[] GeneratePointsOnSphere (int NumberOfPoints, float TurnFraction, float Pow)
    {
        Vector3[] pointsGenerated = new Vector3[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            float t = Mathf.Pow(i / (numberOfPoints - 1f), -pow);
            float inclination = Mathf.Acos(1 - 2 * t);
            float theta = 2 * Mathf.PI * turnFraction * i;

            float x = Mathf.Sin(inclination)* Mathf.Cos(theta);
            float y = Mathf.Sin(inclination)* Mathf.Sin(theta);
            float z = Mathf.Cos(inclination);

            pointsGenerated[i] = new Vector3(x, y, z);

        }

        return pointsGenerated;

    }
    /// <summary>
    /// Generates the specified number of points spaced out according to the 
    /// turn fraction
    /// </summary>
    /// <param name="NumberOfPoints">Number of points to generate</param>
    /// <param name="TurnFraction">a default value of 1.6180339f gives an
    /// even spread</param>
    /// <param name="Pow">The value that normalizes the spread of the points, negated </param>
    /// <returns></returns>
    private Vector3[] GeneratePointsOnDisk (int NumberOfPoints, float TurnFraction,float Pow)
    {
        Vector3[] pointsGenerated = new Vector3[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            float dist =  Mathf.Pow( i / (numberOfPoints - 1f), -pow);
            float angle = 2 * Mathf.PI * turnFraction * i;

            float x = dist * Mathf.Cos(angle);
            float y = dist * Mathf.Sin(angle);

            pointsGenerated[i] = new Vector3(x *(i-1)/10, y*(i - 1)/10, 0);
            
        }
        
        return pointsGenerated;
       
    }
}
