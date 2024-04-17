using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock1/Behavior/Avoidance 1")]
public class AvoidanceBehavior1 : FilteredFlockBehavior1
{
    public LayerMask mask;
    public Collision collision;

    // var Raycast hit;
    // var forward = transform.TransformDirection(Vector3.forward);

    public override Vector3 CalculateMove(FlockAgent1 agent, List<Transform> context, Flock1 flock)
    {
       
        //if no neighbors, return no adjustment
        if (context.Count == 0)
            return Vector3.zero;

        //add all points together and average
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        foreach (Transform item in filteredContext)
        {
            if (Vector3.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                nAvoid++;
                avoidanceMove += (Vector3)(agent.transform.position - item.position);

                //if(mask == (mask | (1 << item.gameObject.layer)))
                //{
                    //Vector3 targetDirection = item.position - agent.position;


                    // Destroy(item.gameObject);
                //}
            }

            

        }
        if (nAvoid > 0)
            avoidanceMove /= nAvoid;

        return avoidanceMove;
    }

   
}
