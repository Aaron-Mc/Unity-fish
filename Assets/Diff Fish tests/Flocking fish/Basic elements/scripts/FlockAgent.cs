using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// hads to have some form of collider, generally a mesh collieder
[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{

    Flock agentFlock;
    public Flock AgentFlock { get { return agentFlock; } }

    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }

    [Header("Steering")]
    [Tooltip("How quickly the agent rotates to face its movement direction (degrees/sec).")]
    [SerializeField] float turnSpeed = 720f;

    [Tooltip("If the velocity magnitude is below this threshold, rotation will not be updated (prevents flicker).")]
    [SerializeField] float minVelocityToRotate = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();

        // Randomize animation start offset if an Animator or legacy Animation is present
        RandomizeAnimationStart();
    }

    public void Initialize(Flock flock)
    {
        agentFlock = flock;
    }

    public void Move(Vector3 velocity)
    {
        // If the velocity is too small, don't update rotation this frame.
        // Snapping orientation to very small vectors can cause rapid flip/flop ("flicker").
        if (velocity.sqrMagnitude >= (minVelocityToRotate * minVelocityToRotate))
        {
            // Preserve your existing convention that the fish's "up" points along its movement direction.
            Vector3 desiredUp = velocity.normalized;

            // Smoothly rotate to avoid jitter when direction changes quickly.
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, desiredUp) * transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        transform.position += velocity * Time.deltaTime;
    }

    // Randomizes the playback start time for continuous animations to avoid synchronized motion.
    void RandomizeAnimationStart()
    {
        // Animator (Mecanim)
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            // Ensure the animator has updated once to have a valid state
            animator.Update(0f);
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // Play current state from a random normalized time [0,1]
            animator.Play(stateInfo.fullPathHash, 0, Random.value);
            // Apply immediately
            animator.Update(0f);
        }

        // Legacy Animation component
        var legacyAnimation = GetComponent<Animation>();
        if (legacyAnimation != null)
        {
            // If multiple clips, offset each state; then play default
            foreach (AnimationState s in legacyAnimation)
            {
                if (s.length > 0f)
                {
                    s.time = Random.Range(0f, s.length);
                }
            }
            if (legacyAnimation.clip != null)
            {
                legacyAnimation.Play(legacyAnimation.clip.name);
            }
            else
            {
                legacyAnimation.Play();
            }
        }
    }
}