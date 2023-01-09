using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;
    [TextArea(3, 10)]
    public string[] sentences;

    public Dialogue(string[] sentences, string name) {
        this.sentences = sentences;
        this.name = name;
    }


}