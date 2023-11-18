using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosHelper
{
    // Start is called before the first frame update
    public static void DrawWireCapsule(Vector3 center1, Vector3 center2, float radius, float maxDistance, int num)
    {
        Vector3 centerPoint = (center2 + center1) / 2;

        Vector3 topPoint = (center1 - center2).normalized * radius;
        Vector3 bottomPoint = (center2 - center1).normalized * radius;

        List<Vector3> vectors = new List<Vector3>();

        //Gizmos.DrawLineList(vectors);



    }
}
