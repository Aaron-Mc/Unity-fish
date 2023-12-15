using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Flock1/Behavior/Stay In Radius")]
public class StayInRadiusBehavior1 : FlockBehavior1
{
    public Vector3 center;
    public float radius = 15f;

    public override Vector3 CalculateMove(FlockAgent1 agent, List<Transform> context, Flock1 flock)
    {
        //Vector3 m_NewPosition;
        Vector3 centerOffset = center - (Vector3)agent.transform.position;
        float t = centerOffset.magnitude / radius;
        if (t < 0.9f)
        {
            return Vector3.zero;
        }

        return centerOffset * t * t;
    }
}
