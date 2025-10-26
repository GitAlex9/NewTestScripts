using UnityEngine;
using Ink.Runtime;
using System;
using System.Collections;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    [Header("Ink Sotry")]
    [SerializeField] private TextAsset inkJson;

    private Story story;
    private bool dialoguePlaying = false;
    private int currentChoiceIndex = -1;
    private InkExternalFunctions inkExternalFunctions;
    private InkDialogueVariables inkDialogueVariables;

    private void Awake()
    {
        story = new Story(inkJson.text);
        inkExternalFunctions = new InkExternalFunctions();
        inkExternalFunctions.Bind(story);
        inkDialogueVariables = new InkDialogueVariables(story);
    }

    private void OnDestroy()
    {
        inkExternalFunctions.Unbind(story);
    }

    private void OnEnable()
    {
        GameManager.Instance.dialogueEvents.onEnterDialogue += EnterDialogue;
        GameManager.Instance.OnInteractPress += SubmitPressed;
        GameManager.Instance.dialogueEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
        GameManager.Instance.dialogueEvents.onUpdateInkDialogueVariable += UpdateInkDialogueVariable;
        GameManager.Instance.questEvents.onQuestStateChange += QuestStateChange;
    }
    private void OnDisable()
    {
        GameManager.Instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
        GameManager.Instance.OnInteractPress -= SubmitPressed;
        GameManager.Instance.dialogueEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
        GameManager.Instance.dialogueEvents.onUpdateInkDialogueVariable -= UpdateInkDialogueVariable;
        GameManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void QuestStateChange(Quest quest)
    {
        GameManager.Instance.dialogueEvents.UpdateInkDialogueVariable(
            quest.info.id + "State",
            new StringValue(quest.state.ToString())
        );
    }

    private void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value)
    {
        inkDialogueVariables.UpdateVariableState(name, value);
    }

    private void UpdateChoiceIndex(int choiceIndex)
    {
        this.currentChoiceIndex = choiceIndex;
    }

    private void SubmitPressed(InputEventContext inputEventContext)
    {
        if(!inputEventContext.Equals(InputEventContext.DIALOGUE))
        {
            return;
        }
        ContinueOrExitStory();
    }
    private void EnterDialogue(string knotName)
    {

       if (dialoguePlaying)
       {
            return;
       }
       
        dialoguePlaying = true;

        //informar que o evento de entrar em um diálogo aconteceu.
        GameManager.Instance.dialogueEvents.DialogueStarted();

        //Congelar a movimentação do player.
        GameManager.Instance.DialogueOn();
        //Mudar o Contexto do evento.
        GameManager.Instance.ChangeInputEventContext(InputEventContext.DIALOGUE);

        if (!knotName.Equals(""))
        {
            story.ChoosePathString(knotName);
        }
        else
        {
            Debug.LogWarning("Nome do nó está vazio ao entrar no diálogo");
        }

        inkDialogueVariables.SyncVariablesAndStartListening(story);

        ContinueOrExitStory();
    }

    private void ContinueOrExitStory()
    {
        //faz uma escolha, se for aplicável
        if (story.currentChoices.Count > 0 && currentChoiceIndex != -1)
        {
            story.ChooseChoiceIndex(currentChoiceIndex);
            //Reinicia o index para a próxima vez
            currentChoiceIndex = -1;
        }     
                
        if (story.canContinue)
        {
            string dialogueLine = story.Continue();

            while (IsLineBlank(dialogueLine) && story.canContinue)
            {
                dialogueLine = story.Continue();
            }

            if (IsLineBlank(dialogueLine) && !story.canContinue)
            {
                ExitDialogue();
            }
            else
            {
                GameManager.Instance.dialogueEvents.Displaydialogue(dialogueLine, story.currentChoices);    
            }


        }
        else if (story.currentChoices.Count == 0)
        {
            ExitDialogue();
        }
    }

    private void ExitDialogue()
    {
        dialoguePlaying = false;

        GameManager.Instance.dialogueEvents.DialogueFinished();

        //descongelar a movimentação do player
        GameManager.Instance.DialogueOff();
        //voltar com o contexto Default.
        GameManager.Instance.ChangeInputEventContext(InputEventContext.DEFAULT);

        inkDialogueVariables.StopListening(story);

        story.ResetState();
    }

    private bool IsLineBlank(string dialogueLine)
    {
        return dialogueLine.Trim().Equals("") || dialogueLine.Trim().Equals("\n");
    }
}
