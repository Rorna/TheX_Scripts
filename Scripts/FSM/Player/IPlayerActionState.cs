using UnityEngine;

public interface IPlayerActionState : IState
{
    public void HandleDialogInput(bool isDialogActive);
    public void HandleFireInput();
    public void HandleReloadInput();
}
