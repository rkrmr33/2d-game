using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{

    // Variables
    public float movementSpeedForce = 50;
    public float maxMovementSpeed = 5;
    public float jumpSpeed = 770f;
    public float projectileSpeed = 150f;
    public float verticalProjectileSpeed = 10f;
    public float attackSpeed = 0.7f;
    public int   startingArrowAmount = 20;
    
    public float health;
    public float damage;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public GameObject doubleJumpPrefab;
    public GameObject arrows;

    // Attributes
    private int strength = 10;
    private int dexterity = 10;
    private int vitality = 1;

    private float strDamageMultiplier = 0.4f;
    private float dexDamageMultiplier = 0.2f;
    private float dexAttackSpeedMultiplier;
    private float vitLifeMultiplier;


    private int level = 1;
    private float levelDamageMultiplier = 1.001f;
    private float baseDamage = 2f;
    private float damageBuff = 1;
    private float maxRandMultiplier = 1.3f;
    private float minRandMultiplier = 0.8f;
    private float attackTimer = 0f;
    private float restartButtonTimer = 0f;
    private int   currentArrowAmount;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer m_spriteRenderer;
    private Animator m_animator;


    // Indicators:
    private bool isFacingRight;
    private bool isMovable;
    private bool isGrounded;
    private bool doubleJump = false;


    void Awake()
    {
        // Setting importent variables:
        rb = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();

        // Setting Indicators: 
        isFacingRight = m_spriteRenderer.flipX;
        //isMoving = false;
        isMovable = true;
        isGrounded = true;
        currentArrowAmount = startingArrowAmount;
        
    }

    private void Start()
    {
        GameManager.GM.playerAttackCooldown.GetComponentInChildren<TextMeshProUGUI>().text = currentArrowAmount.ToString();
    }

    void FixedUpdate()
    {
        // Checking if player is grounded:
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(1, 0.2f), 0, groundLayer);

        m_animator.SetBool("Grounded", isGrounded);
        if (isGrounded)
            doubleJump = false;


        // Getting input from player
        if (isMovable)
        {
            float input = Input.GetAxis("Horizontal");
            PlayerMove(input);
        }

    }


    void Update()
    {
        if ((isGrounded || !doubleJump) && Input.GetButtonDown("Jump") && isMovable)
            PlayerJump();


        if (isMovable)
        {
            // flip player back up
            if (transform.rotation.eulerAngles.z != 0)
            {
                float angle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, 0, 1);
                Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.1f);
            }
        }

        if (Input.GetButtonDown("Shoot"))
            PlayerShoot();

        //Player Dead
        if (transform.position.y < GameManager.GM.DeadZone)
        {
            PlayerDead();
        }  

        if (attackTimer > 0)
        {
            AttackCooldown();
        }
        else if (GameManager.GM.playerAttackCooldown.fillAmount != 1)
        {
            attackTimer = 0;
            GameManager.GM.playerAttackCooldown.fillAmount = 1;
        }
    }


    private void AttackCooldown()
    {
        attackTimer -= Time.deltaTime;
        GameManager.GM.playerAttackCooldown.fillAmount = (attackTimer / attackSpeed);
    }


    private void PlayerShoot()
    {
        if (attackTimer == 0 && currentArrowAmount > 0)
        {
            int direction;
            float space = 0.65f;

            if (isFacingRight)
                direction = 1;
            else
                direction = -1;

            // Trigger the shoot animation.
            m_animator.SetTrigger("Shoot");

            // Creates an arrow from prefab.
            GameObject arrow = Instantiate(arrows, new Vector2(transform.position.x + (space * direction), transform.position.y), Quaternion.identity);

            // Recalculate player damage.
            CalculateDamage();

            arrow.GetComponent<SpriteRenderer>().flipX = isFacingRight;


            float vert = Input.GetAxis("Vertical");


            // change arrow direction and speed depeding on player input
            if (vert < 0)  // shoot down
                arrow.GetComponent<Rigidbody2D>().AddForce(new Vector2(((projectileSpeed -300) + (rb.velocity.x * 4)) * direction, - (verticalProjectileSpeed * 2) + (rb.velocity.y * 0.001f) + (Mathf.Abs(rb.velocity.x))));
            else if (vert > 0)  // shoot up
                arrow.GetComponent<Rigidbody2D>().AddForce(new Vector2(((projectileSpeed - 50) + (rb.velocity.x * 4)) * direction, (verticalProjectileSpeed * 5) + (rb.velocity.y * 0.001f) + (Mathf.Abs(rb.velocity.x))));
            else  // shoot straight
                arrow.GetComponent<Rigidbody2D>().AddForce(new Vector2((projectileSpeed + (rb.velocity.x * 4)) * direction, verticalProjectileSpeed + (rb.velocity.y * 0.001f) + (Mathf.Abs(rb.velocity.x))));

            attackTimer = attackSpeed;
            currentArrowAmount--;
            GameManager.GM.playerAttackCooldown.GetComponentInChildren<TextMeshProUGUI>().text = currentArrowAmount.ToString();
        }
    }


    private void CalculateDamage()
    {
        damage = (int)((baseDamage
            + (level * levelDamageMultiplier)
            + (dexterity * dexDamageMultiplier)
            + (strength * strDamageMultiplier)
            * damageBuff)
            * UnityEngine.Random.Range(minRandMultiplier, maxRandMultiplier));
    }


    private void PlayerMove(float input)
    {
        float airSlow = (isGrounded) ? 1 : 0.9f;

        if (input > 0) 
            isFacingRight = true;
        else if (input < 0) 
            isFacingRight = false;
        
        // Move
        rb.AddForce(Vector2.right * movementSpeedForce * airSlow * input);

        m_spriteRenderer.flipX = isFacingRight;
        m_animator.SetFloat("Speed", Math.Abs(input));

        //controll speed overflow
        if (rb.velocity.x > maxMovementSpeed)
        {
            rb.velocity = new Vector2(maxMovementSpeed, rb.velocity.y);
        }
        else if (rb.velocity.x < -maxMovementSpeed)
        {
            rb.velocity = new Vector2(-maxMovementSpeed, rb.velocity.y);
        }

    }


    private void PlayerJump()
    {
        // First Jump
        if (!doubleJump && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpSpeed));
        }

        // Second Jump
        if (!doubleJump && !isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);    // reset the velocity on the y axis
            rb.AddForce(new Vector2(0, jumpSpeed / 1.20f));   // jump again but a bit lower
            doubleJump = true;

            // Creating double jump trace
            GameObject go = Instantiate(doubleJumpPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
            Destroy(go, 2);
        }

        //Setting animation
        isGrounded = false;
        m_animator.SetBool("Grounded", isGrounded);

    }

    private void PlayerDead()
    {
        isMovable = false;
        TextMeshProUGUI[] texts = GameManager.GM.MessageBoard.GetComponentsInChildren<TextMeshProUGUI>();   // Setting message board text
        texts[0].text = "YOU DEAD!";
        texts[1].text = "PRESS [R] TO TRY AGAIN...\nHOLD [R] TO RELOAD THE LEVEL...";
        texts[1].fontSize = 42;
        GameManager.GM.MessageBoard.GetComponent<Animator>().SetInteger("State", 1); // Appear animation.
        GameManager.GM.deathCount++;

        //shutting down player
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        if (Input.GetKey(KeyCode.R))
        {
            restartButtonTimer += Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.R))    
        {
            if (restartButtonTimer <= 0.4f) // Retry
            {
                restartButtonTimer = 0;
                GameManager.GM.MessageBoard.GetComponent<Animator>().SetInteger("State", 2);    //Disappear
                rb.gravityScale = GameManager.GM.playerGravityScale;
                transform.position = GameManager.GM.PlayerSpawn.transform.position;
                isMovable = true;

            }
            else   // reload level
            {
                GameManager.GM.ReloadScene();
            }
        }

        
    }

    public IEnumerator MakeHit(float knockbackDir, float knockbackForce, float distanceFromMob, float damage = 100)
    {
        m_animator.SetTrigger("Hit");
        rb.constraints = RigidbodyConstraints2D.None;
        float timer = 0;

        while (timer < 0.4f)
        {
            timer += Time.deltaTime;
            rb.AddForce(new Vector3((knockbackDir * knockbackForce) / (distanceFromMob), knockbackForce / distanceFromMob, transform.position.z));
        }

        yield return new WaitForSeconds(1.8f);
        isMovable = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void GiveArrows(int amount)
    {
        currentArrowAmount += amount;   // give arrows
        GameManager.GM.playerAttackCooldown.GetComponentInChildren<TextMeshProUGUI>().text = currentArrowAmount.ToString(); //update current arrow count.
    }

    public void EnableMovement(bool isEnabled)
    {
        isMovable = isEnabled;
    }


}