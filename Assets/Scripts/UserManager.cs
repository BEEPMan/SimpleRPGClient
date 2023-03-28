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
        public Protocol.MoveInfo moveInfo;
        public UserMove move;
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
        newPlayer.move = newPlayer.body.GetComponent<UserMove>();
        newPlayer.move.playerId = playerId;
        playerList.Add(playerId, newPlayer);
        playerCount++;
    }

    public void LeavePlayer(UInt64 playerId)
    {
        GameObject.Destroy(playerList[playerId].body);
        playerList.Remove(playerId);
    }

    public void MovePlayer(UInt64 playerId, float x, float y, Protocol.MoveInfo info)
    {
        playerList[playerId].move.SetMoveInfo(info.H, info.V, (int)info.HKey, (int)info.VKey);

        playerList[playerId].body.transform.position = new Vector3(x, y, 0);
    }
}
