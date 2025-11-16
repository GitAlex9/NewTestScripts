using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private InterfaceGameplayManager interfaceGameplayManager;

    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private QuestLogScrollingList scrollingList;
    [SerializeField] private TextMeshProUGUI questDisplayNameText;
    [SerializeField] private TextMeshProUGUI questStatusText;
    [SerializeField] private TextMeshProUGUI contributionPointsRewardsText;
    [SerializeField] private TextMeshProUGUI experienceRewardsText;
    [SerializeField] private TextMeshProUGUI levelRequirementsText;
    [SerializeField] private TextMeshProUGUI questRequirementsText;

    private Button firstSelectedButton;
    private void Start()
    {
        StartCoroutine(UpdateQuestLog());
    }

    public Button FirstSelectedButton => firstSelectedButton;

    private void OnEnable()
    {
        GameManager.Instance.questEvents.onQuestStateChange += QuestStateChange;
        // GameManager.Instance.OnQuestLogPressed += QuestLogTogglePressed;
    }
    
    IEnumerator UpdateQuestLog()
    {
        yield return new WaitForSeconds(0.2f);
        QuestManager.Instance.UpdateQuestLog();
    }

    private void OnDisable()
    {
        GameManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
        // GameManager.Instance.OnQuestLogPressed -= QuestLogTogglePressed;
    }

    /*
    private void QuestLogTogglePressed()
    {
        if (contentParent.activeInHierarchy)
        {
            HideUI();
        }
        else
        {
            ShowUI();
        }
    }

    private void ShowUI()
    {
        if (firstSelectedButton != null)
        {
            firstSelectedButton.Select();
        }
    }
    private void HideUI()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
    */

    private void QuestStateChange(Quest quest)
    {
        QuestLogButton questLogButton = scrollingList.CreateButtonIfNotExists(quest, () => {
            SetQuestLogInfo(quest);
        });

        if (firstSelectedButton == null)
        {
            firstSelectedButton = questLogButton.button;
        }
        
        questLogButton.SetState(quest.state);
    }

    // NENHUMA MUDANÇA AQUI.
    private void SetQuestLogInfo(Quest quest)
    {
        questDisplayNameText.text = quest.info.displayName;
        questStatusText.text = quest.GetFullStatusText();
        levelRequirementsText.text = "Level " + quest.info.levelRequirement;
        questRequirementsText.text = "";
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            questRequirementsText.text += prerequisiteQuestInfo.displayName + "\n";
        }
        experienceRewardsText.text = quest.info.experienceReward + " XP";
    }
}