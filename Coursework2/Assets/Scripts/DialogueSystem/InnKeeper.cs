using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnKeeper : NPCBaseClass
{


    public Dialogue d1Pacifist;
    public Dialogue d2Pacifist;
    public Dialogue d3Pacifist;
    public Dialogue d1Violent;
    public Dialogue d2Violent;
    public Dialogue d3Violent;
    public Dialogue final;
    private int step = 0;

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
        if (eventType == "TalkedTo" + characterName && playerPath == "Pacifist" && step == 0) {
            SetDialogue(d2Pacifist);
            step += 1;
        } else if (eventType == "TalkedTo" + characterName && playerPath == "Pacifist" && step == 1) {
            SetDialogue(d3Pacifist);
            step += 1;
        } else if (eventType == "TalkedTo" + characterName && playerPath == "Violent" && step == 0) {
            SetDialogue(d2Violent);
            step += 1;
        } else if (eventType == "TalkedTo" + characterName && playerPath == "Violent" && step == 0) {
            SetDialogue(d3Violent);
            step += 1;
        } else if (eventType == "TalkedTo" + characterName && step == 2) {
            SetDialogue(final);
        } else if (eventType == "PacifistChoice") {
            SetDialogue(d1Pacifist);
            step = 0;
        } else if (eventType == "ViolentChoice") {
            SetDialogue(d1Violent);
            step = 0;
        }
    }
}
