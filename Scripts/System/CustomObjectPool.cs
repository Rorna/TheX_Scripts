using System.Collections.Generic;
using UnityEngine;

public class CustomObjectPool<T> where T : Component
{
    private T m_prefab;
    private Transform m_poolRoot;
    private Queue<T> m_pool = new Queue<T>();

    private readonly int m_maxCapacity = 30;

    public void Init(T prefab, int capacity, Transform parent)
    {
        m_prefab = prefab;

        GameObject rootObj = new GameObject($"{prefab.name}_Pool");
        m_poolRoot = rootObj.transform;
        m_poolRoot.transform.SetParent(parent);

        for (int i = 0; i < capacity; i++)
        {
            T obj = CreateObject();
            m_pool.Enqueue(obj);
            obj.gameObject.SetActive(false);
        }
    }

    public T Get()
    {
        T obj = null;
        if (m_pool.Count > 0)
        {
            obj = m_pool.Dequeue();
        }
        else
        {
            obj = CreateObject();
        }

        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(T obj)
    {
        if (m_pool.Contains(obj))
            return;

        //最大限界値に達したらキューに入れずに破棄する
        if (m_pool.Count >= m_maxCapacity)
        {
            Object.Destroy(obj.gameObject);
            return;
        }

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(m_poolRoot);
        m_pool.Enqueue(obj);
    }

    private T CreateObject()
    {
        T obj = Object.Instantiate(m_prefab, m_poolRoot);
        obj.name = m_prefab.name;

        return obj;
    }

    public void ClearPool()
    {
        while (m_pool.Count > 0)
        {
            T obj = m_pool.Dequeue();
            if (obj != null && obj.gameObject != null)
            {
                Object.Destroy(obj.gameObject);
            }
        }
        m_pool.Clear();

        if (m_poolRoot != null)
        {
            Object.Destroy(m_poolRoot.gameObject);
            m_poolRoot = null;
        }
    }
}
