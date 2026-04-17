using System.Collections.Generic;
using UnityEngine;

public class ObjectController
{
    private ObjectManager m_manager;

    private HashSet<string> m_inactiveObjHash = new HashSet<string>();

    public void Init(ObjectManager manager)
    {
        m_manager = manager;
        BoundEvent();
    }

    private void BoundEvent()
    {
        GameSceneManager.OnCompleteLoadSceneAction += SetSceneObject;
        CutsceneManager.OnCutsceneStateChangeAction += HandleCutsceneStateChangeAction;
    }

    private void UnBoundEvent()
    {
        GameSceneManager.OnCompleteLoadSceneAction -= SetSceneObject;
        CutsceneManager.OnCutsceneStateChangeAction -= HandleCutsceneStateChangeAction;
    }

    public void SetSceneObject()
    {
        var sceneObjData = m_manager.GetSceneObjectData(GameSceneManager.Instance.CurrentSceneName);
        if (sceneObjData == null)
        {
            return;
        }

        foreach (var sceneObj in sceneObjData.SceneObjDic)
        {
            if(m_inactiveObjHash.Contains(sceneObj.Key))
                continue;

            GameObject obj = CreateObject(sceneObj.Key, sceneObj.Value);
            SetObjectValue(obj, sceneObj.Value);

            m_manager.AddObjectDic(obj.name, obj);
            m_manager.HandleCreateObject(obj);
            m_manager.AddObjectDataDic(obj.name, sceneObj.Value);
        }
    }

    private void SetObjectValue(GameObject obj, ObjectData objData)
    {
        SetObjectPosition(obj, objData.SpawnPos);
        SetObjectRotation(obj, objData.Rotation);

        //object root に設定
        if (m_manager.ObjectRoot == null)
            m_manager.CreateObjectRoot();

        //プレーヤーはプレーヤーマネージャーが管理する
        if (objData.ObjectType != CreateObjectTypeEnum.Player)
            m_manager.SetParentObjectRoot(obj);
        else
        {
            Facade.Instance.RequestSetPlayerCamera(obj);
        }
    }

    public GameObject SpawnObject(string name, ObjectData data)
    {
        GameObject obj = CreateObject(name, data);
        SetObjectValue(obj, data);

        m_manager.AddObjectDic(obj.name, obj);
        m_manager.HandleCreateObject(obj);
        m_manager.AddObjectDataDic(obj.name, data);

        return obj;
    }

    private GameObject CreateObject(string name, ObjectData data)
    {
        //createObj
        GameObject obj = null;
        switch (data.ObjectType)
        {
            case CreateObjectTypeEnum.Player:
                obj = CreatePlayer(name, data);
                break;
            case CreateObjectTypeEnum.Field:
                obj = CreateFieldObject(name, data);
                break;
            case CreateObjectTypeEnum.Observe:
                obj = CreateObserveObject(data);
                break;
            case CreateObjectTypeEnum.Enemy:
                obj = CreateEnemyObject(name, data);
                break;
        }

        return obj;
    }

    private GameObject CreatePlayer(string name, ObjectData data)
    {
        if(PlayerManager.Instance.GetPlayer() != null)
            return PlayerManager.Instance.Player;

        string path = GetPathByPrefabName(data.PrefabName);
        return UnityUtil.Instantiate(name, path, data.Tag, data.Layer);
    }

    private GameObject CreateFieldObject(string name, ObjectData data)
    {
        string path = GetPathByPrefabName(data.PrefabName);
        return UnityUtil.Instantiate(name, path, data.Tag, data.Layer);
    }

    private GameObject CreateObserveObject(ObjectData data)
    {
        string objName = DialogManager.Instance.CurrentDialogObjName;
        string observePrefabName = objName + DefineString._Observe;
        string path = GetPathByPrefabName(observePrefabName);

        return UnityUtil.Instantiate(objName, path, data.Tag, data.Layer);
    }

    private GameObject CreateEnemyObject(string name, ObjectData data)
    {
        string path = GetPathByPrefabName(data.PrefabName);
        GameObject enemyObj = UnityUtil.Instantiate(name, path, data.Tag, data.Layer);

        Facade.Instance.RequestSetupEnemy(enemyObj);
        return enemyObj;
    }

    private void SetObjectPosition(GameObject obj, Vector3 pos)
    {
        obj.transform.position = pos;
    }

    private void SetObjectRotation(GameObject obj, Vector3 rotation)
    {
        Quaternion rot = Quaternion.Euler(rotation);
        obj.transform.rotation = rot;
    }

    private string GetPrefabName(string objName)
    {
        string prefabName = m_manager.TryGetPrefabName(objName);
        return prefabName;
    }

    private string GetPathByPrefabName(string prefabName)
    {
        string path = m_manager.GetPrefabPath(prefabName);
        return path;
    }

    public void ActiveAllSceneObject(Dictionary<string, GameObject> sceneObjDic, bool active)
    {
        foreach (var sceneObj in sceneObjDic)
        {
            sceneObj.Value.gameObject.SetActive(active);
        }
    }

    private void HandleCutsceneStateChangeAction(CutsceneStateEnum state)
    {
        var dic = m_manager.GetCurrentSceneObjDic();
        switch (state)
        {
            case CutsceneStateEnum.Play:
                ActiveAllSceneObject(dic, false);
                break;
            case CutsceneStateEnum.End:
                ActiveAllSceneObject(dic, true);
                break;
        }
    }

    public void RefreshController()
    {
        if(GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.MainMenu)
            m_inactiveObjHash.Clear();
    }

    public void Release()
    {
        UnBoundEvent();
    }

    public void ActiveObject(GameObject obj, bool active)
    {
        obj.gameObject.SetActive(active);
    }


    public void SaveGame(SaveData saveData)
    {
        ObjectSaveData objSaveData = new ObjectSaveData();

        objSaveData.InactiveObjectHash = m_inactiveObjHash;

        saveData.ObjectData = objSaveData;
    }

    public void LoadGame(SaveData saveData)
    {
        ObjectSaveData objSaveData = saveData.ObjectData;
        if (objSaveData == null)
            return;

        if (objSaveData.InactiveObjectHash != null)
            m_inactiveObjHash = objSaveData.InactiveObjectHash;
    }

    public void RegisterInactiveObj(string objName)
    {
        if (m_inactiveObjHash.Contains(objName))
            return;

        m_inactiveObjHash.Add(objName);
    }
}
