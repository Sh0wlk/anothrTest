using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    private bool isGrounded;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    public float acceleration = 10f;
    public float deceleration = 20f;
    public float maxSpeed = 5f;
    private bool isJumping = false;
    public float jumpcutpow;
    public float jumpcutmaxvel;
    public float jumpcutminvel;
    public float overlapSize;

    [Header("visual")]
    private bool shouldFlip;
    private bool currentFacing;
    private Vector2 normalScale;
    private Vector2 squashed;
    public float squashPow;
    private bool loadsquashX = false;
    private bool loadsquashY = false;
    public float timeToSquash = 1f;
    private float squashTime;
    public SpriteRenderer spriteRenderer;
    private Vector2 newScale;
    public CinemachineImpulseSource impulseSource;
    public float impulcePow = 0.4f;

    [Header("grabbing")]
    public float grabStamina = 110f;
    public float grabStaminaCont = 110f;
    public float speedLeakPower = 1f;
    public Transform grabCheckTransform;
    private bool canGrab = false;
    public float speeGrabMovement;
    private bool delgrav = false;
    private bool isGrabbing = false;
    public GameObject staminaBar;
    private Vector2 startBarSize;
    public ParticleSystem staminaParticles;

    [Header("dash")]
    public int dashcou;
    public float impulsepow;
    public ForceMode2D forceMode;
    public Vector2 worldPositionF;
    public Vector2 worldPositionS;
    private int strdashcou;
    private bool isdashing;
    public float dashtime;
    private bool isDashing = false;
    public float timerOfDash;
    private bool checkForDashBoost = false;
    public float antiDashMultiplayer = 2f;
    private bool startAntiDashTimer = false;
    private int couOfAntiDash = 1;

    [Header("colliders")]
    public BoxCollider2D boxcol;
    public CapsuleCollider2D capscol;

    [Header("inputbuffer")]
    public float coyottime;
    private bool wasgrounded;
    private bool coyo;
    private float coyotimer;
    public float jumpbuffertime;
    private float dashtimer;
    private float waitjumptimer;
    private bool waitforjumpBL = false;

    [Header("effects")]
    public ParticleSystem landJumpParticles;
    public ParticleSystem dashParticles;
    public ParticleSystem grabParticles;
    public ParticleSystem walkParticles;
    public AudioSource walksrc;
    public AudioSource jumpsrc;
    public AudioSource landsrc;
    public AudioSource dashsrc;
    public AudioSource grabsrc;
    public float timerForParticles;
    private bool waitForParticles = false;
    private float timerParticles;

    [Header("Other Stuff")]
    public Vector2 UpVec = Vector2.up;
    public Vector2 RiVec = Vector2.right;
    public Vector2 DoVec = Vector2.down;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        strdashcou = dashcou;
        normalScale = spriteRenderer.transform.localScale;
        squashed = spriteRenderer.transform.localScale * squashPow;
        timerOfDash = dashtime;
        startBarSize = staminaBar.transform.localScale;
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump") && (isGrounded || coyo))//jump
        {
            jump();
            isJumping = true;
        }
        if (Input.GetButtonDown("Jump") && !isGrounded && !coyo)//coyote time
        {
            waitforjumpBL = true;
        }
        if (Input.GetButtonUp("Jump") && isJumping && rb.linearVelocity.y < jumpcutmaxvel && rb.linearVelocity.y > jumpcutminvel)//jumpcut
        {
            rb.AddForce(DoVec * jumpcutpow, ForceMode2D.Impulse);
            isJumping = false;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            squash();

        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            squash();

        }
        if (waitforjumpBL)
        {
            waitforjump();
        }
        if (Input.GetMouseButtonDown(0))//dashpos1
        {
            worldPositionF = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }
        if (Input.GetMouseButtonUp(0))//dashpos2
        {
            worldPositionS = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            TryDash(worldPositionF, worldPositionS);
            isdashing = true;
        }
        if (isGrounded)
        {
            if (rb.linearVelocity.x > 0.2 || rb.linearVelocity.x < -0.2)
            {
                walkParticles.Play();
            }
            else { walkParticles.Stop(); }
            dashcou = strdashcou;
            boxcol.enabled = true;
            capscol.enabled = false;
        }
        else
        {
            boxcol.enabled = false;
            capscol.enabled = true;
            walkParticles.Stop();
        }
        if (isdashing)
        {
            dashtimer += Time.deltaTime;
            if (dashtimer > dashtime)
            {
                dashtimer = 0f;
                isdashing = false;
            }
        }
        if (coyo && coyotimer <= coyottime)
        {
            coyotimer += Time.deltaTime;
        }
        else
        {
            coyo = false;
            coyotimer = 0f;
        }
        if (waitForParticles && timerForParticles > timerParticles)
        {
            float lol = Time.deltaTime;
            timerParticles += lol;
        }
        else { timerParticles = 0f; waitForParticles = false; }
        if (loadsquashX && squashTime < timeToSquash) //SQUASHING Y
        {
            float ant = Time.deltaTime;
            squashTime += ant;
            squash();
        }
        else if (loadsquashX && squashTime > timeToSquash)
        {
            squashTime = 0f;
            loadsquashX = false;
            squash();
        }
        if (loadsquashY && squashTime < timeToSquash) //SQUASHING X
        {
            float ant = Time.deltaTime;
            squashTime += ant;
            squash();
        }
        else if (loadsquashY && squashTime > timeToSquash)
        {
            squashTime = 0f;
            loadsquashY = false;
            squash();
        }
        if (isGrabbing)
        {
            squash();
        }
        if (!isGrabbing || !canGrab)
        {
            squash();
        }
        if (checkForDashBoost & timerOfDash > 0 & isGrounded & couOfAntiDash == 1)
        {
            tryAntiBoost(worldPositionF, worldPositionS);
            startAntiDashTimer = true;
        }
        else if (timerOfDash < 0)
        {
            checkForDashBoost = false;
            timerOfDash = dashtime;
            startAntiDashTimer = false;
        }
        if (startAntiDashTimer)
        {
            float sugma = Time.deltaTime;
            timerOfDash -= sugma;
        }
        checkFacing();//CHECK FOR FACING DIRECTION
        Flip();
        grabCheck();
    }
    public void tryAntiBoost(Vector2 w1, Vector2 w2)
    {
        if (Input.GetButtonDown("Jump") && ((w1.y - w2.y) * -1) > 0f)
        {
            shakeEffect();
            couOfAntiDash -= 1;
            dashParticles.Play();
            isJumping = false;
            rb.linearVelocity = Vector2.zero;
            Vector2 boostDirection = new Vector2((w1.x - w2.x), (w1.y - w2.y) * -1);
            Vector3 direction = boostDirection.normalized;
            rb.AddForce(direction * impulsepow * antiDashMultiplayer, forceMode);
            checkForDashBoost = false;
            timerOfDash = dashtime;
        }
    }
    public void grabCheck()
    {
        canGrab = Physics2D.OverlapCircle(grabCheckTransform.position, 0.1f, groundLayer);
        if (canGrab && grabStaminaCont > 0 && Input.GetKey(KeyCode.Space))
        {
            grab();
            if (delgrav)
            {
                Vector2 velocity = rb.linearVelocity;
                velocity.y = 0f;
                velocity.x = 0f;
                rb.linearVelocity = velocity;
                delgrav = false;
            }
        }
        else
        {
            grabsrc.Stop();
            rb.gravityScale = 1f;
            isGrabbing = false;
            grabParticles.Stop();
            if (isGrounded)
            {
                grabStaminaCont = grabStamina;
                staminaBar.transform.localScale = startBarSize;
                staminaParticles.Play();
                delgrav = true;
            }
        }
    }

    public void grab()
    {
        rb.gravityScale = 0f;

        float time = Time.deltaTime;
        grabStaminaCont -= time * speedLeakPower;
        staminaBar.transform.localScale = new Vector2(grabStaminaCont / 100, startBarSize.y);
        isGrabbing = true;
        float pitch = Random.Range(0.8f, 1.2f);
        grabsrc.pitch = pitch;
        grabsrc.Play();
        if (Input.GetKey(KeyCode.W))//UP
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            rb.AddForce(UpVec * speeGrabMovement, ForceMode2D.Force);
            grabParticles.Play();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            rb.AddForce(DoVec * speeGrabMovement, ForceMode2D.Force);//DOWN
            grabParticles.Play();
        }
        else { rb.linearVelocity = Vector2.zero; grabParticles.Stop(); }
        if (rb.linearVelocity.y > speeGrabMovement)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, speeGrabMovement);
        }
        else if (rb.linearVelocity.y < -1 * speeGrabMovement)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -1 * speeGrabMovement);
        }
    }

    public void checkFacing()
    {
        if (moveInput.x > 0 && shouldFlip)
        {
            shouldFlip = false;
        }
        else if (moveInput.x < 0 && !shouldFlip)
        {
            shouldFlip = true;
        }

    }

    void Flip()
    {
        if (shouldFlip != currentFacing)
        {
            currentFacing = shouldFlip;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    public void squash()
    {
        newScale = normalScale;

        if (isGrabbing)
        {
            newScale.x = squashed.x;
            newScale.y = normalScale.y;
        }
        else if (loadsquashY)
        {
            newScale.y = squashed.y;
            newScale.x = normalScale.x;
        }
        else if (loadsquashX)
        {
            newScale.x = squashed.x;
            newScale.y = normalScale.y;
        }
        else if (!loadsquashX)
        {
            newScale.x = normalScale.x;
        }
        else if (!loadsquashY)
        {
            newScale.y = normalScale.y;
        }
        spriteRenderer.transform.localScale = newScale;
    }
    public void jump()
    {
        float pitch = Random.Range(0.8f, 1.2f);
        jumpsrc.pitch = pitch;
        jumpsrc.Play();
        loadsquashY = true;
        Vector2 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;
        rb.AddForce(UpVec * jumpForce, ForceMode2D.Impulse);
        isJumping = true;
        if (!waitForParticles)
        {
            landJumpParticles.Play();
            waitForParticles = true;
        }
    }

    public void waitforjump()
    {
        waitjumptimer += Time.deltaTime;
        if (waitjumptimer <= jumpbuffertime && isGrounded)
        {
            jump();
        }
        else if (waitjumptimer > jumpbuffertime)
        {
            waitjumptimer = 0f;
            waitforjumpBL = false;
        }
    }

    public void TryDash(Vector2 w1, Vector2 w2)
    {
        if (dashcou > 0)
        {
            shakeEffect();
            float pitch = Random.Range(0.8f, 1.2f);
            dashsrc.pitch = pitch;
            dashsrc.Play();
            couOfAntiDash = 1;
            dashParticles.Play();
            isJumping = false;
            rb.linearVelocity = Vector2.zero;
            Vector2 boostDirection = new Vector2(w1.x - w2.x, w1.y - w2.y);
            Vector3 direction = boostDirection.normalized;
            rb.AddForce(direction * impulsepow, forceMode);
            dashcou -= 1;
            isDashing = true;
            PlayBoostEffects();
        }
    }
    public void shakeEffect()
    {
        impulseSource.GenerateImpulseWithForce(impulcePow);
    }

    public void PlayBoostEffects()
    { 
        
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = new Vector2(moveInput.x * maxSpeed, rb.linearVelocity.y);

        if (moveInput.x != 0)
        {
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, deceleration * Time.fixedDeltaTime);
        }

        if (!isdashing)
        {
            rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed), rb.linearVelocity.y);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, overlapSize, groundLayer);
        if (wasgrounded!=isGrounded)
        {
            coyo = true;
            wasgrounded= isGrounded;
            if (!waitForParticles && isGrounded)
            {
                float pitch = Random.Range(0.8f, 1.2f);
                landsrc.pitch = pitch;
                landsrc.Play();
                landJumpParticles.Play();
                loadsquashX = true;
                waitForParticles = true;
                checkForDashBoost=true;
            }
        }

    }
}