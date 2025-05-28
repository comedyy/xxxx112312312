using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteNetLib.Utils;
using UnityEngine;

public enum TeamRoomEnterFailedReason
{
    OK,
    LogicCondition,
    SelfVersionTooLow,
    SelfVersionTooHigh,
    RoomNotExist,
    ConnectionFailed,
    JustInsideRoom,
    GetServerUniqueIdFailed,
    RoomLevelCondition,
}

public enum TeamRoomState
{
    InSearchRoom,
    InRoom,
    InBattle,
}

public class ClientBattleRoomMgr : MonoBehaviour
{
    public string LastClientBattleRoomMgrState => UserId + "_RoomState";
    public string LastClientBattleRoomMgrStateLevel => UserId + "_RoomStateLevel";
    GameClientSocket _socket;
    int _overrideUserId;
    
    public int UserId{
        get{
            return _overrideUserId;
        }
    }

    static ClientBattleRoomMgr _instance = null;
    
    public RoomUser[] _userList;
    public UpdateRoomMemberList _updateRoomInfo;
    public RoomInfoMsg[] _roomMsgList;
    Dictionary<int, UpdateRoomMemberList> _dicRoomInfo = new Dictionary<int, UpdateRoomMemberList>();
    public int enterRoomId{get; private set;}
    HashSet<int> _allRequiredMsg = new HashSet<int>();
    public TeamRoomState _roomState {get; private set;}= TeamRoomState.InSearchRoom;

    float _lastCheckRoomStateTime = 0;
    float _lastSendRoomStateQuery = 0;
    GetUserStateMsg.UserState _serverUserState = GetUserStateMsg.UserState.None;
    public GetUserStateMsg.UserState ServerUserState
    {
        get => _serverUserState;
        set{
            if(_serverUserState == value) return;

            _serverUserState = value;
            _lastCheckRoomStateTime = Time.realtimeSinceStartup;
        }
    }
    public string _battleGUID = "";
    public static int MAX_RETRY_COUNT = 2;
    int _reconnectCount = 0;
    TeamConnectParam _connectToServerParam = TeamConnectParam.None;


    public Dictionary<int, int> _dicLoadingProcess = new Dictionary<int, int>();

    public event Action<TeamRoomState, TeamRoomState> OnSwitchState;
    public event Action<int, int> OnReceveReplacePos;
    public event Action OnTeamInfoChange;
    public event Action<int> OnTeamRoomInfoChange;
#pragma warning disable 0067
    public event Action<JoinMessage> OnGetUserJoinInfo;
#pragma warning disable 0067
    public event Action OnQueryRoomList;
    public event Action<int> OnUserQuit;
    public event Action<BroadCastMsg> OnChatAdd;
    public event Action<BattleStartMessage> OnBattleStart;

    public Action<int> TipAction;
    public Action<int> AlertAction;
    Queue<int> _serverUniqueIds = new Queue<int>();

    public static ClientBattleRoomMgr Instance()
    {
        if(_instance == null)
        {
            _instance = new GameObject().AddComponent<ClientBattleRoomMgr>();
            _instance.Init();
        }

        return _instance;
    }

    public static bool IsCreated() => _instance != null;

    public void Init()
    {
        var ip = "101.132.100.216";
        var port = 10088;
        _socket = new GameClientSocket(ip, port, 0);
        _socket.OnConnected = OnConnected;
        _socket.OnDisConnected = OnDisConnected;
        _socket.OnReceiveMsg += OnReceiveMessage;
    }

