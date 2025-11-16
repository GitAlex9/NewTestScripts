using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MaterialRequirement
{
    public ItemData item;
    public int quantity;
}

public class CraftButtonAction : MonoBehaviour
{
    [Header("Item que será criado")]
    public ItemData itemToCraft;

    [Header("Materiais necessários")]
    public List<MaterialRequirement> requiredMaterials = new List<MaterialRequirement>();

    [Header("Receita (se necessário desbloquear)")]
    public RecipeSO recipe;

    [Header("UI de aviso de materiais insuficientes")]
    public GameObject imagemFaltamMateriais;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        CheckRecipeUnlock();
        if (CraftingManager.Instance != null)
            CraftingManager.Instance.OnRecipeUnlocked += OnRecipeUnlocked;
    }

    private void OnDisable()
    {
        if (CraftingManager.Instance != null)
            CraftingManager.Instance.OnRecipeUnlocked -= OnRecipeUnlocked;
    }

    private void OnRecipeUnlocked(string recipeID)
    {
        if (recipe != null && recipe.recipeID == recipeID)
        {
            CheckRecipeUnlock();
        }
    }

    public void RefreshButtonState()
    {
        CheckRecipeUnlock();
    }

    private void CheckRecipeUnlock()
    {
        if (recipe == null || !recipe.needsToBeUnlocked)
        {
            button.interactable = true;
            return;
        }

        if (CraftingManager.Instance != null && CraftingManager.Instance.IsRecipeUnlocked(recipe.recipeID))
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

    public void CraftItem()
    {
        foreach (MaterialRequirement requirement in requiredMaterials)
        {
            int playerItemCount = CountItemInInventory(requirement.item);

            if (playerItemCount < requirement.quantity)
            {
                Debug.Log($"Faltam materiais: {requirement.item.itemName} (precisa {requirement.quantity}, você tem {playerItemCount})");
                StartCoroutine(ShowMissingMaterialsWarning());
                return;
            }
        }

        foreach (MaterialRequirement requirement in requiredMaterials)
        {
            Inventory.Instance.RemoveItemQuantity(requirement.item, requirement.quantity);
        }

        bool added = Inventory.Instance.AddItem(itemToCraft);

        if (added)
        {
            Debug.Log("Item craftado com sucesso: " + itemToCraft.itemName);
        }
        else
        {
            Debug.Log("Inventário cheio. Não foi possível adicionar o item.");
        }
    }

    private int CountItemInInventory(ItemData item)
    {
        int count = 0;
        foreach (var inventoryItem in Inventory.Instance.items)
        {
            if (inventoryItem.item == item)
            {
                count += inventoryItem.quantity;
            }
        }
        return count;
    }

    private IEnumerator ShowMissingMaterialsWarning()
    {
        imagemFaltamMateriais.SetActive(true);

        Image img = imagemFaltamMateriais.GetComponent<Image>();
        Color originalColor = img.color;

        img.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        yield return new WaitForSeconds(0.6f);

        float fadeDuration = 0.3f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            img.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        imagemFaltamMateriais.SetActive(false);
    }
}
