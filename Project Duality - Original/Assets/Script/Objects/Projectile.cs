using UnityEngine;
using UnityEngine.Events;

using System.Collections;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float remainTime;
    [SerializeField]
    private float velocity;
    
    private entityGroup group = entityGroup.Default;

    float delta;

    public UnityEvent onEnter = new UnityEvent();
    public UnityEvent onExit = new UnityEvent();

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        StartCoroutine("DurationTime");

        if (this.group == entityGroup.Black) 
            BulletWarningManager.Instance.InstantiateWarning(this.transform);
    }
    void Update()
    {
        delta = Time.deltaTime;
    }

    public void Impulse(Vector3 dir)
    {
        rb.AddForce(dir * rb.mass * velocity, ForceMode.Force);
    }
    public void SetGroup(entityGroup value)
    {
        group = value;
    }
    public void ToDestroy()
    {
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        if (other.TryGetComponent<Interactive>(out Interactive _col))
            if (_col.group != this.group)
                _col.TakeDmg();

        onEnter.Invoke();
    }
    void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;

        onExit.Invoke();
    }

    IEnumerator DurationTime()
    {
        yield return new WaitForSeconds(remainTime);
        ToDestroy();
    }
}
