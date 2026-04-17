using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public interface IHUD
{
    public void ShowHUD();
    public void HideHUD();
    public void RefreshViewChanged(CameraTypeEnum cameraType);
    public void RefreshInteractElement(bool active, string text = null);
    public void RefreshQuestElement(string title, string desc, string addText);
    public void ActiveQuestElement(bool active);
    public void RefreshClueElement(List<ClueData> clueList, Action<string> onClickAction);
    public void ShowQuestComplete();
    public void RefreshCrossHairElement(bool active);
    public void RefreshHUDElement(SceneTypeEnum sceneType);
    public void RefreshObserveElement(bool active);
    public void RefreshObservationIndicator(bool active);
    public void SetClueElementActive(bool isActive);

    // combat
    public void ShowReloadBar(float reloadTime, string ammoText);
    public void RefreshAmmo(string ammoText);
    public void RefreshHPBar(int currentHP, int maxHP);
    public void ActiveHPDanger(bool active);
    public void RefreshTargetHPBar(bool active, int currentHP, int maxHP);

    // volume
    public void SetGlobalVolume(Volume volume);
    public void ShowDamageEffect();
    public void RefreshCombatElement(bool active);
}
