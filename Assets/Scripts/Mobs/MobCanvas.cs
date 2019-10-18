using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MobCanvas : MonoBehaviour {

    
    public Image healthFill;
    public TextMeshProUGUI healthText;
    public GameObject DmgPopupObject;
    public GameObject CritDmgPopupObject;
    private float lerpSpeed = 20;
    private Enemy mob;

    // Use this for initialization
    void Start () {
        mob = GetComponentInParent<Enemy>();
	}
	
	// Update is called once per frame
	void Update () {
        HandleBar();
	}

    void HandleBar()
    {
        float health = mob.GetHealth(), maxHealth = mob.GetMaxHealth();
        
        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, health / maxHealth, Time.deltaTime * lerpSpeed);
        healthText.text = health + " / " + maxHealth;
    }

    public void CreateDmgPopup(float damage, bool crit)
    {
        GameObject dmg;
        if (crit)
        {
            dmg = CritDmgPopupObject;
        }
        else
        {
            dmg = DmgPopupObject;
        }

        GameObject dmgPopupPrefab = Instantiate(dmg);
        dmgPopupPrefab.transform.SetParent(transform);
        dmgPopupPrefab.transform.position = mob.transform.position;
        TextMeshProUGUI popupText = dmgPopupPrefab.GetComponentInChildren<TextMeshProUGUI>();
        popupText.text = "-" + ((int)damage).ToString(); //Set text to dmg
        Destroy(dmgPopupPrefab, popupText.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);  //Destroy prefab after the animation
    }
}
