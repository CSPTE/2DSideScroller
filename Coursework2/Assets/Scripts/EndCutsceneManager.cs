using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using TMPro;

public class EndCutsceneManager : MonoBehaviour
{
    [Header("Required Objects")]
    public AudioSource music;
    public PlayableDirector timeline;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;
    public int defaultEnding;

    [Header("Ending 1: You kill friend")]
    public string titleText1;
    [TextArea(5, 15)]
    public string contentText1;

    [Header("Ending 2: You forgive friend")]
    public string titleText2;
    [TextArea(5, 15)]
    public string contentText2;

    [Header("Ending 3: You Leave friend")]
    public string titleText3;
    [TextArea(5, 15)]
    public string contentText3;

    [Header("Ending 4: Friend kills you")]
    public string titleText4;
    [TextArea(5, 15)]
    public string contentText4;

    [Header("Ending 5: Friend forgives you")]
    public string titleText5;
    [TextArea(5, 15)]
    public string contentText5;

    // Start is called before the first frame update
    void Start()
    {
        if (StoryEngine.endingNumber == 0) {
            StoryEngine.endingNumber = defaultEnding;
        }
        if (StoryEngine.endingNumber == 1) {
            titleText.text = titleText1;
            contentText.text = contentText1;
        } else if (StoryEngine.endingNumber == 2) {
            titleText.text = titleText2;
            contentText.text = contentText2;
        } else if (StoryEngine.endingNumber == 3) {
            titleText.text = titleText3;
            contentText.text = contentText3;
        } else if (StoryEngine.endingNumber == 4) {
            titleText.text = titleText4;
            contentText.text = contentText4;
        } else if (StoryEngine.endingNumber == 5) {
            titleText.text = titleText5;
            contentText.text = contentText5;
        } else {
            titleText.text = "Ending 0: Default text";
            contentText.text = "Something went wrong and we don't have an ending for you :( Sorry xx";
        }
        music.Play();
        timeline.Play();
    }

    public void ReturnToMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
