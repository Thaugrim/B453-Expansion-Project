using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class Entity : Interactive
{
    [Header("Entity Info")]
    [SerializeField] protected float speed = 3;
    [SerializeField] protected bool dead = false;

    protected bool canTakeDmg = true;

    [Header("Interface")]
    public Slider lifeSlide;

    public override void TakeDmg()
    {
        if (canTakeDmg)
        {
            life -= 1;

            if (lifeSlide)
                lifeSlide.value = life;

            StartCoroutine("CanTakeDmg");

            if (life <= 0)
                dead = true;
        }
    }
    IEnumerator CanTakeDmg()  // Invincibility frames
    {
        canTakeDmg = false;
        yield return new WaitForSeconds(0.25f);
        canTakeDmg = true;
    }
}
