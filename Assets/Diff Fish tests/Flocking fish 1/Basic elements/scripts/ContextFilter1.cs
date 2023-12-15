using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContextFilter1 : ScriptableObject
{
    public abstract List<Transform> Filter(FlockAgent1 agent, List<Transform> original);
}
