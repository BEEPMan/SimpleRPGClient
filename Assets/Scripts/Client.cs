using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Google.Protobuf;
using Google.Protobuf.Collections;
using JetBrains.Annotations;
using System.IO;
using TMPro;

public class Client : MonoBehaviour
{
    public static Client instance = null;

    public TextMeshProUGUI chatBox;

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct sPacketHeader
    {
        public UInt16 size;
        public UInt16 id;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct sPacket
    {
        public sPacketHeader header;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_BUFFER)] public byte[] buffer;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct sPacket_S_LOGIN
    {
        public sPacketHeader header;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_BUFFER)] public Protocol.S_LOGIN buffer;
    }
    public enum ePKCMD : UInt16
    {
        PKT_C_LOGIN = 1000,
        PKT_S_LOGIN = 1001,
        PKT_C_ENTER_GAME = 1002,
        PKT_S_ENTER_GAME = 1003,
        PKT_C_LEAVE_GAME = 1004,
        PKT_S_LEAVE_GAME = 1005,
        PKT_C_CHAT = 1006,
        PKT_S_CHAT = 1007,
        PKT_C_MOVE = 1008,
        PKT_S_MOVE = 1009,
    }

    public const int MAX_BUFFER = 65532;
    private string iPAdress = "127.0.0.1";
    private const int port = 29000;

    private byte[] recvPacket = new byte[MAX_BUFFER];

    private string playerName;
    private UInt64 playerID = 0;

    private Socket socket;

    private string recvString;

    private Queue<byte[]> messageQueue = new Queue<byte[]>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);

        try
        {
            IPAddress ipAddr = System.Net.IPAddress.Parse(iPAdress);
            IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, port);
            socket.Connect(ipEndPoint);
        }
        catch (SocketException SCE)
        {
            Debug.Log("Socket Connection Error: " + SCE.ToString());
            return;
        }

        Debug.Log("Server Connected!");
        socket.BeginReceive(recvPacket, 0, MAX_BUFFER, SocketFlags.None, new AsyncCallback(OnReceiveCallBack), socket);
    }

    private void Update()
    {
        if (messageQueue.Count > 0)
        {
            ProcessMessage(messageQueue.Dequeue());
        }
    }

    void OnApplicationQuit()
    {
        LeaveGame();
        socket.Close();
        socket = null;
    }

    IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOp.isDone)
        {
            Debug.Log(asyncOp.progress);
            yield return null;
        }
    }

    void ProcessMessage(byte[] packet)
    {
        Debug.Log("Recv Packet");

        sPacketHeader header = ByteToHeader(packet);
        ePKCMD cmd = (ePKCMD)((int)header.id);
        Debug.Log(cmd);

        switch (cmd)
        {
            case ePKCMD.PKT_S_LOGIN:
                {
                    Debug.Log("Recv S_LOGIN Packet");
                    Protocol.S_LOGIN recvData = Protocol.S_LOGIN.Parser.ParseFrom(packet, 4, header.size - 4);

                    playerID = recvData.PlayerId;

                    for (int i = 0; i < recvData.Players.Count; i++)
                    {
                        if (playerID == recvData.Players[i].Id)
                        {
                            playerName = recvData.Players[i].Name;
                            UserManager.instance.EnterMyPlayer(playerID, playerName);
                            continue;
                        }
                        Debug.Log(recvData.Players[i].Id + "_" + recvData.Players[i].Name);
                        UserManager.instance.EnterPlayer(recvData.Players[i].Id, recvData.Players[i].Name, recvData.Players[i].X, recvData.Players[i].Y);
                    }

                    EnterGame();

                    chatBox.text += playerName + " logged in.\n";

                    break;
                }
            case ePKCMD.PKT_S_CHAT:
                {
                    Debug.Log("Recv S_CHAT Packet");
                    Protocol.S_CHAT recvData = Protocol.S_CHAT.Parser.ParseFrom(packet, 4, header.size - 4);

                    string userName = UserManager.instance.playerList[recvData.PlayerId].name;
                    chatBox.text += userName + ": " + recvData.Msg + "\n";

                    break;
                }
            case ePKCMD.PKT_S_ENTER_GAME:
                {
                    Debug.Log("Recv S_ENTER_GAME Packet");
                    Protocol.S_ENTER_GAME recvData = Protocol.S_ENTER_GAME.Parser.ParseFrom(packet, 4, header.size - 4);

                    Protocol.Player enterPlayer = recvData.Player;
                    UserManager.instance.EnterPlayer(enterPlayer.Id, enterPlayer.Name, enterPlayer.X, enterPlayer.Y);
                    chatBox.text += UserManager.instance.GetPlayerName(enterPlayer.Id) + " logged in.\n";

                    break;
                }
            case ePKCMD.PKT_S_LEAVE_GAME:
                {
                    Debug.Log("Recv S_ENTER_GAME Packet");
                    Protocol.S_LEAVE_GAME recvData = Protocol.S_LEAVE_GAME.Parser.ParseFrom(packet, 4, header.size - 4);

                    UInt64 leavePlayerId = recvData.PlayerId;
                    chatBox.text += UserManager.instance.GetPlayerName(leavePlayerId) + " logged out.\n";
                    UserManager.instance.LeavePlayer(leavePlayerId);

                    break;
                }
            case ePKCMD.PKT_S_MOVE:
                {
                    Debug.Log("Recv S_MOVE Packet");
                    Protocol.S_MOVE recvData = Protocol.S_MOVE.Parser.ParseFrom(packet, 4, header.size - 4);

                    UserManager.instance.MovePlayer(recvData.PlayerId, recvData.X, recvData.Y);

                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    void OnReceiveCallBack(IAsyncResult iAR)
    {
        Socket clientSocket = (Socket)iAR.AsyncState;
        int recvBytes = clientSocket.EndReceive(iAR);

        if (recvBytes > 0)
        {
            messageQueue.Enqueue(recvPacket);
        }
        AsyncReceive();
    }

    void AsyncReceive()
    {
        socket.BeginReceive(recvPacket, 0, MAX_BUFFER, SocketFlags.None, new AsyncCallback(OnReceiveCallBack), socket);
    }

    void StructToBytes(object obj, ref byte[] packet)
    {
        int size = Marshal.SizeOf(obj);
        IntPtr buffer = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(obj, buffer, false);
        Marshal.Copy(buffer, packet, 0, size);
        Marshal.FreeHGlobal(buffer);
    }

    sPacketHeader ByteToHeader(byte[] buffer)
    {
        int size = Marshal.SizeOf<sPacketHeader>();
        Debug.Log(size);
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(buffer, 0, ptr, size);
        sPacketHeader obj = (sPacketHeader)Marshal.PtrToStructure(ptr, typeof(sPacketHeader));
        Marshal.FreeHGlobal(ptr);
        return obj;
    }

    public void Login(string name)
    {
        sPacketHeader header = new sPacketHeader();
        Protocol.C_LOGIN tmp = new Protocol.C_LOGIN();
        tmp.PlayerName = name;
        MemoryStream memoryStream = new MemoryStream();
        tmp.WriteTo(memoryStream);
        header.id = (UInt16)ePKCMD.PKT_C_LOGIN;
        header.size = (UInt16)(sizeof(byte) * memoryStream.Length + 4);

        byte[] sendBuffer = new byte[header.size];
        StructToBytes(header, ref sendBuffer);
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.Read(sendBuffer, 4, (int)memoryStream.Length);

        socket.Send(sendBuffer);
    }

    public void EnterGame()
    {
        sPacketHeader header = new sPacketHeader();
        Protocol.C_ENTER_GAME tmp = new Protocol.C_ENTER_GAME();
        tmp.PlayerId = playerID;
        MemoryStream memoryStream = new MemoryStream();
        tmp.WriteTo(memoryStream);
        header.id = (UInt16)ePKCMD.PKT_C_ENTER_GAME;
        header.size = (UInt16)(sizeof(byte) * memoryStream.Length + 4);

        byte[] sendBuffer = new byte[header.size];
        StructToBytes(header, ref sendBuffer);
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.Read(sendBuffer, 4, (int)memoryStream.Length);

        socket.Send(sendBuffer);
    }

    public void LeaveGame()
    {
        sPacketHeader header = new sPacketHeader();
        Protocol.C_LEAVE_GAME tmp = new Protocol.C_LEAVE_GAME();
        tmp.PlayerId = playerID;
        MemoryStream memoryStream = new MemoryStream();
        tmp.WriteTo(memoryStream);
        header.id = (UInt16)ePKCMD.PKT_C_LEAVE_GAME;
        header.size = (UInt16)(sizeof(byte) * memoryStream.Length + 4);

        byte[] sendBuffer = new byte[header.size];
        StructToBytes(header, ref sendBuffer);
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.Read(sendBuffer, 4, (int)memoryStream.Length);

        socket.Send(sendBuffer);
    }

    public void Chat(string msg)
    {
        sPacketHeader header = new sPacketHeader();
        Protocol.C_CHAT tmp = new Protocol.C_CHAT();
        tmp.PlayerId = playerID;
        tmp.Msg = msg;
        MemoryStream memoryStream = new MemoryStream();
        tmp.WriteTo(memoryStream);
        header.id = (UInt16)ePKCMD.PKT_C_CHAT;
        header.size = (UInt16)(sizeof(byte) * memoryStream.Length + 4);

        byte[] sendBuffer = new byte[header.size];
        StructToBytes(header, ref sendBuffer);
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.Read(sendBuffer, 4, (int)memoryStream.Length);

        socket.Send(sendBuffer);
    }

    public void Move(float x, float y)
    {
        sPacketHeader header = new sPacketHeader();
        Protocol.C_MOVE tmp = new Protocol.C_MOVE();
        tmp.PlayerId = playerID;
        tmp.X = x;
        tmp.Y = y;
        MemoryStream memoryStream = new MemoryStream();
        tmp.WriteTo(memoryStream);
        header.id = (UInt16)ePKCMD.PKT_C_MOVE;
        header.size = (UInt16)(sizeof(byte) * memoryStream.Length + 4);

        byte[] sendBuffer = new byte[header.size];
        StructToBytes(header, ref sendBuffer);
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.Read(sendBuffer, 4, (int)memoryStream.Length);

        socket.Send(sendBuffer);
    }
}