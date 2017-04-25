using UnityEngine;

public class ChatMessage
{
    public string message;
    public float time;

    public ChatMessage(string message)
    {
        this.message = message;
        this.time = Time.time;
    }
}
