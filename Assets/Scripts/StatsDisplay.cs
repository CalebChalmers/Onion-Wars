using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    private Text text;

    private const float measureTime = 1f;
    private int frames = 0;
    private float prevTime = 0f;
    private int fps = 0;

    void Start()
    {
        text = GetComponent<Text>();
    }

	void Update ()
	{
        frames++;

        if (Time.time - prevTime >= measureTime)
        {
            fps = (int)(frames * (1f / measureTime));
            frames = 0;
            prevTime = Time.time;
        }

        int ping = 0;

        if(NetworkManager.singleton.client != null)
        {
            ping = NetworkManager.singleton.client.GetRTT();
        }

        text.text = string.Format("FPS: {0}\nPing: {1}", fps, ping);
    }
}
