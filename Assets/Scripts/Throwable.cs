using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Throwable : NetworkBehaviour
{
    private Player thrower;
    private bool hitGround = false;

    [Server]
    public void Throw(Player thrower)
    {
        this.thrower = thrower;
        hitGround = false;
    }

    [ServerCallback]
    void OnCollisionEnter(Collision col)
    {
        if (!hitGround && col.gameObject.CompareTag("Player"))
        {
            if(col.gameObject != thrower.gameObject)
            {
                thrower.score++;
            }
        }
        else
        {
            hitGround = true;
        }
    }
}
