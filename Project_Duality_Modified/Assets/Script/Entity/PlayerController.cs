using UnityEngine;
using UnityEngine.Rendering;

using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : Entity
{
    [Header("Behaviours")]
    private Rigidbody rb;

    [Header("Interface")]
    [SerializeField] private GameObject intButton;

    [Header("Visual")]
    public GameObject gfx;
    public GameObject shield;

    [Header("Fire System")]
    [SerializeField] private float bulletsCount;
    [SerializeField] private float maxBullets;
    public Transform spawnBPos;
    [SerializeField] private GameObject prefabBullet;

    [Header("Inventory")]
    public int orbsCount;

    [Header("Defend System")]
    [SerializeField] private Shield shieldInfo;
    bool defend = false;

    [Header("Audio Manager")]
    [SerializeField] private AudioSource source;

    [Header("Helpers")]
    public Transform pivotCam;
    public SphereCollider vault;
    public LayerMask gMask;
    public LayerMask aMask;

    float inputX;
    float inputY;
    Vector3 direction;

    float camInputX;
    Vector3 camRotation;

    bool canShoot = true;

    bool gravityOn = true;
    bool onIce = false;

    bool onGround = false;
    bool canJump = true;

    float actualSpeed;
    bool reloading = false;

    float delta;

    private GameObject basePlayer;

    // ADDED
    [SerializeField] private GameObject mainCam;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        basePlayer = GameObject.Find("Base");

        actualSpeed = speed;
    }
    void Update()
    {
        delta = Time.deltaTime;

        onGround = Physics.CheckCapsule(vault.bounds.center, // Check on ground for jumping or falling
        new Vector3(vault.bounds.center.x, vault.bounds.min.y, vault.bounds.center.z),
        vault.radius * 0.9f, gMask, QueryTriggerInteraction.Ignore);

        HandleInput();
        HandleMovement();

        if (bulletsCount < maxBullets && !reloading)
            StartCoroutine("Reload");
    }

    void HandleInput()
    {
        inputX = Input.GetAxisRaw("Vertical"); // W + S Movement
        inputY = Input.GetAxisRaw("Horizontal"); // A + D Movement

        Ray camR = Camera.main.ScreenPointToRay(Input.mousePosition);  // This is used for interactive objects
        RaycastHit hit;

        Physics.Raycast(camR, out hit);

        //Verifica objetos interativos - Check interactive Objects
        if (hit.transform.TryGetComponent<InspectorBase>(out InspectorBase _obj))
        {
            intButton.SetActive(true); // This displays the F (or interact key) when near an interactable object?

            if (Input.GetKeyDown(KeyCode.F))
                _obj.Interact();
        }
        else
            intButton.SetActive(false);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //direction = (pivotCam.forward * inputX + pivotCam.right * inputY).normalized; // This handles Player model movement direction
        direction = (mainCam.transform.forward * inputX + mainCam.transform.right * inputY).normalized; // This handles Player model movement direction
        direction.y = 0;  // remove the vertical access

       
        Vector3 camPos = new Vector3(mainCam.transform.position.x, 0, mainCam.transform.position.z);
        gfx.transform.rotation = Quaternion.LookRotation(gfx.transform.position - camPos);  // Helped with the math of looking away from camera https://answers.unity.com/questions/566404/why-does-lookat-look-away.html

        /*Physics.Raycast(camR, out hit, Mathf.Infinity, aMask);
        var lookPos = new Vector3(hit.point.x, gfx.transform.position.y, hit.point.z);
        gfx.transform.LookAt(lookPos, Vector3.up);  // Rotate player model toward mouse pointer

        //Rotaciona a camera - Rotation of camera
        /*if (Input.GetMouseButton(2))
        {
            camInputX = Input.GetAxisRaw("Mouse X");

            camRotation += new Vector3(0, camInputX * 100f * delta); // Rotate camera around the Y axis if middle mouse wheel is pressed
            pivotCam.rotation = Quaternion.Euler(camRotation);
        } // */

        //Atirar - Shoot
        if (Input.GetMouseButton(0))  // Would be nice to have some sort of UI element for showing remaining bullets
        {
            if (bulletsCount <= 0 || !canShoot || defend)
                return;

            source.clip = GameController.sFire;
            source.Play();

            StartCoroutine("CanShoot");

            Instantiate(prefabBullet, spawnBPos.position, Quaternion.identity).TryGetComponent<Projectile>(out Projectile _bullet);
            _bullet.SetGroup(this.group);
            _bullet.Impulse(gfx.transform.forward);

            bulletsCount -= 1;
        }

        //Escudo - Shield
        if (Input.GetMouseButtonDown(1) && shieldInfo.getLife() > 0)
        {
            shield.SetActive(true);
            defend = true;
        }
        if (Input.GetMouseButtonUp(1) || shieldInfo.getLife() <= 0)
        {
            shield.SetActive(false);
            defend = false;
        }
    }
    void HandleMovement()
    {
        if (defend)
            speed = actualSpeed / 2; // Speed is reduced by half if shielding
        else
            speed = actualSpeed;

        if (direction != Vector3.zero)
        {
            if (CanMove(direction))
            {
                if (onIce)
                    rb.AddForce(rb.mass * (speed * 2) * direction * delta, ForceMode.Impulse);
                else
                    this.transform.position += direction * speed * delta;

                Quaternion target = Quaternion.Euler(inputX * 30f, 0, inputY * 30f); // Magic numbers
                basePlayer.transform.rotation = basePlayer.transform.rotation * Quaternion.Slerp(basePlayer.transform.rotation, target, Time.deltaTime); // Smooth movement to position
            }
        }
        
        //Simula Vôo - Simulate flight
        if (gravityOn)
        {
            if (!onGround)
                rb.AddForce(Physics.gravity * rb.mass);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (onGround && canJump)
                {
                    StartCoroutine("CanJump");
                    rb.AddForce(Vector3.up * rb.mass * 750f);
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
                this.transform.position += Vector3.up * speed * delta;

            if (Input.GetKey(KeyCode.LeftControl))
                this.transform.position += -Vector3.up * speed * delta;
        }
    }

    bool CanMove(Vector3 direction) // This seems to check if the move position is a valid movement position?
    {
        if (Physics.Raycast(this.transform.position, direction * 2f, out RaycastHit hit, 0.5f) &&
            Physics.Raycast(this.transform.position, direction * 2f + new Vector3(0.5f, 0, 0), out RaycastHit hit1, 0.5f) &&
            Physics.Raycast(this.transform.position, direction * 2f + new Vector3(0.5f, 0, 0), out RaycastHit hit2, 0.5f))
        {
            if (hit.collider.isTrigger && hit1.collider.isTrigger && hit2.collider.isTrigger)
                return true;
            else
                return false;
        }
        else
            return true;
    }

    IEnumerator Reload()  // Effectively reloads 1 bullet every 3 seconds
    {
        reloading = true;

        while (bulletsCount < maxBullets)
        {
            bulletsCount += 1;
            yield return new WaitForSeconds(3f);
        }

        reloading = false;
    }
    IEnumerator CanJump() // If player is jumping, set to canJump to false, wait 1 second and reset to true
    {
        canJump = false;
        yield return new WaitForSeconds(1f);
        canJump = true;
    }
    IEnumerator CanShoot() // Shot delay 0.5 seconds
    {
        canShoot = false;
        yield return new WaitForSeconds(0.5f);
        canShoot = true;
    }

    public void CleanVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    public void OnIce(bool value)
    {
        onIce = value;
    }
    public void InverseGravity(bool value)
    {
        gravityOn = value;
    }

    public override void TakeDmg()
    {
        if (canTakeDmg && !defend)
        {
            life -= 1;

            if (life <= 0)
            {
                source.clip = GameController.sExplosion;
                source.Play();

                SceneManager.LoadSceneAsync("World");
            }

            if (lifeSlide)
                lifeSlide.value = life;

            StartCoroutine("CanTakeDmg");
        }
    }
    public void Cure()
    {
        if (life < 3)
            life += 1;

        if (lifeSlide)
            lifeSlide.value = life;
    }

    public void CollectOrb()
    {
        orbsCount += 1;
    }
    public int DeliveryOrb()
    {
        var orbs = orbsCount;
        orbsCount = 0;
        return orbs;
    }

}
