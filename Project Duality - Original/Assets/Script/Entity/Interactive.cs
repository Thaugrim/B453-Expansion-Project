using UnityEngine;

public enum entityGroup { White, Black, Default }
public abstract class Interactive : MonoBehaviour
{
    [Header("Base Info")]
    [SerializeField] protected int life = 3;
    public entityGroup group = entityGroup.Default;

    public abstract void TakeDmg();
}
