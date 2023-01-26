using UnityEngine;
using UnityEngine.AI;

using System.Collections;

public class EnemyController : Entity
{
    [Header("Info")]
    [SerializeField]
    private int level = 1;
    public float viewDist = 0;

    [Header("Behaviours")]
    public NavMeshAgent agent;

    [Header("Visual")]
    public GameObject gfx;

    [Header("Fire System")]
    [SerializeField] private float maxBullets;
    public float bullets;

    [Header("Audio Manager")]
    [SerializeField] private AudioSource source;

    public GameObject prefabBullet;
    public Transform spawnBPos;

    [SerializeField] private bool canShoot = true;
    [SerializeField] private Transform target;

    private Vector3 startPos;

    void Start()
    {
        level = GameController.difficutyAcess + 1;
        source = GetComponent<AudioSource>();

        agent = this.GetComponent<NavMeshAgent>();
        agent.speed = speed;

        agent.stoppingDistance = viewDist / 3;
        startPos = this.transform.position;
    }

    void FixedUpdate()
    {
        if (level == 3)
        {
            if (Vector3.Distance(target.position, this.transform.position) > viewDist)
                StartCoroutine("LooseTarget");
        }
        if (level >= 2)
        {
            if (target)
            {
                agent.SetDestination(target.position);

                if (level == 2)
                    if (Vector3.Distance(target.position, this.transform.position) >= viewDist / 2)
                    {
                        target = null;
                        agent.SetDestination(startPos);
                    }
            }
        }
        if (level >= 1)
        {
            if (!target)
                return;

            gfx.transform.LookAt(target);
            if (Vector3.Distance(this.transform.position, target.position) <= viewDist)
                if (Physics.Raycast(this.transform.position, this.transform.forward, viewDist)||
                    Physics.Raycast(this.transform.position, this.transform.forward + new Vector3(0.5f, 0, 0), viewDist)||
                    Physics.Raycast(this.transform.position, this.transform.forward + new Vector3(0.5f, 0, 0), viewDist))
                    Attack();
        }

        if (life <= 0)
        {
            Destroy(this.gameObject);

            source.clip = GameController.sExplosion;
            source.Play();
        }
            
    }
    void LateUpdate()
    {
        //Search for target
        RaycastHit[] objects = Physics.SphereCastAll(this.transform.position, viewDist, this.transform.forward); // Throw a raw in all directions
        foreach (var i in objects)
        {
            if (level >= 5)
            {
                if (i.transform.TryGetComponent<GameController>(out GameController _orb))
                {
                    target = i.transform;
                    return;
                }
            }

            if (i.transform.TryGetComponent<Entity>(out Entity _entity))
            {
                if (_entity.group != this.group)
                    target = i.transform;
            }
        }
    }

    void Attack()
    {
        if (target && canShoot) // If target is not null and canShoot is true
        {
            if (bullets <= 0)
            {
                StartCoroutine("Reload");
                return;
            }

            source.clip = GameController.sFire;
            source.Play();

            StartCoroutine("CanShoot");

            Instantiate(prefabBullet, spawnBPos.position, Quaternion.identity).TryGetComponent<Projectile>(out Projectile _bullet);
            _bullet.SetGroup(this.group);
            _bullet.Impulse(gfx.transform.forward);

            bullets -= 1;
        }
    }

    IEnumerator LooseTarget()
    {
        yield return new WaitForSeconds(5f);
        target = null;
        agent.SetDestination(startPos);
    }
    IEnumerator Reload()
    {
        yield return new WaitForSeconds(5f);

        bullets = maxBullets;
    }
    IEnumerator CanShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(0.5f);
        canShoot = true;
    }
}
