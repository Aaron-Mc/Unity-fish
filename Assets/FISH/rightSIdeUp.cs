/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rightSIdeUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float negX = -45f;
    public float posX = 45f;
    // public float negY = -90f;
    // public float posY = 90f;
    public float negZ = -60f;
    public float posZ = 60f;


    // Update is called once per frame
    void Update()
    {
        float xCurr = transform.eulerAngles.x;
        float yCurr = transform.eulerAngles.y;
        float zCurr = transform.eulerAngles.z;
        if(xCurr < negX)
        {
            transform.eulerAngles.x = negX;
        }
        else if(xCurr > posX)
        {
            transform.eulerAngles.x = posX;
        }

        if(zCurr < negZ)
        {
            transform.eulerAngles.z = negZ;
        }
        else if(zCurr > posZ)
        {
            transform.eulerAngles.z = posZ;
        }
    }
}*/
//////////////////////////////////////////////////////////////
// RotationConstraint.js
// Penelope iPhone Tutorial
//
// RotationConstraint constrains the relative rotation of a
// Transform. You select the constraint axis in the editor and
// specify a min and max amount of rotation that is allowed
// from the default rotation
//////////////////////////////////////////////////////////////

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rightSIdeUp : MonoBehaviour
{

    private enum ConstraintAxis
    {
        X = 0,
        Y,
        Z
    }

    public ConstraintAxis axis;           // Rotation around this axis is constrained
    public float min;                     // Relative value in degrees
    public float max;                     // Relative value in degrees
    private Transform thisTransform;
    private Vector3 rotateAround;
    private Quaternion minQuaternion;
    private Quaternion maxQuaternion;
    private float range;

    void Start()
    {
        thisTransform = transform;

        // Set the axis that we will rotate around
        switch (axis)
        {
            case ConstraintAxis.X:
                rotateAround = Vector3.right;
                break;

            case ConstraintAxis.Y:
                rotateAround = Vector3.up;
                break;

            case ConstraintAxis.Z:
                rotateAround = Vector3.forward;
                break;
        }

        // Set the min and max rotations in quaternion space
        var axisRotation = Quaternion.AngleAxis(thisTransform.localRotation.eulerAngles[axis], rotateAround);
        minQuaternion = axisRotation * Quaternion.AngleAxis(min, rotateAround);
        maxQuaternion = axisRotation * Quaternion.AngleAxis(max, rotateAround);
        range = max - min;
    }

    // We use LateUpdate to grab the rotation from the Transform after all Updates from
    // other scripts have occured
    void LateUpdate()
    {
        // We use quaternions here, so we don't have to adjust for euler angle range [ 0, 360 ]
        var localRotation = thisTransform.localRotation;
        var axisRotation = Quaternion.AngleAxis(localRotation.eulerAngles[axis], rotateAround);
        var angleFromMin = Quaternion.Angle(axisRotation, minQuaternion);
        var angleFromMax = Quaternion.Angle(axisRotation, maxQuaternion);

        if (angleFromMin <= range && angleFromMax <= range )
            return; // within range
        else
        {
            // Let's keep the current rotations around other axes and only
            // correct the axis that has fallen out of range.
            var euler = localRotation.eulerAngles;
            if (angleFromMin > angleFromMax)
                euler[axis] = maxQuaternion.eulerAngles[axis];
            else
                euler[axis] = minQuaternion.eulerAngles[axis];

            thisTransform.localEulerAngles = euler;
        }
    }

}*/

