using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && StoryEngine.current.HasOccured("PacifistChoice")) {
            Player.current.SetHiding(true);
            InteractIcon.current.Bind(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Player.current.SetHiding(false);
            InteractIcon.current.UnBind();
        }
    }
}
