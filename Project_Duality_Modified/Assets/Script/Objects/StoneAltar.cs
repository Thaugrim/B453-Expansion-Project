using UnityEngine;
using UnityEngine.Events;

using System.Collections;

public class StoneAltar : InspectorBase
{
    public UnityEvent onInteract =  new UnityEvent();
    public UnityEvent endInteract = new UnityEvent();

    public override void Interact()
    {
        onInteract.Invoke();
        StartCoroutine("WaitToRevertGravity");
    }
    IEnumerator WaitToRevertGravity()
    {
        yield return new WaitForSeconds(3f);
        endInteract.Invoke();
    }
}
