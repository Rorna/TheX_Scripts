using Unity.Cinemachine;
using UnityEngine;

///<summary>
///カメラとカーソルを設定して制御する
///</summary>
public class CameraController
{
    public CameraTypeEnum m_currentCameraType;
    private int m_currentPriority;

    //cam
    private GameObject m_firstCamera;
    private GameObject m_thirdCamera;
    private GameObject m_mainCamera;

    //cineCam
    private CinemachineCamera m_firstCineCam;
    private CinemachineCamera m_thirdCineCam;

    private float m_scrollInput = 0f;  //マウスホイール値
    private float m_defaultFOV = 60f; //デフォルト画面サイズ
    private float m_minFOV = 30f;     //最大拡大サイズ

    private float m_targetFOV;
    private float m_currentFOV;

    private CinemachineImpulseSource m_impulseSource;

    public void InitCamera(GameObject firstCam, GameObject thirdCam, GameObject mainCam,
        CinemachineCamera firstCineCam, CinemachineCamera thirdCineCam)
    {
        m_firstCamera = firstCam;
        m_thirdCamera = thirdCam;
        m_mainCamera = mainCam;

        m_firstCineCam = firstCineCam;
        m_thirdCineCam = thirdCineCam;

        m_currentPriority = m_thirdCineCam.Priority;


        SetFirstCamFOV();
        BindEvent();

        RefreshViewByScene();
        RefreshCursorBySceneType();
    }

    private void SetFirstCamFOV()
    {
        if (m_firstCineCam == null)
            return;

        m_defaultFOV = m_firstCineCam.Lens.FieldOfView;
        m_targetFOV = m_defaultFOV;
        m_currentFOV = m_defaultFOV;

    }

    private void BindEvent()
    {
        InputManager.OnMouseZoomAction += ZoomInput;
        InputManager.OnSwitchInputMap += ActiveCursorByInput;
        GameSceneManager.OnCompleteLoadSceneAction += HandleCompleteLoadSceneAction;
    }

    private void UnBindEvent()
    {
        InputManager.OnMouseZoomAction -= ZoomInput;
        InputManager.OnSwitchInputMap -= ActiveCursorByInput;
        GameSceneManager.OnCompleteLoadSceneAction -= HandleCompleteLoadSceneAction;
    }

    public void Release()
    {
        UnBindEvent();
    }

    public void RefreshController()
    {
        RefreshViewByScene();
    }

    private void RefreshCursorBySceneType()
    {
        if(GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.MainMenu)
            ActiveCursor(true);
        else
        {
            ActiveCursor(false);
        }
    }

    public void UpdateObservationZoom()
    {
        if (ObservationManager.Instance.ObservationMode == false)
            return;

        if (m_currentCameraType != CameraTypeEnum.FirstCamera) 
            return;

        if (m_firstCineCam == null) 
            return;

        if (m_scrollInput != 0)
        {
            m_targetFOV -= m_scrollInput * 10f;
            m_targetFOV = Mathf.Clamp(m_targetFOV, m_minFOV, m_defaultFOV);
            m_scrollInput = 0f;
        }

        m_currentFOV = Mathf.Lerp(m_currentFOV, m_targetFOV, Time.deltaTime * 10f);
        m_firstCineCam.Lens.FieldOfView = m_currentFOV;
    }

    private void ZoomInput(float delta)
    {
        // カーソルがPopupの上にある　ー＞　Zoomしない
        GameObject uiObj = UnityUtil.GetTopUIOnCursorPos();
        if (uiObj != null)
        {
            var popup = uiObj.GetComponentInParent<UIPopup>();
            if (popup != null)
                return;
        }

        m_scrollInput = delta;
    }

    private void ResetZoom()
    {
        m_targetFOV = m_defaultFOV;
        m_currentFOV = m_defaultFOV;
        if (m_firstCineCam != null)
        {
            m_firstCineCam.Lens.FieldOfView = m_defaultFOV;
        }
    }

    public void ActiveAxisInputCamera(bool active)
    {
        var fistCamAxisInput = m_firstCamera.GetComponent<CinemachineInputAxisController>();
        var thirdCamAxisInput = m_thirdCamera.GetComponent<CinemachineInputAxisController>();

        fistCamAxisInput.enabled = active;
        thirdCamAxisInput.enabled = active;
    }

