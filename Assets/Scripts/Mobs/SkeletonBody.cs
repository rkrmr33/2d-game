using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBody : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("PlayerProjectiles"))
        {
            transform.parent.gameObject.SendMessage("BodyHit");
        }
    }
}
