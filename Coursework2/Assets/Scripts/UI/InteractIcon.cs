using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractIcon : MonoBehaviour
{
    public static InteractIcon current;
    public Animator animator;

    private void Awake() {
        current = this;
    }

    public void Bind(Transform o) {
        transform.position = new Vector3(o.position.x, o.position.y, transform.position.z);
        animator.SetBool("Bound", true);
    }

    public void UnBind() {
        animator.SetBool("Bound", false);
    }
}
