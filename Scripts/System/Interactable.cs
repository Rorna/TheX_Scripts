using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private string m_message;
    [SerializeField] private string m_dialogID;
    [SerializeField] private InteractTypeEnum m_interactType;

    [Header("Observation Settings")]
    [SerializeField] private bool m_isObserveTarget;

    public bool IsObserveTarget => m_isObserveTarget;

    public string Message => m_message;
    public string ObjectName => gameObject.name;
    public InteractTypeEnum InteractType => m_interactType;
    private Outline m_outline;

    private void Start()
    {
        m_outline = GetComponent<Outline>();
        if(m_outline != null)
            m_outline.enabled = false;
    }

    public void RefreshOutline(bool active)
    {
        if (m_outline != null)
            m_outline.enabled = active;
    }

    public void RotateToTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - gameObject.transform.position;
        dir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.DORotateQuaternion(targetRotation, 0.5f);
    }

    /// <summary>
    /// m_dialogID -> ダイアログの json ファイル
    /// </summary>
    public void Interact()
    {
        if (string.IsNullOrEmpty(m_dialogID))
            return;

        Facade.Instance.RequestStartDialog(m_dialogID);
    }
}