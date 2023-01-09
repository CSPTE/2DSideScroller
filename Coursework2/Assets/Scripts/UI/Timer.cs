using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float startTime = 180f;
    public TextMeshProUGUI text;
    private static float timeRemaining ;
    private bool hasFinished = false;
    private bool stopped = false;
    private static bool started = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!started) {
            timeRemaining = startTime;
            started = true;
        } else if (timeRemaining <= 0) {
            hasFinished = true;
            timeRemaining = 0;
            StoryEngine.current.TriggerEvent("TimerRanOut");
        }
    }

    public static void ResetValues() {
        started = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasFinished && !stopped) {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0) {
                hasFinished = true;
                timeRemaining = 0;
                StoryEngine.current.TriggerEvent("TimerRanOut");
            }
            string minutes = Mathf.FloorToInt(timeRemaining / 60).ToString();
            string seconds = Mathf.FloorToInt(timeRemaining % 60).ToString();
            
            if (minutes.Length == 1) {
                minutes = "0" + minutes;
            }
            if (seconds.Length == 1) {
                seconds = "0" + seconds;
            }
            text.text = minutes + ":" + seconds;
        }
    }

    public void StopTimer() {
        stopped = true;
    }
}
