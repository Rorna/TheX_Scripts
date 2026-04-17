using System;

public interface IDialog
{
    public void SetData(DialogNode node, Action<string> buttonAction = null);
    public void ShowDialog();
    public void HideDialog();
    public bool IsProgressing();
    public void EndThisDialog();
}
