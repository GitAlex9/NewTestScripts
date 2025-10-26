using UnityEngine;
using System;
using Ink.Runtime;
using System.Collections.Generic;

public class DialogueEvents
{
    public event Action<string> onEnterDialogue;
    public event Action onDialogueStarted;
    public event Action onDialogueFinished;
    public event Action<string, List<Choice>> onDisplaydialogue; 
    public event Action<int> onUpdateChoiceIndex;
    public event Action<string, Ink.Runtime.Object> onUpdateInkDialogueVariable;

    public string activeDialogueLine = "";
    public List<Choice> activeDialogueChoices = new List<Choice>();

    public void EnterDialogue(string knotName)
    {
        onEnterDialogue?.Invoke(knotName);
    }

    public void DialogueStarted()
    {
        onDialogueStarted?.Invoke();
    }
    public void DialogueFinished()
    {
        onDialogueFinished?.Invoke();
    }

    public void Displaydialogue(string dialogueLine, List<Choice> dialogueChoice)
    {
        activeDialogueLine = dialogueLine;
        activeDialogueChoices = dialogueChoice;
        onDisplaydialogue?.Invoke(dialogueLine, dialogueChoice);
    }

    public void UpdateChoiceIndex(int choiceIndex)
    {
        onUpdateChoiceIndex?.Invoke(choiceIndex);
    }

    public void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value)
    {
        onUpdateInkDialogueVariable?.Invoke(name, value);
    }
}
