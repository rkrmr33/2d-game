using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Drop : MonoBehaviour {

    public DropType type { get; set; }
    public float amount { get; set; }

    private string path_arrows = "Sprites/drop_arrows";
    private bool startPickupAnimation = false;

    public Drop(DropType type, float amount, Vector3 position)
    {
        
        if (type != DropType.NONE)
        {
            GameObject drop = Instantiate(Resources.Load<GameObject>("Prefabs/drop"), position, Quaternion.identity);
            drop.GetComponent<Drop>().type = type;
            drop.GetComponent<Drop>().amount = amount;
            drop.GetComponentInChildren<Canvas>().GetComponentInChildren<TextMeshProUGUI>().text = "x " + amount.ToString();
            drop.GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(path_arrows);
        }
    }

    

	// Update is called once per frame
	void Update () {
        bool pickedUp = Physics2D.OverlapCircle(transform.position, .5f, GameManager.GM.playerLayer);

        if (GameManager.GM._debugMode)
        {
            Debug.DrawRay(transform.position, Vector2.up * .5f);
            Debug.DrawRay(transform.position, Vector2.down * .5f);
            Debug.DrawRay(transform.position, Vector2.left * .5f);
            Debug.DrawRay(transform.position, Vector2.right * .5f);
        }
                        // so the pickup is called only once
        if (pickedUp && !startPickupAnimation)
        {
            PickedUp();
            startPickupAnimation = true;
        }

        if (startPickupAnimation)
        {
            PickUpAnimation();
        }
    }

    private void PickedUp()
    {
        switch (this.type)
        {
            case DropType.ARROWS:
                GameManager.GM.Player.GetComponent<Player>().GiveArrows((int)this.amount);
                break;
        }
    }

    private void PickUpAnimation()
    {

        if (transform.localScale.x <= 0.1f)
        {
            Destroy(gameObject);
        }
        Vector3 newScale = Vector3.Slerp(transform.localScale, new Vector3(0, 0, 1), 0.05f);
        Vector3 newPosition = Vector3.Slerp(transform.position, GameManager.GM.Player.transform.position, 0.1f);
        transform.localScale = newScale;
        transform.position = newPosition;
    }

    public enum DropType
    {
        NONE = 0,
        ARROWS = 1,

    }
}
