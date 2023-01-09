using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCBaseClass : MonoBehaviour
{
    public AudioSource greetingAudio;

    public String characterName;


    private Dialogue currentDialogue;
    private bool canInteract = false;
    private bool facingRight = true;

    private Transform player;

    protected string playerPath = "Pacifist";

    protected virtual void Start() {
        StoryEngine.current.EventOccured += this.EventHappened;
        // Check which events have occured and set correct dialogue accordingly.
        string[] sentences = { "sentence 1", "sentence 2" };
        SetDialogue(new Dialogue(sentences, "default"));
        player = FindObjectOfType<Player>().transform;
    }

    public virtual void TriggerDialogue() {
        DialogueManager.current.StartDialogue(currentDialogue);
        greetingAudio.Play();
        StoryEngine.current.TriggerEvent("TalkedTo" + characterName);
    }

    protected virtual void EventHappened(String eventType) {
        // Implement logic for figuring out which dialogue to present next based on current state and the event which occured
        if (eventType == "PacifistChoice") {
            playerPath = "Pacifist";
        } else if (eventType == "ViolentChoice") {
            playerPath = "Violent";
        }
    }

    protected void SetDialogue(Dialogue dialogue) {
        currentDialogue = dialogue;
    }

    private void Update() {
        if (canInteract && Input.GetButtonDown("Interact")) {
            TriggerDialogue();
        }
        if (player.position.x < transform.position.x) {
            FaceLeft();
        } else {
            FaceRight();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            canInteract = true;
            InteractIcon.current.Bind(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            canInteract = false;
            InteractIcon.current.UnBind();
        }
    }

    public void Flip() {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void FaceLeft() {
        if (facingRight) {
            Flip();
        }
    }

    public void FaceRight() {
        if (!facingRight) {
            Flip();
        }
    }
    private void OnDestroy() {
        StoryEngine.current.EventOccured -= EventHappened;
    }
}