//search unity fish AI tutorial to finish (currentlly on part 2 24:32)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[System.Serializable]
public class AIObjects
{
    //variables
    public string AIGroupName { get { return m_aiGroupName; }}
    public GameObject objectPrefab {get { return m_prefab; }}
    public int maxAI { get { return m_maxAI; }}
    public int spawnRate { get { return m_spawnRate; }}
    public int spawnAmount { get { return m_maxSpawnAmount; }}
    public bool randomizeStats { get { return m_randomizeStats; }}
    public bool enableSpawner { get { return m_enableSpawner; }}


    //serlize the private variables
    [Header("AI Group Stats")]
    [SerializeField]
    private string m_aiGroupName;
    [SerializeField]
    private GameObject m_prefab;
    [SerializeField]
    [Range(0f, 30f)]
    private int m_maxAI;
    [SerializeField]
    [Range(0f, 20f)]
    private int m_spawnRate;
    [SerializeField]
    [Range(0f, 10f)]
    private int m_maxSpawnAmount;

    //new variable
    [Header("Main Settings")]
    [SerializeField]
    private bool m_enableSpawner;
    [SerializeField]
    private bool m_randomizeStats;

    public AIObjects(string Name, GameObject Prefab, int MaxAI, int SpawnRate, int SpawnAmount, bool RandomizeStats)
    {
        this.m_aiGroupName = Name;
        this.m_prefab = Prefab;
        this.m_maxAI = MaxAI;
        this.m_spawnRate = SpawnRate;
        this.m_maxSpawnAmount = SpawnAmount;
        this.m_randomizeStats = RandomizeStats;
    }

    public void setValues(int MaxAI, int SpawnRate, int SpawnAmount)
    {
        this.m_maxAI = MaxAI;
        this.m_spawnRate = SpawnRate;
        this.m_maxSpawnAmount = SpawnAmount;
    }

}


public class spawner : MonoBehaviour
{
    //declare variable

    public List<Transform> Waypoints = new List<Transform>();

    public float spawnTimer { get { return m_SpawnTimer; }}
    public Vector3 spawnArea { get { return m_SpawnArea; }}
    //serliaze the private variables
    [Header("Global Stats")]
    [Range(0f, 600f)]
    [SerializeField]
    private float m_SpawnTimer;
    [SerializeField]
    private Color m_SpawnColor = new Color(1.000f, 0.000f, 0.000f, 0.300f);
    [SerializeField]
    private Vector3 m_SpawnArea = new Vector3(20f, 10f, 20f);

    //array of new class
    [Header("AI Group Settings")]
    public AIObjects[] AIObject = new AIObjects[5];

    // Start is called before the first frame update
    void Start()
    {
        GetWaypoints();
        RandomiseGroups();
        CreateAIGroups();
        InvokeRepeating("SpawnNPC", 0.5f, spawnTimer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnNPC()
    {
        //loop through all AI gorups
        for(int i = 0; i < AIObject.Count(); i++)
        {
            //Check spawner enabled
            if(AIObject[i].enableSpawner && AIObject[i].objectPrefab != null)
            {
                //make sure there is not more than max NPCs// need better way to find the go for the AI gorups
                GameObject tempGroup = GameObject.Find(AIObject[i].AIGroupName);
                if(tempGroup.GetComponentInChildren<Transform>().childCount < AIObject[i].maxAI)
                {
                    //Spawn random number of npc from 0 to max
                    for(int j = 0; j < Random.Range(0, AIObject[i].spawnAmount); j++)
                    {
                        //random rotation
                        Quaternion randomRotation = Quaternion.Euler(Random.Range(-20, 20), Random.Range(0,360), 0);
                        // create spawned gameobject
                        GameObject tempSpawn;
                        tempSpawn = Instantiate(AIObject[i].objectPrefab, RandomPosition(), randomRotation);
                        //put spawned NPC as child of group
                        tempSpawn.transform.parent = tempGroup.transform;
                        //Add the AIMove script and class to the new NPC
                        tempSpawn.AddComponent<AIMove>();
                    }
                }
            }
        }
    }

    //public method for Random Position within the spawn area
    public Vector3 RandomPosition()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            Random.Range(-spawnArea.y, spawnArea.y),
            Random.Range(-spawnArea.z, spawnArea.z)
        );
        randomPosition = transform.TransformPoint(randomPosition * .5f);
        return randomPosition;
    }

    //public method for getting random waypoint
    public Vector3 RandomWayPoint()
    {
        int randomWP = Random.Range(0, (Waypoints.Count - 1));
        Vector3 randomWaypoint = Waypoints[randomWP].transform.position;
        return randomWaypoint;
    }

    // putting random values in group settings 
    void RandomiseGroups()
    {
        for(int i = 0; i < AIObject.Count(); i++)
        {
            if(AIObject[i].randomizeStats)
            {
                //AIObject[i].maxAI = Random.Range(1, 30);
                //AIObject[i] = new AIObjects(AIObject[i].AIGroupName, AIObject[i].objectPrefab, Random.Range(1, 30), Random.Range(1, 20), Random.Range(1, 10), AIObject[i].randomizeStats);
                AIObject[i].setValues(Random.Range(1, 30), Random.Range(1, 20), Random.Range(1, 10));
            }
        }
    }

    //Method for creating empty worldobject groups
    void CreateAIGroups()
    {
        for (int i = 0; i < AIObject.Count(); i++)
        {
            //Empty gameobject to keep AI in
            GameObject m_AIGroupSpawn;

            //new game object
            m_AIGroupSpawn = new GameObject(AIObject[i].AIGroupName);
            m_AIGroupSpawn.transform.parent = this.gameObject.transform;
        }
    }

    void GetWaypoints()
    {
        //list form standard library
        // looks though nested children
        Transform[] wpList = this.transform.GetComponentsInChildren<Transform>();// 'this' is not required
        for(int i = 0; i < wpList.Length; i++)
        {
            if(wpList[i].tag == "wayoint")
            {
                Waypoints.Add(wpList[i]);
            }
        }
    }

    //show the gizmos in color
    void OnDrawGizmosSelected()
    {
        Gizmos.color = m_SpawnColor;
        Gizmos.DrawCube(transform.position, spawnArea);
    }

}