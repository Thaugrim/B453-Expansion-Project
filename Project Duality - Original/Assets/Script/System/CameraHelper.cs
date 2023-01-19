using UnityEngine;

using UnityEngine.Rendering;

public class CameraHelper : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent<MeshRenderer>(out MeshRenderer _mesh))
            _mesh.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent<MeshRenderer>(out MeshRenderer _mesh))
            _mesh.shadowCastingMode = ShadowCastingMode.On;
    }
}