    private async void OnDisConnected(ConnectErrorCode code)
    {
        if(code != ConnectErrorCode.None)
        {
            if(code == ConnectErrorCode.ConnectMax)
            {
                Alert.CreateAlert("server is full, wait a while and connect again.", null, false).Show();
            }
            else if(code == ConnectErrorCode.ConnectVersion)
            {
                Alert.CreateAlert(590037, null, false).Show();
            }
            else if(code == ConnectErrorCode.ConnectionIdOccupy)
            {
                Alert.CreateAlert("Your account is logged in on another device.");
            }

            _connectToServerParam = TeamConnectParam.None;
            SwitchRoomState(TeamRoomState.InSearchRoom);
            return;
        }

        // try max
        if(_reconnectCount >= MAX_RETRY_COUNT)
        {
            LogMessage("reconnect Count max");
            _connectToServerParam = TeamConnectParam.None;
            SwitchRoomState(TeamRoomState.InSearchRoom, true, false);
            return;
        }

        // check reconnect
        await Task.Delay(100);
        if(_roomState != TeamRoomState.InSearchRoom)
        {
            _reconnectCount ++;
            ReconnectToServer(TeamConnectParam.SyncInfoInRoom);
            LogMessage("reconnect" + _reconnectCount);
        }
    }

    private void OnConnected()
    {
        LogMessage("onConnected");

        _reconnectCount = 0;
        _socket.SendMessage(new RoomUserIdMsg(){
            userId = UserId, connectParam = _connectToServerParam
        });
        _connectToServerParam = default;
    }


