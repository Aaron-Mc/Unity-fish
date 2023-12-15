using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContextFilter2 : ScriptableObject
{
    public abstract List<Transform> Filter(FlockAgent2 agent, List<Transform> original);
}
