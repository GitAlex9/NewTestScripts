using UnityEngine;
using System;
using nTools.PrefabPainter;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public InputReaderGameManager _input;
    public InputEventContext inputEventContext { get; private set; } = InputEventContext.DEFAULT;

    [SerializeField] public bool IsQuestLog = false;
    [SerializeField] public bool IsPaused = false;
    [SerializeField] public bool SoftPaused = false;
    [SerializeField] public bool IsDialogue = false;

    // Quest Events
    public QuestEvents questEvents;
    public DialogueEvents dialogueEvents;
    public event Action OnQuestLogPressed;
    public event Action<InputEventContext> OnInteractPress;
    public event Action OnCraftPressed;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    public PlayerEntity playerEntity;
    public int teleportIndex = -1;
    private Transform playerSpawnPoint;


    public event Action OnGamePaused;
    public event Action OnGameUnpaused;

    public event Action OnInventoryPressed;

    public event Action OnGameOver;

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("GameManager instance created.");
            DontDestroyOnLoad(this);

            questEvents = new QuestEvents();
            dialogueEvents = new DialogueEvents();
            // DontDestroyOnLoad(gameObject); Se for necessário
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        _input.PauseEvent += PauseGame;
        _input.QuestLogEvent += QuestLogUI;
        _input.InteractEvent += InteractPress;
    }

    private void OnDisable()
    {
        _input.PauseEvent -= PauseGame;
        _input.QuestLogEvent -= QuestLogUI;
        _input.InteractEvent -= InteractPress;
    }
    #endregion

    #region Input
    public void ChangeInputEventContext(InputEventContext newContext)
    {
        this.inputEventContext = newContext;
    }

    public void InteractPress()
    {
        if (OnInteractPress != null)
        {
            OnInteractPress(this.inputEventContext);
        }
    }
    #endregion

    #region Dialogue
    public void DialogueOn()
    {
        IsDialogue = true;
        SoftPaused = true;
    }

    public void DialogueOff()
    {
        IsDialogue = false;
        SoftPaused = false;
    }
    #endregion

    #region UI
    public void QuestLogUI()
    {
        IsQuestLog = !IsQuestLog;
        SoftPauseGame();
        if (IsQuestLog)
        {
            OnQuestLogPressed?.Invoke();
        }
        else
        {
            OnQuestLogPressed?.Invoke();
        }
    }

    public void CraftLogUi()
    {
        OnCraftPressed?.Invoke();
    }
    
    public void InventoryUI()
    {
        OnInventoryPressed?.Invoke();
    }
    #endregion

    #region Pause
    public void PauseGame()
    {
        IsPaused = !IsPaused;
        if (IsPaused)
        {
            OnGamePaused?.Invoke();
        }
        else
        {
            OnGameUnpaused?.Invoke();
        }
    }

    public void UnPauseGame()
    {
        IsPaused = false;
        OnGameUnpaused?.Invoke();
    }

    public void SoftPauseGame()
    {
        SoftPaused = !SoftPaused;
    }
    #endregion

    #region Scene
    public void SceneChange(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    #endregion

    #region Player
    public void SpawnPlayer(PlayerSpawn spawn)
    {
        playerSpawnPoint = spawn.transform;

        if (playerEntity == null)
        {
            Vector3 playerPosition = playerSpawnPoint.position;
            Quaternion playerRotation = playerSpawnPoint.rotation;
            bool applyFullData = false;

            // Verificar se há dados salvos no Metadados
            if (Metadados.playerData != null && !string.IsNullOrEmpty(Metadados.playerData.lastScene))
            {
                // Verificar se estamos na mesma cena que foi salva
                string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                
                if (currentScene == Metadados.playerData.lastScene)
                {
                    // Mesma cena: usar posição e rotação salvos
                    playerPosition = Metadados.playerData.lastPosition;
                    playerRotation = Metadados.playerData.lastRotation;
                    applyFullData = true;
                    Debug.Log($"Player spawned with saved data at saved position (Scene: {currentScene})");
                }
                else
                {
                    // Cena diferente: usar spawn point mas marcar para aplicar stats
                    applyFullData = false;
                    Debug.Log($"Player spawned at spawn point with saved stats (Different scene - Saved: {Metadados.playerData.lastScene}, Current: {currentScene})");
                }
            }
            else
            {
                Debug.Log("Player spawned at spawn point (no saved data)");
            }

            // Criar player na posição determinada
            GameObject playerInstance = Instantiate(playerPrefab, playerPosition, playerRotation);
            playerEntity = playerInstance.GetComponent<PlayerEntity>();

            // Aplicar dados salvos se necessário
            if (Metadados.playerData != null)
            {
                if (applyFullData)
                {
                    // Aplicar tudo (stats, posição já foi aplicada)
                    Metadados.playerData.ApplyToEntity(playerEntity);
                }
                else if (!string.IsNullOrEmpty(Metadados.playerData.lastScene))
                {
                    // Aplicar apenas stats
                    playerEntity.stats.SetStats(Metadados.playerData.stats);
                    playerEntity.baseStats.SetStats(Metadados.playerData.baseStats);
                }
            }
        }
        else if (playerPrefab == null)
        {
            Debug.LogWarning("PlayerPrefab is not assigned in GameManager.");
        }
    }

    public void SpawnPlayerAtPosition(Vector3 position, Quaternion rotation)
    {
        if (playerEntity == null && playerPrefab != null)
        {
            // Criar novo player na posição especificada
            GameObject playerInstance = Instantiate(playerPrefab, position, rotation);
            playerEntity = playerInstance.GetComponent<PlayerEntity>();

            // Aplicar apenas stats salvos (não posição/rotação, pois já foram definidos)
            if (Metadados.playerData != null)
            {
                playerEntity.stats.SetStats(Metadados.playerData.stats);
                playerEntity.baseStats.SetStats(Metadados.playerData.baseStats);
            }
        }
        else if (playerEntity != null)
        {
            // Mover player existente
            playerEntity.transform.position = position;
            playerEntity.transform.rotation = rotation;
        }
    }

    public void PlayerDeath()
    {
        Debug.Log("Player has died. Triggering Game Over sequence.");
        OnGameOver?.Invoke();
    }
    
    #endregion
}
