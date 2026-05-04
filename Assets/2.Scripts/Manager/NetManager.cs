using DefineStructure;
using Protocols;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class NetManager : TSingleton<NetManager>
{
    const string _serverHost = "127.0.0.1";
    const short _serverPort = 222;
    const short _clientPort = 333;

    public ulong _uuid {  get; private set; }

    Queue<Packet> _sendQueue;
    Queue<Packet> _receiveQueue;

    bool _isQuit;

    Socket _socketServer;

    private void Update()
    {
        //if (_socketServer == null || !_socketServer.Connected) return;

        //if (_socketServer.Poll(0, SelectMode.SelectRead))
        //{
        //    byte[] buffer = new byte[1024];
        //    int receiveLength = _socketServer.Receive(buffer);

        //    if (receiveLength > 0)
        //    {
        //        Packet pack = PacketConverter.ByteArrayToStructure<Packet>(buffer, 1024);
        //        Debug.Log(pack._uuid + ":" + pack._protocol);

        //        ReceiveQueueIn(pack);
        //    }
        //}
    }

    public void ConnectServer()
    {
        _sendQueue = new Queue<Packet>();
        _receiveQueue = new Queue<Packet>();

        _socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socketServer.Connect(_serverHost, _serverPort);

        StartCoroutine(SendQueue());
        StartCoroutine(ReceiveQueue());
        StartCoroutine(SocketRead());   
        Debug.Log("서버 연결 성공");
    }

    public void SendQueueIn(Packet pack)
    {
        _sendQueue.Enqueue(pack);
    }
    public void ReceiveQueueIn(Packet pack)
    {
        _receiveQueue.Enqueue(pack);
    }

    public void GetAllUserInfo(Packet_UserData userdata)
    {
        TotalData data = DataManger._instance._totalData;

        data._currentDiamond._totalDiamond = (int)userdata._diamond;
        data._score._monsterKillCountDic.Add("Skeleton", (int)userdata._skeleton);
        data._score._monsterKillCountDic.Add("Slime", (int)userdata._slime);
        data._score._monsterKillCountDic.Add("Bat", (int)userdata._bat);
        data._score._monsterKillCountDic.Add("Golem", (int)userdata._golem);
        data._score._monsterKillCountDic.Add("DireBat", (int)userdata._direBat);
        data._score._monsterKillCountDic.Add("RedDragon", (int)userdata._redDragon);
        data._score._monsterKillCountDic.Add("Banshee", (int)userdata._banshee);

        GetUnlockItemList(userdata, data);
        Debug.Log("스켈레톤 수 불러오기 완료" + (int)userdata._skeleton);
        Debug.Log("스켈레톤 수 불러오기 완료" + data._score._monsterKillCountDic["Skeleton"]);
    }

    public void GetUnlockItemList(Packet_UserData userdata, TotalData tdata)
    {
        tdata._unlockDate._unlockedItem.Clear();

        if (!string.IsNullOrEmpty(userdata._itemID))
        {
            string[] items = userdata._itemID.Split(',');

            tdata._unlockDate._unlockedItem.AddRange(items);
        }
    }

    public void UpdateMonsterNDiamondData()
    {
        Packet_Update pack;
        TotalData tData = DataManger._instance._totalData;

        pack._diamond = (uint)tData._currentDiamond._totalDiamond;


        byte[] data = PacketConverter.StructureToByteArray(pack);
        NetManager._instance.SendQueueIn(PacketConverter.CreatePacket(NetManager._instance._uuid, (uint)CLProtocol.Send.Client_Update, data.Length, data));

        UpdateMonster();
    }

    public void UpdateMonster()
    {
        TotalData tData = DataManger._instance._totalData;
        Packet_UpdateMonster pack;
        pack._skeleton = (uint)tData._score._monsterKillCountDic["Skeleton"];
        pack._slime = (uint)tData._score._monsterKillCountDic["Slime"];
        pack._bat = (uint)tData._score._monsterKillCountDic["Bat"];
        pack._golem = (uint)tData._score._monsterKillCountDic["Golem"];
        pack._direBat = (uint)tData._score._monsterKillCountDic["DireBat"];
        pack._redDragon = (uint)tData._score._monsterKillCountDic["RedDragon"];
        pack._banshee = (uint)tData._score._monsterKillCountDic["Banshee"];

        byte[] data = PacketConverter.StructureToByteArray(pack);
        NetManager._instance.SendQueueIn(PacketConverter.CreatePacket(NetManager._instance._uuid, (uint)CLProtocol.Send.Client_UpdateMosnter, data.Length, data));
    }

    public void UpdateItemUnlock(string itemid)
    {
        Packet_UpdateItem packet;
        packet._uuid = NetManager._instance._uuid;
        packet._itemID = itemid;
        byte[] data = PacketConverter.StructureToByteArray(packet);
        NetManager._instance.SendQueueIn(PacketConverter.CreatePacket(NetManager._instance._uuid, (uint)CLProtocol.Send.Client_UpdateItem, data.Length, data));
    }

    IEnumerator SocketRead()
    {
        byte[] buffer = new byte[1024];
        while (!_isQuit)
        {
            // 서버로부터 읽을 데이터가 있는지 확인
            if (_socketServer != null && _socketServer.Available > 0)
            {
                int recvLen = _socketServer.Receive(buffer);
                if (recvLen > 0)
                {
                    // 바이트 데이터를 패킷 구조체로 변환
                    Packet pack = PacketConverter.ByteArrayToStructure<Packet>(buffer, 1024);
                    // 분석 큐에 삽입
                    ReceiveQueueIn(pack);
                }
            }
            yield return null;
        }
    }

    IEnumerator SendQueue()
    {
        while (!_isQuit)
        {
            if (_sendQueue.Count > 0)
            {
                Packet pack = _sendQueue.Dequeue();
                _socketServer.Send(PacketConverter.StructureToByteArray(pack));
            }

            yield return null;
        }
    }

    IEnumerator ReceiveQueue()
    {
        while (!_isQuit)
        {
            if (_receiveQueue.Count > 0)
            {
                Packet pack = _receiveQueue.Dequeue();

                switch ((CLProtocol.Receive)pack._protocol)
                {
                    case CLProtocol.Receive.Client_Connect_Success:
                        _uuid = pack._uuid;
                        Debug.Log("클라이언트 연결 성공");
                        break;
                    case CLProtocol.Receive.Client_Join_Success:
                        Debug.Log("회원가입 성공");
                        break;
                    case CLProtocol.Receive.Client_Join_Failed:
                        Debug.Log("회원가입 실패");
                        break;
                    case CLProtocol.Receive.Client_Login_Success:
                        Packet_UserData userData = PacketConverter.ByteArrayToStructure<Packet_UserData>(pack._data, pack._dataSize);
                        _uuid = userData._uuid;
                        GetAllUserInfo(userData);
                        Debug.Log("로그인 성공");
                        break;
                    case CLProtocol.Receive.Client_Login_Failed:
                        Debug.Log("로그인 실패");
                        break;
                    case CLProtocol.Receive.Client_Check_Success:
                        Debug.Log("확인 성공");
                        break;
                    case CLProtocol.Receive.Client_Check_Failed:
                        Debug.Log("확인 실패");
                        break;
                    case CLProtocol.Receive.Client_Update_Success:
                        Packet_Update packet = PacketConverter.ByteArrayToStructure<Packet_Update>(pack._data, pack._dataSize);
                        Debug.Log("업데이트 성공");
                        break;
                }
            }

            yield return null;
        }
    }
}
