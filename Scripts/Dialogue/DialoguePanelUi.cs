using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;

public class DialoguePanelUi : MonoBehaviour
{
    [SerializeField] private InterfaceGameplayManager interfaceGameplayManager;

    [Header("Components")]

    [SerializeField] private GameObject contentParent;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private DialogueChoiceButton[] choiceButtons;


    private void Awake()
    {
        //contentParent.SetActive(false);
        ResetPanel();
    }

    private void OnEnable() 
    {
        GameManager.Instance.dialogueEvents.onDialogueStarted += DialogueStarted;
        GameManager.Instance.dialogueEvents.onDialogueFinished += DialogueFinished;
        GameManager.Instance.dialogueEvents.onDisplaydialogue += DisplayDialogue; 

        DisplayDialogue(
            GameManager.Instance.dialogueEvents.activeDialogueLine,
            GameManager.Instance.dialogueEvents.activeDialogueChoices
        );
    }

    private void OnDisable() 
    {
        GameManager.Instance.dialogueEvents.onDialogueStarted -= DialogueStarted;
        GameManager.Instance.dialogueEvents.onDialogueFinished -= DialogueFinished;
        GameManager.Instance.dialogueEvents.onDisplaydialogue -= DisplayDialogue; 
    }

    private void DialogueStarted()
    {
        Debug.Log("Dialogue Started - Showing Dialogue Panel UI");
        //contentParent.SetActive(true);
    }

    private void DialogueFinished()
    {
        //contentParent.SetActive(false);
        ResetPanel();
    }

    private void DisplayDialogue(string dialogueLine, List<Choice> dialogueChoices)
    {
        Debug.Log("Displaying Dialogue Line: " + dialogueLine);
        dialogueText.text = dialogueLine;

        if (dialogueChoices.Count > choiceButtons.Length)
        {
            Debug.LogError("Tem opções demais para escolher (" + dialogueChoices.Count + "), mais do que o suportado (" + choiceButtons.Length + ").");
        }


        //iniciar com todos os botões de escolhas ocultos.
        foreach (DialogueChoiceButton choiceButton in choiceButtons)
        {
            choiceButton.gameObject.SetActive(false);
        }

        //habilita a quantidade de botões conforme informação recebida do Ink
        int choiceButtonIndex = dialogueChoices.Count -1;
        for (int inkChoiceIndex = 0; inkChoiceIndex < dialogueChoices.Count; inkChoiceIndex++)
        {
            Choice dialogueChoice = dialogueChoices[inkChoiceIndex];
            DialogueChoiceButton choiceButton = choiceButtons[choiceButtonIndex];

            choiceButton.gameObject.SetActive(true);
            choiceButton.SetChoiceText(dialogueChoice.text);
            choiceButton.SetChoiceIndex(inkChoiceIndex);

            if (inkChoiceIndex == 0)
            {
                choiceButton.SelectButton();
                GameManager.Instance.dialogueEvents.UpdateChoiceIndex(0);
            }


            choiceButtonIndex--;
        }
    }

    private void ResetPanel()
    {
        dialogueText.text = "";
    }

}
