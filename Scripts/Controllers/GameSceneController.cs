using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneController
{
    public SceneTypeEnum CurrentSceneType { get; private set; }
    public string CurrentSceneName { get; private set; }

    public void Init(string sceneName, SceneData sceneData)
    {
        //最初に生成されたとき、現在のシーン情報を設定する
        CurrentSceneName = sceneName;
        CurrentSceneType = sceneData.SceneType;
    }

    ///<summary>
    ///Fade 時間だけロード遅延
    ///</summary>
    public void MoveScene(string sceneName, SceneData sceneData)
    {
        GameSceneManager.Instance.StartCoroutine(CoMoveScene(sceneName, sceneData));
    }

    private IEnumerator CoMoveScene(string sceneName, SceneData sceneData)
    {
        GameSceneManager.Instance.HandleStartLoadScene();

        CurrentSceneType = sceneData.SceneType;
        CurrentSceneName = sceneName;

        //フェードタイムの間待機
        float fadeTime = LoadingManager.Instance.FadeTime;
        yield return new WaitForSecondsRealtime(fadeTime);

        GameSceneManager.Instance.HandleMidLoadScene();

        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = false; //ロード完了後、自動転換防止

        //フェードタイム経過後もロードが完了しない場合は終了するまで待機
        while (op.progress < 0.9f)      
        {
            yield return null;
        }

        //シーンを有効にする
        op.allowSceneActivation = true;

        //シーントランジションが完全に終了するまで待機
        while (op.isDone == false)
        {
            yield return null;
        }

        //シーンロード後処理(観察シーンオブジェクトの生成など)
        HandleSceneCompleted(op);
    }

    public void SaveGame(SaveData saveData)
    {
        SceneSaveData sceneData = new SceneSaveData();
        sceneData.CurrentSceneName = CurrentSceneName;
        sceneData.CurrentSceneType = CurrentSceneType;

        saveData.SceneData = sceneData;
    }

    public void LoadGame(SaveData saveData)
    {
        SceneSaveData sceneData = saveData.SceneData;

        CurrentSceneName = sceneData.CurrentSceneName;
        CurrentSceneType = sceneData.CurrentSceneType;
    }

    private void HandleSceneCompleted(AsyncOperation op)
    {
        GameSceneManager.Instance.HandleCompleteLoadScene();
    }

    public void Release()
    {

    }
}
