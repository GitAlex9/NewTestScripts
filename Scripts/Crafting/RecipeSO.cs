using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class RecipeSO : ScriptableObject
{
    [Header("Identificação")]
    public string recipeID;
    public string recipeName;

    [Header("Exibição")]
    public Sprite icon;

    [Header("Desbloqueio")]
    public bool needsToBeUnlocked = false;
}
