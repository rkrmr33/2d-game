using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScript : MonoBehaviour {

    public TextMeshProUGUI velocityMeter;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (GameManager.GM._debugMode)
        {
            velocityMeter.text = "Velocity: " + GameManager.GM.Player.GetComponent<Rigidbody2D>().velocity.x +"\nInput: " + Input.GetAxis("Horizontal");

        }
	}
}
