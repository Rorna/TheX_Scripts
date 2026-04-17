using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class StringSignalEmitter : Marker, INotification
{
    [Header("Input Parameter(Key:Value)")]
    [SerializeField] public string param;

    [Header("Repeat")]
    [SerializeField] public bool emitOnce = true;

    public PropertyName id => new PropertyName();
}
