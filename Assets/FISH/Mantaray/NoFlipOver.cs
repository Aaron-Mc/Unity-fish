// This will prevent the MantaRay from flipping up side down when swimming around

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoFlipOver : MonoBehaviour
{
    float rotationAngle = 80;
    float smoothTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion desiredRotation = Quaternion.Euler(0, 0, rotationAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothTime);
    }
}
