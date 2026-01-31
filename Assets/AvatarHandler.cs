using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NuitrackSDK.Avatar;

public class AvatarHandler : MonoBehaviour
{
    private NuitrackAvatar avatar;

    // Configure which joints must be tracked for the avatar to remain enabled
    [SerializeField]
    private nuitrack.JointType[] requiredJoints = new nuitrack.JointType[]
    {
        // Head
        nuitrack.JointType.Head,
        // Arms (shoulders, elbows, wrists)
        nuitrack.JointType.LeftShoulder,
        nuitrack.JointType.LeftElbow,
        nuitrack.JointType.LeftWrist,
        nuitrack.JointType.RightShoulder,
        nuitrack.JointType.RightElbow,
        nuitrack.JointType.RightWrist,
        // Legs (hips, knees, ankles)
        nuitrack.JointType.LeftHip,
        nuitrack.JointType.LeftKnee,
        nuitrack.JointType.LeftAnkle,
        nuitrack.JointType.RightHip,
        nuitrack.JointType.RightKnee,
        nuitrack.JointType.RightAnkle
    };

    // Minimum confidence required; if 0, falls back to the avatar's JointConfidence setting
    [SerializeField, Range(0f, 1f)]
    private float minConfidence = 0f;

    // Log frequency control to avoid spamming the console
    [SerializeField]
    private float debugLogIntervalSeconds = 0.5f;
    private float _nextLogTime;
    private bool _loggedOnce;

    void Start()
    {
        avatar = GetComponent<NuitrackAvatar>();
        // If not present, disable this handler
        if (avatar == null)
        {
            enabled = false;
            return;
        }
        // Force an immediate first log so we see output even if the object would disable this frame
        _nextLogTime = Time.time; // no delay for the first print
        _loggedOnce = false;
    }

    void Update()
    {
        if (avatar == null)
            return;

        int satisfiedCount;
        bool tracked = AreRequiredJointsTracked(out satisfiedCount);

        // Debug: how many joints meet the confidence threshold
        if (Time.time >= _nextLogTime || !_loggedOnce)
        {
            int total = requiredJoints != null ? requiredJoints.Length : 0;
            float threshold = minConfidence > 0f ? minConfidence : avatar.JointConfidence;
            Debug.Log($"AvatarHandler: {satisfiedCount}/{total} required joints >= confidence {threshold:F2} (tracked: {tracked})");
            _nextLogTime = Time.time + debugLogIntervalSeconds;
            _loggedOnce = true;
        }

        // Do not disable the avatar for now; only logging is requested
        // if (avatar.gameObject.activeSelf != tracked)
        // {
        //     avatar.gameObject.SetActive(tracked);
        // }
    }

    private bool AreRequiredJointsTracked(out int satisfiedCount)
    {
        satisfiedCount = 0;

        // Use avatar's configured confidence if local minConfidence is not set
        float threshold = minConfidence > 0f ? minConfidence : avatar.JointConfidence;

        // If there are no required joints specified, consider as tracked
        if (requiredJoints == null || requiredJoints.Length == 0)
            return true;

        for (int i = 0; i < requiredJoints.Length; i++)
        {
            var joint = avatar.GetJoint(requiredJoints[i]);
            if (joint != null && joint.Confidence >= threshold)
            {
                satisfiedCount++;
            }
            else
            {
                // Early exit: missing one of required joints
                return false;
            }
        }

        return true;
    }
}
