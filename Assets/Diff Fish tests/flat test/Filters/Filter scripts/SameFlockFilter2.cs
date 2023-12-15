using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock2/Filter/Same Flock 2")]
public class SameFlockFilter2 : ContextFilter2
{
    public override List<Transform> Filter(FlockAgent2 agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (Transform item in original)
        {
            FlockAgent2 itemAgent = item.GetComponent<FlockAgent2>();
            if (itemAgent != null && itemAgent.AgentFlock == agent.AgentFlock)
            {
                filtered.Add(item);
            }
        }
        return filtered;
    }
}
