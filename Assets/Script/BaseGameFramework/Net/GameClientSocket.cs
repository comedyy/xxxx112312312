using System.Net;
using System.Net.Sockets;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using Game;
using UnityEngine.Assertions;

public class GameClientSocket : IClientGameSocket, INetEventListener, INetLogger
{
    private NetManager _netClient;
    private NetDataWriter _dataWriter;

    public int RoundTripTime => _netClient.FirstPeer == null ? -1 : _netClient.FirstPeer.RoundTripTime;
    IPEndPoint _endPoint;
    NetPeer _server;
    bool _needCreateNewSocket = false;

    public void SetIp(string ip, int port)
    {
        var isEndPointNull = _endPoint == null;
        var endPointChanged = false;

        if(!isEndPointNull)
        {
            endPointChanged = !_endPoint.Address.Equals(IPAddress.Parse(ip)) || _endPoint.Port != port;
        }

        if(isEndPointNull || endPointChanged)
        {
            _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }
    }

    public GameClientSocket(string targetIp, int port, int delay)
    {
        NetDebug.Logger = this;

        if (string.IsNullOrEmpty(targetIp))
        {
            throw new Exception("targetip 为空");
        }

        SetIp(targetIp, port);

        CreateNewSocket();
    }

    void CreateNewSocket()
    {
        _netClient = new NetManager(this, new LiteNetLib.Layers.Crc32cLayer());
        _dataWriter = new NetDataWriter();
        _netClient.UnconnectedMessagesEnabled = true;
        _netClient.AutoRecycle = true;
        _netClient.UpdateTime = 15;
        _netClient.SimulateLatency = false;
        _netClient.Start();
        connectResult = ConnectResult.NotConnect;
        _needCreateNewSocket = false;

        ClientBattleRoomMgr.Instance().LogMessage("createNew Socket " + _endPoint);
    }

#region ILifeCircle
    public void Start()
    {
    }

    public void Connect(ushort msgVersionId)
    {
        CheckConnectState();

        if(connectResult == ConnectResult.Connecting || connectResult == ConnectResult.Connnected)
        {
            return;
        }

        // if(_needCreateNewSocket)
        {
            CreateNewSocket();
        }

        connectResult = ConnectResult.Connecting;
        NetDataWriter writer = new NetDataWriter();
        writer.Put("wsa_game");
        writer.Put(msgVersionId);
        _server = _netClient.Connect(_endPoint, writer);

        ClientBattleRoomMgr.Instance().LogMessage("connect " + _endPoint);
    }

    private void CheckConnectState()
    {
        var socketConnectState = _server != null 
            && (_server.ConnectionState == ConnectionState.Outgoing || _server.ConnectionState == ConnectionState.Connected);
        var logicState = connectResult == ConnectResult.Connecting || connectResult == ConnectResult.Connnected;

        if(socketConnectState != logicState)
        {
            // GameCore.AdService.FirebaseExceptionOnRelease($"socket state not same, {socketConnectState} {logicState}");
        }
    }


    public void DisConnect()
    {
        ClientBattleRoomMgr.Instance().LogMessage("try disconnect " + _endPoint);

        _netClient.DisconnectAll();
        connectResult = ConnectResult.Disconnect;
    }

    public void Update(float deltaTime)
    {
        _netClient.PollEvents();
    }

    public void OnDestroy()
    {
        if (_netClient != null)
        {
            _netClient.Stop();
            _netClient = null;
            connectResult = ConnectResult.NotConnect;
        }
    }
#endregion

#region IMessageSendReceive
    public Action<NetDataReader> OnReceiveMsg{get;set;}

    public ConnectResult connectResult{get; private set;} = ConnectResult.NotConnect;
    public Action OnConnected { get;  set; }
    public Action<ConnectErrorCode> OnDisConnected { get;  set; }

    public void SendMessage<T>(T t) where T : INetSerializable
    {
        // Debug.LogError("===>>>>>>> " + typeof(T));
        UnityEngine.Profiling.Profiler.BeginSample("NETBATTLE_GameClientSocket.SendMessage");

        var peer = _netClient.FirstPeer;
        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
            _dataWriter.Reset();
            _dataWriter.Put(t);
            peer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void SendMessageNotReliable<T>(T t) where T : INetSerializable
    {
        UnityEngine.Profiling.Profiler.BeginSample("NETBATTLE_GameClientSocket.SendMessageNotReliable");
        var peer = _netClient.FirstPeer;
        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
            _dataWriter.Reset();
            _dataWriter.Put(t);
            peer.Send(_dataWriter, DeliveryMethod.Unreliable);
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void SendSequenced<T>(T t) where T : INetSerializable
    {
         UnityEngine.Profiling.Profiler.BeginSample("NETBATTLE_GameClientSocket.SendSequenced");
        var peer = _netClient.FirstPeer;
        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
            _dataWriter.Reset();
            _dataWriter.Put(t);
            peer.Send(_dataWriter, DeliveryMethod.Sequenced);
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void SendUnConnectedMessage<T>(T t) where T : INetSerializable
    {
        _dataWriter.Reset();
        _dataWriter.Put(t);
        _netClient.SendUnconnectedMessage(_dataWriter, _endPoint);
    }
#endregion

#region INetEventListener
    public void OnPeerConnected(NetPeer peer)
    {
        if(peer != _server)
        {
            // GameCore.AdService.FirebaseExceptionOnRelease("OnPeerConnected peer not the request one!");
        }

        // Debug.LogError("[CLIENT] We connected to " + peer.EndPoint);
        connectResult = ConnectResult.Connnected;

        OnConnected?.Invoke();
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.LogError("[CLIENT] We received error " + socketErrorCode);

        if(socketErrorCode == SocketError.NetworkDown)
        {
            _needCreateNewSocket = true;
        }
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        UnityEngine.Debug.Assert(peer == _server);

        OnReceiveMsg(reader);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if(!_endPoint.Address.Equals(remoteEndPoint.Address) || _endPoint.Port != remoteEndPoint.Port) return;

        OnReceiveMsg(reader);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        Debug.LogError("OnConnectionRequest 不应该走到");
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        if(peer != _server)
        {
            // GameCore.AdService.FirebaseExceptionOnRelease("OnPeerDisconnected peer not the request one!");
        }

        connectResult = ConnectResult.Disconnect;
        _server = null; // 服务器断开了

        ConnectErrorCode code = ConnectErrorCode.None;
        if(disconnectInfo.AdditionalData != null && disconnectInfo.AdditionalData.AvailableBytes >= 1)
        {
            code = (ConnectErrorCode)disconnectInfo.AdditionalData.GetByte();
        }

        ClientBattleRoomMgr.Instance().LogMessage($"[CLIENT] We disconnected because {disconnectInfo.Reason} {code}");

        OnDisConnected?.Invoke(code);
    }
    #endregion

    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        if(level == NetLogLevel.Error)
        {
            #if UNITY_EDITOR
            UnityEngine.Debug.LogError($"{str} {string.Join(",", args)}");
            #else
            Console.WriteLine($"{str} {string.Join(",", args)}");
            #endif
        }
        else
        {
            // ignore
        }
    }

    public string GetInfo()
    {
        return $"SocketPackage:{NetPeer.GetPackgeInfo()}";
    }

}
