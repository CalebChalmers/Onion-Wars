using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;

public class Matchmaking : MonoBehaviour
{
    NetworkMatch matchMaker;
    void Awake()
    {
        matchMaker = gameObject.AddComponent<NetworkMatch>();

        // Create
        matchMaker.CreateMatch("roomName", 4, true, "", "", "", 0, 0, OnMatchCreate);
        
        // List
        matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);

        // Join
        matchMaker.JoinMatch(networkId, "", "", "", 0, 0, OnMatchJoined);
    }

    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {

    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {

    }

    public void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {

    }
}
