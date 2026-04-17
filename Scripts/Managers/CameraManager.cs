using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : BaseManager
{
    public static CameraManager Instance { get; private set; }

    public static event Action OnChangeViewAction;

    public GameObject FirstCamera { get; private set; }
    public GameObject ThirdCamera { get; private set; }
    public GameObject MainCamera { get; private set; }
    public CinemachineCamera FirstCineCam { get; private set; }
    public CinemachineCamera ThirdCineCam { get; private set; }

    public CameraTypeEnum CurrentCameraType => m_controller.m_currentCameraType;

    private CameraController m_controller;

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
        InitCamera();
    }

    public override void ExternalInitManager()
    {
        m_controller = new CameraController();
        m_controller.InitCamera(FirstCamera, ThirdCamera, MainCamera,
            FirstCineCam, ThirdCineCam);
    }

    public override void RefreshManager()
    {
        m_controller.RefreshController();
    }

    public override void ClearManager()
    {
        m_controller.Release();
    }

    private void Update()
    {
        m_controller.UpdateObservationZoom();
    }

    private void InitCamera()
    {
        MainCamera = GameObject.FindWithTag("MainCamera");
        if (MainCamera == null)
            MainCamera = UnityUtil.Instantiate(DefineString.MainCameraPath, Vector3.zero, transform);
        else
        {
            MainCamera.transform.SetParent(transform);
            var cinemachine = MainCamera.GetOrAddComponent<CinemachineBrain>();
            cinemachine.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
        }

        FirstCamera = UnityUtil.Instantiate(DefineString.FirstCameraPath, Vector3.zero, transform);
        ThirdCamera = UnityUtil.Instantiate(DefineString.ThirdCameraPath, Vector3.zero, transform);

        // setup tracking target
        FirstCineCam = FirstCamera.GetComponent<CinemachineCamera>();
        ThirdCineCam = ThirdCamera.GetComponent<CinemachineCamera>();
    }

    public void TrySetPlayerTarget(GameObject player)
    {
        if (player == null) 
            return;

        m_controller.SetPlayerTarget(player);
    }

    public void HandleChangeView()
    {
        OnChangeViewAction?.Invoke();
    }

    public void TryGenerateCameraShake()
    {
        m_controller.GenerateCameraShake();
    }
}