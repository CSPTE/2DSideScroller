using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class StoryEngine : MonoBehaviour
{

    public static StoryEngine current;
    public static int endingNumber;
    public event Action<String> EventOccured;
    private static List<String> OccuredEvents = new();
    private List<String> OccuredEventsSinceCheckpoint;

    private void Awake() {
        StoryEngine.current = this;
        OccuredEventsSinceCheckpoint = new List<String>();
    }

    public void TriggerEvent(String eventType) {
        // If event hasn't already occured, add it to the list of events
        if (!OccuredEvents.Contains(eventType) && !OccuredEventsSinceCheckpoint.Contains(eventType)) {
            OccuredEventsSinceCheckpoint.Add(eventType);
        }
        // Trigger the event, so that any listening objects are notified that the event occured
        if (EventOccured != null) {
            EventOccured(eventType);
        }
    }

    public bool HasOccured(String eventType) {
        return (OccuredEvents.Contains(eventType) || OccuredEventsSinceCheckpoint.Contains(eventType));
    }

    public void RestartFromCheckpoint() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetValues() {
        OccuredEvents = new List<String>();
        OccuredEventsSinceCheckpoint = new List<String>();
    }

    public void CheckPoint() {
        foreach (String e in OccuredEventsSinceCheckpoint) {
            if (!OccuredEvents.Contains(e)) {
                OccuredEvents.Add(e);
            }
        }
        OccuredEventsSinceCheckpoint = new List<String>();
    }

    public void TriggerEnding(int endingNum) {
        Debug.Log("Ending: " + endingNum);
        endingNumber = endingNum;
        SceneManager.LoadScene("Ending");
    }
}