    private void OnReceiveMessage(NetDataReader reader)
    {
        var msgType = (MsgType1)reader.PeekByte();
        if(msgType < MsgType1.ServerMsgEnd___)
        {
            return;
        }

        LogMessage("<<<<<<<<<<<===== " + msgType);
        
        if(msgType == MsgType1.ServerClose)
        {
            // if(LocalFrame.Instance is LocalFrameNetGame netGame)
            // {
            //     netGame.OnTeamRoomEnd(103364);
            // }
            Debug.Log("Server closed connection.");
            return;
        }
        else if(msgType == MsgType1.GetAllRoomList)
        {
            _roomMsgList = reader.Get<RoomListMsg>().roomList;

            foreach(var x in _roomMsgList)
            {
                _dicRoomInfo[x.updateRoomMemberList.roomId] = x.updateRoomMemberList;
            }
            _allRequiredMsg.Remove((int)MsgType1.GetAllRoomList);

            OnQueryRoomList?.Invoke();
        }
        else if(msgType == MsgType1.ChangeRoomInfo)
        {
            Alert.CreateAlert(590052, null, false).Show();
        }
        else if(msgType == MsgType1.RoomChangeUserPos)
        {
            var msg = reader.Get<RoomChangeUserPosMsg>();
            OnReceveReplacePos?.Invoke(msg.fromIndex, msg.toIndex);
        }
        else if(msgType == MsgType1.BroadCastMsg)
        {
            var msg = reader.Get<BroadCastMsg>();
            if(msg.broadCoastType == BroadCoastType.Chat)
            {
                OnChatAdd?.Invoke(msg);
            }
            else if(msg.broadCoastType == BroadCoastType.LoadingProgress)
            {
                // var process = msg.loadingProcessItem.percent;
                // var peer = msg.loadingProcessItem.id;

                // _dicLoadingProcess[peer] = process;
            }
        }
        else if(msgType == MsgType1.GetUniqueIdInServer)
        {
            var msg = reader.Get<GetServerUniqueIdMsg>();
            for(int i = 0; i < msg.count; i++)
            {
                _serverUniqueIds.Enqueue(msg.id + i);
            }
        }
        else if(msgType == MsgType1.GetRoomStateResponse)
        {
            var msg = reader.Get<GetRoomStateResponse>();
            _dicRoomInfo[msg.roomId] = msg.infoMsg.updateRoomMemberList;

            OnTeamRoomInfoChange?.Invoke(msg.roomId);
        }
        else if(msgType == MsgType1.SyncRoomMemberList)
        {
            var msg = reader.Get<UpdateRoomMemberList>();
            _userList = msg.userList;
            _updateRoomInfo = msg;
            enterRoomId = msg.roomId;

            _dicRoomInfo[enterRoomId] = msg;

            if(_userList.Length == 0)
            {
                SwitchRoomState(TeamRoomState.InSearchRoom);
            }
            else if(!msg.HasBattle)
            {
                SwitchRoomState(TeamRoomState.InRoom);
            }

            OnTeamInfoChange?.Invoke();
        }
        else if(msgType == MsgType1.RoomEventSync)
        {
            var msg = reader.Get<SyncRoomOptMsg>();
            if(msg.state == SyncRoomOptMsg.RoomOpt.Kick)
            {
                if(msg.param == UserId)
                {
                    // Tip.CreateTip(590038).Show();
                }
            }

            
            if((msg.state == SyncRoomOptMsg.RoomOpt.Leave || msg.state == SyncRoomOptMsg.RoomOpt.Kick || msg.state == SyncRoomOptMsg.RoomOpt.MasterLeaveRoomEnd) && msg.param == UserId)
            {
                var teamMaster = isTeamMaster;
                _userList = null;
                _updateRoomInfo = default;
                enterRoomId = 0;

                // if(msg.state == SyncRoomOptMsg.RoomOpt.MasterLeaveRoomEnd 
                //     && !teamMaster && LocalFrame.Instance is LocalFrameNetGame netGame)
                // {
                //     netGame.OnTeamRoomEnd(590016);
                // }

                SwitchRoomState(TeamRoomState.InSearchRoom, false);
            }

            var onlyNotice = msg.state == SyncRoomOptMsg.RoomOpt.Leave || msg.state == SyncRoomOptMsg.RoomOpt.Kick || msg.state == SyncRoomOptMsg.RoomOpt.Join;
            if(onlyNotice && _userList != null)
            {
                var param = msg.param;
                var user = _userList.FirstOrDefault(m=>m.userId == param);
                if(user.userId != 0)
                {
                    // var format = GetFormatByOpt(msg.state);
                    // var context = LanguageUtils.GetTextFormat(format, user.Name);
                    // OnChatAdd?.Invoke(new BroadCastMsg(){roomChatItem = new RoomChatItem(){context = context, head = user.playerHead, id = (int)user.userId, name = user.Name}});
                }

                if(msg.state == SyncRoomOptMsg.RoomOpt.Leave || msg.state == SyncRoomOptMsg.RoomOpt.Kick)
                {
                    OnUserQuit?.Invoke(msg.param);
                }
                return;
            }

        }
        else if(msgType == MsgType1.ErrorCode)
        {
            var msg = reader.Get<RoomErrorCode>();
            LogMessage(msg.roomError.ToString());

            if(msg.roomError == RoomError.RoomFull)
            {
                Alert.CreateAlert(590004, null, false).Show();
            }
            else if(msg.roomError == RoomError.RoomNotExist)
            {
                Alert.CreateAlert(590046, null, false).Show();
            }
            else if(msg.roomError == RoomError.JoinRoomErrorInsideRoom || msg.roomError == RoomError.JoinRoomErrorHasRoom)
            {
                Alert.CreateAlert(590045, null, false).Show();
            }
            else if(msg.roomError == RoomError.RobertPlayerIdWhichCannotStayTogether)
            {
                Alert.CreateAlert(590087, null, false).Show();
            }
            else if(msg.roomError == RoomError.RobertPlayerIdWhichCannotStayTogetherBeKick)
            {
                Alert.CreateAlert(590086, null, false).Show();
            }
            else
            {
                Alert.CreateAlert(msg.roomError.ToString());
            }

            if(msg.roomError == RoomError.RoomFull || msg.roomError == RoomError.RoomNotExist)
            {
                QueryRoomList();
            }
        }
        else if(msgType == MsgType1.GetUserState)
        {
            var msg = reader.Get<GetUserStateMsg>();
            LogMessage(msg.state.ToString());
            ServerUserState = msg.state;
        }
        else if(msgType == MsgType1.RoomStartBattle)
        {
                        var roomStartBattle = reader.Get<RoomStartBattleMsg>();
                BattleStartMessage startMessage = ReadObj<BattleStartMessage>(roomStartBattle.StartMsg);

                if(_roomState == TeamRoomState.InBattle && _battleGUID == startMessage.guid)
                {
                    // 战斗已经开始
                    return;
                }
                _battleGUID = startMessage.guid;
                startMessage.guid = roomStartBattle.BattleGuid;

                startMessage.joins = new JoinMessage[roomStartBattle.joinMessages.Count];
                for(int i = 0; i < roomStartBattle.joinMessages.Count; i++)
                {
                    startMessage.joins[i] = ReadObj<JoinMessage>(roomStartBattle.joinMessages[i]);
                }

                startMessage.seed += (uint)roomStartBattle.battleCount;

                SwitchRoomState(TeamRoomState.InBattle);
                OnBattleStart(startMessage);
                _roomMsgList = null;
        }
    }

