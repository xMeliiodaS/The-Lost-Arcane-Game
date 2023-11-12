using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingScreen;
    public Image loadingBarFill;
    public float loadingTime = 1f;

    public void LoadingScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        loadingScreen.SetActive(true);

        float elapsedTime = 0f;

        while (elapsedTime < loadingTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / loadingTime);
            loadingBarFill.fillAmount = progress;
            yield return null;
        }

        loadingBarFill.fillAmount = 1f;
        operation.allowSceneActivation = true;
    }
}
