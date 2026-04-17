using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// シグナルをカットシーンマネージャーへ伝達する中継クラス。
/// 
/// 実装の背景
/// シグナルレシーバーからオブジェクトのスクリプトを介して関数を呼び出す必要があるが、
/// カットシーンマネージャーはランタイム時に動的生成されるため、エディター上で事前設定ができない。
/// その問題を解決するため、シーンに配置された本クラスがシグナルを受け取り、窓口へ伝達する役割を担う。
/// </summary>
public class SignalConnector : MonoBehaviour, INotificationReceiver
{
    // 実行済みのマーカーを保持するリスト（HashSetのため重複しない）
    private HashSet<StringSignalEmitter> m_processedMarkers = new HashSet<StringSignalEmitter>();

    public void OnStringSignal(string param)
    {
        if (string.IsNullOrEmpty(param) || param.Contains(":") == false)
        {
            return;
        }

        // 文字列の分割（Split）処理
        string[] strArr = param.Split(':');

        // KeyとValueの抽出
        string key = strArr[0];
        string value = strArr[1];

        Facade.Instance.RequestHandleCutsceneSignal(key, value);
    }

    /// <summary>
    /// シグナルトラックから呼び出されるコールバック
    /// </summary>
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is StringSignalEmitter emitter)
        {
            // 1. emitOnceが有効、かつ既に実行済みの場合は無視してリターン
            if (emitter.emitOnce && m_processedMarkers.Contains(emitter))
                return;

            // 2. 実行済みリストへ登録
            if (emitter.emitOnce)
                m_processedMarkers.Add(emitter);

            OnStringSignal(emitter.param);
        }
    }
}