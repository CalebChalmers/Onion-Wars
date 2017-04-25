using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public Text chatText;
    public InputField chatInputField;
    public GameObject chatInputPanel;

    public bool ChatInputOpen { get; private set; }

    public static ChatManager instance = null;

    private const short code = 131;
    private List<ChatMessage> messages;

    private const float messageLifeTime = 6.0f;
    private const float messageFadeTime = 0.5f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance == this)
            Destroy(this);
    }

    void Start()
    {
        messages = new List<ChatMessage>();

        NetworkManager.singleton.client.RegisterHandler(code, ReceiveMessage);
        
        if (NetworkServer.active)
        {
            NetworkServer.RegisterHandler(code, ServerReceiveMessage);
        }
    }

    void Update()
    {
        StringBuilder chatBuilder = new StringBuilder();
        bool first = true;
        for (int i = 0; i < messages.Count; i++)
        {
            ChatMessage message = messages[i];
            byte alpha = 255;

            if(!ChatInputOpen)
            {
                float time = Time.time - message.time;

                if (time > messageLifeTime + messageFadeTime)
                {
                    continue;
                }
                else if (time > messageLifeTime)
                {
                    alpha = (byte)((messageLifeTime - time) / messageFadeTime * 255);
                }
            }

            if (first)
            {
                first = false;
            }
            else
            {
                chatBuilder.Append(Environment.NewLine);
            }

            chatBuilder.Append(string.Format("<color=#FFFFFF{1}>{0}</color>", message.message, alpha.ToString("X2")));
        }
        chatText.text = chatBuilder.ToString();

        if (Input.GetKeyDown(KeyCode.T) && !GameManager.instance.MenuOpen)
        {
            chatInputPanel.SetActive(true);
            chatInputField.Select();
            chatInputField.ActivateInputField();
            ChatInputOpen = true;
            CursorHelper.CursorLocked = false;
        }

        if(ChatInputOpen)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessage();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                chatInputPanel.SetActive(false);
                ChatInputOpen = false;
                CursorHelper.CursorLocked = true;
            }
        }
    }

    private void AddMessage(string message)
    {
        messages.Add(new ChatMessage(message));
    }

    private void ReceiveMessage(NetworkMessage message)
    {
        string text = message.ReadMessage<StringMessage>().value;
        AddMessage(text);
    }

    public void SendMessage()
    {
        string text = chatInputField.text;

        if (string.IsNullOrEmpty(text))
            return;

        chatInputField.text = "";
        CursorHelper.CursorLocked = true;
        ChatInputOpen = false;

        chatInputPanel.SetActive(false);

        StringMessage myMessage = new StringMessage();
        myMessage.value = "[" + PlayerPrefs.GetString("username", "Player") + "] " + text;

        NetworkManager.singleton.client.Send(code, myMessage);
    }

    private void ServerReceiveMessage(NetworkMessage message)
    {
        StringMessage myMessage = new StringMessage();
        myMessage.value = message.ReadMessage<StringMessage>().value;
        
        NetworkServer.SendToAll(code, myMessage);
    }
}
