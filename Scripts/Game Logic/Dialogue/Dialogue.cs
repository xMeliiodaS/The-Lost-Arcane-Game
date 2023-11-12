using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{

    public new string name; // The character name
    public bool alreadyTalkedThisSen;

    public Sprite characterSprite; // The sprite for the character
    public bool isTask = false;

    [TextArea(5, 15)]
    public string[] sentences; // The dialogue itself
    public bool alreadyFinishedTask;

    public bool wantToTalk;
    public int npcNumber;
}
