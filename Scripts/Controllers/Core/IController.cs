using UnityEngine;

public interface IController
{
    public void BindEvent();
    public void UnBindEvent();
    public void Release();
    public void RefreshController();
}
