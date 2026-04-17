using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// タイムラインコンポーネント（PlayableDirector）を直接制御するハンドラー。
/// トラックのバインディング、再生、一時停止など、タイムライン関連のロジックを専任する。
/// </summary>
public class TimelineHandler
{
    private PlayableDirector m_director;
    private GameObject m_cutsceneRoot;

    private GameObject m_boundTargetObj;　// カットシーン終了後ターゲット位置の復元用

    private Vector3 m_originTargetPos;
    private Quaternion m_originTargetRot;

    public void Init(GameObject cutsceneRoot)
    {
        m_cutsceneRoot = cutsceneRoot;
        m_director = cutsceneRoot.GetComponentInChildren<PlayableDirector>();
    }
  
    public void Play()
    {
        if (m_director != null)
            m_director.Play();
    }

    public void Pause()
    {
        if (m_director != null)
            m_director.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }

    public void Resume()
    {
        if (m_director != null)
            m_director.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

    /// <summary>
    /// JSONデータを基に、トラックへオブジェクトを動的に紐づける
    /// </summary>
    public void SetDynamicBindings(Dictionary<string, string> bindingDic)
    {
        foreach (var pair in bindingDic)
        {
            string trackName = pair.Key; // "Target_Activation"
            string value = pair.Value; // "CurrentTarget"

            // 1. トラックの検索
            TrackAsset track = FindTrackByName(trackName);
            if (track == null)
            {
                Debug.LogWarning($"[TimelineHandler] トラックが見つかりません: {trackName}");
                continue;
            }

            // 2. トラックの型に応じたバインディングの実行
            BindTrack(track, value);
        }
    }

    private TrackAsset FindTrackByName(string trackName)
    {
        var timelineAsset = m_director.playableAsset as TimelineAsset;
        if (timelineAsset == null) 
            return null;

        foreach (var track in timelineAsset.GetOutputTracks())
        {
            if (track.name == trackName)
                return track;
        }
        return null;
    }

    private void BindTrack(TrackAsset track, string value)
    {
        // 1. Activation Trackの紐づけ
        if (track is ActivationTrack activationTrack)
        {
            BindActivationTrack(activationTrack, value);
            return;
        }

        // 2. Cinemachine Trackの紐づけ
        if (track is CinemachineTrack cinemachineTrack)
        {
            BindCameraBrain(cinemachineTrack);
            return;
        }
    }

    private void BindActivationTrack(ActivationTrack track, string value)
    {
        GameObject targetObj = ResolveActivationTarget(value);
        if (targetObj != null)
        {
            m_director.SetGenericBinding(track, targetObj);
        }
    }

    private void BindCameraBrain(CinemachineTrack track)
    {
        var mainCam = CameraManager.Instance.MainCamera;
        var brain = mainCam != null ? mainCam.GetComponent<CinemachineBrain>() : null;

        if (brain != null)
        {
            m_director.SetGenericBinding(track, brain);
        }
    }

    /// <summary>
    /// 文字列（Value）を解析し、実際にバインディングするゲームオブジェクトを特定するロジック。
    /// </summary>
    private GameObject ResolveActivationTarget(string value)
    {
        // 現在インタラクト中のターゲット
        if (value == DefineString.CurrentTarget)
        {
            // 1. カットシーンプレハブ内の「ダミー」を検索（位置参照用）
            // カットシーンプレハブの子オブジェクト名 = "CurrentTarget"
            GameObject dummyObj = UnityUtil.GetChildObject(m_cutsceneRoot, value);

            if (dummyObj == null)
            {
                return null;
            }

            // 2. 実際のターゲットを取得
            string realTargetName;
            if (GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.Observe)
                realTargetName = ObservationManager.Instance.CurrentTargetName;
            else
            {
                realTargetName = DialogManager.Instance.CurrentDialogObjName;
            }
            GameObject realTarget = Facade.Instance.RequestSceneObject(realTargetName);

            if (realTarget == null)
            {
                return null;
            }

            // 3. 位置の同期（ダミーの位置へ実際のターゲットを移動）
            // 後で復元するために元の位置を保存しておく
            m_boundTargetObj = realTarget;
            m_originTargetPos = realTarget.transform.position;
            m_originTargetRot = realTarget.transform.rotation;

            realTarget.transform.position = dummyObj.transform.position;
            realTarget.transform.rotation = dummyObj.transform.rotation;

            // 4. ダミーを非アクティブにし、実際のターゲットを返す
            dummyObj.SetActive(false);
            return realTarget;
        }
        else
        {
            return null;
        }
    }

    // カットシーン終了後の後処理（位置の復元など）
    public void RestoreBindings()
    {
        if (m_boundTargetObj != null)
        {
            m_boundTargetObj.transform.position = m_originTargetPos;
            m_boundTargetObj.transform.rotation = m_originTargetRot;
            m_boundTargetObj = null;
        }
    }

    public void ClearHandler()
    {
        m_director = null;
        m_originTargetPos = Vector3.zero;
        m_originTargetRot = Quaternion.identity;
    }
}