using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLevelCollider : MonoBehaviour
{
    public string nextOrPrevious = "Next";

    private void OnTriggerEnter2D(Collider2D collision) {
        if (nextOrPrevious == "Next") {
            SceneController.current.LoadNext();
        } else if (nextOrPrevious == "Previous") {
            SceneController.current.LoadPrev();
        } else {
            Debug.Log("You fucked up");
        }
    }
}
