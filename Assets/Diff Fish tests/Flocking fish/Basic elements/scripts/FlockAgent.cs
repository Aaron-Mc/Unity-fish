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
        transform.up = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;
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