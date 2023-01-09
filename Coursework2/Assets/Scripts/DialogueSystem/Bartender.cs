using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bartender : NPCBaseClass
{
    public Dialogue d1Pacifist;
    public Dialogue d2Pacifist;
    public Dialogue d3Pacifist;

    public Dialogue d1Violent;
    public Dialogue d2Violent;
    public Dialogue d3Violent;

    private bool spokeAtLeastOnce = false;
    private bool onFinal= false;
    private bool hiddenOrKilled = false;

    protected override void Start() {
        base.Start();
        if (StoryEngine.current.HasOccured("PacifistChoice")) {
            SetDialogue(d1Pacifist);
            playerPath = "Pacifist";
            onFinal = false;
            hiddenOrKilled = false;
        } else {
            SetDialogue(d1Violent);
            playerPath = "Violent";
            onFinal = false;
            hiddenOrKilled = false;
        }
        if (StoryEngine.current.HasOccured("EnemyKilled") || StoryEngine.current.HasOccured("Hidden")) {
            hiddenOrKilled = true;
        }
    }

    protected override void EventHappened(string eventType) {
        base.EventHappened(eventType);
        if (eventType == "PacifistChoice") {
            SetDialogue(d1Pacifist);
            playerPath = "Pacifist";
            onFinal = false;
            hiddenOrKilled = false;
        } else if (eventType == "ViolentChoice") {
            SetDialogue(d1Violent);
            playerPath = "Violent";
            onFinal = false;
            hiddenOrKilled = false;
        } else if (eventType == "Hidden" && playerPath == "Pacifist" && !onFinal) {
            hiddenOrKilled = true;
            if (spokeAtLeastOnce) {
                SetDialogue(d2Pacifist);
            }
        } else if (eventType == "EnemyKilled" && playerPath == "Violent" && !onFinal) {
            hiddenOrKilled = true;
            if (spokeAtLeastOnce) {
                SetDialogue(d2Violent);
            }
        } else if (eventType == "TalkedTo" + characterName && playerPath == "Pacifist") {
            spokeAtLeastOnce = true;
            if (hiddenOrKilled && !onFinal) {
                SetDialogue(d2Pacifist);
            } else {
                SetDialogue(d3Pacifist);
            }
            if (hiddenOrKilled) {
                onFinal = true;
            }
        } else if (eventType == "TalkedTo" + characterName && playerPath == "Violent") {
            spokeAtLeastOnce = true;
            if (hiddenOrKilled && !onFinal) {
                SetDialogue(d2Violent);
            } else {
                SetDialogue(d3Violent);
            }
            if (hiddenOrKilled) {
                onFinal = true;
            }
        }
    }
}
