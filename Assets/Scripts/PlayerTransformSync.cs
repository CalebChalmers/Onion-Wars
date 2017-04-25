using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[NetworkSettings(channel = 0, sendInterval = 1f/30f)]
public class PlayerTransformSync : NetworkBehaviour
{
    public float lerpRate = 15f;
    public float posThreshold = 0.1f;
    public float rotThreshold = 1f;

    private Player player;

    [SyncVar]
    private Vector3 syncPos;
    [SyncVar]
    private Vector2 syncRot;

    private Vector3 lastPos;
    private Vector2 lastRot;

    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            transform.position = Vector3.Lerp(transform.position, syncPos, lerpRate * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, syncRot.y, 0f), lerpRate * Time.deltaTime);
            player.neck.localRotation = Quaternion.Lerp(player.neck.localRotation, Quaternion.Euler(syncRot.x * 0.65f, 0f, 0f), lerpRate * Time.deltaTime);
            player.head.localRotation = Quaternion.Lerp(player.head.localRotation, Quaternion.Euler(syncRot.x * 0.35f, 0f, 0f), lerpRate * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        if (Vector3.Distance(transform.position, lastPos) >= posThreshold)
        {
            lastPos = transform.position;
            CmdUpdatePosition(lastPos);
        }

        float rotX = player.rotationX;
        float rotY = transform.eulerAngles.y;

        if (Mathf.Abs(rotX - lastRot.x) >= rotThreshold || Mathf.Abs(rotY - lastRot.y) >= rotThreshold)
        {
            lastRot = new Vector2(rotX, rotY);
            CmdUpdateRotation(lastRot);
        }
    }
    
    [Command]
    void CmdUpdatePosition(Vector3 pos)
    {
        syncPos = pos;
    }

    [Command]
    void CmdUpdateRotation(Vector2 rot)
    {
        syncRot = rot;
    }
}