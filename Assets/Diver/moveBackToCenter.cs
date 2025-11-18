using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveBackToCenter : MonoBehaviour
{
    // starting position
    // -0.144f, 0.7824962f, -3.43896f
   

    private int counter = 0;
    private float lastCheckTime = 0.0f;
    private Vector3 lastCheckPos = new Vector3();
    private float Sec = 1.0f;
    private Vector3 dead = new Vector3(0f, 0f, 0f);



    // Update is called once per frame
    void Update()
    {
        //looks ot see if a second has passeed
        if((Time.time - lastCheckTime) > Sec)
        {
            // if the model has not moved, begine reset countdown
            // otherwise reset counter
            if(transform.position == lastCheckPos)
            {
                counter++;
            }
            else
            {
                counter = 0;
            }

            //after 5 seconds of no movement the model returns to start position
            if (counter >= 5)
            {
                this.transform.position = new Vector3(-0.144f, 0.7824962f, -3.43896f);
            }

            // set last position and time every second
            lastCheckPos = transform.position;
            lastCheckTime = Time.time;
        }
        
    }
}
