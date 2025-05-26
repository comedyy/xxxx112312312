
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteNetLib.Utils;
using Unity.Assertions;
using UnityEngine;
using UnityEngine.Networking;

public enum RecordErrorReason
{
    None,
    Exception,
    FileEmpty
}

public class PlaybackReader
{
    NetDataReader _reader;
    public NetDataReader Reader => _reader;
    PlaybackMessageItem? _current;
    Dictionary<int, MessageHash> _dicHash = new Dictionary<int, MessageHash>();
    int _currentClientStage = 0;
    public static string _battleId = "";
    public int totalFrame{get;private set;}
    List<string> _hashStringParam;
    public float ReaderProcess => 1.0f * _reader.Position / _reader.RawDataSize;
    // PlayBackTransformDiff[] _preStates;

    public PlaybackReader(byte[] binary, byte[] symbol, bool hasTotalFrame)
    {
        if(binary == null || binary.Length == 0)
        {
            NotMatchRecord = RecordErrorReason.FileEmpty;
            return;
        }

        _reader = new NetDataReader();
        InitReader(binary, hasTotalFrame);

#if false
        HashSet<int> _allHash = new HashSet<int>();
        GetBattleStartMessage();
        while(!_reader.EndOfData)
        {
            var message = GetMessage<PlaybackMessageItem>();
            if(!_allHash.Add(message.frame))
            {
                Debug.LogError(message.frame);
            }
        }

        InitReader(binary);
#endif
        // read symbol 
        ReadSymbol(symbol);
    }

    private void ReadSymbol(byte[] symbol)
    {
        if(symbol == null) return;
        _hashStringParam = new List<string>();

        using(MemoryStream stream = new MemoryStream(symbol))
        {
            using(var reader = new BinaryReader(stream))
            {
                while(reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    _hashStringParam.Add(reader.ReadString());
                }
            }                
        }
    }

    void InitReader(byte[] binary, bool hasTotalFrame)
    {
        try
        {
            if(hasTotalFrame)
            {
                using(var x = new MemoryStream(binary, binary.Length - 4, 4))
                {
                    BinaryReader reader = new BinaryReader(x);
                    totalFrame = reader.ReadInt32();
                    reader.Dispose();
                }

                _reader.SetSource(binary, 0, binary.Length - 4);
            }
            else
            {
                _reader.SetSource(binary, 0, binary.Length);
            }
            
            ReadHeader();
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            NotMatchRecord = RecordErrorReason.Exception;
        }
    }

    void ReadHeader()
    {
        // var version = _reader.GetString();
        // if(version != Core.VersionUtil.GetVersionStr())
        // {
        //     // Debug.LogError($"not same version, will cause unsync currentVersion{Core.VersionUtil.GetVersionStr()} {version}");
        //     // NotMatchRecord = RecordErrorReason.VersionError;
        // }

        var platform = _reader.GetInt();
        // if(platform != (int)Application.platform)
        // {
        //     var p1IsWindows = (RuntimePlatform)platform == RuntimePlatform.WindowsEditor || (RuntimePlatform)platform == RuntimePlatform.WindowsPlayer;
        //     var p2IsWindows = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer;
        //     var allWindows = p1IsWindows && p2IsWindows;

        //     if(!allWindows)
        //     {
        //         NotMatchRecord = RecordErrorReason.PlatformError;
        //     }
        // }
    }

    public BattleStartMessage GetBattleStartMessage()
    {
        var startMessage = GetMessage<BattleStartMessage>();
        _battleId = startMessage.guid;

        // _preStates = new PlayBackTransformDiff[startMessage.joins.Length];

        return startMessage;
    }

    T GetMessage<T>() where T : INetSerializable
    {
        T t = default;
        try
        {
            t.Deserialize(_reader);
        }
        catch(Exception e)
        {
            IsUnSync = true;
            Debug.LogException(e);
        }
        return t;
    }

    public void GetMessageItem(int frame, List<UserFrameInput> listOut)
    {
        if(_reader.EndOfData && _current == null) return;

        LoadCurrentMessage();
        
        var packageFrame = _current.Value.frame;
        if(packageFrame < frame) 
        {
            throw new Exception($"_current.Value.frame < frame {packageFrame} {frame}");
        }
        else if(packageFrame > frame)
        {
            return;
        }

        if((_current.Value.playbackBit & PlaybackBit.Package) > 0)
        {
            var list = _current.Value.list;
            listOut.AddRange(list);
            ListPool<UserFrameInput>.Release(list);
        }

        _current = null;
    }

    private void LoadCurrentMessage()
    {
        if(_current == null)
        {
            _current = GetMessage<PlaybackMessageItem>();

            OptReadCurrentTransform();

            if((_current.Value.playbackBit & PlaybackBit.Hash) > 0)
            {
                _dicHash.Add(_current.Value.frame, _current.Value.hash);
            }

            if((_current.Value.playbackBit & PlaybackBit.ChangeState) > 0)
            {
                _currentClientStage = _current.Value.currentState;
            }
        }
    }

