using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;
using Object = UnityEngine.Object;

public static class UnityUtil
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static GameObject GetChildObject(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChildComponent<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChildComponent<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }
        return null;
    }

    public static GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Resources.Load<GameObject>($"Prefabs/{path}");
        if (original == null)
            return null;

        GameObject go = Object.Instantiate(original, parent);

        return go;
    }

    /// <summary>
    /// No Instantiate, just return origin
    /// </summary>
    public static GameObject LoadPrefab(string path)
    {
        GameObject original = Resources.Load<GameObject>($"Prefabs/{path}");
        if (original == null)
            return null;

        return original;
    }

    public static GameObject Instantiate(string name, string path, string tag, string layer)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
            return null;

        GameObject go = Object.Instantiate(prefab);

        go.name = name;
        go.tag = tag;
        go.layer = LayerMask.NameToLayer(layer);

        return go;
    }


    public static GameObject Instantiate(string path, Vector2 position, Quaternion direction, Transform parent = null) //pos, dir
    {
        GameObject original = Resources.Load<GameObject>($"Prefabs/{path}");
        if (original == null)
            return null;

        GameObject go = Object.Instantiate(original, position, direction);
        go.transform.SetParent(parent);

        return go;
    }

    public static GameObject Instantiate(string path, Vector2 position, Transform parent = null) //pos, dir
    {
        GameObject original = Resources.Load<GameObject>($"Prefabs/{path}");
        if (original == null)
            return null;

        GameObject go = Object.Instantiate(original, position, Quaternion.identity);
        go.transform.SetParent(parent);

        return go;
    }

    public static GameObject Instantiate(string objName, string path, Vector3 position, Vector3 rotation,
        string tag, string layer)
    {
        GameObject original = Resources.Load<GameObject>($"Prefabs/{path}");

        if (original == null)
            return null;

        Quaternion rot = Quaternion.Euler(rotation);
        GameObject go = Object.Instantiate(original, position, rot);

        go.name = objName;
        go.tag = tag;
        go.layer = LayerMask.NameToLayer(layer);

        foreach (Transform child in go.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layer);
        }

        return go;
    }

    public static void ShowNoticePopUp(string msg)
    {
        Facade.Instance.RequestNoticePopup(msg);
    }

    public static Sprite GetSprite(string path, string spriteName)
    {
        // マルチスプライトの場合、先頭のスプライトを取得する。
        Sprite sprite = Resources.Load<Sprite>(path + spriteName);
        if (sprite != null && sprite.name == spriteName)
            return sprite;

        // ダブルスプライト
        Sprite[] spriteArr = Resources.LoadAll<Sprite>(path);

        sprite = Array.Find<Sprite>(spriteArr, (s) => s.name.Equals(spriteName));
        if (sprite != null)
            return sprite;

        return null;
    }

    // カーソルの下にある一番上にあるUIリターン
    public static GameObject GetTopUIOnCursorPos()
    {
        // EventSystemがないと不可
        if (EventSystem.current == null) 
            return null;

        // 現在カーソルの位置情報が込められたEventData生成
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();

        // 全てのUIを探してListに入れる
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count == 0)
            return null;

        // 一番上のUIリターン
        return results[0].gameObject;
    }
}
