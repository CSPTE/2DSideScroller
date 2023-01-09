using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friend : NPCBaseClass
{

    public Dialogue findFriendInLab;
    public Dialogue onReleaseOutOfTime;
    public Dialogue onReleaseWithTimeLeft;
    public Dialogue leaveFriend;

    public Dialogue findFriendInForestRageFull;
    public Dialogue findFriendInForestRageNotFull;

    private int state = 0;
    // 0 means haven't spoken yet
    // 1 means trigger ending 1 next time you finish speaking: you kill friend
    // 2 means trigger ending 2 next time you finish speaking: you forgive friend
    // 3 means trigger ending 3 next time you finish speaking: you leave friend
    // 4 means trigger ending 4 next time you finish speaking: friend kills you
    // 5 means trigger ending 5 next time you finish speaking: friend forgives you

    protected override void Start() {
        base.Start();
        if (StoryEngine.current.HasOccured("PacifistChoice")) {
            SetDialogue(findFriendInLab);
        } else if (StoryEngine.current.HasOccured("ViolentChoice") && StoryEngine.current.HasOccured("RageBarFilled")) {
            SetDialogue(findFriendInForestRageFull);
            state = 1;
        } else if (StoryEngine.current.HasOccured("ViolentChoice")) {
            SetDialogue(findFriendInForestRageNotFull);
            state = 2;
        }
    }

    public override void TriggerDialogue() {
        base.TriggerDialogue();
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null) {
            timer.StopTimer();
        }
    }

    protected override void EventHappened(string eventType) {
        base.EventHappened(eventType);
        if (eventType == "FriendReleased" && StoryEngine.current.HasOccured("TimerRanOut")) {
            SetDialogue(onReleaseOutOfTime);
            StoryEngine.current.TriggerEvent("DeactivateReleaseOptions");
            state = 4;
        } else if (eventType == "FinishedDialogueWithFriend") {
            if (state == 0) {
                StoryEngine.current.TriggerEvent("ActivateReleaseOptions");
            } else {
                StoryEngine.current.TriggerEnding(state);
            }
        } else if (eventType == "FriendReleased") {
            SetDialogue(onReleaseWithTimeLeft);
            StoryEngine.current.TriggerEvent("DeactivateReleaseOptions");
            state = 5;
        } else if (eventType == "FriendLeft") {
            StoryEngine.current.TriggerEvent("DeactivateReleaseOptions");
            SetDialogue(leaveFriend);
            state = 3;
        }
    }

}
