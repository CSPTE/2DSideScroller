using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CinematicManager : MonoBehaviour
{
    public PlayableDirector timeline1;
    public PlayableDirector timelineOnDefendFriend;
    public PlayableDirector timelineOnHide;

    public string SceneToLoadForDefend;
    private bool defending = false;

    public string SceneToLoadForHide;
    private bool hiding = false;


    public void Start() {
        timeline1.time = 0;
        timelineOnDefendFriend.time = 0;
        timelineOnHide.time = 0;
        timeline1.Play();
    }
    public void Defend() {
        timelineOnDefendFriend.Play();
        StoryEngine.current.TriggerEvent("ViolentChoice");
        StoryEngine.current.CheckPoint();
        defending = true;
    }

    public void Hide() {
        timelineOnHide.Play();
        StoryEngine.current.TriggerEvent("PacifistChoice");
        StoryEngine.current.CheckPoint();
        hiding = true;
    }

    public void Next() {

        if (hiding) {
            SceneManager.LoadScene(SceneToLoadForHide);
        } else if (defending) {
            SceneManager.LoadScene(SceneToLoadForDefend);
        }
    }

    public void Skip() {
        if (timeline1.state == PlayState.Playing) {
            timeline1.time = 70f;
        }
    }
}
