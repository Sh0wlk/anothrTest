using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class ghost : MonoBehaviour
{
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public float lerpDuration = 2f;
    public AudioSource AudioSource;
    public float pow = 1f;
    public float timer = 1f;
    private bool canDash = true;
    public Transform player;
    private float timeCont;
    private Rigidbody2D rb;
    public bool randomPow = false;
    public float maxRandPow;
    public float minRandPow;
    public bool randomTime = false;
    public float maxRandTime;
    public float minRandTime;
    private float lerpTime = 0f;
    private bool startLerp=false;
    private CinemachineImpulseSource impulseSource;
    public float impulcePow = 1f;

    [Header("OnHit")]
    public GameObject playerObj;
    public GameObject playerObj1;
    public Animator animator;
    public float timerAfterHit = 2f;
    public ParticleSystem deathParticles;
    public AudioSource deathAudio;
    private bool startAnimWait;
    private float animTimeCont;
    public death death;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        timeCont = timer;
    }

    void Update()
    {
        if (timeCont < 0)
        {
            if (randomTime)
            {
                timeCont = Random.Range(minRandTime, maxRandTime);
            }
            else 
            {
                timeCont = timer;
            }
            dash();
            float pitch = Random.Range(0.8f, 1.2f);
            AudioSource.pitch = pitch;
            AudioSource.Play();
            startLerp=true;
            lerpTime = 0f;


        }
        else
        { 
            float curtime=Time.deltaTime;
            timeCont-=curtime;
        }
        if (startLerp)
        {
            lerpTime += Time.deltaTime;
            float t = lerpTime / lerpDuration;

            body.color = Color.Lerp(Color.white, Color.red, t);
            eyes.color = Color.Lerp(Color.white, Color.red, t);

            if (t >= 1f)
            {
                lerpTime = 0f;
                startLerp=false;
            }
        }
        else
        {
            lerpTime += Time.deltaTime;
            float t = lerpTime / lerpDuration;

            body.color = Color.Lerp(Color.red, Color.white, t);
            eyes.color = Color.Lerp(Color.red, Color.white, t);

            if (t >= 1f)
            {
                lerpTime = 0f;
            }
        }
        if (startAnimWait)
        {
            if (animTimeCont<0)
            {
                //загрузсцену
            }
            else { float lol = Time.deltaTime; animTimeCont-=lol; }
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            impulseSource.GenerateImpulseWithForce(impulcePow);
            startAnimWait=true;
            Destroy(playerObj);
            Destroy(playerObj1);
            death.deaath();
            deathParticles.Play();
            float pitch = Random.Range(0.8f, 1.2f);
            deathAudio.pitch = pitch;
            deathAudio.Play() ;
        }
    }
    public void dash()
    {
        rb.linearVelocity = Vector3.zero;
        if (randomPow)
        {
            Vector3 boostDirection = new Vector3(player.position.x - transform.position.x, player.position.y - transform.position.y, transform.position.z);
            rb.linearVelocity = boostDirection * pow;
        }
        else
        {
            pow=Random.Range(minRandPow, maxRandPow);
            Vector3 boostDirection = new Vector3(player.position.x - transform.position.x, player.position.y - transform.position.y, transform.position.z);
            rb.linearVelocity = boostDirection * pow;
        }
    }
}
