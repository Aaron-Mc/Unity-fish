using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject fishPreFab;
    public GameObject goalPrefab;
    public static int tankSize = 5;
     public Vector3 spawnArea { get { return m_SpawnArea; }}
     [SerializeField]
    private Color m_SpawnColor = new Color(1.000f, 0.000f, 0.000f, 0.300f);
    [SerializeField]
    private Vector3 m_SpawnArea = new Vector3(10f, 10f, 10f);

    
    static int numFish = 10;
    public static GameObject[] allFish = new GameObject[numFish];

    public static Vector3 goalPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < numFish; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-tankSize, tankSize),
                                      Random.Range(-tankSize, tankSize), 
                                      Random.Range(-tankSize, tankSize));
            // Vector3 pos = new Vector3(Random.Range(-spawnArea.x, spawnArea.x),
            //                           Random.Range(-spawnArea.y, spawnArea.y), 
            //                           Random.Range(-spawnArea.z, spawnArea.z));                          

            allFish[i] = (GameObject) Instantiate(fishPreFab, pos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Random.Range(0, 50000) < 50)
        {
            
            goalPos = new Vector3(Random.Range(-tankSize, tankSize),
                                      Random.Range(-tankSize, tankSize), 
                                      Random.Range(-tankSize, tankSize));
            // goalPos = new Vector3(Random.Range(-spawnArea.x, spawnArea.x),
            //                           Random.Range(-spawnArea.y, spawnArea.y), 
            //                           Random.Range(-spawnArea.z, spawnArea.z));                       
            goalPrefab.transform.position = goalPos;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = m_SpawnColor;
        Gizmos.DrawCube(transform.position, spawnArea);
    }
}
