using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock1/Filter/Same Flockc 1")]
public class SameFlockFilter1 : ContextFilter1
{
    public override List<Transform> Filter(FlockAgent1 agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (Transform item in original)
        {
            FlockAgent1 itemAgent = item.GetComponent<FlockAgent1>();
            if (itemAgent != null && itemAgent.AgentFlock == agent.AgentFlock)
            {
                filtered.Add(item);
            }
        }
        return filtered;
    }
}
