using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UIHUD : UIObject, IHUD
{
    [Header("Interact")]
    [SerializeField] private TMP_Text m_interactionText;
    [SerializeField] private GameObject m_interactionObj;

    [Header("First Person Cross Hair")]
    [SerializeField] private GameObject m_crossHairObj;

    [Header("Quest")]
    [SerializeField] private TMP_Text m_questTitleText;
    [SerializeField] private TMP_Text m_questDescText;
    [SerializeField] private TMP_Text m_questAdditionalText;

    [SerializeField] private GameObject m_questObj;
    [SerializeField] private GameObject m_questIndicatorObj;
    [SerializeField] private GameObject m_clearQuestObj;
    
    [Header("Clue")]
    [SerializeField] private GameObject m_clueObj;
    [SerializeField] private List<ClueSlot> m_clueSlotList;

    [Header("Observe")] 
    [SerializeField] private GameObject m_observeObj;
    [SerializeField] private Image m_mouseModeImage;
    [SerializeField] private Image m_judgeImage;
    [SerializeField] private GameObject m_bulletContainer;
    [SerializeField] private GameObject m_observationIndicatorObj;

    [Header("Combat")]
    [SerializeField] private GameObject m_combatObj;

    // reload
    [SerializeField] private GameObject m_reloadBarObj;
    [SerializeField] private Image m_reloadFillImage;

    // ammo
    [SerializeField] private GameObject m_ammoObj;
    [SerializeField] private TMP_Text m_ammoText;

    // hp
    [SerializeField] private GameObject m_hpObj;
    [SerializeField] private Animator m_hpBarAnimator;
    [SerializeField] private GameObject m_hpDangerObj;
    [SerializeField] private Image m_hpBarImage;

    // targetHP
    [SerializeField] private GameObject m_targetHPObj;
    [SerializeField] private Image m_targetHpBarImage;

    // volume
    // ダメージエフェクト
    private Tween m_damageTween;
    private Volume m_globalVolume;
    private Vignette m_vignette;

    private List<GameObject> m_bulletList = new List<GameObject>();

    public override void OnShow()
    {
    }

    public override void OnHide()
    {
    }

    public void ShowHUD()
    {
        Show();
    }

    public void HideHUD()
    {
        Hide();
    }

    public void RefreshViewChanged(CameraTypeEnum cameraType)
    {
        switch (cameraType)
        {
            case CameraTypeEnum.FirstCamera:
                if (m_crossHairObj.activeSelf == false)
                    m_crossHairObj.SetActive(true);
                break;

            case CameraTypeEnum.ThirdCamera:
                if (m_crossHairObj.activeSelf)
                    m_crossHairObj.SetActive(false);
                break;
        }
    }

    public void RefreshCrossHairElement(bool active)
    {
        m_crossHairObj.SetActive(active);
    }

    public void RefreshInteractElement(bool active, string text = null)
    {
        if(string.IsNullOrEmpty(text) == false)
            m_interactionText.text = text;
        else
        {
            m_interactionText.text = "";
        }
        m_interactionObj.SetActive(active);
    }

    public IEnumerator UpdateQuestCompleteCoroutine()
    {
        m_questIndicatorObj.SetActive(false);

        m_clearQuestObj.SetActive(true);
        yield return new WaitForSeconds(3.0f);

        m_clearQuestObj.SetActive(false);
    }

    public void ShowQuestComplete()
    {
        StartCoroutine(UpdateQuestCompleteCoroutine());
    }

    public void ActiveQuestElement(bool active)
    {
        m_questObj.SetActive(active);
    }

    public void RefreshQuestElement(string title, string desc, string addText)
    {
        if(m_questObj.activeSelf == false)
            m_questObj.SetActive(true);

        if (m_interactionObj.activeSelf == false)
            m_questIndicatorObj.SetActive(true);

        m_questTitleText.text = title;
        m_questDescText.text = desc;

        string additionalText = addText;
        if (string.IsNullOrEmpty(additionalText))
        {
            m_questAdditionalText.gameObject.SetActive(false);
        }
        else
        {
            m_questAdditionalText.gameObject.SetActive(true);
            m_questAdditionalText.text = additionalText;
        }
    }

    public void RefreshClueElement(List<ClueData> clueList, Action<string> onClickAction)
    {
        // set empty
        foreach (var slot in m_clueSlotList)
        {
            slot.SetEmpty();
        }

        // set ui slot
        for (int i = 0; i < clueList.Count; i++)
        {
            ClueData clue = clueList[i];
            m_clueSlotList[i].Setup(clue.ID, clue.Title, clue.Description,
                clue.SpriteID, onClickAction);
        }
    }

    public void RefreshHUDElement(SceneTypeEnum sceneType)
    {
        m_interactionObj.SetActive(false);

        switch (sceneType)
        {
            case SceneTypeEnum.Field:
                m_clueObj.SetActive(false);
                m_observeObj.SetActive(false);
                m_combatObj.SetActive(false);
                break;
        }
    }

    public void RefreshCombatElement(bool active)
    {
        m_clueObj.SetActive(!active);
        m_observeObj.SetActive(!active);
        m_combatObj.SetActive(active);
    }

    // observation, judgement, bullet
    public void RefreshObserveElement(bool active)
    {
        // refresh bullet
        RefreshJudgementBulletElement();

        m_observeObj.SetActive(active);
        m_clueObj.SetActive(active);
    }

    public void RefreshJudgementBulletElement()
    {
        // プレイヤーマネージャーから必要な弾丸の数を取得
        int requireBulletCount = PlayerManager.Instance.JudgementBullet;
        for (int i = 0; i < requireBulletCount; i++)
        {
            GameObject bullet;

            // 既存のオブジェクトがあれば再利用（プーリング）
            if (m_bulletList.Count > i)
            {
                bullet = m_bulletList[i];
            }
            else // 足りない場合は新規生成してリストへ追加
            {
                string path = DefineString.UIPath + DefineString.JudgementBullet;
                bullet = UnityUtil.Instantiate(path, m_bulletContainer.transform);
                m_bulletList.Add(bullet);
            }

            bullet.gameObject.SetActive(true);
        }

        // 使用しない余分のオブジェクトを無効化
        for (int i = requireBulletCount; i < m_bulletList.Count; i++)
        {
            m_bulletList[i].gameObject.SetActive(false);
        }
    }

    public void RefreshObservationIndicator(bool active)
    {
        m_judgeImage.gameObject.SetActive(!active);
        m_observationIndicatorObj.SetActive(active);
        m_interactionObj.SetActive(!active);
    }

    public void SetClueElementActive(bool isActive)
    {
        m_clueObj.SetActive(isActive);
    }

    public void ShowReloadBar(float reloadTime, string ammoText)
    {
        m_reloadBarObj.SetActive(true);
        m_reloadFillImage.fillAmount = 0;

        //show coroutine
        StartCoroutine(CoReload(reloadTime, ammoText));
    }

    private IEnumerator CoReload(float reloadTime, string ammoText)
    {
        float currentTime = 0f;
        while (currentTime < reloadTime)
        {
            currentTime += Time.deltaTime;
            m_reloadFillImage.fillAmount = currentTime / reloadTime;
            yield return null;
        }

        m_reloadFillImage.fillAmount = 1f;
        m_reloadBarObj.SetActive(false);

        RefreshAmmo(ammoText);
    }

    public void RefreshAmmo(string ammoText)
    {
        m_ammoText.text = ammoText;
    }

    public void RefreshHPBar(int currentHP, int maxHP)
    {
        m_hpBarImage.fillAmount = 1;
        m_hpBarImage.fillAmount = (float)currentHP / maxHP;

        // 現在HPと最大HPが異なる場合（ダメージを受けた場合
        if (currentHP != maxHP)
        {
            m_hpBarAnimator.SetTrigger("Damaged");
        }
    }

    public void ActiveHPDanger(bool active)
    {
        if (m_hpDangerObj.activeSelf == active)
            return;

        m_hpDangerObj.SetActive(active);
    }

    public void RefreshTargetHPBar(bool active, int currentHP, int maxHP)
    {
        if(active == false)
        {
            m_targetHPObj.SetActive(false);
            return;
        }

        if (m_targetHPObj.activeSelf == false)
            m_targetHPObj.SetActive(true);

        m_targetHpBarImage.fillAmount = 1;
        m_targetHpBarImage.fillAmount = (float)currentHP / maxHP;
    }

    public void SetGlobalVolume(Volume volume)
    {
        m_globalVolume = volume;

        if (m_globalVolume != null && m_globalVolume.profile.TryGet(out m_vignette))
        {
            m_vignette.intensity.value = 0f;
        }
    }

    public void ShowDamageEffect()
    {
        if (m_vignette == null)
            return;

        // 1. 実行中のTweenが存在する場合は破棄（Kill）し、エフェクトの重複を防ぐ。
        m_damageTween?.Kill();

        // 2. 強制的に値を0へリセットし、Yoyo（往復）ループの基準点を確定させる。
        m_vignette.intensity.value = 0f;

        // 3. ダメージ演出のTween再生を開始
        m_damageTween = DOTween.To(() => m_vignette.intensity.value,
                x => m_vignette.intensity.value = x,
                0.45f, 0.1f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad);
    }
}