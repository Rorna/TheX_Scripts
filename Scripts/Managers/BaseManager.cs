using UnityEngine;

///<summary>
///インターナルは、クラスの内部データを参照して初期化する場合。
///エクステンナルは、外部データを参照して初期化する場合。
///</summary>
public abstract class BaseManager : MonoBehaviour
{
    public abstract void CreateManager(); //マネージャインスタンス化
    public abstract void InternalInitManager(); //マネージャ内部変数の設定
    public abstract void ExternalInitManager(); //内部データを外部からのデータを参照して設定する場合に使う
    public abstract void RefreshManager();
    public abstract void ClearManager();

}
