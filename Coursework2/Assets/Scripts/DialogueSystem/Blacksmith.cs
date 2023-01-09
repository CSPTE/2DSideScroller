using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blacksmith : NPCBaseClass
{

    public Dialogue d1Pacifist;
    public Dialogue d2Pacifist;
    public Dialogue d3Pacifist;

    public Dialogue d1Violent;
    public Dialogue d2Violent;
    public Dialogue d3Violent;

    private bool talkedTo = false;

    protected override void Start() {
        base.Start();
        if (StoryEngine.current.HasOccured("PacifistChoice")) {
            SetDialogue(d1Pacifist);
            playerPath = "Pacifist";
        } else {
            SetDialogue(d1Violent);
            playerPath = "Violent";
        }
    }

    protected override void EventHappened(string eventType) {
        base.EventHappened(eventType);
        if (eventType == "PacifistChoice") {
            SetDialogue(d1Pacifist);
            playerPath = "Pacifist";
            talkedTo = false;
        } else if (eventType == "ViolentChoice") {
            SetDialogue(d1Violent);
            playerPath = "Violent";
            talkedTo = false;
        } else if (eventType == "TalkedTo" + characterName && playerPath == "Pacifist") {
            SetDialogue(d3Pacifist);
            talkedTo = true;
        } else if (eventType == "TalkedTo" + characterName && playerPath == "Violent") {
            SetDialogue(d3Violent);
            talkedTo = true;
        } else if (eventType == "TalkedToMerchant" && playerPath == "Pacifist" && !talkedTo) {
            SetDialogue(d2Pacifist);
        } else if (eventType == "TalkedToMerchant" && playerPath == "Violent" && !talkedTo) {
            SetDialogue(d2Violent);
        }
    }

}
