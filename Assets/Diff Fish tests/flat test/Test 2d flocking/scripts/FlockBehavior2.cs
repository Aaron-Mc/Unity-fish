using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBehavior2 : ScriptableObject
{
    public abstract Vector2 CalculateMove(FlockAgent2 agent, List<Transform> context, Flock2 flock);
}