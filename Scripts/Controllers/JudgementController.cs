using System.Collections;
using UnityEngine;

public class JudgementController
{
    private bool m_controllerActive;
    private int m_bullet;

    private ObservationManager m_manager;

    private string m_xName;

    public void Init(ObservationManager manager)
    {
        BindEvent();
        m_manager = manager;
        m_xName = null;
    }

    public void UpdateActivation(bool isActive)
    {
        m_controllerActive = isActive;

        if(isActive)
            m_bullet = PlayerManager.Instance.JudgementBullet; //コントローラがオンのときの設定。
    }


    private void BindEvent()
    {
        InputManager.OnJudgementAction += HandleJudgement;
        CutsceneManager.OnCutsceneStateChangeAction += HandleCutsceneChangeAction;
    }

    private void UnBindEvent()
    {
        InputManager.OnJudgementAction -= HandleJudgement;
        CutsceneManager.OnCutsceneStateChangeAction -= HandleCutsceneChangeAction;
    }

    private void HandleJudgement()
    {
        if (m_controllerActive == false)
            return;

        TryJudgement();
    }

    private void TryJudgement()
    {
        //ターゲットがある場合にのみ有効
        var currentTarget = Facade.Instance.RequestGetCurrentTarget();
        if (currentTarget == null)
            return;

        //ダイアログアクションの実行。
        Facade.Instance.RequestPlaySFX(DefineString.SFX_Click, Vector3.zero, 0.3f, false);
        Facade.Instance.RequestRunSituationAction(SituationTypeEnum.Judgement);
    }

    public void RunJudgement(string xName)
    {
        if (m_controllerActive == false)
            return;

        if (string.IsNullOrEmpty(xName)) 
            return;

        m_xName = null;

        //この名前と現在の観察対象の名前を比較する
        string observationObjName = ObservationManager.Instance.CurrentTargetName;
        if (observationObjName == xName)
        {
            m_xName = xName;

            //感染者カットシーン再生
            Facade.Instance.RequestRunSituationAction(SituationTypeEnum.PlayJudgementCutsceneSuccess);
        }
        else
        {
            //非感染者カットシーン再生
            Facade.Instance.RequestRunSituationAction(SituationTypeEnum.PlayJudgementCutsceneFail);

            //弾丸を減らす
            m_bullet--;
            Facade.Instance.RequestRefreshJudgementBullet(m_bullet);
        }

        //成功、失敗関係なくすべてのシンオブジェクトの無効化リクエスト
        Facade.Instance.RequestRegisterInactive(observationObjName);
        ObservationManager.Instance.HandleJudgementAction();
    }

    private void HandleCutsceneChangeAction(CutsceneStateEnum state)
    {
        if (GameSceneManager.Instance.CurrentSceneType != SceneTypeEnum.Observe)
            return;

        if (state == CutsceneStateEnum.End)
        {
            //objを無効
            string targetName = ObservationManager.Instance.CurrentTargetName;

            if(targetName != null)
                Facade.Instance.RequestActiveObject(targetName, false);

            m_bullet = PlayerManager.Instance.JudgementBullet;
            if (m_bullet == 0)
            {
                m_manager.StartCoroutine(CoRunSituationAction());
                return;
            }

            //感染者の場合、戦闘開始のリクエスト
            Facade.Instance.RequestStartCombat(m_xName);
        }
    }

    private IEnumerator CoRunSituationAction()
    {
        yield return new WaitForSeconds(1.5f);
        Facade.Instance.RequestRunSituationAction(SituationTypeEnum.GameOverZeroBullet);
    }

    public void Release()
    {
        UnBindEvent();
    }
}
