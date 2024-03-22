using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

//Scene Controller manages loading different unity scenes as well as the loading screen in between
//Also, it serves as the top level of a series of objects which persist throughout different scenes by using Dont Destroy On Load

public class SceneController : MonoBehaviour
{
    public static SceneController sceneControllerInstance;

    public CanvasGroup canvasGroup;
    public UnityEngine.UI.Image progressBar;

    string scene;

    public DataPersistenceManager dataPersistenceManager;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (sceneControllerInstance == null)
        {
            sceneControllerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (!canvasGroup.gameObject.activeSelf)
        {
            canvasGroup.gameObject.SetActive(true);
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeToGameplay(.8f));
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SetProgressBarValue(float value)
    {
        progressBar.fillAmount = value;

    }

    public void LoadScene(string sceneName)
    {
        //canvasGroup.enabled = true;

        scene = sceneName;

        StartCoroutine(FadeToLoadingScreen(1, sceneName));
        //prevScene = currentScene;
        //SceneManager.LoadScene(sceneName);

        SetProgressBarValue(0f);

    }

    IEnumerator FadeToLoadingScreen(float duration, string scene)
    {
        float startValue = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startValue, 1, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;

        if (canvasGroup.alpha == 1)
        {
            StartCoroutine(LoadYourAsyncScene(scene));
        }
    }

    IEnumerator FadeToGameplay(float duration)
    {
        float startValue = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startValue, 0, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0;

        if(scene == "MainMenu")
        {
            //GetComponentInChildren<RaceManager>().Load

            print("Returning to main menu");

            dataPersistenceManager.LoadGame();
        }
        else
        {
            OnSceneIsReady();

        }

    }

    public void OnSceneIsReady()
    {
        GetComponentInChildren<RaceManager>().FindStartZone();
    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        // can you not reload a scene with this?

        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneName);


        // Wait until the asynchronous scene fully loads
        while (!loadingOperation.isDone)
        {
            SetProgressBarValue(Mathf.Clamp01(loadingOperation.progress / 0.9f));
            yield return null;
        }


        StartCoroutine(FadeToGameplay(1));
    }
}
