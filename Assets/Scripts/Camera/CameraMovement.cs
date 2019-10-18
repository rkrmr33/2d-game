using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    private Vector2 velocity;
    private bool passed = false;

    public float maxDistance;
    public float smoothX;
    public float smoothY; 
    public GameObject player;
	
	void FixedUpdate () {
        if ((Vector2.Distance(transform.position, player.transform.position) > maxDistance || passed ) && player.transform.position.y >= (GameManager.GM.DeadZone+10))
        {
            passed = true;
            MoveCamera();
        }
	}

    private void MoveCamera()
    {
        float posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, smoothX);
        float posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y, ref velocity.y, smoothY);

        transform.position = new Vector3(posX, posY, -1);

        if (transform.position == player.transform.position)
        {
            passed = false;
        }
        
    }
}
