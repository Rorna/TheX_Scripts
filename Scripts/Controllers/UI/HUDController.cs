using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HUDController 
{
    private IHUD m_hud;
    private Interactable m_cachedTarget; //アウトライン管理のための古いターゲットを覚えておいてください
    private InputTypeEnum m_cachedInputType;

    public void Init(IHUD uiInterface)
    {
        //setup HUD
        m_hud = uiInterface;

        BindEvent();
        RefreshActivateHUD();
    }

    public void Release()
    {
        UnBindEvent();
    }

    private void RefreshActivateHUD()
    {
        if (GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.MainMenu)
        {
            HideHUD();
        }
        else
        {
            ShowHUD();
        }
    }

    private void BindEvent()
    {
        //購読
        PlayerManager.OnInteractTargetChanged += HandleInteractTargetChanged;
        DialogManager.OnDialogStateChangedAction += HandleDialogStateChangedAction;
        QuestManager.OnQuestUpdateAction += HandleQuestUpdate;
        ClueManager.OnUpdateClueAction += HandleUpdateClue;
        CameraManager.OnChangeViewAction += HandleChangeView;
        ObservationManager.OnObservationAction += HandleObservationModeChanged;
        CutsceneManager.OnCutsceneStateChangeAction += HandleCutsceneStateChangeAction;
        GameSceneManager.OnCompleteLoadSceneAction += HandleCompleteLoadSceneAction;
        ObservationManager.OnJudgementAction += HandleJudgementAction;
        InputManager.OnClueAction += HandleClueInputAction;
        InputManager.OnReloadAction += HandleReloadAction;
        InputManager.OnFireAction += RefreshAmmo;

        PlayerManager.OnDamageAction += RefreshHP;

        EnemyManager.OnEnemyDamageAction += RefreshTargetHP;
        EnemyManager.OnSpawnEnemyAction += SetTargetHP;

        CombatManager.OnCombatActiveAction += HandleCombatActiveAction;
    }

    private void UnBindEvent()
    {
        PlayerManager.OnInteractTargetChanged -= HandleInteractTargetChanged;
        DialogManager.OnDialogStateChangedAction -= HandleDialogStateChangedAction;
        QuestManager.OnQuestUpdateAction -= HandleQuestUpdate;
        ClueManager.OnUpdateClueAction -= HandleUpdateClue;
        CameraManager.OnChangeViewAction -= HandleChangeView;
        ObservationManager.OnObservationAction -= HandleObservationModeChanged;
        CutsceneManager.OnCutsceneStateChangeAction -= HandleCutsceneStateChangeAction;
        GameSceneManager.OnCompleteLoadSceneAction -= HandleCompleteLoadSceneAction;
        ObservationManager.OnJudgementAction -= HandleJudgementAction;
        InputManager.OnClueAction -= HandleClueInputAction;
        InputManager.OnReloadAction -= HandleReloadAction;
        InputManager.OnFireAction -= RefreshAmmo;

        PlayerManager.OnDamageAction -= RefreshHP;

        EnemyManager.OnEnemyDamageAction -= RefreshTargetHP;
        EnemyManager.OnSpawnEnemyAction -= SetTargetHP;

        CombatManager.OnCombatActiveAction -= HandleCombatActiveAction;
    }

    private void HandleChangeView()
    {
        CameraTypeEnum cameraType = CameraManager.Instance.CurrentCameraType;
        m_hud.RefreshViewChanged(cameraType);
    }

    public void ShowHUD()
    {
        //起動
        if (GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.MainMenu)
            return;

        m_hud.ShowHUD();
    }

    public void HideHUD()
    {
        m_hud.HideHUD();
    }

    public void RefreshHUD()
    {
        HandleChangeView();
        m_hud.RefreshHUDElement(GameSceneManager.Instance.CurrentSceneType);
    }

    private void HandleCompleteLoadSceneAction()
    {
        m_cachedInputType = InputTypeEnum.None;
        m_cachedTarget = null;

        RefreshHUDElement();
        RefreshQuestElement();
        ShowHUD();
        RefreshGlobalVolume();
    }

    private void RefreshQuestElement()
    {
        m_hud.ActiveQuestElement(QuestManager.Instance.CurrentQuest != null);
    }

    private void HandleQuestUpdate(IQuest currentQuest)
    {
        //update quest data.
        //クエストの状態と情報だけ取得して出力。インジケーターオン
        //クリアするとインジケータをオフにしてクエストクリアオブジェクトを有効にした後、コルーチンで3秒後に無効
        if (QuestManager.Instance.QuestState == QuestStateEnum.Complete)
        {
            m_hud.ShowQuestComplete();
            return;
        }

        m_hud.RefreshQuestElement(currentQuest.Title, currentQuest.Description,
            currentQuest.GetAdditionalText());
    }

    private void HandleUpdateClue(Dictionary<string, ClueData> clueDic)
    {
        if (clueDic == null || clueDic.Count == 0)
            return;

        List<ClueData> clueList = new List<ClueData>();
        foreach (var clue in clueDic)
        {
            clueList.Add(clue.Value);
        }

        m_hud.RefreshClueElement(clueList, (clueID) =>
        {
            Facade.Instance.RequestShowCluePopup(clueID);
        });
    }

    //会話スタート、終了時に呼び出されるアクション
    private void HandleDialogStateChangedAction(bool isDialogActive)
    {
        //会話の開始時にインタラクションオブジェクトを無効にする
        if(isDialogActive)
            m_hud.RefreshInteractElement(false);
        else
        {
            var currentTarget = PlayerManager.Instance.CurrentTarget;
            HandleInteractTargetChanged(currentTarget);
        }

        //一人称であればクロスヘアを更新
        if (CameraManager.Instance.CurrentCameraType == CameraTypeEnum.FirstCamera)
            m_hud.RefreshCrossHairElement(!isDialogActive);
    }

    private void HandleInteractTargetChanged(Interactable newTarget)
    {
        RefreshOutline(m_cachedTarget, newTarget);
        RefreshHUDElement(newTarget);

        m_cachedTarget = newTarget;
    }

    private void HandleClueInputAction()
    {
        var currentType = InputManager.Instance.CurrentInputType;
        bool isTurningOff = (currentType == InputTypeEnum.UI);

        Facade.Instance.RequestPlaySFX(DefineString.SFX_CluePopup, Vector3.zero, 0.3f, false);
        if (isTurningOff) //オフ
        {
            Facade.Instance.RequestSwitchInputMap(m_cachedInputType);

            m_hud.SetClueElementActive(false);

            Facade.Instance.RequestHideCluePopup();
        }
        else //オン
        {
            m_cachedInputType = currentType;
            Facade.Instance.RequestSwitchInputMap(InputTypeEnum.UI);

            var clueDic = ClueManager.Instance.CurrentClueDic;
            HandleUpdateClue(clueDic);

            m_hud.SetClueElementActive(true);
        }
    }

    private void RefreshOutline(Interactable oldTarget, Interactable newTarget)
    {
        if (oldTarget != null)
            oldTarget.RefreshOutline(false);

        if (newTarget == null)
            return;

        if (ShouldEnableOutline())
            newTarget.RefreshOutline(true);
    }

    private void RefreshHUDElement(Interactable target = null)
    {
        bool hasTarget = target != null;

        //1.インタラクションUIの更新（Fキーガイドなど）
        bool showInteract = ShouldShowInteractUI(target);
        string message = hasTarget ? target.Message : string.Empty;

        m_hud.RefreshInteractElement(showInteract, message);

        //2. 観察UIの更新（審判/ブレットなど）
        //観察シーンじゃなかったら更新する必要なし
        if (ObservationManager.Instance.Active)
        {
            bool showObserve = ShouldShowObserveUI(target);
            m_hud.RefreshObserveElement(showObserve);
        }

        bool isFirstPerson = CameraManager.Instance.CurrentCameraType == CameraTypeEnum.FirstCamera;
        bool isObservationMode = ObservationManager.Instance.ObservationMode;

        //一人称で観測モードではない場合にのみオン
        bool crossHairActive = isFirstPerson && (isObservationMode == false);
        m_hud.RefreshCrossHairElement(crossHairActive);
    }

    ///<summary>
    ///相互作用
    ///ターゲットが必要+（フィールドシーンまたは観察モードがオンになっている必要がある）
    ///</summary>
    private bool ShouldShowInteractUI(Interactable target)
    {
        //ターゲットなかったら絶対オフ
        if (target == null)
            return false;

        //フィールドシーンだったら、ターゲットがあるときに絶対にオン
        if (GameSceneManager.Instance.CurrentSceneType != SceneTypeEnum.Observe)
            return true;

        //ターゲットが観察対象じゃなかったら、フィールドのように一般の対話UIをオン
        if (target.IsObserveTarget == false)
            return true;

        //ターゲットが観察対象だったら、観察モード（ズームなど）の場合にのみ相互作用UIをオンにする
        return ObservationManager.Instance.ObservationMode;
    }

    ///<summary>
    ///観察UI（審判、ブレットなど）
    ///観察シーンである必要がある+（観察モードがオンになっているか、ターゲットを見ている必要がある）
    ///</summary>
    private bool ShouldShowObserveUI(Interactable target)
    {
        //観察シーンでなければ無条件にオフ
        if (GameSceneManager.Instance.CurrentSceneType != SceneTypeEnum.Observe)
            return false;

        //観察モード常時有効
        if (ObservationManager.Instance.ObservationMode)
            return true;

        //観察モードがオフになっていても、ターゲットを見ている+そのターゲットが観察対象だったら
        if (target != null && target.IsObserveTarget)
            return true;

        //モードがオフになってもターゲットを見ている場合は有効
        return false;
    }

    ///<summary>
    ///現在の状況でアウトラインをオンにする必要があるかどうかを判断するロジック
    ///</summary>
    private bool ShouldEnableOutline()
    {
        //フィールドシーン（Field）、観測モードがオンの場合（True）
        //つまり、「観察シーン　+　観察モードがオフになっている場合」のみ false
        if (GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.Observe)
        {
            //観測シーンでは、観測モード中のみ
            return ObservationManager.Instance.ObservationMode;
        }
        else
        {
            return true;
        }
    }

    //観測モードオン/オフ
    private void HandleObservationModeChanged(bool isObservationMode)
    {
        m_hud.RefreshObservationIndicator(isObservationMode);

        //アウトライン　/　インタラクションなど即時更新
        //ターゲット変わらなくても更新しなければならないから。
        RefreshOutline(m_cachedTarget, m_cachedTarget);
        RefreshHUDElement(m_cachedTarget);

        if(isObservationMode == false)
            Facade.Instance.RequestHideCluePopup();
    }

    private void HandleCutsceneStateChangeAction(CutsceneStateEnum state)
    {
        switch (state)
        {
            case CutsceneStateEnum.Play:
                m_hud.HideHUD();
                break;
            case CutsceneStateEnum.End:
                m_hud.ShowHUD();
                break;
        }
    }

    private void HandleJudgementAction()
    {
        RefreshHUDElement();
    }

    private void HandleReloadAction()
    {
        //インプットタイプ戦闘じゃなかったらリターン
        if (InputManager.Instance.CurrentInputType != InputTypeEnum.Combat)
            return;

        bool isReloading = PlayerManager.Instance.WeaponHandler.IsReloading;
        if (isReloading)
        {
            return;
        }

        float reloadTime = PlayerManager.Instance.WeaponHandler.ReloadTime;
        int maxAmmo = PlayerManager.Instance.WeaponHandler.MaxAmmo;
        string ammoText = maxAmmo + " / " + maxAmmo;

        m_hud.ShowReloadBar(reloadTime, ammoText);
    }

    private void RefreshAmmo()
    {

        //インプットタイプ戦闘じゃなかったらリターン
        if (InputManager.Instance.CurrentInputType != InputTypeEnum.Combat)
            return;

        int currentAmmo = PlayerManager.Instance.WeaponHandler.CurrentAmmo;
        int maxAmmo = PlayerManager.Instance.WeaponHandler.MaxAmmo;

        string ammoText = currentAmmo + " / " + maxAmmo;
        m_hud.RefreshAmmo(ammoText);
    }

    private void RefreshHP(int damage)
    {
        //現在hpが一定未満の場合、HPDangerにtrue -> hudで実行。

        //インプットタイプ戦闘じゃなかったらリターン
        if (InputManager.Instance.CurrentInputType != InputTypeEnum.Combat)
            return;

        int currentHP = PlayerManager.Instance.HP;
        if (currentHP <= 0)
            currentHP = 0;

        int maxHP = PlayerManager.Instance.MaxHP;

        float hpRatio = (float)currentHP / maxHP;

        //hpが30パー未満の場合、オン
        m_hud.ActiveHPDanger(hpRatio < 0.3f);
        m_hud.RefreshHPBar(currentHP, maxHP);
    }

    private void SetTargetHP(EnemyController enemy)
    {
        RefreshTargetHP(enemy, 0);
    }

    private void RefreshTargetHP(EnemyController enemy, float damage)
    {
        int currentHP = enemy.CurrentHP;
        int maxHP = enemy.MaxHP;

        m_hud.RefreshTargetHPBar(currentHP > 0, currentHP, maxHP);
    }

    public void PlayDamageFeedback()
    {
        Facade.Instance.RequestCameraShake();

        //赤く
        m_hud.ShowDamageEffect();
    }

    private void RefreshGlobalVolume()
    {
        Volume volume = Object.FindAnyObjectByType<Volume>();
        if (volume == null)
            return;

        m_hud.SetGlobalVolume(volume);
    }

    private void HandleCombatActiveAction(bool active)
    {
        m_hud.ActiveQuestElement(!active);

        if (active)
        {
            m_hud.RefreshCombatElement(true);
            RefreshAmmo();
            RefreshHP(0);
            return;
        }

        m_hud.RefreshCombatElement(false);
        RefreshHUDElement();
    }
}