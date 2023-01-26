using UnityEngine;

public class ColorInverter : MonoBehaviour
{
    [SerializeField] private MeshRenderer render;
    [SerializeField] private Material blackColor;
    [SerializeField] private Material whiteColor;

    private bool color = true;

    private void OnValidate()
    {
        render = this.GetComponent<MeshRenderer>();
    }

    public void ColorInvert(bool value)
    {
        color = value;

        if (color)
            render.material = whiteColor;
        else
            render.material = blackColor;
    }
}
