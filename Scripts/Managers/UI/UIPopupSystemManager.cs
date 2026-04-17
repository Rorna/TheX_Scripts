using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ポップアップ管理
/// バインディングとポップアップ show / hide のときソートオーダーを管理する
/// </summary>
public partial class UISystemManager : BaseManager
{
    private int m_defaultOrder = 10;
    private Dictionary<Type, Type> m_uiPopupBindingDic = new Dictionary<Type, Type>();
    private Stack<UIPopup> m_popupStack = new Stack<UIPopup>();

    public bool HasActivePopup => m_popupStack.Count > 0;
    public void TryControlPopupSortOrder(UIPopup popup, bool isShow)
    {
        if(isShow)
            PushPopup(popup.GetType());
        else
        {
            PopPopup(popup.GetType());
        }
    }

    private void BindPopupUI<TInterface, T>() where T : UIPopup
    {
        Type interfaceType = typeof(TInterface);
        Type uiType = typeof(T);

        if (interfaceType.IsInterface == false)
        {
            return;
        }
        // インタフェースが該当uiクラスに実装されているか確認、なかったら　false
        if (interfaceType.IsAssignableFrom(uiType) == false)
        {
            return;
        }

        m_uiPopupBindingDic[interfaceType] = uiType;
    }

    private void PushPopup(Type type)
    {
        var popup = GetUIObject(type) as UIPopup;
        m_defaultOrder++;
        popup.PopUpCanvas.sortingOrder = m_defaultOrder;

        m_popupStack.Push(popup);
    }

    private void PopPopup(Type type)
    {
        if (m_popupStack.Count == 0)
            return;

        var popup = GetUIObject(type) as UIPopup;
        if (m_popupStack.Peek() != popup)
        {
            return;
        }

        if (popup.isActiveAndEnabled)
        {
            popup.Hide();
        }

        m_defaultOrder--;
        m_popupStack.Pop();
    }

    public void HideAllPopup()
    {
        var popups = Instance.m_popupStack.ToArray();

        foreach (var popup in popups)
        {
            if (popup == null)
                continue;

            popup.Hide();
        }

        Instance.m_popupStack.Clear();
    }
}
