using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    static GameStateManager Instance;

    [SerializeField] SceneReference gamePlayScene;
    [SerializeField] GameObject optionsWindow;
    void Awake() => Instance = this;


    public static void StartGame()
    {
        Instance.StartCoroutine(Instance.LoadScenesCoroutine((int)Scenes.MainMenu, Instance.GetSceneID(Instance.gamePlayScene)));
    }

    public static void OptionsWindow(bool status = true)
    {
        Instance.optionsWindow?.SetActive(status);
        PauseManager.Instance.PauseLogic();
    }

    public static void GoToMainMenu()
    {
        Instance.StartCoroutine(Instance.LoadScenesCoroutine(Instance.GetSceneID(Instance.gamePlayScene), (int)Scenes.MainMenu));
    }

    public static void ReloadGameScene()
    {
        Instance.StartCoroutine(Instance.ReloadGameSceneCoroutine());
    }

    /// <summary> Depends on the naming (0_Scene), since its gets the first char and ints it</summary>
    int GetSceneID(SceneReference sceneRef)
    {
        return sceneRef.GetSceneIndex();
    }

    IEnumerator LoadScenesCoroutine(int oldScene, int newScene)
    {
        yield return null;
        LoadingScreen.Show(this);
        yield return SceneLoader.Instance.LoadSceneViaIndex(newScene);
        yield return SceneLoader.Instance.UnloadSceneViaIndex(oldScene);
        LoadingScreen.Hide(this);
    }

    IEnumerator ReloadGameSceneCoroutine()
    {
        LoadingScreen.Show(this);
        SkillManager.CloseSkillManager();
        yield return new WaitForSeconds(.7f);
        yield return SceneLoader.Instance.UnloadSceneViaIndex(GetSceneID(Instance.gamePlayScene));
        yield return SceneLoader.Instance.LoadSceneViaIndex(GetSceneID(Instance.gamePlayScene));
        LoadingScreen.Hide(this);
    }
}
