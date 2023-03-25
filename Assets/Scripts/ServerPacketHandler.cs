using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using Google.Protobuf;
using Google.Protobuf.Collections;

//[Serializable]
//public struct PacketHeader
//{
//    public UInt16 size;
//    public UInt16 id;
//}

//[Serializable]
//public struct Packet
//{
//    public PacketHeader header;
//    public byte[] buffer;
//}
//public enum PId : UInt16
//{
//    PKT_C_LOGIN = 1000,
//    PKT_S_LOGIN = 1001,
//    PKT_C_ENTER_GAME = 1002,
//    PKT_S_ENTER_GAME = 1003,
//    PKT_C_CHAT = 1004,
//    PKT_S_CHAT = 1005,
//}

public class ServerPacketHandler
{
    //private ClientInterface client;
    //public bool HandlePacket(Packet pkt, Int32 len)
    //{
    //    switch((PId)pkt.header.id)
    //    {
    //        case PId.PKT_S_LOGIN:
    //            Handle_S_LOGIN(Protocol.S_LOGIN.Parser.ParseFrom(pkt.buffer));
    //            break;
    //        case PId.PKT_S_ENTER_GAME:
    //            Handle_S_ENTER_GAME(Protocol.S_ENTER_GAME.Parser.ParseFrom(pkt.buffer));
    //            break;
    //        case PId.PKT_S_CHAT:
    //            Handle_S_CHAT(Protocol.S_CHAT.Parser.ParseFrom(pkt.buffer));
    //            break;
    //        default:
    //            Handle_INVALID(pkt.buffer);
    //            break;
    //    }
    //    return true;
    //}

    //bool Handle_INVALID(byte[] buffer)
    //{
    //    Debug.Log("Invalid Packet");
    //    return true;
    //}
    //bool Handle_S_LOGIN(Protocol.S_LOGIN pkt)
    //{
    //    if (pkt.Success == false)
    //    {
    //        Debug.Log("Login Failed");
    //        return true;
    //    }

    //    for (int i = 0; i < pkt.Players.Count; i++)
    //    {
    //        Debug.Log("Player " + pkt.Players[i].Id + " : " + pkt.Players[i].Name);
    //    }

    //    // TODO : Load Other Players

    //    Protocol.C_ENTER_GAME sendPkt = new Protocol.C_ENTER_GAME();
    //    sendPkt.PlayerName = "Hero1";

    //    client.SendPacket(sendPkt);

    //    return true;
    //}
    //bool Handle_S_ENTER_GAME(Protocol.S_ENTER_GAME pkt)
    //{
    //    // TODO : Delete Later

    //    return true;
    //}
    //bool Handle_S_CHAT(Protocol.S_CHAT pkt)
    //{
    //    Debug.Log(pkt.Msg);
    //    return true;
    //}

}
