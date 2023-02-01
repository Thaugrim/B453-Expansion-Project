using UnityEngine;
using UnityEngine.Rendering.Universal;

using TMPro;

public class GameController : Interactive
{
    [SerializeField] private int difficuty; //Programar uma lista que relacione a dificuldade
    public static int difficutyAcess;

    [Header("Settings")]
    [SerializeField] private SphereCollider col;
    [SerializeField] private float startSize;
    [SerializeField] private float endSize;

    [Header("Sounds")]
    [SerializeField] private AudioClip explosion;
    [SerializeField] private AudioClip fire;

    public static AudioClip sExplosion;
    public static AudioClip sFire;

    public TextMeshProUGUI txtCounter;

    private float seconds;
    private int minutes;
    private int secAAdd;
    private int totalSeconds;

    private bool runCounter;

    private int orbsDeliveried = 0;

    void Start()
    {
        // Added to lock Cursor
        Cursor.lockState = CursorLockMode.Locked;

        sExplosion = explosion;
        sFire = fire;

        col.radius = startSize;
        orbsDeliveried = 0;

        this.difficuty = ((int)GameObject.FindObjectOfType<DifficultyLevel>().difficulty);

        this.runCounter = true;

        DefiningTimer();
    }

    void FixedUpdate()  // This just runs the timer
    {
        if (runCounter)
        {
            this.seconds -= Time.deltaTime;

            if(this.seconds <= 0 && this.minutes > 0)
            {
                this.minutes -= 1;
                this.seconds = 60;
            } else if(this.seconds <= 0 && this.minutes <= 0)
            {
                this.runCounter = false;
            }

            if(this.seconds >= 10)
            {
                this.txtCounter.text = this.minutes.ToString() + ":" + ((int)this.seconds).ToString();
            } else if(this.seconds < 10)
            {
                this.txtCounter.text = this.minutes.ToString() + ":0" + ((int)this.seconds).ToString();
            }
        }

        if (col.radius < endSize)
            col.radius = totalSeconds / (seconds + (minutes * 60)) * startSize;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController _player))
        {
            var orbsValue = _player.DeliveryOrb();

            orbsDeliveried += orbsValue;
            if(orbsDeliveried >= 18)   // 18 orbs to win?
            {
                runCounter = false;
                return;
            }

            if (orbsValue > 1)
                CounterSummation(orbsValue - 1);

            for (int i = 0; i < orbsValue; i++)
            {
                CounterSummation(this.secAAdd);
            }
        }

        if (other.TryGetComponent<ColorInverter>(out ColorInverter _color))
        {
            _color.ColorInvert(false);//Deixa os materiais pretos  - Make the materials black
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ColorInverter>(out ColorInverter _color))
        {
            _color.ColorInvert(true);//Deixa os materiais brancos - Make the materials white
            
        }
    }

    public override void TakeDmg()  // I didn't even realize that taking damage reduced the timer
    {
        seconds -= 1;
    }

    void CounterSummation(int value)
    {
        for (int j = 0; j < value; j++)
        {
            if (seconds < 60)
                seconds += 1;
            else
            {
                seconds = 0;
                minutes += 1;
            }
        }
    }
    void DefiningTimer()  // This just reduces the timer as difficulty increases
    {
        this.secAAdd = 20 - (2 * difficuty);
        this.totalSeconds = 720 - (120 * difficuty);

        this.minutes = this.totalSeconds / 60;
        this.seconds = 0;
    }
}
