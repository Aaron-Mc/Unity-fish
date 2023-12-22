using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Avoidance")]
public class AvoidanceBehavior : FilteredFlockBehavior
{
    public LayerMask mask;
    public Collision collision;

    // var Raycast hit;
    // var forward = transform.TransformDirection(Vector3.forward);

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
       
        //if no neighbors, return no adjustment
        if (context.Count == 0)
            return Vector3.zero;

        //add all points together and average
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        // avoids members of its own group
        foreach (Transform item in filteredContext)
        {
            if (Vector3.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                nAvoid++;
                avoidanceMove += (Vector3)(agent.transform.position - item.position);

                if(mask == (mask | (1 << item.gameObject.layer)))
                {
                    // would do things to 'obstical' layer objects
                }
                
                
            }

            

        }
        if (nAvoid > 0)
            avoidanceMove /= nAvoid;

        return avoidanceMove;
    }

   
}
