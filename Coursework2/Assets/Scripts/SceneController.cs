using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    public bool mainMenu = false;
    public static SceneController current;
    public GameObject DeathMenu;
    public GameObject PauseMenu;
    public GameObject PacifistTutorialPrefab;
    public GameObject ViolentTutorialPrefab;
    public GameObject FriendPrefab;
    public GameObject releaseOptions;

    public Transform PacifistStartPoint;
    public Transform ViolentStartPoint;
    public string nextScene;
    public string previousScene;

    private bool paused = false;
    public static string sideToEnter = "None";

    private void Awake() {
        current = this;
    }

    void EventHandler(string eventType) {
        if (!mainMenu) {
            if (eventType == "PacifistChoice") {
                Player.current.SetPosition(PacifistStartPoint);
                if (SceneManager.GetActiveScene().name == "ForestScene") {
                    Instantiate(PacifistTutorialPrefab);
                }
            } else if (eventType == "ViolentChoice") {
                Player.current.SetPosition(ViolentStartPoint);
                if (SceneManager.GetActiveScene().name == "CastleScene") {
                    Instantiate(ViolentTutorialPrefab);
                }
            }
        }
        
        if (eventType == "ActivateReleaseOptions" && releaseOptions != null) {
            releaseOptions.SetActive(true);
        } else if (eventType == "DeactivateReleaseOptions" && releaseOptions != null) {
            releaseOptions.SetActive(false);
        }
    }

    private void Start() {
        if (!mainMenu) {

            DeathMenu.SetActive(false);
            PauseMenu.SetActive(false);
            if (StoryEngine.current.HasOccured("PacifistChoice") || sideToEnter == "Left") {
                Player.current.SetPosition(PacifistStartPoint);
                if (SceneManager.GetActiveScene().name == "ForestScene") {
                    Instantiate(PacifistTutorialPrefab);
                } else if (SceneManager.GetActiveScene().name == "CastleScene" && StoryEngine.current.HasOccured("PacifistChoice")) {
                    Instantiate(FriendPrefab, GameObject.Find("FriendSpawnPoint").transform);
                }
            } else if (StoryEngine.current.HasOccured("ViolentChoice") || sideToEnter == "Right") {
                Player.current.SetPosition(ViolentStartPoint);
                if (SceneManager.GetActiveScene().name == "CastleScene") {
                    Instantiate(ViolentTutorialPrefab);
                } else if (SceneManager.GetActiveScene().name == "ForestScene" && StoryEngine.current.HasOccured("ViolentChoice")) {
                    Instantiate(FriendPrefab, GameObject.Find("FriendSpawnPoint").transform);
                }
            }
            StoryEngine.current.EventOccured += EventHandler;
        } else {
            // Restart all static variables to reset game
            StoryEngine.current.ResetValues();
            Player.ResetValues();
            Timer.ResetValues();
        }

        releaseOptions = GameObject.Find("ReleaseFriendOptions");
        if (releaseOptions != null) {
            releaseOptions.SetActive(false);
        }
    }

    private void Update() {
        if (!mainMenu) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (paused) {
                    UnPause();
                } else {
                    Pause();
                }
            }
        }
    }

    public void Pause() {
        paused = true;
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
    }

    public void UnPause() {
        paused = false;
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
    }

    public void Restart() {
        StoryEngine.current.RestartFromCheckpoint();
    }

    public void PlayerDied() {
        DeathMenu.SetActive(true);
    }

    public void MainMenu() {
        UnPause();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadNext() {
        StoryEngine.current.CheckPoint();
        sideToEnter = "Left";
        SceneManager.LoadScene(nextScene);
    }

    public void LoadPrev() {
        StoryEngine.current.CheckPoint();
        sideToEnter = "Right";
        SceneManager.LoadScene(previousScene);
    }

    public void ExitGame() {
        Debug.Log("Quitting");
        Application.Quit();
    }

    private void OnDestroy() {
        StoryEngine.current.EventOccured -= EventHandler;
    }
}
