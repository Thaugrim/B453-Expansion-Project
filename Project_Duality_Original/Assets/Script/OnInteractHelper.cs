using UnityEngine;
using UnityEngine.Events;

public class OnInteractHelper : MonoBehaviour
{
    public UnityEvent onEnter = new UnityEvent();
    public UnityEvent onExit = new UnityEvent();

    public void DestroyTrigger()
    {
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerController>(out PlayerController _entity))
            return;

        onEnter.Invoke();
    }
    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<PlayerController>(out PlayerController _entity))
            return;

        onExit.Invoke();
    }
}
