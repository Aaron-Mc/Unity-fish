using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMove : MonoBehaviour
{
    //variables
    private spawner m_AIManager;

    //moving variables
    private bool m_hasTarget = false;
    private bool m_isTurning;

    //variable for waypoint
    private Vector3 m_wayPoint;
    private Vector3 m_lastWaypoint = new Vector3 (0f,0f,0f);

    //animation speed
    //private Animator m_animator;
    private float m_speed;

    private Collider m_collider;
    
    public bool useRandomTarget;

    //use this for initialization
    void Start()
    {
        m_AIManager = transform.parent.GetComponentInParent<spawner>();
        //m_animator = GetComponent<Animator>();

        SetUpNPC();
    }

    void SetUpNPC()
    {
        //randomly scale fish
        //float m_scale = Random.Range(1f, 2f);
        //transform.localScale += new Vector3(m_scale * 1.5f, m_scale, m_scale);

        if(transform.GetComponent<Collider>() != null && transform.GetComponent<Collider>().enabled == true)
        {
            m_collider = transform.GetComponent<Collider>();
        }
        else if(transform.GetComponentInChildren<Collider>() != null && transform.GetComponentInChildren<Collider>().enabled == true)
        {
            m_collider = transform.GetComponentInChildren<Collider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if no waypoint to move
        //if found
        if(!m_hasTarget)
        {
            m_hasTarget = CanFindTarget();
        
        }
        else
        {
            RotateNPC(m_wayPoint, m_speed);
            transform.position = Vector3.MoveTowards(transform.position, m_wayPoint, m_speed * Time.deltaTime);

            //checks collision
            CollidedNPC();
        }

        if(transform.position == m_wayPoint)
        {
            m_hasTarget = false;
        }
    }

    void CollidedNPC()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, transform.localScale.z))
        {
            if(hit.collider == m_collider | hit.collider.tag == "wayoint")
            {
                return;
            }
            
            int randomNum = Random.Range(1, 100);
            if(randomNum < 40)
                m_hasTarget = false;
            
            Debug.Log(hit.collider.transform.parent.name + " " + hit.collider.transform.parent.position);
        }
    }

    //Make waypoint
    Vector3 GetWaypoint(bool isRandom)
    {
        if(isRandom)
        {
            return m_AIManager.RandomPosition();
        }
        else
        {
            return m_AIManager.RandomWayPoint();
        }
    }

    bool CanFindTarget(float start = 1f, float end = 7f)
    {
       

        if(m_lastWaypoint == m_wayPoint)
        {
            m_wayPoint = GetWaypoint(true);
            return false;
        }
        else
        {
            m_lastWaypoint = m_wayPoint;
            m_speed = Random.Range(start, end);
            //m_animator.speed = m_speed;
            return true;
        }
    }

    //rotating npc to look in direction
    void RotateNPC(Vector3 waypoint, float currentSpeed)
    {
        float TurnSpeed = currentSpeed * Random.Range(1f, 3f);

        Vector3 LookAt = waypoint - this.transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(LookAt), TurnSpeed * Time.deltaTime);
    }
}
