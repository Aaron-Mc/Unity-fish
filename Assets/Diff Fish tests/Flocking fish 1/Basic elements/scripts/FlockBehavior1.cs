using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBehavior1 : ScriptableObject
{
    public abstract Vector3 CalculateMove(FlockAgent1 agent, List<Transform> context, Flock1 flock);
}