using UnityEngine;
using UnityEngine.Events;

public class OnEnableEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent onEnableEvent;

    private void OnEnable() => onEnableEvent.Invoke();
}