    private void RefreshViewByScene()
    {
        var sceneType = GameSceneManager.Instance.CurrentSceneType;
        switch (sceneType)
        {
            //三人称
            case SceneTypeEnum.Field:
                m_thirdCineCam.Priority = 10;
                m_firstCineCam.Priority = -1;
                m_currentCameraType = CameraTypeEnum.ThirdCamera;

                m_thirdCamera.gameObject.SetActive(true);
                m_firstCineCam.gameObject.SetActive(false);

                //切り替えたら即時位置同期
                m_thirdCineCam.PreviousStateIsValid = false;

                m_mainCamera.GetComponent<Camera>().cullingMask = -1; //everything
                break;

            //1人称
            case SceneTypeEnum.Observe:
                m_firstCineCam.Priority = 10;
                m_thirdCineCam.Priority = -1;
                m_currentCameraType = CameraTypeEnum.FirstCamera;

                m_thirdCamera.gameObject.SetActive(false);
                m_firstCineCam.gameObject.SetActive(true);

                //切り替えたら即時位置同期
                m_firstCineCam.PreviousStateIsValid = false;

                ResetZoom();

                //except player layer
                int playerLayerBit = 1 << LayerMask.NameToLayer("Player");
                m_mainCamera.GetComponent<Camera>().cullingMask = -1 & ~playerLayerBit;

                break;
        }

        CameraManager.Instance.HandleChangeView();
    }

    //カメラフォローターゲットの設定
    public void SetPlayerTarget(GameObject player)
    {
        GameObject headTarget = UnityUtil.GetChildObject(player, DefineString.HeadCamera);
        Transform targetTr = headTarget != null ? headTarget.transform : player.transform;

        //シネマシーンターゲットを紐づける
        if (m_firstCineCam != null) 
            m_firstCineCam.Target.TrackingTarget = targetTr;

        if (m_thirdCineCam != null) 
            m_thirdCineCam.Target.TrackingTarget = targetTr;

        var brain = m_mainCamera.GetComponent<CinemachineBrain>();

        //以前の情報の初期化
        if (brain != null) 
            brain.enabled = false;

        //メインカメラプレーヤーの位置、角度で同期
        if (m_mainCamera != null)
        {
            m_mainCamera.transform.position = targetTr.position;
            m_mainCamera.transform.rotation = targetTr.rotation;
        }

        //一人称カメラ回転軸を「プレイヤー」を基準に即座に設定
        if (m_currentCameraType == CameraTypeEnum.FirstCamera)
        {
            SyncRotationToTargetImmediately(targetTr);
        }

        //仮想カメラ内部状態リセット
        if (m_firstCineCam != null) 
            m_firstCineCam.PreviousStateIsValid = false;

        if (m_thirdCineCam != null)
            m_thirdCineCam.PreviousStateIsValid = false;

        //ブレインオン、カメラの即時切り替え
        //(これでブレインは、移動したばかりのMainCamera位置からレンダリングを開始 ->即カット)
        //切り替えのとき遅れなし
        if (brain != null) 
            brain.enabled = true;

        m_impulseSource = player.GetComponent<CinemachineImpulseSource>();
    }

    //ターゲットが見ている方向に即座に同期する
    private void SyncRotationToTargetImmediately(Transform targetTr)
    {
        if (m_firstCamera == null) 
            return;

        var panTilt = m_firstCamera.GetComponent<CinemachinePanTilt>();
        if (panTilt == null)
            return;

        //プレイヤーが見る方向
        Vector3 targetEuler = targetTr.eulerAngles;

        //カメラ軸の強制割り当て
        panTilt.PanAxis.Value = targetEuler.y;
        panTilt.TiltAxis.Value = targetEuler.x;
    }

    private void HandleCompleteLoadSceneAction()
    {
        var dummyCamArr = GameObject.FindGameObjectsWithTag(DefineString.DummyCamera);
        foreach (var dummyCam in dummyCamArr)
        {
            dummyCam.SetActive(false);
        }
    }

    private void ActiveCursorByInput()
    {
        var currentInputType = InputManager.Instance.CurrentInputType;
        switch (currentInputType)
        {
            case InputTypeEnum.Field:
            case InputTypeEnum.Observe:
            case InputTypeEnum.Combat:
                ActiveCursor(false);
                ResetZoom();
                break;
            case InputTypeEnum.Dialog:
            case InputTypeEnum.ObservationMode:
            case InputTypeEnum.UI:
            case InputTypeEnum.Ending:
                ActiveCursor(true);
                break;
        }
    }

    //カーソルの起動中は、カメラの動きを止めなければならない
    public void ActiveCursor(bool active)
    {
        if (active)
        {
            Cursor.lockState = CursorLockMode.None; //マウスを有効にする
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; //マウスを隠す
        }

        ActiveAxisInputCamera(!active);
    }

    public void GenerateCameraShake()
    {
        if (m_impulseSource != null)
        {
            m_impulseSource.GenerateImpulse();
        }
    }
}