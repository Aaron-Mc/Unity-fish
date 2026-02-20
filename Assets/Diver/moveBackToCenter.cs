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

    [Header("Tracking threshold")]
    [Tooltip("If true, ALL required joints must meet the confidence threshold for the avatar to be considered tracked.")]
    [SerializeField]
    private bool requireAllRequiredJoints = true;

    [Header("Grace seconds by tracked joint count")]
    [Tooltip("Grace (seconds) before hiding when only N required joints are currently tracked. Keys are tracked-counts (0..requiredJoints.Length-1).")]
    [SerializeField]
    private TrackedCountGraceEntry[] graceByTrackedCount = new TrackedCountGraceEntry[]
    {
        // Example for 7 required joints: 6->1, 5->0.3, 4->0.15, everything else ->0
        new TrackedCountGraceEntry { trackedCount = 6, graceSeconds = 1f },
        new TrackedCountGraceEntry { trackedCount = 5, graceSeconds = 0.3f },
        new TrackedCountGraceEntry { trackedCount = 4, graceSeconds = 0.15f },
        new TrackedCountGraceEntry { trackedCount = 3, graceSeconds = 0f },
        new TrackedCountGraceEntry { trackedCount = 2, graceSeconds = 0f },
        new TrackedCountGraceEntry { trackedCount = 1, graceSeconds = 0f },
        new TrackedCountGraceEntry { trackedCount = 0, graceSeconds = 0f },
    };

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

    private float _requirementsUnmetSince = -1f;

    [Serializable]
    private struct TrackedCountGraceEntry
    {
        [Min(0)] public int trackedCount;
        [Min(0f)] public float graceSeconds;
    }

    private Dictionary<int, float> _graceByTrackedCount;

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

        RebuildGraceLookup();

        // Force an immediate first log so we see output even if the object would disable this frame
        _nextLogTime = Time.time; // no delay for the first print
        _loggedOnce = false;
    }

    void OnValidate()
    {
        // Keep runtime lookup consistent in-editor.
        RebuildGraceLookup();
    }

    void Update()
    {
        if (avatar == null)
            return;

        int satisfiedCount;
        int total;
        bool meetsRequirements = AreTrackingRequirementsMet(out satisfiedCount, out total);

        // When requirements are not met, grace depends on how many are currently tracked.
        float graceSeconds = meetsRequirements ? 0f : GetGraceSecondsForTrackedCount(satisfiedCount, total);

        if (!meetsRequirements)
        {
            if (_requirementsUnmetSince < 0f)
                _requirementsUnmetSince = Time.time;

            if (graceSeconds <= 0f || (Time.time - _requirementsUnmetSince) >= graceSeconds)
                HideAvatar();
        }
        else
        {
            _requirementsUnmetSince = -1f;
            ShowAvatar();
        }

        // Debug: how many joints meet the confidence threshold
        if (Time.time >= _nextLogTime || !_loggedOnce)
        {
            float threshold = minConfidence > 0f ? minConfidence : avatar.JointConfidence;
            Debug.Log($"moveBackToCenter: {satisfiedCount}/{total} required joints >= confidence {threshold:F2} (requireAll: {requireAllRequiredJoints}, grace: {graceSeconds:F2}s, tracked: {meetsRequirements})");
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

    private bool AreTrackingRequirementsMet(out int satisfiedCount, out int total)
    {
        satisfiedCount = 0;

        // Use avatar's configured confidence if local minConfidence is not set
        float threshold = minConfidence > 0f ? minConfidence : avatar.JointConfidence;

        // If there are no required joints specified, consider as tracked
        if (requiredJoints == null || requiredJoints.Length == 0)
        {
            total = 0;
            return true;
        }

        total = requiredJoints.Length;

        for (int i = 0; i < requiredJoints.Length; i++)
        {
            var joint = avatar.GetJoint(requiredJoints[i]);
            if (joint != null && joint.Confidence >= threshold)
                satisfiedCount++;
        }

        if (requireAllRequiredJoints)
            return satisfiedCount >= total;

        // Backward compatible: if not requiring all, then require at least 1.
        return satisfiedCount > 0;
    }

    private void RebuildGraceLookup()
    {
        if (_graceByTrackedCount == null)
            _graceByTrackedCount = new Dictionary<int, float>();
        else
            _graceByTrackedCount.Clear();

        if (graceByTrackedCount == null)
            return;

        for (int i = 0; i < graceByTrackedCount.Length; i++)
        {
            int key = graceByTrackedCount[i].trackedCount;
            float value = graceByTrackedCount[i].graceSeconds;

            if (key < 0)
                continue;

            // last entry wins
            _graceByTrackedCount[key] = Mathf.Max(0f, value);
        }
    }

    private float GetGraceSecondsForTrackedCount(int trackedCount, int totalRequired)
    {
        if (totalRequired <= 0)
            return 0f;

        // Expected keys: 0..totalRequired-1; but be tolerant.
        int clamped = Mathf.Clamp(trackedCount, 0, totalRequired - 1);

        if (_graceByTrackedCount != null && _graceByTrackedCount.TryGetValue(clamped, out float grace))
            return grace;

        // Default: everything else 0
        return 0f;
    }
}
