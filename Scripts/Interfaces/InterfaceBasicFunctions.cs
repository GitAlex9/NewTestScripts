using UnityEngine;

public class InterfaceBasicFunctions : MonoBehaviour
{
    public void SceneChange(int sceneIndex)
    {
      SceneTransition.Instance.transitionFade(sceneIndex);
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
