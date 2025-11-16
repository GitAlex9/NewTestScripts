using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "Quest/QuestInfoSO", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    //Unique name ID for each quest.
    [field: SerializeField] public string id {get; private set;}

    [Header("General")]
    public string displayName;

    // If we use the player's level as a requirement for quests, or not, depending on the game's progression system.
    [Header("Requirements")]
    public int levelRequirement; 
    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    [Header("Rewards")]
    public int contributionPoints;
    public int experienceReward;
    
    [Header("Crafting Unlock (Opcional)")]
    public RecipeSO recipeToUnlock;


    // ID is always the same name of the "SO" asset
    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
