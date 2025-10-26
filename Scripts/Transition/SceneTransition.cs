using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeTimer = 0.5f;

    public void transitionFade(int sceneIndex, int teleportIndex = -1)
    {
        if (GameManager.Instance != null) GameManager.Instance.teleportIndex = teleportIndex;
        StartCoroutine(TrasitionWithFade(sceneIndex));
    }

    IEnumerator TrasitionWithFade(int nextScene)
    {
        // Fade para preto
        fadeImage?.DOFade(1, fadeTimer);
        yield return new WaitForSeconds(fadeTimer);

        // Carregar nova cena
        SceneManager.LoadScene(nextScene);

        // Aguardar um frame para a cena carregar
        yield return null;

        // Fade de volta (transparente)
        if (fadeImage != null)
        {         
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
            fadeImage.DOFade(0, fadeTimer);

            yield return new WaitForSeconds(0.1f); // Pequena pausa antes do save
            GameManager.Instance.UnPauseGame();
            Metadados.Instance.SaveGameData();
        }
    }

    private void Start()
    {
        // Garantir que começa transparente
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
            fadeImage.DOFade(0, fadeTimer);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Multiple instances of SceneTransition detected. Destroying duplicate instance.");
        }
    }
}
