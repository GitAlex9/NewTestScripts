using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class RecipeSO : ScriptableObject
{
    public string recipeID;
    public string recipeName;
    public Sprite icon;
    public bool needsToBeUnlocked = false;
}
