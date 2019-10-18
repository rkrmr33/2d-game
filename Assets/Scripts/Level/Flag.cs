using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Flag : MonoBehaviour {

    private Animator m_anim; 

	// Use this for initialization
	void Start () {
        m_anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Physics2D.CircleCast(transform.position, 1.5f, Vector3.forward, 1.5f, GameManager.GM.playerLayer))
        {
            m_anim.SetBool("reached", true);
            GameManager.GM.Player.GetComponent<Player>().EnableMovement(false);
            TextMeshProUGUI[] texts = GameManager.GM.MessageBoard.GetComponentsInChildren<TextMeshProUGUI>();   // Setting message board text
            texts[0].text = "YOU WON!";
            texts[1].text = "PRESS [ENTER] TO GO TO THE NEXT LEVEL...";
            texts[1].fontSize = 36;
            GameManager.GM.MessageBoard.GetComponent<Animator>().SetInteger("State", 1); // Appear animation.
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GameManager.GM.LoadScene("Level2");
            }
        }
	}
}