    private void OptReadCurrentTransform()
    {
        if(_current == null) return;

        PlaybackMessageItem data = _current.Value;
        
        if(data.list == null) return;

        // for(int i = 0; i < data.list.Count; i++)
        // {
        //     var x = data.list[i];
        //     if(x.HasFlag(MessageBit.Pos))
        //     {
        //         _preStates[x.id].RestorePos(ref x.posItem);
        //         data.list[i] = x;
        //     }
        //     if(x.HasFlag(MessageBit.Rotation))
        //     {
        //         _preStates[x.id].RestoreRotation(ref x.rotationItem);
        //         data.list[i] = x;
        //     }
        // }

        _current = data;
    }

    internal void Destroy()
    {
    }

    public bool IsUnSync{get;private set;} = false;

    internal void CheckHash(int frame, MessageHash checkMessage)
    {
        if(IsUnSync) return;

        if(_dicHash.TryGetValue(frame, out var innerMessage))
        {
            if(checkMessage.hash != innerMessage.hash)
            {
                var context = $"回放hash不同，第{frame}帧 【{checkMessage.hash}】【{innerMessage.hash}】 + batleID:【{_battleId}】" ;
                Debug.LogError(context);

                #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                context = DateTime.Now + " "+ context;
                File.AppendAllText(MainTestUnLimited.GetAppPath() + "ChallengeSyncLog.log", context + "\n");
                #endif

                IsUnSync = true;

#if DEBUG_1
                WriteUnsyncToFile(checkMessage.allHashDetails, checkMessage.escaped, Application.platform + "_" + _battleId + "_ReplayErrorLog.log", _hashStringParam);
                WriteUnsyncToFile(innerMessage.allHashDetails, innerMessage.escaped, Application.platform + "_" + _battleId + "InnerErrorLog.log", _hashStringParam);
#endif
            }
            else
            {
                _dicHash.Remove(frame);
            }
        }
    }

    public static void WriteUnsyncToFile(FrameHashItem[] allHashDetails, fp escaped, string logFile, List<string> symbol)
    {
        if(allHashDetails != null)
        {
            var list = new List<string>();
            var hashTypeCount = allHashDetails.Length;
            for(var hashTypeIndex = 0; hashTypeIndex < hashTypeCount; hashTypeIndex ++)
            {
                var hashItem = allHashDetails[hashTypeIndex];
                list.Add($"时间{escaped}【{hashItem.GetString(symbol)}】");
            }
            SaveLogError($"frame: {allHashDetails[0].frame}" + string.Join("\n", list), logFile /*"ReplayErrorLog.log"*/);
        }
    }

    internal bool CanFetchData(int clientStageIndex, int receivedServerFrame)
    {
        if(_reader.EndOfData && _current == null) return false;

        LoadCurrentMessage();

        if(_currentClientStage < clientStageIndex)
        {
            IsUnSync = true;
            return false;
        }

        if(_currentClientStage == clientStageIndex) return true;   // 如果当前帧有数据，就读取。

        return receivedServerFrame < _current.Value.frame;  // _current.Value.frame 是下一个回合的第一帧
    }

    public bool AllEnd => _reader.EndOfData && _current == null;

    public RecordErrorReason NotMatchRecord { get; private set; } = RecordErrorReason.None;

    public static void SaveLogError(string v, string path)
    {
        Debug.LogError(v);
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        File.WriteAllText("d://" + path, v + "\n");
        #endif

        // HeroHandlerMonoCom.Instance().StartCoroutine(PushLog(MainDownLoadAndReplay.URL_TEST_SYNC, v, path));
    }
    
    // public static IEnumerator PushLog(string uploadUrl, string unsyncMsg, string error)
    // {
    //     var form1 = new WWWForm();
    //     form1.AddField("msg", unsyncMsg);
    //     form1.AddField("path", error);
    //     form1.AddField("app", "wsa");
    //     var x = UnityWebRequest.Post($"{uploadUrl}/ComprareLog", form1);
    //     x.certificateHandler = new IgnoreCertificateHander();
    //     yield return x.SendWebRequest();

    //     if(x.IsNetworkError()) throw new Exception("网络连接错误");
    //     if(x.IsHttpError()) throw new Exception($"服务器返回错误码 {x.responseCode} {x.downloadHandler.text}");
    // }

    internal List<PlaybackMessageItem> GetAllInputs()
    {
        var list = new List<PlaybackMessageItem>();
        while(!_reader.EndOfData)
        {
            var message = GetMessage<PlaybackMessageItem>();
            list.Add(message);
        }

        return list;
    }
}