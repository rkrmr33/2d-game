using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour {

    [SerializeField]
    protected int level;
    [SerializeField]
    protected float maxHealth;  // the starting health of the mob.
    [SerializeField]
    protected float health;    // the current health of the mob.
    [SerializeField]
    protected float headshotMultiplier;
    protected float deathTime;
    [SerializeField]
    protected float patrolSpeed;
    [SerializeField]
    protected float runningSpeed;
    [SerializeField]
    protected float maxDamage;
    [SerializeField]
    protected float minDamage;
    [SerializeField]
    protected LayerMask hitLayer;
    [SerializeField]
    protected LayerMask clashDetectLayer;

    public CanvasGroup mobCanvas;

    private bool fade = false;
    private bool wasHitThisFrame = false;
    private float fadeSpeed = 20f;
    private float fadeTime = 0.5f;

    protected void Update()
    {
        
        if (fade)
        {
            float a = Mathf.SmoothDamp(mobCanvas.alpha, 0, ref fadeSpeed,fadeTime);
            mobCanvas.alpha = a;
        }

        if (transform.position.y < GameManager.GM.DeadZone && health != 0)
        {
            health = 0;
            Dead();
        }
    }

    protected void LateUpdate()
    {
        wasHitThisFrame = false;
    }

    public float GetHealth() { return health; }

    public float GetMaxHealth() { return maxHealth; }

    protected void Hit(float damage, bool crit = false)
    {
        if (wasHitThisFrame)    // prevent double hitting with same player projectile
        {
            return;
        }
        else
        {
            damage = (int)damage;  // have not decided if float yet...
        
            //Debug.Log("damage=" + damage);
            if (health - damage <= 0)
            {
                health = 0;
                Dead();
            }
            else
            {
                health -= damage;
            }
            MobCanvas MobCanvasScprit = mobCanvas.GetComponentInChildren<MobCanvas>();

            MobCanvasScprit.CreateDmgPopup(damage, crit);
            wasHitThisFrame = true;
        }
    }

    protected void Dead()
    {
        //update kill count
        GameManager.GM.killCount++;
        GameManager.GM.killCountText.text = "X " + GameManager.GM.killCount;


        int dropTypes = Enum.GetValues(typeof(Drop.DropType)).Length;
        
        int amount = (int)Mathf.Pow(UnityEngine.Random.Range(1, 3),level);

        //Drop a new drop
        new Drop((Drop.DropType)UnityEngine.Random.Range(0,dropTypes), amount, transform.position + new Vector3(0, 0.5f, 0));

       

        var m_Animator = GetComponent<Animator>();
        m_Animator.SetBool("dead", true);
        fade = true; // fade GUI
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponentInChildren<CircleCollider2D>());
        Destroy(GetComponentInChildren<CapsuleCollider2D>());

        foreach (var arrow in GetComponentsInChildren<Arrow>())  //Destroy all arrows attached
        {
            Destroy(arrow.gameObject);
        }
        
        Destroy(gameObject, deathTime);  // Finally destroy gameobject
    }
}
