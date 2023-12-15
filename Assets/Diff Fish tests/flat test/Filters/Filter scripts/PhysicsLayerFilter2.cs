using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock2/Filter/Physics Layer 2")]
public class PhysicsLayerFilter2 : ContextFilter2
{
    public LayerMask mask;

    public override List<Transform> Filter(FlockAgent2 agent, List<Transform> original)
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