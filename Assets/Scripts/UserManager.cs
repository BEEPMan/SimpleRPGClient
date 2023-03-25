using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using TMPro;

public class UserManager : MonoBehaviour
{
    public static UserManager instance = null;

    public GameObject player;

    public enum PlayerType
    {
        PLAYER_TYPE_NONE = 0,
        PLAYER_TYPE_KNIGHT = 1,
        PLAYER_TYPE_MAGE = 2,
        PLAYER_TYPE_ARCHER = 3,
    }

    public struct sPlayer
    {
        public UInt64 playerId;
        public string name;
        public PlayerType playerType;
        public GameObject body;
        public Animator anim;
    }

    public Dictionary<UInt64,sPlayer> playerList;
    public UInt16 playerCount = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            playerList = new Dictionary<ulong, sPlayer>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public string GetPlayerName(UInt64 playerId)
    {
        return playerList[playerId].name;
    }

    public void EnterMyPlayer(UInt64 playerId, string name)
    {
        sPlayer newPlayer = new sPlayer();
        newPlayer.playerId = playerId;
        newPlayer.name = name;
        playerList.Add(playerId, newPlayer);
        playerCount++;
    }

    public void EnterPlayer(UInt64 playerId, string name, float x, float y)
    {
        sPlayer newPlayer = new sPlayer();
        newPlayer.playerId = playerId;
        newPlayer.name = name;
        newPlayer.body = Instantiate(player, new Vector3(x, y, 0), Quaternion.identity);
        newPlayer.body.GetComponentInChildren<TextMeshPro>().text = newPlayer.name;
        newPlayer.anim = newPlayer.body.GetComponent<Animator>();
        playerList.Add(playerId, newPlayer);
        playerCount++;
    }

    public void LeavePlayer(UInt64 playerId)
    {
        GameObject.Destroy(playerList[playerId].body);
        playerList.Remove(playerId);
    }

    public void MovePlayer(UInt64 playerId, float x, float y)
    {
        float deltaX = x - playerList[playerId].body.transform.position.x;
        float deltaY = y - playerList[playerId].body.transform.position.y;

        if (Math.Abs(deltaX) > Math.Abs(deltaY))
        {
            playerList[playerId].anim.SetInteger("vAxisRaw", 0);
            playerList[playerId].anim.SetBool("isChange", true);
            if (deltaX > 0)
                playerList[playerId].anim.SetInteger("hAxisRaw", 1);
            else
                playerList[playerId].anim.SetInteger("hAxisRaw", -1);
        }
        else if (Math.Abs(deltaX) < Math.Abs(deltaY))
        {
            playerList[playerId].anim.SetInteger("hAxisRaw", 0);
            playerList[playerId].anim.SetBool("isChange", true);
            if (deltaY > 0)
                playerList[playerId].anim.SetInteger("vAxisRaw", 1);
            else
                playerList[playerId].anim.SetInteger("vAxisRaw", -1);
        }

        playerList[playerId].body.transform.position = new Vector3(x, y, 0);
    }
}
