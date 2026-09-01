using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S08_MainMenuManager : MonoBehaviour
{
    [Header("Main Menu Objects")]
    [SerializeField] private GameObject _loadingBarObject;
    [SerializeField] private Image _loadingBar;
    [SerializeField] private GameObject[] _objectToHide;


    [Header("Scenes to Load")]
    [SerializeField] private SceneField _persistentGameplay;    /// [SerializeField] private string _persistentGameplay = "SC06_PersistentGameplay";
                                                                /// [SerializeField] private string _levelScene = "SC06_Level1";
    [SerializeField] private SceneField _levelScene;

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    private void Awake()
    {
        if (_loadingBarObject != null)
        {
            _loadingBarObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        //hide button and text
        HideMenu();

        _loadingBarObject.SetActive(true);

        //start Loading the scenes we need
        // SceneManager.LoadScene("", LoadSceneMode.Additive); //load the scene with the level in it
        SceneManager.LoadSceneAsync(_persistentGameplay);
        SceneManager.LoadSceneAsync(_levelScene, LoadSceneMode.Additive);

        //update the loading bar
        StartCoroutine(ProgressLoadingBar());

    }


    private void HideMenu()
    {
        //hide the menu
        for (int i = 0; i < _objectToHide.Length; i++)
        {
            _objectToHide[i].SetActive(false);
        }
    }


    private IEnumerator ProgressLoadingBar()
    {
        float loadProgress = 0f;
        for (int i = 0; i < _scenesToLoad.Count; i++)
        {
            while (!_scenesToLoad[i].isDone)
            {
                loadProgress += _scenesToLoad[i].progress;
                //update the loading bar here with loadProgress / _scenesToLoad.Count
                _loadingBar.fillAmount = loadProgress / _scenesToLoad.Count;
                yield return null;
            }
        }
    }
}
