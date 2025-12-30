using Unity.Cinemachine;
using UnityEngine;

public class fish : MonoBehaviour
{
    public GameObject fishh;
    private bool shouldWait = false;
    public ParticleSystem deathParticles;
    public ParticleSystem deathParticles2;
    public AudioSource deathAudio;
    public score score;
    public float cost = 100f;
    public float timeToRespawn = 10f;
    private float timeCou;
    public BoxCollider2D boxCollider;
    public CinemachineImpulseSource impulseSource;
    public float impulsePow=0.4f;
    void Start()
    {
        timeCou = timeToRespawn;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldWait)
        { 
            if (timeCou<0)
            {
                shouldWait = false;
                timeCou=timeToRespawn;
                fishh.SetActive(true);
                boxCollider.enabled = true;
            }
            else
            {
                timeCou -= Time.deltaTime;
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !shouldWait)
        {
            impulseSource.GenerateImpulseWithForce(impulsePow);
            boxCollider.enabled = false;
            fishh.SetActive(false);
            shouldWait=true;
            score.scoreCou+=cost;
            deathParticles.Play();
            deathParticles2.Play();
            float pitch = Random.Range(0.8f, 1.2f);
            deathAudio.pitch = pitch;
            deathAudio.Play();
        }
    }
}
