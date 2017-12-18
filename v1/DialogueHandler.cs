using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHandler : MonoBehaviour {

    private static Queue<Sentence> sentences = new Queue<Sentence>();
    public static string currentSentence;
    public static Sentence currentFullSentence;
    private static int currentSubString = 0;

    public float letterDelay;
    public List<Dialogue> dialogues = new List<Dialogue>();

    public static DialogueHandler dialogueHandler;

	// Use this for initialization
	void Start () {
        dialogueHandler = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void startConversation(string dialogueToStart)
    {
        currentSubString = 0;

        var newDialogue = findDialogue(dialogueToStart);
        if (newDialogue == null)
        {
            Debug.LogError("Bad dialogue: " + dialogueToStart);
            return;
        }

        sentences.Clear();

        foreach (Sentence sentence in newDialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        displayNextSentence();
    }

    public static void displayNextSentence()
    {
        if (sentences.Count == 0)
        {
            endDialogue();
            return;
        }
        
        dialogueHandler.StopCoroutine("slowlyDisplaySentence");
        currentSubString = 0;

        Sentence newSentence = sentences.Dequeue();

        currentFullSentence = newSentence;
        dialogueHandler.StartCoroutine("slowlyDisplaySentence");
    }

    IEnumerator slowlyDisplaySentence()
    {
        while (currentSubString <= currentFullSentence.text.Length)
        {
            currentSentence = currentFullSentence.text.Substring(0, currentSubString);
            currentSubString++;
            yield return new WaitForSeconds(1/letterDelay);
        }
    }

    static void endDialogue()
    {
        GUIHandler.uiState = GUIHandler.UIState.exploring;
    }

    public static Dialogue findDialogue(string dialogueName)
    {
        foreach (Dialogue dialogue in dialogueHandler.dialogues)
        {
            if (dialogue.name == dialogueName) return dialogue;
        }

        return null;
    }
}
