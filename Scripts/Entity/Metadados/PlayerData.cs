using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    // Stats do player
    public EntityStats stats = new EntityStats();
    public EntityStats baseStats = new EntityStats();
    
    // Posição e cena
    public string lastScene = "FormigueiroCidade";
    public Vector3 lastPosition = Vector3.zero;
    public Quaternion lastRotation = Quaternion.identity;

    // Construtor
    public PlayerData()
    {
        stats = new EntityStats();
        baseStats = new EntityStats();
        lastScene = "FormigueiroCidade";
        lastPosition = Vector3.zero;
        lastRotation = Quaternion.identity;
    }
    
    public void ExtractEntityData(Entity playerEntity)
    {
        // Copia os valores dos stats, não as referências
        stats.SetStats(playerEntity.stats);
        baseStats.SetStats(playerEntity.baseStats);
        
        // Salvar posição e rotação
        lastPosition = playerEntity.transform.position;
        lastRotation = playerEntity.transform.rotation;
        
        // Salvar cena atual
        lastScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }
    
    public void ApplyToEntity(Entity playerEntity)
    {
        if (playerEntity == null) return;
        
        // Aplicar stats
        playerEntity.stats.SetStats(stats);
        playerEntity.baseStats.SetStats(baseStats);
        
        // Aplicar posição e rotação
        playerEntity.transform.position = lastPosition;
        playerEntity.transform.rotation = lastRotation;
    }
    
    public string DataToString()
    {
        return JsonUtility.ToJson(this, true);
    }
    
    public static PlayerData StringToData(string jsonString)
    {
        return JsonUtility.FromJson<PlayerData>(jsonString);
    }
}