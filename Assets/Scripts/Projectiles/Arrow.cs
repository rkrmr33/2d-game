using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    private float hit_depth = 0.30f;
    private float die_out_time_on_miss = 10;
    private float die_out_time_on_hit = 10;
    private bool hit;

    private void OnEnable()
    {   
        // set timer to destroy 
        Destroy(gameObject, die_out_time_on_miss);

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // check for hit on every layer but the player's
        if (col.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            // call function to 
            ArrowStick(col);
            // set timer to destroy the arrow
            Destroy(gameObject, die_out_time_on_hit);
        }
    }

    private void ArrowStick(Collision2D target)
    {
        if (transform.position.x < target.transform.position.x)  //facing right
        {
            transform.Translate(hit_depth * Vector2.right);
            
        }
        else     // facing left
        {
            transform.Translate(hit_depth * Vector2.left);
            
        }

        transform.parent = target.transform;
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Rigidbody2D>());
    }
}
