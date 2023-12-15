using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock1/Behavior/Alignment 1")]
public class AlignmentBehavior1 : FilteredFlockBehavior1
{
    public override Vector3 CalculateMove(FlockAgent1 agent, List<Transform> context, Flock1 flock)
    {
        //if no neighbors, maintain current alignment
        if (context.Count == 0)
            return agent.transform.up;

        //add all points together and average
        Vector3 alignmentMove = Vector3.zero;
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        foreach (Transform item in filteredContext)
        {
            alignmentMove += (Vector3)item.transform.up;
        }
        alignmentMove /= context.Count;

        return alignmentMove;
    }
}