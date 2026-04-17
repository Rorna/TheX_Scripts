using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController
{
    private AudioSource m_bgmSource;
    private List<AudioSource> m_sfxPool;

    private SoundManager m_manager;

    private Coroutine m_fadeCoroutine;

    private Dictionary<string, Dictionary<SceneBGMTypeEnum, SceneBGMData>> m_sceneSoundDic =>
        m_manager.m_sceneSoundInfoDic;

    public void Init(SoundManager manager)
    {
        m_manager = manager;

        InitAudioSource();
        BindEvent();
    }

    private void InitAudioSource()
    {
        if (m_manager == null)
            return;

        //BGM用のオーディオソース設定
        GameObject bgmObj = new GameObject("BGM_Source");
        bgmObj.transform.SetParent(m_manager.transform);

        m_bgmSource = bgmObj.AddComponent<AudioSource>();
        m_bgmSource.loop = true; //BGM 無限ループ

        //SFX用オーディオソースPOOLING、まず20個程度
        m_sfxPool = new List<AudioSource>();
        for (int i = 0; i < 20; i++)
        {
            GameObject sfxObj = new GameObject($"SFX_Source_{i}");
            sfxObj.transform.SetParent(m_manager.transform);

            AudioSource source = sfxObj.AddComponent<AudioSource>();
            source.playOnAwake = false;

            m_sfxPool.Add(source);
        }
    }

    private void BindEvent()
    {
        GameSceneManager.OnStartLoadSceneAction += StopSceneSound;
        GameSceneManager.OnCompleteLoadSceneAction += PlayCurrentSceneDefaultBGM;
    }

    private void UnBindEvent()
    {
        GameSceneManager.OnStartLoadSceneAction -= StopSceneSound;
        GameSceneManager.OnCompleteLoadSceneAction -= PlayCurrentSceneDefaultBGM;
    }

    public void RefreshController()
    {

    }

    private void StopSceneSound()
    {
        StopSceneSFX();
        //fadeIn Stop
        StopCurrentBGM(true, 1.0f);
    }

    private void StopSceneSFX()
    {
        foreach (var audio in m_sfxPool)
        {
            audio.Stop();
        }
    }

    private void PlayCurrentSceneDefaultBGM()
    {
        //シーン名でマネージャからSceneSoundInfoデータを取得
        string currentSceneName = GameSceneManager.Instance.CurrentSceneName;
        var sceneSoundData = m_manager.GetSceneSoundData(currentSceneName);
        if (sceneSoundData == null)
            return;

        //シンサウンド内部のサウンドIDでオーディオクリップを取得
        var sceneBGMData = sceneSoundData[SceneBGMTypeEnum.DefaultBGM];
        AudioClip clip = m_manager.GetAudioClip(sceneBGMData.SoundID);
        if (clip == null)
            return;

        //フェードインでプレイ
        PlayBGM(clip, sceneBGMData.Volume, true, 1f);
    }

    public void PlaySceneBGM(SceneBGMData bgmTypeInfo, float bgmVolume, bool fadeIn, float fadeDuration)
    {
        AudioClip clip = m_manager.GetAudioClip(bgmTypeInfo.SoundID);
        if (clip == null)
            return;

        float volume = bgmVolume == 0 ? bgmTypeInfo.Volume : bgmVolume;

        //フェードインでプレイ
        PlayBGM(clip, volume, true, 1f);
    }

    //オーディオクリップとサウンドデータ、音量などを受け取ります。
    public void PlayBGM(AudioClip clip, float volume,
        bool fadeOut, float duration = 0f)
    {
        //オーディオ設定時にフェードアウトの有無によって設定する
        if (clip == null)
            return;

        m_bgmSource.clip = clip;
        m_bgmSource.spatialBlend = 0; //BGM 0
        m_bgmSource.volume = fadeOut ? 0f : volume;

        if (fadeOut)
        {
            FadeInBGM(volume, duration);
        }

        m_bgmSource.Play();
    }

    public void StopCurrentBGM(bool fadeOut, float duration)
    {
        //即時停止
        if (fadeOut == false)
        {
            m_bgmSource.Stop();
            return;
        }

        //コルーチン
        FadeOutBGM(duration);
    }

    public void FadeInBGM(float targetVolume, float duration)
    {
        if (m_bgmSource == null)
            return;

        if (m_fadeCoroutine != null)
            m_manager.StopCoroutine(m_fadeCoroutine);

        m_fadeCoroutine = m_manager.StartCoroutine(CoFadeIn(targetVolume, duration));
    }

    public void FadeOutBGM(float duration)
    {
        if (m_bgmSource == null || m_bgmSource.isPlaying == false)
            return;

        if (m_fadeCoroutine != null)
            m_manager.StopCoroutine(m_fadeCoroutine);

        m_fadeCoroutine = m_manager.StartCoroutine(CoFadeOut(duration));
    }

    //どんどん小さく
    private IEnumerator CoFadeOut(float duration)
    {
        float startVolume = m_bgmSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            m_bgmSource.volume = Mathf.Lerp(startVolume, 0f, (timer / duration));
            yield return null;
        }

        m_bgmSource.volume = 0f;
        m_bgmSource.Stop();
    }

    //どんどん大きく
    private IEnumerator CoFadeIn(float targetVolume, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            m_bgmSource.volume = Mathf.Lerp(0f, targetVolume, (timer / duration));
            yield return null;
        }

        m_bgmSource.volume = targetVolume;
    }

    //オーディオクリップとサウンドデータ、音量などを取得
    public void PlaySFX(AudioClip clip, SoundData data, Vector3? position,
        float volume, bool fadeIn, float duration = 0f)
    {
        if (clip == null)
            return;

        //再生中のオーディオソースを探す
        AudioSource availableSource = null;
        foreach (var source in m_sfxPool)
        {
            if (source.isPlaying == false)
            {
                availableSource = source;
                break;
            }
        }

        //全部再生中 ->一番最初のもの
        if (availableSource == null)
        {
            availableSource = m_sfxPool[0];
        }

        //3D位置設定
        if (position.HasValue)
        {
            availableSource.transform.position = position.Value;
        }

        availableSource.clip = clip;
        availableSource.volume = volume;
        availableSource.spatialBlend = data.SpatialBlendRatio; //3D -> 1
        availableSource.loop = data.Loop;
        availableSource.Play();
    }

    public void Release()
    {
        UnBindEvent();
    }
}