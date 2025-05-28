
using System;
using System.Collections.Generic;
using LiteNetLib.Utils;
using UnityEngine;

internal class OnlineBattleNetHelper
{
    public int LastReconnectFrame { get; private set; }
    int _receivedFrame => _putMessage.ReceivedServerFrame;
    public bool PendingReconnectReceiveMsg { get; private set; }
    public IClientGameSocket _clientGameSocket;
    IPutMessage _putMessage;
    bool _serverBattleReady = false;

    public bool IsInReconnectState { get; internal set; }

    public OnlineBattleNetHelper(IClientGameSocket clientGameSocket, IPutMessage syncFrameInputCache)
    {
        _clientGameSocket = clientGameSocket;
        _clientGameSocket.OnReceiveMsg += OnReceive;
        _clientGameSocket.OnConnected += OnConnected;
        _putMessage = syncFrameInputCache;

        UpdateSendBattleReady();
    }

    public void OnDispose()
    {
        _clientGameSocket.OnReceiveMsg -= OnReceive;
    }

    private void OnConnected()
    {
        ReGetMsgFromServer(true);
        UpdateSendBattleReady();
    }

    void UpdateSendBattleReady()
    {
        if (!_serverBattleReady)
        {
            _clientGameSocket.SendMessage(new ReadyStageMsg()
            {
                stageIndex = 1
            });
        }
    }

    private void ReGetMsgFromServer(bool force)
    {
        if (PendingReconnectReceiveMsg && !force) return;

        _clientGameSocket.SendMessage(new ServerReconnectMsg()
        {
            startFrame = _receivedFrame
        });
        PendingReconnectReceiveMsg = true;
    }

    internal void OnReceive(NetDataReader reader)
    {
        if (reader.AvailableBytes == 0) return; // 被room处理了

        var msgType = (MsgType1)reader.PeekByte();
        if (msgType > MsgType1.ServerMsgEnd___)// room msg
        {
            return;
        }

        if (msgType == MsgType1.Unsync)
        {
            var unSync = reader.Get<UnSyncMsg>();
            OnProcessUnsync(unSync);
            return;
        }
        else if (msgType == MsgType1.ServerReadyForNextStage || msgType == MsgType1.ServerEnterLoading)
        {
            _serverBattleReady = true;
            return;
        }
        else if (msgType == MsgType1.ServerReConnect)        // 断线重连
        {
            var msg = reader.Get<ServerReconnectMsgResponse>();
            foreach (var x in msg.bytes)
            {
                var getPackageItem = ClientBattleRoomMgr.ReadObj<ServerPackageItem>(x);
                ProcessPackageItem(getPackageItem);
                LastReconnectFrame = getPackageItem.frame;
            }

            PendingReconnectReceiveMsg = false;
            return;
        }

        if (PendingReconnectReceiveMsg)           // 断线重连中，等待服务器的大包。
        {
            return;
        }

        ServerPackageItem item = reader.Get<ServerPackageItem>();
        ProcessPackageItem(item);
    }

    private void ProcessPackageItem(ServerPackageItem item)
    {
        int frame = item.frame;
        if (frame <= 0) return;

        var list = item.list;
        if (frame <= _receivedFrame)
        {
            Debug.LogError($"frame <= frameServer {frame} {_receivedFrame}");
            return;
        }

        if (_receivedFrame != frame - 1)
        {
            Debug.LogError($"frame 不连续 {frame}  {_receivedFrame}");
            ReGetMsgFromServer(false);
            return;
        }

        _putMessage.AddFrameWithList(frame, list);
    }


    private void OnProcessUnsync(UnSyncMsg unSync)
    {
        Debug.LogError("Unsync");
    //         var hashIndex = unSync.unSyncHashIndex;
        //         var frame = unSync.frame;
        //         var unsyncType = unSync.errorType;
        //         IsUnSync = true;

        //         // unsync
        //         Alert.CreateAlert($"unsync occurs frame: {frame}, type {unsyncType}")
        //             .SetRightButton(() => { }).Show();

        //         InputUserPositionController.SetBattleLoseEnd(true);
        //         var x = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentObject<CheckSumComponet>(CheckSumComponet.entity);
        //         var messageHash = x.checkSum.GetMessageHash(hashIndex, frame);

        // #if DEVELOPMENT_BUILD || UNITY_EDITOR
        //         var fileName = $"{_guid}_{ControllerId}.log";
        //         PlaybackReader.WriteUnsyncToFile(messageHash.allHashDetails, 0, fileName, null);
        // #endif
    }
    
}