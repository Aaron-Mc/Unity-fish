using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock1/Filter/Physics Layer 1")]
public class PhysicsLayerFilter1 : ContextFilter1
{
    public LayerMask mask;

    public override List<Transform> Filter(FlockAgent1 agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (Transform item in original)
        {
            if (mask == (mask | (1 << item.gameObject.layer)))
            {
                filtered.Add(item);
            }
        }

        return filtered;
    }
}