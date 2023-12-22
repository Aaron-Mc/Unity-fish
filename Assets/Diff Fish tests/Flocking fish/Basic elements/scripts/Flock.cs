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
    [Range(10, 500)]
    public int startingCount = 250;
    // how tightly they are spawned
    const float AgentDensity = 0.08f;

    // The speed multiplier
    [Range(1f, 100f)]
    public float driveFactor = 10f;
    // The max speed that the fish objects can move
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    // The closets distance that the fish objects can be in the space of one another
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    // the distance the fish objects avoid 'obsticals' 
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

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
        }
    }

    // Update is called once per frame
    void Update()
    {
        // possibly edit
        float _angle;
        float minAngle = -45f; // original -45f
        float maxAngle = 0f; // original 0f
        //

        // calculates movement for each fish object
        foreach (FlockAgent agent in agents)
        {
            // transform.eulerAngles.x;
            List<Transform> context = GetNearbyObjects(agent);

            Vector3 move = behavior.CalculateMove(agent, context, this);
            
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }

            //
            // adjust the y position
            move.y += Time.deltaTime;
            transform.position = move * Time.deltaTime;

            float newAngle = Mathf.Atan2(move.y, move.x);

            newAngle = Mathf.Clamp(newAngle * Mathf.Rad2Deg, minAngle, maxAngle);

            _angle = Mathf.Lerp(move.z , newAngle, Time.deltaTime);

            transform.localEulerAngles = new Vector3(0, 0, _angle);
            //

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
