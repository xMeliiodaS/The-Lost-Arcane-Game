using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CheckPoint : MonoBehaviour
{
    private static int index = 0;
    private readonly string protal = "Portal";
    private readonly string player = "Player";
    private readonly string lobbyManager = "LobbyManager";

    [SerializeField] private bool dontLoad;


    private void Start()
    {
        if(dontLoad)
        {
            return;
        }

        index = 0;
        for(int i = 0; i < 2; i++)
        {
            StartCoroutine(Load());
        }
    }
    public static IEnumerator Load()
    {
        yield return new WaitForSeconds(1f);
        if (index == 0)
        {
            DatabaseSystem.instance.Load();
            index++;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(player))
        {
            Debug.Log("This is a check point");
            DatabaseSystem.instance.Save();
            Debug.Log("Saved EVERYTHING");

            // Checks if this is a portal so it saves everything and go to new scene
            if (gameObject.CompareTag(protal))
            {
                Transform lobbyRef = GameObject.Find(lobbyManager).transform;
                var loadingScreenRef = lobbyRef.GetComponent<LoadingScreen>();
                AudioManager.Instance.StopAllSounds();
                loadingScreenRef.LoadingScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }


    /// <summary>
    /// Skip the storyline and load next scene.
    /// </summary>
    public void SkipScene()
    {
        Transform lobbyRef = GameObject.Find(lobbyManager).transform;
        var loadingScreenRef = lobbyRef.GetComponent<LoadingScreen>();
        AudioManager.Instance.StopAllSounds();
        loadingScreenRef.LoadingScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
