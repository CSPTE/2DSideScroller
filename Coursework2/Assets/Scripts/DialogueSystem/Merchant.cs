using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : NPCBaseClass
{

    public Dialogue d1Pacifist;
    public Dialogue d2Pacifist;
    public Dialogue d3Pacifist;

    public Dialogue d1Violent;
    public Dialogue d2Violent;
    public Dialogue d3Violent;

    private bool spokeAtLeastOnce = false;
    private bool talkedToBlackSmith = false;
    private bool onFinal = false;

    protected override void Start() {
        base.Start();
        if (StoryEngine.current.HasOccured("PacifistChoice")) {
            SetDialogue(d1Pacifist);
            playerPath = "Pacifist";
            spokeAtLeastOnce = false;
            talkedToBlackSmith = false;
            onFinal = false;
        } else {
            SetDialogue(d1Violent);
            playerPath = "Violent";
            spokeAtLeastOnce = false;
            talkedToBlackSmith = false;
            onFinal = false;
        }
    }

    protected override void EventHappened(string eventType) {
        base.EventHappened(eventType);
        if (eventType == "PacifistChoice") {
            SetDialogue(d1Pacifist);
            playerPath = "Pacifist";
            spokeAtLeastOnce = false;
            talkedToBlackSmith = false;
            onFinal = false;
        } else if (eventType == "ViolentChoice") {
            SetDialogue(d1Violent);
            playerPath = "Violent";
            spokeAtLeastOnce = false;
            talkedToBlackSmith = false;
            onFinal = false;
        } else if (eventType == "TalkedToBlacksmith" && !onFinal) {
            talkedToBlackSmith = true;
            if (spokeAtLeastOnce && playerPath == "Pacifist") {
                SetDialogue(d2Pacifist);
            } else if (spokeAtLeastOnce && playerPath == "Violent") {
                SetDialogue(d2Violent);
            }
        } else if (eventType == "TalkedTo" + characterName && playerPath == "Pacifist") {
            spokeAtLeastOnce = true;
            SetDialogue(d3Pacifist);
            if (talkedToBlackSmith) {
                onFinal = true;
            }
        } else if (eventType == "TalkedTo" + characterName && playerPath == "Violent") {
            spokeAtLeastOnce = true;
            SetDialogue(d3Violent);
            if (talkedToBlackSmith) {
                onFinal = true;
            }
        }
    }
}
