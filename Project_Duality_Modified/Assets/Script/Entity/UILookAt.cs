using UnityEngine;

public class UILookAt : MonoBehaviour
{
    [SerializeField] private Transform transfCamera;

    private void LateUpdate()
    {
        transform.LookAt(transform.position + transfCamera.transform.rotation * Vector3.forward, transfCamera.transform.rotation * Vector3.up); // What is this for?
    }
}
