using UnityEngine;
using System.Collections;

public class SetAnimationSpeed : MonoBehaviour 
{
    public Animator animator;           //The target animatior
    public float animatorSpeed;         //The speed of the animation [0..1]

	// Use this for initialization
	void Start ()
    {
        animator.speed = animatorSpeed;
	}
}
