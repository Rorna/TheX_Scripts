using System;
using UnityEngine;

public partial class PlayerController
{
    public LayerMask InteractableMask { get; private set; }
    public float FirstPersonReach { get; private set; }
    public float ThirdPersonRange { get; private set; }

    public Interactable CurrentTarget { get; private set; }

    private static readonly RaycastHit[] m_hitBuffer = new RaycastHit[5];
    private static readonly Collider[] m_colBuffer = new Collider[1];

    private Transform m_firstCamTr; //一人称カメラ

    private void HandleInteract()
    {
        if (CurrentTarget == null)
            return;

        //観測対象は、観測モードがオンになっている場合のみ相互作用が可能
        if (CurrentTarget.IsObserveTarget &&
            ObservationManager.Instance.ObservationMode == false)
            return;

        Facade.Instance.RequestPlaySFX(DefineString.SFX_Click, Vector3.zero, 0.3f, false);

        //look at me
        if (CurrentTarget.InteractType == InteractTypeEnum.NPC)
            CurrentTarget.RotateToTarget(gameObject.transform.position);

        CurrentTarget.Interact();
    }

    private void UpdateCamInfo()
    {
        var firstCam = CameraManager.Instance.FirstCamera;
        m_firstCamTr = firstCam ? firstCam.transform : null;
    }

    private void CheckInteractable()
    {
        switch (CameraManager.Instance.CurrentCameraType)
        {
            case CameraTypeEnum.FirstCamera:
                if (ObservationManager.Instance.ObservationMode)
                {
                    //観察モードならマウスでオブジェクトを検知する。
                    UpdateMouseInteraction();
                }
                else
                {
                    UpdateFirstPersonInteraction();
                }
                break;
            case CameraTypeEnum.ThirdCamera:
                UpdateThirdPersonInteraction();
                break;
        }
    }

    private void UpdateFirstPersonInteraction()
    {
        Vector3 origin = m_firstCamTr.position;
        Vector3 dir = m_firstCamTr.forward;

#if UNITY_EDITOR
        if (m_drawDebugRay)
            Debug.DrawLine(origin, origin + dir * FirstPersonReach, Color.coral);
#endif

        int hitCount = Physics.RaycastNonAlloc(origin, dir, m_hitBuffer,
            FirstPersonReach, InteractableMask);

        Interactable newTarget = null;
        if (hitCount > 0)
            m_hitBuffer[0].collider.TryGetComponent(out newTarget);

        //if new target
        if (CurrentTarget != newTarget)
        {
            CurrentTarget = newTarget;
            PlayerManager.Instance.HandleTargetChange(CurrentTarget);
        }
    }

    private void UpdateMouseInteraction()
    {
        //ダイアログが表示されている場合は動作しない
        if (DialogManager.Instance.IsOnDialog)
            return;

        Camera mainCam = CameraManager.Instance.MainCamera.GetComponent<Camera>();
        Vector2 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        Ray ray = mainCam.ScreenPointToRay(mousePos);

        Interactable newTarget = null;
        RaycastHit hitInfo;

#if UNITY_EDITOR
        if (m_drawDebugRay)
            Debug.DrawRay(ray.origin, ray.direction * FirstPersonReach, Color.cyan);
#endif

        if (Physics.Raycast(ray, out hitInfo, FirstPersonReach, InteractableMask))
        {
            hitInfo.collider.TryGetComponent(out newTarget);
        }

        if (CurrentTarget != newTarget)
        {
            CurrentTarget = newTarget;
            PlayerManager.Instance.HandleTargetChange(CurrentTarget);
        }
    }

    private void UpdateThirdPersonInteraction()
    {
        Interactable newTarget = null;
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, ThirdPersonRange, m_colBuffer, InteractableMask);
        if (hitCount > 0)
        {
            m_colBuffer[0].TryGetComponent(out newTarget);
        }

        if (CurrentTarget != newTarget)
        {
            CurrentTarget = newTarget;
            PlayerManager.Instance.HandleTargetChange(CurrentTarget);
        }
    }

#if UNITY_EDITOR
    [SerializeField] private bool m_drawInteractGizmo = true;
    [SerializeField] private Color m_interactWireColor = new Color(1f, 0.55f, 0f, 1f);
    [SerializeField] private Color m_interactFillColor = new Color(1f, 0.55f, 0f, 0.06f);

    [SerializeField] private bool m_drawDebugRay = true;
    private void OnDrawGizmos()
    {
        if (m_drawInteractGizmo == false) 
            return;

        //プレーヤーがいたらその位置、いなかったらこれが位置を使う
        Vector3 center = transform.position;

        //半透明塗りつぶし
        Gizmos.color = m_interactFillColor;
        Gizmos.DrawSphere(center, ThirdPersonRange);

        //ワイヤー
        Gizmos.color = m_interactWireColor;
        Gizmos.DrawWireSphere(center, ThirdPersonRange);
    }
#endif

}
