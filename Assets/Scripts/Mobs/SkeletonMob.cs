using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMob : Enemy  {


    private float playerDamage;
    private Animator anim;

    private bool agroed = false;
    private bool hit = false;
    private bool facingRight = false;
    private float direction = -1;
    private float agroDistance = 6.5f;
    private float autoAgroRadius = 2;
    private float deagroTime = 5;
    private float deagroTimer = 5;
    private float watchTime = 0.5f;
    private float watchTimer = 0.5f;
    public float attackCooldownTime = 1.5f;
    private float attackTimer = 0;
    private bool attackOnCooldown = false;

    [SerializeField]
    private Transform groundCheck;
    private Rigidbody2D rb;

    void Awake () {
        // Setting parameters for enemy script.
        level = 1;
        maxHealth = 10;   
        health = 10; // the current health of the mob.
        headshotMultiplier = 1.4f;
        deathTime = 5.5f;
        patrolSpeed = 1.2f;
        runningSpeed = 2.6f;
        minDamage = 4f;
        maxDamage = 10f;


        // Setting up animator
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        // Sets up a player gameobject reference.
	}

    private new void Update()
    {
        if (health > 0)
        {
            CheckAgro();  // searches for player and handles agro
            //Debug.Log(agroed);
            if (agroed)
            {
                Chase();
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            anim.SetInteger("State", (int)States.DEAD);
        }

        if (attackOnCooldown) { AttackCooldown(); }

        base.Update();
    }


    void Chase()
    {
        if (IsStuck(debug: true))
        {
            
            Deagro();
            anim.SetInteger("State", 6);
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        float directionToPlayer = (GameManager.GM.Player.transform.position.x > transform.position.x) ? 1 : -1;

        //change directions
        if (directionToPlayer == 1)    // player is to the right
        {
            ChangeDirectionToRight();
            
        }
        else    // player is to the left
        {
            ChangeDirectionToLeft();  
        }

        if (Vector2.Distance(GameManager.GM.Player.transform.position, transform.position) > (autoAgroRadius-1))
        {
            if (Math.Abs(transform.position.x - GameManager.GM.Player.transform.position.x) < 0.2f)  //same x as player, don't go crazy!
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                anim.SetInteger("State", 6);    // set angry state and don't move.
            }
            else
            {
                RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, hitLayer);
                if (groundInfo)
                {
                    Walk(directionToPlayer, States.RUN);
                    anim.SetInteger("State", 3);
                }
                else
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    anim.SetInteger("State", 6);
                }
            }
        }
        else // close enogh to attack
        {
            Attack();
        }
    }


    private void Attack()
    {
        if (!attackOnCooldown)
        {
            attackOnCooldown = true;
            attackTimer = attackCooldownTime;
            anim.SetInteger("State", (int)States.ATTACK);
        }
        else
        {
            anim.SetInteger("State", (int)States.ANGRY);
        }
    }


    private void MakeAttack()
    {
        
        float directionToPlayer = (GameManager.GM.Player.transform.position.x > transform.position.x) ? 1 : -1;
        
        float distanceFromPlayer = Vector2.Distance(transform.position, GameManager.GM.Player.transform.position);
        
        if (distanceFromPlayer < (autoAgroRadius - 1))
        {
            GameManager.GM.Player.GetComponent<Player>().EnableMovement(false);
            StartCoroutine(GameManager.GM.Player.GetComponent<Player>().MakeHit(directionToPlayer, 20, distanceFromPlayer));
            
        }
    }


    private void AttackCooldown()
    {
        
        if (attackTimer <= 0)
        {
            attackOnCooldown = false;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }


    private void CheckAgro()
    {

        
        RaycastHit2D searchInfo = Physics2D.Raycast(transform.position, Vector2.right * direction, agroDistance, hitLayer);
        Debug.DrawRay(transform.position, Vector2.right * direction * agroDistance);

        if (Vector2.Distance(transform.position, GameManager.GM.Player.transform.position) <= autoAgroRadius)  // if player comes too close
        {
            agroed = true;
            deagroTimer = deagroTime;
        }
        else if (searchInfo && searchInfo.transform.gameObject == GameManager.GM.Player)  // if scanner ray hit player
        {
            agroed = true;
            deagroTimer = deagroTime;
        }
        else if (hit)  //player is shooting at enemy
        {
            hit = false;
            agroed = true;
            deagroTimer = deagroTime;
        }
        else    // start deagro process
        {
            if (agroed) Deagro();
        }
    }


    private void Patrol()
    {
        //Debug.Log("patrolling");
        anim.SetInteger("State", (int)States.PATROL); //back to patrol animation

        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1, hitLayer);
        RaycastHit2D clashDetector = Physics2D.Raycast(groundCheck.position, new Vector2(direction,0), 0.2f, clashDetectLayer);

        if (groundInfo && !IsStuck(debug:true) && !clashDetector)  // if enemy is ontop of ground, should always be true.
        {
            // keep walking
            Walk(direction, States.PATROL);
        }
        else
        {
            if (watchTimer <= 0)   // flip direction & keep going
            {
                watchTimer = watchTime; // reset timer to stop

                if (direction == 1) // right
                {
                    ChangeDirectionToLeft();
                }
                else // left
                {
                    ChangeDirectionToRight();
                }
            }
            else   // stop to watch
            {
                Watch();
            }
        }
    }


    void Deagro()
    {
        if (deagroTimer < 0) // time to deagro
        {
            deagroTimer = deagroTime;
            agroed = false;
        }
        else
        {
            deagroTimer -= Time.deltaTime;
        }
    }

    
    void Walk(float dir, States walkingState)
    {
        if (walkingState == States.PATROL)
        {
            rb.velocity = new Vector2(dir * patrolSpeed, rb.velocity.y);
        }
        else if (walkingState == States.RUN)
        {
            rb.velocity = new Vector2(dir * runningSpeed, rb.velocity.y);
        }

        anim.SetInteger("State", (int)walkingState);
    }


    void Watch()
    {
        watchTimer -= Time.deltaTime;
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetInteger("State", (int)States.WATCH);
    }


    void ChangeDirectionToLeft()
    {
        if (facingRight)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);       //flipping sprite
            Vector3 theScale = mobCanvas.transform.localScale;  //flipping health
            theScale.x *= -1;
            mobCanvas.transform.localScale = theScale;
            direction *= -1;
            facingRight = false;
        }
    }


    void ChangeDirectionToRight()
    {
        if (!facingRight)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);   //flipping sprite
            Vector3 theScale = mobCanvas.transform.localScale; //flipping health
            theScale.x *= -1;
            mobCanvas.transform.localScale = theScale;
            direction *= -1;
            facingRight = true;
        }
    }


    void HeadShot()
    {
        playerDamage = GameManager.GM.Player.GetComponent<Player>().damage;
        Hit(playerDamage * headshotMultiplier, true);
        // animate the hit animation
        anim.SetTrigger("headshot");
        hit = true;
    }


    void BodyHit()
    {

        playerDamage = GameManager.GM.Player.GetComponent<Player>().damage;
        Hit(playerDamage);
        // animate the hit animation
        anim.SetTrigger("bodyhit");
        hit = true;  
    }


    private bool IsStuck(float distance = 0.01f, bool debug = false)
    {
        RaycastHit2D stuckInfo = Physics2D.Raycast(new Vector2(groundCheck.position.x, groundCheck.position.y + 0.5f), new Vector2(direction, 0), distance, GameManager.GM.environmentLayer);
        if (debug)
        {
            Debug.DrawRay(new Vector2(groundCheck.position.x, groundCheck.position.y + 0.5f), new Vector2(direction, 0) * distance);
        }
        if (stuckInfo && stuckInfo.collider.transform.name != "Player")
        {
            return true;
        }

        return false;
    }


    private enum States
    {
        PATROL = 1,
        WATCH = 2,
        RUN = 3,
        ATTACK = 4,
        DEAD = 5,
        ANGRY = 6
    }
}
