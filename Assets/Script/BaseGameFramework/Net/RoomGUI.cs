using System;
using UnityEngine;
using System.Collections.Generic;
using Game;


public class RoomGUI : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    RoomInfoMsg[] roomList;
    public RoomUser[] _userList => ClientBattleRoomMgr.Instance()._userList;
    bool _canQuery = true;
    string _chatContent = "";

    public Func<(BattleStartMessage, ClientRoomShowInfo)> GetStartMessage;
    public Func<int, bool, (JoinMessage, ClientUserJoinShowInfo)> GetJoinMessage;
    public Action<BattleStartMessage> OnBattleStart;
    uint userId 
    {
        get
        {
            var y = PlayerPrefs.GetInt(Application.dataPath +"netPlayerId");
            if(y == 0)
            {
                y = UnityEngine.Random.Range(1000, 100000);
                PlayerPrefs.SetInt(Application.dataPath + "netPlayerId", y);
            }
            return (uint)y;
        }
    }

    void Start()
    {
        ip = "101.132.100.216";
        port = 10088;

        var mono = gameObject.AddComponent<LocalServerMono>();
        ClientBattleRoomMgr.Instance().SetUserId(userId);

        GetStartMessage = () =>
        {
            var startMessage = new BattleStartMessage()
            {
                seed = 1,
                guid = Guid.NewGuid().ToString(),
                battleType = 0
            };
            return (startMessage, new ClientRoomShowInfo() {  roomType = 11});
        };

        GetJoinMessage = (roomId, isRoomMaster) =>
        {
            var joinMessage = new JoinMessage()
            {
                UserId = (int)userId, idType = 1, skills = new int[] { 1, 2}
            };
            return (joinMessage, new ClientUserJoinShowInfo() {  idType = 1, Name = "1123"});
        };        
    }

    private void OnEnable() {
        ClientBattleRoomMgr.Instance().OnChatAdd += (context) =>
        {
            ClientBattleRoomMgr.Instance().LogMessage($"{context.id} {context.context}");
        };
        ClientBattleRoomMgr.Instance().OnBattleStart += (startMessage) =>
        {
            OnBattleStart?.Invoke(startMessage);
            ClientBattleRoomMgr.Instance().LogMessage($"OnBattleStart {startMessage.seed} {startMessage.guid}");
        };
    }

    public static string ip;
    public static int port;
    void OnGUI()
    {
        if(ClientBattleRoomMgr.Instance()._roomState == TeamRoomState.InSearchRoom)
        {
            DrawOutsideRoom();
        }
        else if(ClientBattleRoomMgr.Instance()._roomState == TeamRoomState.InRoom)
        {
            DrawInsideRoom();
        }
        else if(ClientBattleRoomMgr.Instance()._roomState == TeamRoomState.InBattle)
        {
            // GUI.Label(new Rect(0, 0, 1000, 100), ClientBattleRoomMgr.Instance().SocketInfo);

            // if(LocalFrame.Instance != null && LocalFrame.Instance._clientStageIndex < 1)
            // {
            //     GUI.color = Color.red;
            //     for(int i = 0; i < _userList.Length; i++)
            //     {
            //         int widthIndex = 0;
            //         GUI.Label(new Rect((widthIndex++) * 100, i * 50 + 400, 100, 50), _userList[i].Name);
            //         GUI.Label(new Rect((widthIndex++) * 100, i * 50 + 400, 180, 50), _userList[i].ToString());

            //         widthIndex ++;
            //         ClientBattleRoomMgr.Instance()._dicLoadingProcess.TryGetValue((int)_userList[i].userId, out var process);
            //         GUI.Label(new Rect((widthIndex++) * 100, i * 50 + 400, 100, 50), $"process:{process}");
            //     }
            // }
            // else if(LocalFrame.Instance != null && LocalFrame.Instance is LocalFrameNetGame netPing)
            // {
            //     GUI.Label(new Rect(Screen.width - 300, 0, 100, 50), $"ping:{netPing.SocketRoundTripTime}");

            //     var logicPing = netPing.SocketRoundTripLogicTime;
            //     var processPerSec = netPing.FrameProcessPerSec;
            //     var receivePerSec = netPing.ReceiveFramePerSec;
            //     GUI.Label(new Rect(Screen.width - 200, 0, 200, 50), $"L:{logicPing} P:{processPerSec} R:{receivePerSec}");
            //     if(GUI.Button(new Rect(Screen.width - 400, 0, 100, 50), "disconnect"))
            //     {
            //         ClientBattleRoomMgr.Instance().DEBUG_Disconnect();
            //     }
            // }
        }
    }

    void DrawOutsideRoom()
    {
        // show all Rooms
        if(roomList != null)
        {
            GUI.color = Color.red;
            var roomCount = roomList.Length;

            if(roomCount == 0)
            {
                GUI.Label(new Rect(0, 50, 300, 50), $"暂无房间");
            }
            else
            {
                for(int i = 0; i < roomCount; i++)
                {
                    GUI.Label(new Rect(0, i * 50, 300, 50), $"ID: {roomList[i].updateRoomMemberList.roomId}  userCount: {roomList[i].updateRoomMemberList.userList.Length} ");
                    if(GUI.Button(new Rect(400, i * 50, 100, 50), "加入"))
                    {
                        JoinAsync(roomList[i].updateRoomMemberList);
                    }
                }
            }

            GUI.color = Color.white;
        }
        
        if(GUI.Button(new Rect(0, 150, 100, 50), "创建"))
        {
            (var startMessage, var roomShowInfo) = GetStartMessage();
            (var joinMessage, var joinShowInfo) = GetJoinMessage(0, true);
            
            ClientBattleRoomMgr.Instance().CreateRoom(startMessage, joinMessage, joinShowInfo, roomShowInfo);
        }

        port = int.Parse(GUI.TextField(new Rect(100, 100, 100, 50), port.ToString()));
        ip = GUI.TextField(new Rect(100, 150, 100, 50), ip);
        if(_canQuery && GUI.Button(new Rect(200, 150, 100, 50), "查询房间"))
        {
            QueryAvailableRooms();
        }

        if(LocalServerMono.Instance != null)
        {
            if( !LocalServerMono.Instance.isStartBattle)
            {
                if(GUI.Button(new Rect(300, 150, 100, 50), "启用本地服务器"))
                {
                    LocalServerMono.Instance.StartServer();
                    ip = "127.0.0.1";
                    port = 5000;
                }
                else if(GUI.Button(new Rect(400, 150, 100, 50), "外网服务器"))
                {
                    ip = "101.132.100.216";
                    port = 10055;
                }
                else if(ClientBattleRoomMgr.Instance().ServerUserState != GetUserStateMsg.UserState.None && GUI.Button(new Rect(600, 150, 100, 50), "同步房间数据")) 
                {
                    // OnClickReconnect();
                    ClientBattleRoomMgr.Instance().ReconnectToServer(TeamConnectParam.SyncInfoWhenClientOutsideRoom);
                }
            }
            else
            {
                GUI.Label(new Rect(300 , 150, 100, 50), "本地服务器已经开启");
            }
        }

        ClientBattleRoomMgr.Instance().enableLog = GUI.Toggle(new Rect(500, 150, 100, 50), ClientBattleRoomMgr.Instance().enableLog, "打开日志");

        ClientBattleRoomMgr.Instance().ChangeIp(ip, port);
    }

    private async void JoinAsync(UpdateRoomMemberList updateRoomMemberList)
    {
        var roomId = updateRoomMemberList.roomId;
        (var join, var showInfo) = GetJoinMessage(updateRoomMemberList.roomId, true);
        var ret = await ClientBattleRoomMgr.Instance().JoinRoom(roomId, join, showInfo);
        if(ret != TeamRoomEnterFailedReason.OK)
        {
            Debug.LogError("join failed" + ret);
        }
    }

    private void DrawInsideRoom()
    {
        GUI.color = Color.red;
        var iAmRoomMaster = _userList[0].userId == userId;

        GUI.Label(new Rect(200, 300, 1000, 50), $"roomType11");

        for(int i = 0; i < _userList.Length; i++)
        {
            int widthIndex = 0;
            GUI.Label(new Rect((widthIndex++) * 100, i * 50 + 400, 100, 50), _userList[i].Name);
            GUI.Label(new Rect((widthIndex++) * 100, i * 50 + 400, 180, 50), _userList[i].ToString());
            widthIndex ++;
            GUI.Label(new Rect((widthIndex++) * 100, i * 50 + 400, 100, 50), _userList[i].userId.ToString());
            GUI.Label(new Rect((widthIndex++) * 100, i * 50 + 400, 100, 50), $"在线：{_userList[i].isOnLine}");
            GUI.Label(new Rect((widthIndex++) * 100, i * 50 + 400, 100, 50), $"准备：{_userList[i].isReady}");

            var isSelf = _userList[i].userId == userId;
            var currentIsRoomMaster = i == 0;
            if(isSelf)  // 自己的操作。
            {
                if(GUI.Button(new Rect((widthIndex++) * 100, i * 50 + 400, 100, 50), "退出"))
                {
                    ClientBattleRoomMgr.Instance().LeaveRoom();
                }

                // if(GUI.Button(new Rect((widthIndex++) * 100, i * 50 + 400, 100, 50), "断线"))
                // {
                //     ClientBattleRoomMgr.Instance().DEBUG_Disconnect();
                // }

                if(currentIsRoomMaster)
                {
                    if(iAmRoomMaster && GUI.Button(new Rect((widthIndex++) * 100, i * 50 + 400, 100, 50), "开始"))
                    {
                        ClientBattleRoomMgr.Instance().StartRoom();
                    }
                }
                else
                {
                    if(GUI.Button(new Rect((widthIndex++) * 100, i * 50 + 400, 100, 50), $"ready：{_userList[i].isReady}"))
                    {
                        ClientBattleRoomMgr.Instance().ReadyRoom(!_userList[i].isReady);
                    }
                }
            }
        }

        _chatContent = GUI.TextArea(new Rect(50, 700, 600, 50), _chatContent);
        if(GUI.Button(new Rect(0, 700, 50, 50), "send"))
        {
            var msg = new BroadCastMsg(){
                data = ClientBattleRoomMgr.GetBytes(new RoomChatItem(){
                    id = ClientBattleRoomMgr.Instance().UserId, context = _chatContent, name = "", head = ""
                })
            };
            ClientBattleRoomMgr.Instance().BroadCastMsg(msg);
        }
        GUI.color = Color.white;
    }


    private async void QueryAvailableRooms()
    {
        _canQuery = false;
        roomList = await ClientBattleRoomMgr.Instance().QueryRoomListAsync();
        _canQuery = true;
    }
}
