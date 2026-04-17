
// clue popup 出力

using UnityEngine;

public class CluePopupController
{
    private ICluePopup m_popup;

    public void Init(ICluePopup popup)
    {
        m_popup = popup;
    }

    public void ShowPopup(ClueData data)
    {
        m_popup.ShowPopup(data.SpriteID, data.Title, data.Description);
    }

    public void HidePopup()
    {
        if (m_popup != null && m_popup.IsActive)
        {
            m_popup.HidePopup();
        }
    }
}
