using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameObject menu;
    public Text playerListText;
    public Text scoreListText;
    public Text timerText;
    public Text notifyText;
    public Animator scorePanelAnimator;
    public Animator notifyAnimator;
    public Transform sunDolly;
    public GameObject tempCam;
    public Slider mouseSensitivitySlider;
    public Animator loadingPanelAnimator;

    [Header("Settings")]
    public float gameTimeSeconds;
    public float warmupTimeSeconds;


    [HideInInspector]
    public float mouseSensitivity = 1.0f;

    public GameState State { get; private set; }
    public bool MenuOpen { get; private set; }

    private List<Player> players;
    private Animator menuAnimator;
    private DateTime startTime;
    private TimeSpan gameTime;
    private TimeSpan warmupTime;
    //private float sunRotation;

    private const short starttimecode = 130;
    private const string starttimeformat = "yyyyMMddHHmmssffff";

    public static GameManager instance = null;

    void Awake()
    {
        if (NetworkManager.singleton == null)
        {
            SceneManager.LoadScene(0);
            return;
        }

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        State = GameState.Warmup;
    }
    
    void Start()
    {
        Destroy(tempCam);

        CursorHelper.CursorLocked = true;

        menuAnimator = menu.GetComponent<Animator>();

        players = new List<Player>();
        //GetPlayers();

        startTime = DateTime.Now;
        gameTime = TimeSpan.FromSeconds(gameTimeSeconds);
        warmupTime = TimeSpan.FromSeconds(warmupTimeSeconds);

        //sunRotation = sunDolly.localEulerAngles.x;
        
        mouseSensitivitySlider.value = mouseSensitivity = PlayerPrefs.GetFloat("mouse_sensitivity", mouseSensitivitySlider.value);
        
        if (NetworkServer.active)
        {
            NetworkServer.RegisterHandler(starttimecode, ServerReceiveStartTimeMessage);
        }
        else
        {
            NetworkClient client = NetworkManager.singleton.client;
            client.RegisterHandler(starttimecode, ClientReceiveStartTimeMessage);
            client.Send(starttimecode, new EmptyMessage());
        }
    }

    private void ClientReceiveStartTimeMessage(NetworkMessage message)
    {
        string timestring = message.ReadMessage<StringMessage>().value;
        startTime = DateTime.ParseExact(timestring, starttimeformat, CultureInfo.InvariantCulture);
    }

    private void ServerReceiveStartTimeMessage(NetworkMessage message)
    {
        StringMessage msg = new StringMessage(startTime.ToString(starttimeformat));
        NetworkServer.SendToClient(message.conn.connectionId, starttimecode, msg);
    }

    void Update()
    {
        //sunDolly.Rotate(10f * Time.deltaTime, 0f, 0f, Space.Self);
        /*float sunMovement = 0.1f * Time.deltaTime;
        sunDolly.rotation = sunDolly.rotation * Quaternion.AngleAxis(sunMovement, Vector3.right);
        sunRotation += sunMovement;
        if (sunRotation > 360f) sunRotation -= 360f;
        float factor = Mathf.Cos(sunRotation * Mathf.Deg2Rad);
        RenderSettings.skybox.SetFloat("_Exposure", factor * 0.5f + 0.5f);*/

        if (Input.GetKeyDown(KeyCode.Escape) && !ChatManager.instance.ChatInputOpen)
        {
            SetMenuOpen(!MenuOpen);
        }

        if(State != GameState.Ended)
        {
            DateTime now = DateTime.Now;
            TimeSpan time = now - startTime;
            if (time <= warmupTime)
            {
                timerText.text = "Warmup: " + GetFormattedGameTime(warmupTime - time);
            }
            else
            {
                StartGame();
                timerText.text = "Game: " + GetFormattedGameTime(gameTime - time + warmupTime);
            }

            if (time > warmupTime + gameTime)
            {
                EndGame();
            }
        }
        else
        {
            CursorHelper.CursorLocked = false;
        }
    }

    private void StartGame()
    {
        if (State == GameState.Started || State == GameState.Ended) return;
        State = GameState.Started;

        print("Game Start!");
        Notify("Game Start");
    }

    private void EndGame()
    {
        if (State == GameState.Ended) return;
        State = GameState.Ended;

        print("Game Over!");
        Notify("Game Over");
        Invoke("MoveScorePanel", 3.0f);
        //Invoke("Disconnect", 10.0f);
        //Disconnect();
    }

    public void Notify(string text)
    {
        notifyText.text = text;
        notifyAnimator.SetTrigger("notify");
    }

    public void MoveScorePanel()
    {
        scorePanelAnimator.SetTrigger("endgame");
    }

    public void SetMenuOpen(bool open)
    {
        MenuOpen = open;
        CursorHelper.CursorLocked = !MenuOpen;

        menuAnimator.SetBool("open", MenuOpen);
    }

    public void Disconnect()
    {
        NetworkManager manager = NetworkManager.singleton;

        loadingPanelAnimator.SetBool("open", true);

        if (NetworkServer.active)
        {
            manager.matchMaker.DestroyMatch(manager.matchInfo.networkId, 1, OnMatchDestroy);
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    public void OnMatchDestroy(bool success, string extendedInfo)
    {
        if(success)
        {
            Debug.Log("Match Destroyed");
        }
        else
        {
            Debug.Log("Match Destroy Failed");
        }
        
        NetworkManager.singleton.StopHost();
    }

    private void GetPlayers()
    {
        players.Clear();
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject obj in playerObjects)
        {
            Player player = obj.GetComponent<Player>();
            players.Add(player);
        }
    }

    public void AddPlayer(Player player)
    {
        if (player == null) return;
        players.Add(player);
        UpdateScoreboard();
    }

    public void RemovePlayer(Player player)
    {
        if (player == null) return;
        players.Remove(player);
        UpdateScoreboard();
    }

    public void UpdateScoreboard()
    {
        string playersList = "";
        string scoresList = "";

        Player[] sortedPlayers = players.OrderByDescending(p => p.score).ToArray();
        
        for(int i = 0; i < sortedPlayers.Length; i++)
        {
            Player player = sortedPlayers[i];

            if (i > 0)
            {
                playersList += Environment.NewLine;
                scoresList += Environment.NewLine;
            }

            playersList += player.username + ":";
            scoresList += player.score;
        }

        playerListText.text = playersList;
        scoreListText.text = scoresList;
    }

    private string GetFormattedGameTime(TimeSpan span)
    {
        int precision = 0;
        int factor = (int)Math.Pow(10, (7 - precision));
        TimeSpan rounded = new TimeSpan((long)Mathf.Ceil(1.0f * span.Ticks / factor) * factor);
        return string.Format("{0:00}:{1:00}", rounded.Minutes, rounded.Seconds);
    }

    public void UpdateMouseSensitivity(float newValue)
    {
        mouseSensitivity = newValue;
        PlayerPrefs.SetFloat("mouse_sensitivity", mouseSensitivity);
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.Save();
    }
}

public enum GameState
{
    Warmup,
    Started,
    Ended
}
