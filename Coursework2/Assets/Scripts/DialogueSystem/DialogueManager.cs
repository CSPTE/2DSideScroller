using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager current;
    public GameObject nameText;
    public GameObject dialogueText;
    public Queue<string> sentences;
    public Animator animator;

    // Start is called before the first frame update
    void Start() {
        DialogueManager.current = this;
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue) {
        StopAllCoroutines();
        sentences.Clear();
        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }
        nameText.GetComponent<TextMeshProUGUI>().text = dialogue.name;
        DisplayNextSentence();
        animator.SetBool("IsOpen", true);

    }

    public void DisplayNextSentence() {
        StopAllCoroutines();
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence) {
        TextMeshProUGUI textComponent = dialogueText.GetComponent<TextMeshProUGUI>();
        textComponent.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            textComponent.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void EndDialogue() {
        StopAllCoroutines();
        StoryEngine.current.TriggerEvent("FinishedDialogueWith" + nameText.GetComponent<TextMeshProUGUI>().text);
        dialogueText.GetComponent<TextMeshProUGUI>().text = "";
        nameText.GetComponent<TextMeshProUGUI>().text = "";
        animator.SetBool("IsOpen", false);
    }

}