using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager instance {get; private set; }

    public QuestEvents questEvents;
    public InputReader inputPlayer;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Manager in the scene.");
        }
        instance = this;

        questEvents = new QuestEvents();
        
    }
    

}
