using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public InputField usernameInputField;
    public InputField hostPortInputField;
    public InputField hostMatchNameInputField;
    public Dropdown joinMatchDropdown;

    public Animator loadingPanelAnimator;

    private NetworkManager manager;
    private string matchName = "";
    private int matchIndex = 0;
    private List<MatchInfoSnapshot> matches;

    void Start()
    {
        manager = NetworkManager.singleton;
        matches = new List<MatchInfoSnapshot>();

        CursorHelper.CursorLocked = false;

        LoadPrefs();

        manager.StartMatchMaker();
    }

    void Update()
    {
        Camera cam = Camera.main;
        Vector3 rot = cam.transform.eulerAngles;
        rot.y = 101.44f - Mathf.Sin(Time.time / 40.0f) * 50f;
        cam.transform.eulerAngles = rot;
    }

    public void Host()
    {
        SavePrefs();
        
        Debug.Log("Creating Match...");

        SetLoading(true);
        manager.matchMaker.CreateMatch(matchName, 10, true, "", "", "", 0, 1, OnMatchCreate);
    }

    public void Join()
    {
        if (matches.Count == 0 || matchIndex >= matches.Count) return;

        SavePrefs();
        print(manager.matches == null);
        
        SetLoading(true);
        manager.matchMaker.JoinMatch(matches[matchIndex].networkId, "", "", "", 0, 1, OnMatchJoined);
        Debug.Log("Joining...");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetPort(string val)
    {
        int result;
        if (int.TryParse(val, out result))
        {
            manager.networkPort = result;
        }
    }

    public void SetMatchName(string val)
    {
        matchName = val;
    }

    public void SetMatchIndex(int index)
    {
        matchIndex = index;
    }

    public void RefreshMatchList()
    {
        Debug.Log("Refreshing matches");

        SetLoading(true);
        manager.matchMaker.ListMatches(0, 5, "", false, 0, 1, OnMatchList);
    }

    private void LoadPrefs()
    {
        usernameInputField.text = PlayerPrefs.GetString("username", "");
        hostPortInputField.text = PlayerPrefs.GetString("host_port", manager.networkPort.ToString());
        hostMatchNameInputField.text = PlayerPrefs.GetString("host_match_name", "");
    }

    private void SavePrefs()
    {
        PlayerPrefs.SetString("username", usernameInputField.text);
        PlayerPrefs.SetString("host_port", hostPortInputField.text);
        PlayerPrefs.SetString("host_match_name", hostMatchNameInputField.text);
        PlayerPrefs.Save();
    }

    public void SetLoading(bool loading)
    {
        loadingPanelAnimator.SetBool("open", loading);
    }

    private void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        if (success)
        {
            if (matchList.Count != 0)
            {
                Debug.Log("Matches Found");

                matches = matchList;

                joinMatchDropdown.ClearOptions();
                List<string> options = new List<string>(matchList.Count);

                for (int i = 0; i < matchList.Count; i++)
                {
                    options.Add(matchList[i].name);
                }

                joinMatchDropdown.AddOptions(options);
            }
            else
            {
                Debug.Log("No Matches Found");
            }
        }
        else
        {
            Debug.Log("Match Search Failure");
        }

        SetLoading(false);
    }

    private void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Match Joined");
            manager.StartClient(matchInfo);
        }
        else
        {
            Debug.Log("Match Join Failure");
        }
    }

    private void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Match Created");
            //NetworkServer.Listen(matchInfo, manager.networkPort);
            manager.StartHost(matchInfo);
        }
        else
        {
            Debug.Log("Match Create Failure");
        }
    }
}
