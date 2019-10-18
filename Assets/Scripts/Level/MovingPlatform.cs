using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public float destinationX;
    public float destinationY;
    public float speed;
    public float delayAtDestination;

    private Vector3 posA, posB, nextPosition;
    private float delayTimer;
    private Rigidbody2D rb;
    private Transform pastParent;
    // Use this for initialization
    void Start () {
        posA = transform.localPosition;
        posB = new Vector2(destinationX, destinationY);
        nextPosition = posB;
        delayTimer = 0;
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (delayTimer <= 0)
        {
            MovePlatform();
        }
        else
        {
            delayTimer -= Time.deltaTime;
            rb.velocity = Vector2.zero;
        }
        Debug.Log(delayTimer);
    }

    private void MovePlatform()
    {
        float xMult = nextPosition.x - transform.localPosition.x;
        float yMult = nextPosition.y - transform.localPosition.y;

        if (xMult != 0)
        {
            xMult = xMult / Mathf.Abs(xMult);
        }
        else
        {
            xMult = 0;
        }

        if (yMult != 0)
        {
            yMult = yMult / Mathf.Abs(yMult);
        }
        else
        {
            yMult = 0;
        }
        

        rb.velocity = new Vector2(xMult * speed * Time.deltaTime, yMult * speed);

        if (Vector2.Distance(transform.localPosition, nextPosition) <= 0.1)
        {
            ChangeDestination();
            delayTimer = delayAtDestination;
            
        }
    }

    private void ChangeDestination()
    {
        nextPosition = nextPosition != posA ? posA : posB;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.transform.gameObject == GameManager.GM.Player.gameObject)
        {
            collision.collider.transform.SetParent(transform);
        }
        else if (collision.collider.transform.gameObject.layer == LayerMask.NameToLayer("Mobs"))
        {
            pastParent = collision.collider.transform.parent;
            collision.collider.transform.parent.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.transform.gameObject == GameManager.GM.Player.gameObject)
        {
           collision.collider.transform.SetParent(null);
        }
        else if (collision.collider.transform.gameObject.layer == LayerMask.NameToLayer("Mobs"))
        {
            collision.collider.transform.parent.SetParent(pastParent);
        }
    }
}
