using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プールを使う
/// </summary>
public class SoundManager : BaseManager
{
    public static SoundManager Instance { get; private set; }

    private Dictionary<string, SoundData> m_soundInfoDic; //master db
    public Dictionary<string, Dictionary<SceneBGMTypeEnum, SceneBGMData>> m_sceneSoundInfoDic;
    private SoundController m_controller;

    private GameObject m_bgmSourceObj;
    private GameObject m_sfxSourceObj;

    private AudioSource m_bgmSource;
    private List<AudioSource> m_sfxPool;

    public override void CreateManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    public override void InternalInitManager()
    {
        InitSoundInfoDic();
        InitSceneSoundInfoDic();

        m_controller = new SoundController();
        m_controller.Init(this);
    }

    private void InitSoundInfoDic()
    {
        m_soundInfoDic = new Dictionary<string, SoundData>();

        string jsonPath = DefineString.JsonSoundPath + DefineString.JsonSoundInfo;
        string jsonText = JsonUtil.LoadJson(jsonPath);

        var loadedData = JsonUtil.ParseJson<SoundInfo>(jsonText);

        m_soundInfoDic = loadedData.SoundInfoDic;
    }

    private void InitSceneSoundInfoDic()
    {
        m_sceneSoundInfoDic = new Dictionary<string, Dictionary<SceneBGMTypeEnum, SceneBGMData>>();

        string jsonPath = DefineString.JsonSoundPath + DefineString.JsonSceneSoundInfo;
        string jsonText = JsonUtil.LoadJson(jsonPath);

        var loadedData = JsonUtil.ParseJson<SceneSoundInfo>(jsonText);
        m_sceneSoundInfoDic = loadedData.SceneSoundDic;
    }


    public override void ExternalInitManager()
    {
    }

    public void TryStopBGM(bool fadeOut, float fadeDuration)
    {
        m_controller.StopCurrentBGM(fadeOut, fadeDuration);
    }

    public void TryPlayBGM(string soundID, float volume, 
        bool fadeIn, float fadeDuration)
    {
        if (m_soundInfoDic.TryGetValue(soundID, out SoundData data) == false)
        {
            return;
        }

        // オーディオクリップ取得
        AudioClip clip = Resources.Load<AudioClip>(data.Path);

        //設定したボリュームがなかったら、デフォルトのボリュームで設定 
        float targetVolume = volume >= 0f ? volume : data.DefaultVolume;
        m_controller.PlayBGM(clip, targetVolume, fadeIn, fadeDuration);
    }

    public void TryPlaySceneBGM(SceneBGMTypeEnum bgmType, float volume, bool fadeIn, 
        float fadeDuration = 0f)
    {
        string currentSceneName = GameSceneManager.Instance.CurrentSceneName;
        if (m_sceneSoundInfoDic.TryGetValue(currentSceneName, out var sceneBGMInfo) == false)
            return;

        if (sceneBGMInfo.TryGetValue(bgmType, out var bgmTypeInfo) == false)
            return;

        m_controller.PlaySceneBGM(bgmTypeInfo, volume, fadeIn, fadeDuration);
    }

    public void TryPlaySFX(string soundID, Vector3? position, 
        float volume, bool fadeIn, float duration)
    {
        if (m_soundInfoDic.TryGetValue(soundID, out SoundData data) == false)
        {
            return;
        }

        AudioClip clip = Resources.Load<AudioClip>(data.Path);

        float finalVolume = volume >= 0f ? volume : data.DefaultVolume;
        m_controller.PlaySFX(clip, data, position, finalVolume, fadeIn, duration);
    }

    public SoundData GetSoundData(string soundID)
    {
        if (m_soundInfoDic.TryGetValue(soundID, out SoundData data) == false)
        {
            return null;
        }

        return data;
    }

    public Dictionary<SceneBGMTypeEnum, SceneBGMData> GetSceneSoundData(string sceneName)
    {
        if (m_sceneSoundInfoDic.TryGetValue(sceneName,
                out var sceneSoundData) == false)
            return null;

        return sceneSoundData;
    }

    public AudioClip GetAudioClip(string soundID)
    {
        // マスター DB に soundID があるか確認
        if (m_soundInfoDic.TryGetValue(soundID, out SoundData data) == false)
            return null;

        AudioClip clip = Resources.Load<AudioClip>(data.Path);
        return clip;
    }

    public override void RefreshManager()
    {
    }

    public override void ClearManager()
    {
        m_controller.Release();
    }
}
