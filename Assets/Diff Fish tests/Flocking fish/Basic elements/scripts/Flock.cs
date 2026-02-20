using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//<Summary>
// flocking/ schooling simulator 
// </Summary>
// Original idea and section of original code is from
// Board To bits Gaming https://github.com/boardtobits/flocking-algorithm

public class Flock : MonoBehaviour
{
    //creates a list of all of the fish objects of the flock
    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehavior behavior;

    // Number of fish objests min, max, and starting count
    [Range(1, 500)]
    public int startingCount = 250;
    // how tightly they are spawned
    const float AgentDensity = 0.08f;

    // The speed multiplier
    [Range(0.1f, 100f)]
    public float driveFactor = 10f;
    // The max speed that the fish objects can move
    [Range(0.1f, 100f)]
    public float maxSpeed = 5f;
    // The closets distance that the fish objects can be in the space of one another
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    // the distance the fish objects avoid 'obsticals' 
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    // How strongly agents are pushed away from the flock center when inside the boundary.
    [Range(0f, 10f)]
    public float centerRepulsion = 0f;
    // Maximum radius from flock center within which repulsion applies.
    [Range(0.1f, 100f)]
    public float repulsionRadius = 15f;
    // How strongly agents are pulled back when they drift outside the radius.
    [Range(0f, 10f)]
    public float centerAttraction = 1.5f;
    // Adds a tangential steering component so agents naturally circle around the boundary.
    [Range(0f, 5f)]
    public float orbitStrength = 0.5f;

    [Header("Smoothing")]
    [Tooltip("Smooths commanded velocity to reduce twitching from rapidly changing steering vectors.")]
    public bool smoothVelocity = true;

    [Tooltip("Higher values follow steering more closely; lower values are smoother.")]
    [Range(0.1f, 30f)]
    public float velocityResponsiveness = 8f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    readonly Dictionary<FlockAgent, Vector3> _smoothedVelocities = new Dictionary<FlockAgent, Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        // crates a total number of 'starting count' fish objects 
        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(
                agentPrefab,
                Random.insideUnitSphere * startingCount * AgentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
                );
            newAgent.name = "Agent " + i;
            newAgent.Initialize(this);
            agents.Add(newAgent);
            _smoothedVelocities[newAgent] = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // calculates movement for each fish object
        foreach (FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);

            Vector3 move = behavior.CalculateMove(agent, context, this);

            // Boundary steering: inside -> gentle repulsion; outside -> gentle attraction.
            // Adds tangential steering to avoid sticking along a single axis.
            if (repulsionRadius > 0f)
            {
                Vector3 centerToAgent = agent.transform.position - transform.position;
                float dist = centerToAgent.magnitude;
                if (dist > 0.0001f)
                {
                    Vector3 radialDir = centerToAgent / dist; // normalized

                    if (centerRepulsion > 0f && dist < repulsionRadius)
                    {
                        // Linear falloff inside radius
                        float falloff = 1f - (dist / repulsionRadius);
                        move += radialDir * (centerRepulsion * falloff);
                    }
                    else if (centerAttraction > 0f && dist > repulsionRadius)
                    {
                        // Pull back when outside, with falloff that grows with distance but clamps
                        float falloff = Mathf.Clamp01((dist - repulsionRadius) / repulsionRadius);
                        move += (-radialDir) * (centerAttraction * falloff);
                    }

                    // Tangential component to encourage circling around the boundary
                    if (orbitStrength > 0f)
                    {
                        // Use global up as axis to get a horizontal tangent (assuming Y-up environment)
                        Vector3 tangent = Vector3.Cross(Vector3.up, radialDir);
                        // Randomize orbit direction per agent to avoid uniform rotation
                        int hash = agent.name.GetHashCode();
                        float sign = (hash & 1) == 0 ? 1f : -1f;
                        move += tangent.normalized * (orbitStrength * sign);
                    }
                }
            }

            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }

            // Smooth the commanded velocity to reduce twitching/jitter.
            if (smoothVelocity)
            {
                _smoothedVelocities.TryGetValue(agent, out Vector3 current);
                float t = 1f - Mathf.Exp(-velocityResponsiveness * Time.deltaTime);
                move = Vector3.Lerp(current, move, t);
                _smoothedVelocities[agent] = move;
            }

            // Let the agent handle movement; do not move or rotate the flock transform
            agent.Move(move);
        }
    }

    // creates all informaiton for collision objects for the flock agens (fish objects)
    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        // setting up the overlap collison model
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighborRadius);
        foreach (Collider c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }
}
