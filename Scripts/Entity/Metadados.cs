using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Metadados : MonoBehaviour
{
    public static Metadados Instance;
    public static PlayerData playerData;
    
    [Header("References")]
    [SerializeField] private ItemDB itemDatabase;
    
    public static ItemDB ItemDatabase { get; private set; }

    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ItemDatabase = itemDatabase; // Atribuir o campo serializado à propriedade estática
            DontDestroyOnLoad(gameObject);
            StartCoroutine(InitializeAndLoad());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Debug: F1 para salvar
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SaveGameData();
            Debug.Log("=== SAVE GAME (F1) ===");
        }
        
        // Debug: F2 para deletar todo o save
        if (Input.GetKeyDown(KeyCode.F2))
        {
            DeleteAllSaveData();
            Debug.Log("=== ALL SAVE DATA DELETED (F2) ===");
        }
    }
    #endregion

    #region Load
    private IEnumerator InitializeAndLoad()
    {
        // Aguardar até que GameManager e Inventory estejam prontos
        while (GameManager.Instance == null || Inventory.Instance == null)
        {
            yield return null;
        }

        // Agora que os managers existem, carregar os dados
        LoadGame();
    }

    private void LoadGame()
    {
        Debug.Log("=== LOADING GAME DATA ===");
        
        // Player Data (stats, posição, cena)
        string jsonString = PlayerPrefs.GetString("PlayerData", "");
        
        if (string.IsNullOrEmpty(jsonString))
        {
            playerData = new PlayerData();
            Debug.Log("No saved PlayerData found - Creating new PlayerData");
        }
        else
        {
            playerData = PlayerData.StringToData(jsonString);
            Debug.Log($"PlayerData loaded:");
            Debug.Log($"  - Last Scene: {playerData.lastScene}");
            Debug.Log($"  - Last Position: {playerData.lastPosition}");
            Debug.Log($"  - Health: {playerData.stats.Health}/{playerData.stats.MaxHealth}");
            Debug.Log($"  - Level: {playerData.stats.Level}");
        }
        
        // Inventário (separado)
        LoadInventory();
    }

    private void LoadInventory()
    {
        if (Inventory.Instance != null && ItemDatabase != null)
        {
            string inventoryJson = PlayerPrefs.GetString("InventoryData", "");

            if (!string.IsNullOrEmpty(inventoryJson))
            {
                bool success = Inventory.Instance.ImportFromJson(inventoryJson, ItemDatabase);

                if (success)
                {
                    Debug.Log($"Inventory loaded successfully:");
                    Debug.Log($"  - Items in inventory: {Inventory.Instance.items.Count}");
                    Debug.Log($"  - Equipped items: {Inventory.Instance.equipments.Length}");
                }
                else
                {
                    Debug.LogWarning("Failed to load inventory data");
                }
            }
            else
            {
                Debug.Log("No saved InventoryData found - Starting with empty inventory");
            }
        }
        else
        {
            Debug.LogWarning("Cannot load inventory - Inventory.Instance or ItemDatabase is null");
        }
    }

    public void ResumeSavedGame()
    {
        if (playerData != null && !string.IsNullOrEmpty(playerData.lastScene))
        {
            LoadLastScene();
        }
        else
        {
            SceneTransition.Instance.transitionFade(1);
        }
    }
    
    public void LoadLastScene()
    {
        if (playerData == null)
        {
            Debug.LogError("[Metadados] PlayerData é nulo! Não é possível carregar a última cena.");
            return;
        }
        
        if (string.IsNullOrEmpty(playerData.lastScene))
        {
            Debug.LogWarning("[Metadados] Nenhuma cena salva encontrada no PlayerData.");
            return;
        }
              
       // Obter o build index da cena salva
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(playerData.lastScene);
        
        if (sceneIndex < 0)
        {
            Debug.LogError($"[Metadados] Cena '{playerData.lastScene}' não encontrada no Build Settings!");
            return;
        }
        
        Debug.Log($"[Metadados] Carregando última cena salva: {playerData.lastScene} (Index: {sceneIndex})");
        
        // Fazer transição com fade
        SceneTransition.Instance.transitionFade(sceneIndex);
    }
    #endregion

    #region Save

    private void SaveGame()
    {
        // Salvar Player Data (stats, posição, cena)
        if (playerData != null)
        {
            if (GameManager.Instance?.playerEntity != null)
            {
                playerData.ExtractEntityData(GameManager.Instance.playerEntity);
            }

            string playerDataJson = playerData.DataToString();
            PlayerPrefs.SetString("PlayerData", playerDataJson);
        }

        // Salvar Inventário (separado)
        SaveInventory();
        
        // Salvar Quests (separado)
        SaveQuests();

        PlayerPrefs.Save();
    }

    private void SaveInventory()
    {
        if (Inventory.Instance != null)
        {
            string inventoryJson = Inventory.Instance.ExportToJson();
            PlayerPrefs.SetString("InventoryData", inventoryJson);
        }
    }
    
    private void SaveQuests()
    {
        if (QuestManager.Instance != null)
        {
            Debug.Log("[METADADOS] Calling QuestManager.SaveAllQuests()");
            QuestManager.Instance.SaveAllQuests();
        }
        else
        {
            Debug.LogWarning("[METADADOS] QuestManager.Instance is null - cannot save quests!");
        }
    }

    public void SaveGameData()
    {
        SaveGame();
    }
    
    public void DeleteAllSaveData()
    {
        Debug.Log("=== DELETING ALL SAVE DATA ===");

        Debug.Log("  - Deleting ALL PlayerPrefs data...");
        PlayerPrefs.DeleteAll();
        
        PlayerPrefs.Save();
        Debug.Log("=== SAVE DATA DELETION COMPLETE ===");
        Debug.Log("!!! ALL PlayerPrefs cleared - Restart the game to load fresh data !!!");
    }
    #endregion
}
