using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    private HashSet<string> unlockedRecipes = new HashSet<string>();

    public event Action<string> OnRecipeUnlocked;

    public GameObject craftingCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void UnlockRecipe(string recipeID)
    {
        if (!unlockedRecipes.Contains(recipeID))
        {
            unlockedRecipes.Add(recipeID);
            Debug.Log($"Receita desbloqueada: {recipeID}");
            OnRecipeUnlocked?.Invoke(recipeID);
        }
    }

    public bool IsRecipeUnlocked(string recipeID)
    {
        return unlockedRecipes.Contains(recipeID);
    }
}