    private int GetFormatByOpt(SyncRoomOptMsg.RoomOpt state)
    {
        switch(state)
        {
            case SyncRoomOptMsg.RoomOpt.Join: return 590048;
            case SyncRoomOptMsg.RoomOpt.Leave: return 590049;
            case SyncRoomOptMsg.RoomOpt.Kick: return 590050;
            default: return 0;
        }
    }

    public void OnMemberLeaveBattle()   // 强行断线。
    {
        SwitchRoomState(TeamRoomState.InSearchRoom, false, false);
    }

    public void ForceQuitRoomState()
    {
        SwitchRoomState(TeamRoomState.InSearchRoom, false, false);
    }

    bool SwitchRoomState(TeamRoomState state, bool notifyRoomEnd = true, bool updateRoomState = true)
    {
        if(_roomState == state)
        {
            // _socket.DisConnect();
            return false;
        };

        LogMessage($"switch roomState to {state}");

        var fromState = _roomState;
        _roomState = state;
        if(_roomState == TeamRoomState.InSearchRoom)
        {
            enterRoomId = 0;
            _updateRoomInfo = default;

            // 断开连接
            _socket.DisConnect();

            // if(notifyRoomEnd && LocalFrame.Instance is LocalFrameNetGame netGame)
            // {
            //     netGame.OnTeamRoomEnd(590041);
            // }

            // 请求roomList
            QueryRoomList();
        }
        else if(_roomState == TeamRoomState.InBattle)
        {
            _dicLoadingProcess.Clear();
        }
        else if(_roomState == TeamRoomState.InRoom)
        {
            // 退出到房间。
            // if(notifyRoomEnd && LocalFrame.Instance is LocalFrameNetGame netGame)
            // {
            //     netGame.OnTeamRoomEnd(590041);
            // }
        }

        OnSwitchState?.Invoke(fromState, _roomState);

        return true;
    }

    public bool PossibleEverInRoom{
        get
        {
            var x = PlayerPrefs.GetInt(LastClientBattleRoomMgrState);
            return x != (int)TeamRoomState.InSearchRoom;
        }
    } 

    void Update()
    {
        _socket.Update(Time.deltaTime);

        // check not in room state but has room;
        if(ServerUserState == GetUserStateMsg.UserState.Querying)
        {
            if(Time.realtimeSinceStartup -  _lastSendRoomStateQuery > 5)
            {
                _lastSendRoomStateQuery = Time.realtimeSinceStartup;
            }
        }
    }

    public void SendQueryRoomState()
    {
        _socket.SendMessage(new GetUserStateMsg(){state = GetUserStateMsg.UserState.Querying});
    }

    void OnDestroy()
    {
        _socket?.OnDestroy();
    }

    void OnBattleEnd(string guid)
    {
        // 比如切到外面去了。或者F7重启。
        // if(_battleGUID == guid && isTeamMaster && _socket.connectResult == ConnectResult.Connnected)
        // {
        //     LeaveRoom();
        // }
        // if(IsLastBattleQuitMember) return;

        // if(_roomState != TeamRoomState.InSearchRoom)
        // {
        //     LeaveRoom();
        // }
    }

