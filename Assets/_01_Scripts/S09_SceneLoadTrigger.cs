using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class S09_SceneLoadTrigger : MonoBehaviour
{
    [Header("Scene Configuration")]
    [SerializeField] private SceneField sceneToLoad;
    [SerializeField] private SceneField sceneToUnload;

    [Header("End Game Settings")]
    [Tooltip("Check this ONLY for the trigger at the end of Room 5")]
    [SerializeField] private bool isLastRoom = false;

    private S14_LeaderboardUI leaderboard;
    private S11_GameTimer gameTimer;

    private void Awake()
    {
        leaderboard = FindFirstObjectByType<S14_LeaderboardUI>();
        gameTimer = FindFirstObjectByType<S11_GameTimer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isLastRoom)
            {
                HandleGameEnd();
            }
            else
            {
                HandleSceneTransition();
            }
        }
    }

    private void HandleSceneTransition()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            StartCoroutine(LoadNewSceneAsync());
        }

        if (!string.IsNullOrEmpty(sceneToUnload))
        {
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }
    }

    private IEnumerator LoadNewSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null; 
        }
    }

    public void HandleGameEnd()
    {
        // 1. Stop the timer and save the data
        float finalTime = gameTimer.GetFinalTime();
        leaderboard.AddEntryAndSave(S13_PlayerNameInput.PlayerName, finalTime);
        
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        
        StartCoroutine(BridgeSequence());

        //SceneManager.LoadScene("BE_MainMenu");
    }

    private IEnumerator BridgeSequence()
    {
        SceneManager.LoadScene("BE_Bridge");

        yield return new WaitForSeconds(3f);
        
        // 3. Load the Main Menu
        
        SceneManager.LoadScene("BE_MainMenu");

        Destroy(gameObject);
    }
}