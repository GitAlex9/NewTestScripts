using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

//Recompensas
    private HashSet<string> unlockedRecipes = new HashSet<string>();
    public event Action<string> OnRecipeUnlocked;
//Recompensas
    
    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;

    private Dictionary<string, Quest> questMap;

    // quest start requirements
    private int currentPlayerLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        questMap = CreateQuestMap();
    }

    private void OnEnable() //No lugar de OnEnable - O motor inicializa o OnEnable Antes do Awake ou
     {
         GameManager.Instance.questEvents.onStartQuest += StartQuest;
         GameManager.Instance.questEvents.onAdvanceQuest += AdvanceQuest;
         GameManager.Instance.questEvents.onFinishQuest += FinishQuest;

         GameManager.Instance.questEvents.onQuestStepStateChange += QuestStepStateChange;

        //GameManager.Instance.ClasseACriar.MétodoACriar += PlayerLevelChange;
    }

    private void OnDisable()
    {
        GameManager.Instance.questEvents.onStartQuest -= StartQuest;
        GameManager.Instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameManager.Instance.questEvents.onFinishQuest -= FinishQuest;

        GameManager.Instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;

        //GameManager.Instance.ClasseACriar.MétodoACriar -= PlayerLevelChange;
    }

    private void Start()
    {

        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            GameManager.Instance.questEvents.QuestStateChange(quest);
        }
    }
    
    public void UpdateQuestLog()
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            GameManager.Instance.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameManager.Instance.questEvents.QuestStateChange(quest);
    }

    private void PlayerLevelChange(int level)
    {
        currentPlayerLevel = level;
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        bool meetsRequirements = true;

        if (currentPlayerLevel < quest.info.levelRequirement)
        {
            meetsRequirements = false;
        }

        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
                break;
            }
        }

        return meetsRequirements;
    }

    private void Update()
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }

        if (Input.GetKeyDown(KeyCode.F)) // Aperta F para forçar a finalização da missão
        {
            //GameManager.Instance.questEvents.FinishQuest("nome_da_sua_missao"); // Substitui pelo ID da missão
        }
    }

    private void StartQuest(string id) 
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

        quest.MoveToNextStep();

        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id)
    {
        Debug.Log("FinishQuest chamado para: " + id);
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);

        if (quest.info.recipeToUnlock != null)
        {
            CraftingManager.Instance.UnlockRecipe(quest.info.recipeToUnlock.recipeID);
            Debug.Log("Receita desbloqueada pela missão: " + quest.info.recipeToUnlock.recipeName);
        }
        else
        {
            Debug.Log("Nenhuma receita para desbloquear nesta missão.");
        }

        ChangeQuestState(quest.info.id, QuestState.FINISHED);
    }

//Recompensas
    public void UnlockRecipe(string recipeID)
    {
        if (!unlockedRecipes.Contains(recipeID))
        {
            unlockedRecipes.Add(recipeID);
            Debug.Log($"[CraftingManager] Receita desbloqueada: {recipeID}");
            OnRecipeUnlocked?.Invoke(recipeID);
        }
        else
        {
            Debug.Log($"[CraftingManager] Receita '{recipeID}' já estava desbloqueada.");
        }
    }
    
    public bool IsRecipeUnlocked(string recipeID)
    {
        return unlockedRecipes.Contains(recipeID);
    }
//Recompensas

    private void ClaimRewards(Quest quest)
    {
        //GameManager.Instance.ClasseACriar.MétodoACriar(quest.info.contributionPoints);
        //GameManager.Instance.ClasseACriar.MétodoACriar(quest.info.experienceReward);
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        Debug.Log("=== CREATING QUEST MAP ===");
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        Debug.Log($"Found {allQuests.Length} quest(s) in Resources/Quests folder");
        
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
        }
        Debug.Log($"Quest map created with {idToQuestMap.Count} quest(s)");
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
        }
        return quest;
    }
    
    public System.Collections.Generic.IEnumerable<string> GetAllQuestIds()
    {
        return questMap.Keys;
    }

    public void SaveAllQuests()
    {
        Debug.Log("=== SAVING ALL QUESTS ===");
        int savedCount = 0;
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
            savedCount++;
        }
        Debug.Log($"Total quests saved: {savedCount}");
    }

    private void SaveQuest(Quest quest)
    {
        try 
        {
            QuestData questData = quest.GetQuestData();
            string serializedData = JsonUtility.ToJson(questData);
            PlayerPrefs.SetString(quest.info.id, serializedData);
            Debug.Log($"[SAVE] Quest '{quest.info.id}' saved - State: {questData.state}, Step: {questData.questStepIndex}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save quest with id " + quest.info.id + ": " + e);
        }
    }

    private Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;
        try 
        {
            if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
                Debug.Log($"[LOAD] Quest '{questInfo.id}' loaded from save - State: {questData.state}, Step: {questData.questStepIndex}");
            }
            else 
            {
                quest = new Quest(questInfo);
                if (!PlayerPrefs.HasKey(questInfo.id))
                {
                    Debug.Log($"[LOAD] Quest '{questInfo.id}' - No save data found, creating new quest");
                }
                else if (!loadQuestState)
                {
                    Debug.Log($"[LOAD] Quest '{questInfo.id}' - loadQuestState is disabled, creating new quest");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest with id " + questInfo.id + ": " + e);
            quest = new Quest(questInfo);
        }
        return quest;
    }
}