    public bool GetRoomInfo(int id, out UpdateRoomMemberList msg)
    {
        return _dicRoomInfo.TryGetValue(id, out msg);
    }

    public void QueryRoomInfo(int id)
    {
        if(_dicRoomInfo.TryGetValue(id, out var roomInfo) && roomInfo.roomId == 0)
        {
            OnTeamRoomInfoChange?.Invoke(id);
            return; // 房间已经解散
        }

        _socket.SendUnConnectedMessage(new GetRoomStateMsg(){idRoom = id});
    }

    public void QueryRoomList()
    {
        _socket.SendUnConnectedMessage(new RoomListMsgRequest());
    }


    public async Task<RoomInfoMsg[]> QueryRoomListAsync(int timeout = 3)
    {
        _socket.SendUnConnectedMessage(new RoomListMsgRequest());
        var sendTime = Time.time;
        var intSendTime = Time.time;
        _allRequiredMsg.Add((int)MsgType1.GetAllRoomList);

        while(true)
        {
            await Task.Delay(500);

            if(!_allRequiredMsg.Contains((int)MsgType1.GetAllRoomList))
            {
                return _roomMsgList;
            }
            else if(Time.time - sendTime > 1)
            {
                _socket.SendUnConnectedMessage(new RoomListMsgRequest());
                sendTime = Time.time;
            }
            else if(Time.time - intSendTime > timeout)
            {
                return default;
            }
        }
    }
    
    public async Task<TeamRoomEnterFailedReason> JoinRoom(int enterRoomId, JoinMessage message, ClientUserJoinShowInfo showInfo)
    {
        if(SwitchRoomState(TeamRoomState.InSearchRoom))
        {
            await Task.Delay(500);
        }

        if(!await ConnectToServerInner())
        {
            return TeamRoomEnterFailedReason.ConnectionFailed;
        }

        _socket.SendMessage(new JoinRoomMsg(){
            roomId = enterRoomId, 
            joinMessage = GetBytes(message),
            joinShowInfo = GetBytes(showInfo),
        });

        return TeamRoomEnterFailedReason.OK;
    }

    public async void BroadCastMsg(INetSerializable msg)
    {
        if(!await ConnectToServerInner())
        {
            return;
        }

        _socket.SendMessage(msg);
    }

    public async void ReconnectToServer(TeamConnectParam syncRoomInfo)
    {
        if(_socket.connectResult == ConnectResult.Connnected)
        {
            _socket.SendMessage(new RoomUserIdMsg(){
                userId = UserId, connectParam = syncRoomInfo
            });
            return;
        }

        _connectToServerParam = syncRoomInfo;
        await ConnectToServerInner();
    }

    private async Task<bool> ConnectToServerInner()
    {
        _socket.Connect(RoomMsgVersion.version);

        while(_socket.connectResult == ConnectResult.Connecting)
        {
            await Task.Delay(100);
        }

        return _socket.connectResult == ConnectResult.Connnected;
    }

    public async void CreateRoom( BattleStartMessage startBytes, JoinMessage joins, ClientUserJoinShowInfo joinShowInfo, ClientRoomShowInfo roomShowInfo)
    {
        if(!await ConnectToServerInner())
        {
            return;
        }

        var setting = GetServerSetting();

        _socket.SendMessage(new CreateRoomMsg()
        {
            startBattleMsg = GetBytes(startBytes),
            join = GetBytes(joins),
            joinShowInfo = GetBytes(joinShowInfo),
            roomShowInfo = GetBytes(roomShowInfo),
            setting = setting,
        });
    }

