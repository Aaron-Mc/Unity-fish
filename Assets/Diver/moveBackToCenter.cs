using NuitrackSDK.Avatar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveBackToCenter : MonoBehaviour
{
    private NuitrackAvatar avatar;

    // Configure which joints must be tracked for the avatar to remain enabled
    [SerializeField]
    private nuitrack.JointType[] requiredJoints = new nuitrack.JointType[]
    {
        // Head
        nuitrack.JointType.Head,
        nuitrack.JointType.LeftWrist,
        nuitrack.JointType.RightWrist,
        nuitrack.JointType.LeftKnee,
        nuitrack.JointType.LeftAnkle,
        nuitrack.JointType.RightKnee,
        nuitrack.JointType.RightAnkle
    };

    // Minimum confidence required; if 0, falls back to the avatar's JointConfidence setting
    [SerializeField, Range(0f, 1f)]
    private float minConfidence = 0f;

    [Header("Hide behavior")]
    [Tooltip("Disable the NuitrackAvatar component when required joints are not tracked. This prevents the avatar script from overriding transform changes.")]
    [SerializeField]
    private bool disableAvatarComponentWhenUntracked = true;

    [Tooltip("Also disable all child Renderers when untracked (useful if other scripts still move bones).")]
    [SerializeField]
    private bool disableRenderersWhenUntracked = true;

    // Log frequency control to avoid spamming the console
    [SerializeField]
    private float debugLogIntervalSeconds = 0.5f;
    private float _nextLogTime;
    private bool _loggedOnce;

    private bool _isHidden;
    private Vector3 _lastLocalPosition;
    private Renderer[] _renderers;


    void Start()
    {
        avatar = GetComponentInParent<NuitrackAvatar>();
        // If not present, disable this handler
        if (avatar == null)
        {
            enabled = false;
            return;
        }

        _renderers = disableRenderersWhenUntracked ? avatar.GetComponentsInChildren<Renderer>(true) : null;
        _lastLocalPosition = avatar.transform.localPosition;

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

        if (!tracked)
        {
            HideAvatar();
        }
        else
        {
            ShowAvatar();
        }

        // Debug: how many joints meet the confidence threshold
        if (Time.time >= _nextLogTime || !_loggedOnce)
        {
            int total = requiredJoints != null ? requiredJoints.Length : 0;
            float threshold = minConfidence > 0f ? minConfidence : avatar.JointConfidence;
            Debug.Log($"moveBackToCenter: {satisfiedCount}/{total} required joints >= confidence {threshold:F2} (tracked: {tracked})");
            _nextLogTime = Time.time + debugLogIntervalSeconds;
            _loggedOnce = true;
        }
    }

    private void HideAvatar()
    {
        if (_isHidden)
            return;

        _isHidden = true;
        _lastLocalPosition = avatar.transform.localPosition;

        if (disableAvatarComponentWhenUntracked)
            avatar.enabled = false;

        if (disableRenderersWhenUntracked && _renderers != null)
        {
            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].enabled = false;
        }

        // As a fallback if you don't want to disable the component, move offscreen.
        if (!disableAvatarComponentWhenUntracked)
            avatar.transform.localPosition = new Vector3(0, -1000, 0);
    }

    private void ShowAvatar()
    {
        if (!_isHidden)
            return;

        _isHidden = false;

        if (disableAvatarComponentWhenUntracked)
            avatar.enabled = true;

        if (disableRenderersWhenUntracked && _renderers != null)
        {
            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].enabled = true;
        }

        // Restore previous position when re-enabling.
        avatar.transform.localPosition = _lastLocalPosition;
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
