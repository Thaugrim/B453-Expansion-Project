using UnityEngine;
using System.Collections;

public class Shield : Interactive
{
    [SerializeField] private int maxLife;

    public int getLife()
    {
        return life;
    }
    public override void TakeDmg()
    {
        if (life > 0)
            life -= 1;
        else
            StartCoroutine("RestoreShield");
    }

    IEnumerator RestoreShield()
    {
        yield return new WaitForSeconds(5f);
        life = maxLife;
    }
}