    async Task<int> GetServerUniqueId()
    {
        if(_serverUniqueIds.Count > 0)
        {
            return _serverUniqueIds.Dequeue();
        }

        int retryCount = 30;
        while(_serverUniqueIds.Count == 0 && (retryCount--) >= 0)
        {
            if(retryCount % 10 == 9)
            {
                _socket.SendUnConnectedMessage(new GetServerUniqueIdMsg(){count = 1}); // 一次性先给搞2个
            }
            await Task.Delay(300);
        }

        if(_serverUniqueIds.Count > 0)
        {
            return _serverUniqueIds.Dequeue();
        }

        return -1;
    }

    public async void LeaveRoom()
    {
        if(!await ConnectToServerInner())
        {
            return;
        }

        _socket.SendMessage(new UserLeaveRoomMsg());
    }

    public async void KickUser(int kickedUser)
    {
        if(!await ConnectToServerInner())
        {
            return;
        }

        _socket.SendMessage(new KickUserMsg(){
            userId = kickedUser
        });
    }

    public async void ReadyRoom(bool isReady)
    {
        if(!await ConnectToServerInner())
        {
            return;
        }

        _socket.SendMessage(new RoomReadyMsg(){ isReady = isReady});
    }

    public async void StartRoom()
    {
        if(!await ConnectToServerInner())
        {
            return;
        }

        _socket.SendMessage(new StartBattleRequest());
    }
    
    public void DEBUG_Disconnect()
    {
        if(_socket != null)
        {
            _socket.DisConnect();
        }
    }

    public void ChangeIp(string ip, int port)
    {
        _socket.SetIp(ip, port);
    }

    public void SetUserId(uint userId)
    {
        var preId = userId;
        _overrideUserId = (int)userId;
        LogMessage($"setUserid: {preId} {_overrideUserId}");
    }
    
    internal bool enableLog{
        get{
            return PlayerPrefs.GetInt("enableRoomLog", 0) != 0;
        }
        set
        {
            PlayerPrefs.SetInt("enableRoomLog", value ? 1 : 0);
        }
    }

    public bool isTeamMaster
    {
        get
        {
            return enterRoomId > 0 && _userList[0].userId == UserId;
        }
    }
    public bool isTeamMate
    {
        get
        {
            return enterRoomId > 0 && _userList[0].userId != UserId;
        }
    }

    public bool AllReady { 
        get
        {
            for(int i = 1; i < _userList.Length; i++)
            {
                if(!_userList[i].isReady) return false;
            }

            return true;
        }
    }

    private ServerSetting GetServerSetting()
    {
        var needKeepRoom = true;
        return new ServerSetting() // 目前设置最高10分钟
        {
            tick = 0.05f, maxSec = 60 * 30, masterLeaveOpt = RoomMasterLeaveOpt.RemoveRoomAndBattle, maxCount = 4, 
                keepRoomAfterBattle = needKeepRoom, 
                waitReadyStageTimeMs = 1,
                whoCanLeaveRoomInBattle = WhoCanLeaveRoomInBattle.All,
        };
    }

    public void LogMessage(string context)
    {
        if(enableLog)
        {
            Debug.LogError(context);
        }
    }

#region Util    
    static NetDataWriter _writer = new NetDataWriter();
    public static byte[] GetBytes(INetSerializable netSerializable)
    {
        _writer.Reset();
        _writer.Put(netSerializable);
        return _writer.CopyData();
    }

    static NetDataReader _reader = new NetDataReader();
    public static T ReadObj<T>(byte[] bytes) where T : struct, INetSerializable
    {
        _reader.SetSource(bytes);
        return _reader.Get<T>();
    }

    internal async void ChangeRoomShowInfo(BattleStartMessage startMessage, ClientRoomShowInfo info)
    {
        if(!await ConnectToServerInner())
        {
            return;
        }

        _socket.SendMessage(new ChangeRoomInfoMsg()
        {
            bytesRoomShowInfo = GetBytes(info), needCancelReady = false, bytesStartBattle = GetBytes(startMessage)
        });
    }

    internal IClientGameSocket GetSocket()
    {
        return _socket;
    }
    #endregion

    public string SocketInfo => _socket.GetInfo();

}
