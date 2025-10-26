using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quest/QuestDatabase")]
public class QuestDatabase : ScriptableObject
{
    [SerializeField] public List<QuestInfoSO> quests = new List<QuestInfoSO>();
    
    private Dictionary<string, QuestInfoSO> questDictionary = new Dictionary<string, QuestInfoSO>();
    
    private void OnEnable()
    {
        RebuildDictionary();
    }
    
    private void RebuildDictionary()
    {
        questDictionary.Clear();
        foreach (var quest in quests)
        {
            if (quest != null && !string.IsNullOrEmpty(quest.id))
            {
                if (!questDictionary.ContainsKey(quest.id))
                {
                    questDictionary.Add(quest.id, quest);
                }
                else
                {
                    Debug.LogWarning($"Quest com ID duplicado encontrada: {quest.id}");
                }
            }
        }
    }
    
    // Método para buscar quest por ID
    public QuestInfoSO GetQuestById(string questId)
    {
        if (questDictionary.Count == 0 && quests.Count > 0)
            RebuildDictionary();

        if (questDictionary.TryGetValue(questId, out QuestInfoSO quest))
            return quest;
            
        return null;
    }
    
    // Verificar se quest existe
    public bool ContainsQuest(string questId)
    {
        if (questDictionary.Count == 0 && quests.Count > 0)
            RebuildDictionary();
            
        return questDictionary.ContainsKey(questId);
    }
    
    // Obter todas as quests
    public IEnumerable<QuestInfoSO> GetAllQuests()
    {
        if (questDictionary.Count == 0 && quests.Count > 0)
            RebuildDictionary();
            
        return questDictionary.Values;
    }
    
    // Obter quests por nível mínimo
    public List<QuestInfoSO> GetQuestsByLevel(int playerLevel)
    {
        if (questDictionary.Count == 0 && quests.Count > 0)
            RebuildDictionary();
        
        List<QuestInfoSO> availableQuests = new List<QuestInfoSO>();
        foreach (var quest in questDictionary.Values)
        {
            if (quest.levelRequirement <= playerLevel)
            {
                availableQuests.Add(quest);
            }
        }
        return availableQuests;
    }
    
    // Verificar se o jogador atende os pré-requisitos da quest
    public bool MeetsPrerequisites(string questId, List<string> completedQuestIds)
    {
        QuestInfoSO quest = GetQuestById(questId);
        if (quest == null) return false;
        
        if (quest.questPrerequisites == null || quest.questPrerequisites.Length == 0)
            return true;
        
        foreach (var prerequisite in quest.questPrerequisites)
        {
            if (prerequisite != null && !completedQuestIds.Contains(prerequisite.id))
            {
                return false;
            }
        }
        
        return true;
    }
}
