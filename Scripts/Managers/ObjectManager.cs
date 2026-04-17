using System;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///オブジェクトの作成/管理
///オブジェクトの行為は関与しない。
///生成消滅管理。
///</summary>
public class ObjectManager : BaseManager, ISavable
{
    public static ObjectManager Instance { get; private set; }

    public GameObject ObjectRoot { get; private set; }

    public static event Action<GameObject> OnCreateObjectAction; //サウンドとか…など

    //現在作成されているオブジェクト
    private Dictionary<string, GameObject> m_currentSceneObjDic = new Dictionary<string, GameObject>();
    private Dictionary<string, SceneObjectData> m_sceneObjJsonDic = new Dictionary<string, SceneObjectData>();
    private Dictionary<string, ObjectData> m_prefabInfoJsonDic; //すべてのオブジェクトのプレハブディクショナリー
    private Dictionary<string, ObjectData> m_enemySpawnInfoDic;

    //現在のシーンのオブジェクトのObjectData
    private Dictionary<string, ObjectData> m_currentSceneObjDataDic = new Dictionary<string, ObjectData>();

    private ObjectController m_controller;
    
    public override void CreateManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void InternalInitManager()
    {
        m_controller = new ObjectController();
        m_controller.Init(this);

        InitPrefabInfoDic();
        InitSceneObjectData();
        InitEnemySpawnInfoDic();
    }

    public override void ExternalInitManager()
    {
        Facade.Instance.RequestRegisterSaveData(this);
    }

    private void InitEnemySpawnInfoDic()
    {
        m_enemySpawnInfoDic = new Dictionary<string, ObjectData>();

        //path
        string jsonPath = DefineString.JsonObjPath + DefineString.JsonEnemySpawnInfo;
        string jsonText = JsonUtil.LoadJson(jsonPath);

        var loadedData = JsonUtil.ParseJson<ObjectInfo>(jsonText);

        m_enemySpawnInfoDic = loadedData.ObjInfoDic;
    }

    private void InitSceneObjectData()
    {
        Dictionary<string, string> allJsonDic = JsonUtil.LoadAllJsonFiles(DefineString.JsonSceneObjectPath);

        m_sceneObjJsonDic = new Dictionary<string, SceneObjectData>();
        foreach (var json in allJsonDic)
        {
            string fileName = json.Key;
            string jsonText = json.Value;

            SceneObjectData sceneObjData = JsonUtil.ParseJson<SceneObjectData>(jsonText);
            if (sceneObjData != null && m_sceneObjJsonDic.ContainsKey(fileName) == false)
            {
                m_sceneObjJsonDic.Add(fileName, sceneObjData);
            }
        }
    }

    public SceneObjectData GetSceneObjectData(string sceneName)
    {
        string jsonName = sceneName + DefineString._Obj;
        if (m_sceneObjJsonDic.TryGetValue(jsonName, out SceneObjectData sceneObjData) == false)
            return null;

        return sceneObjData;
    }

    public string GetPrefabPath(string prefabName)
    {
        if (m_prefabInfoJsonDic.TryGetValue(prefabName, out ObjectData objData) == false)
            return null;

        return objData.Path;
    }

    private void InitPrefabInfoDic()
    {
        m_prefabInfoJsonDic = new Dictionary<string, ObjectData>();

        //path
        string jsonPath = DefineString.JsonObjPath + DefineString.JsonPrefabInfo;
        string jsonText = JsonUtil.LoadJson(jsonPath);

        var loadedData = JsonUtil.ParseJson<ObjectInfo>(jsonText);

        m_prefabInfoJsonDic = loadedData.ObjInfoDic;
    }

    public void AddObjectDic(string objName, GameObject obj)
    {
        m_currentSceneObjDic.TryAdd(objName, obj);
    }

    public void AddObjectDataDic(string objName, ObjectData objData)
    {
        m_currentSceneObjDataDic.TryAdd(objName, objData);
    }

    public string TryGetPrefabName(string objName)
    {
        if (m_currentSceneObjDataDic.TryGetValue(objName, out var objData) == false)
            return string.Empty;

        return objData.PrefabName;
    }

    public void HandleCreateObject(GameObject obj)
    {
        OnCreateObjectAction?.Invoke(obj);
    }

    public void CreateObjectRoot()
    {
        if (ObjectRoot != null)
            return;

        ObjectRoot = new GameObject("ObjectRoot");
    }

    public void SetParentObjectRoot(GameObject go)
    {
        go.transform.SetParent(ObjectRoot.transform);
    }

    public Interactable GetSceneInteractable(string objName)
    {
        if (m_currentSceneObjDic.TryGetValue(objName, out var gameObject) == false)
            return null;

        if (gameObject.TryGetComponent<Interactable>(out var interactable) == false)
            return null;

        return interactable;
    }

    public GameObject TryGetSceneGameObject(string objName)
    {
        if (m_currentSceneObjDic.TryGetValue(objName, out var gameObject) == false)
            return null;

        return gameObject;
    }

    public Dictionary<string, GameObject> GetCurrentSceneObjDic()
    {
        return m_currentSceneObjDic;
    }

    public GameObject TrySpawnEnemy(string objName)
    {
        string enemyName = objName + DefineString._Enemy;
        if (m_enemySpawnInfoDic.TryGetValue(enemyName, out var enemyData) == false)
            return null;

        return m_controller.SpawnObject(enemyName, enemyData);
    }

    public override void RefreshManager()
    {
        if(ObjectRoot != null)
            Destroy(ObjectRoot);

        m_controller.RefreshController();
        m_currentSceneObjDic.Clear();
        m_currentSceneObjDataDic.Clear();
    }

    public override void ClearManager()
    {
        
    }

    public void TryActiveObject(string objName, bool active)
    {
        if (m_currentSceneObjDic.TryGetValue(objName, out var obj) == false)
            return;

        m_controller.ActiveObject(obj, active);
    }

    public void TryRegisterInactive(string objName)
    {
        m_controller.RegisterInactiveObj(objName);
    }

    public void SaveGameData(SaveData saveData)
    {
        m_controller.SaveGame(saveData);
    }

    public void LoadGameData(SaveData saveData)
    {
        m_controller.LoadGame(saveData);
    }
}
