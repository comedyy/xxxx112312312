
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.Profiling;

// struct PlayBackTransformDiff
// {
//     public MessagePosItem pos;
//     public short rotationValue;

//     public void MakePosDiff(ref MessagePosItem item)
//     {
//         var x = new MessagePosItem(){
//             posX = item.posX - pos.posX,
//             posY = item.posY - pos.posY,
//             endMoving = item.endMoving
//         };

//         pos = item;
//         item = x;
//     }

//     public void RestorePos(ref MessagePosItem item)
//     {
//         item = new MessagePosItem(){
//             posX = item.posX + pos.posX,
//             posY = item.posY + pos.posY,
//             endMoving = item.endMoving
//         };
//         pos = item;
//     }

//     public void MakeRotationDiff(ref MessageRotationItem item)
//     {
//         var x = new MessageRotationItem(){
//             angle = (short)(item.angle - rotationValue)
//         };
//         rotationValue = item.angle;
//         item = x;
//     }

//     public void RestoreRotation(ref MessageRotationItem item)
//     {
//         item = new MessageRotationItem(){
//             angle = (short)(item.angle + rotationValue)
//         };
//         rotationValue = item.angle;
//     }
// }

public class PlaybackWriter
{
    public bool EndWrite => _writer == null;

    int _saveInterval = 0;
    string _path = "";
    private int _delaySaveFrame = 0;
    int _timer = 0;
    NetDataWriter _writer;
    //  // 如果有值，说明最后一帧还没写入，最后一帧必须要写入，来标记战斗结束。
    // int? _lastFrameIfNeed;
    int _totalFrame = 0;

    Dictionary<string, int> _allStringParams;
    // PlayBackTransformDiff[] _preStates;

    public static string CompressFormat = UseGzip ? ".gz" :".zip";
    public const bool UseGzip = true;

    public PlaybackWriter(string fileName, int saveIntervalSec)
    {
        _path = fileName;
        _saveInterval = saveIntervalSec;
        _writer = new NetDataWriter();

        if(File.Exists(fileName))
        {
            // Debug.LogError("PlaybackWriter file exist" + fileName);
            // File.Delete(fileName);
        }

        WriteHeader();
    }

    private void WriteHeader()
    {
        // _writer.Put(Core.VersionUtil.GetVersionStr()); // version
        _writer.Put((int)Application.platform); // platform
    }

    public void SaveStartMessage(BattleStartMessage data)
    {
        if(_writer == null)
        {
            Debug.LogError("SaveStartMessage error _writer null");
            return;
        }

        _writer.Put(data);
        // _lastFrameIfNeed = null;
        _totalFrame = 0;
        // _preStates = new PlayBackTransformDiff[data.joins.Length];

        SaveToDisk();
    }

    public void SaveFrame(PlaybackMessageItem data)
    {
        if(_writer == null) return;

        // save
        Profiler.BeginSample("saveFrameHash");
        // if((data.playbackBit & PlaybackBit.Hash) > 0 && data.hash.appendDetail)
        // {
        //     for(int i = 0; i < data.hash.allHashDetails.Length; i++)
        //     {
        //         ref var x = ref data.hash.allHashDetails[i];
        //         x.lstParamIndex = new List<short>(x.lstParam.Count);
        //         for(int j = 0; j < x.lstParam.Count; j++)
        //         {
        //             x.lstParamIndex.Add((short)GetParamIndex(x.lstParam[j]));
        //         }
        //     }
        // }
        Profiler.EndSample();

        // OptWriteTransform(ref data);

        _writer.Put(data);
        _totalFrame = data.frame;
    }

    // private void OptWriteTransform(ref PlaybackMessageItem data)
    // {
    //     if(data.list == null)
    //     {
    //         return;
    //     }

    //     for(int i = 0; i < data.list.Count; i++)
    //     {
    //         var x = data.list[i];
    //         if(x.HasFlag(MessageBit.Pos))
    //         {
    //             _preStates[x.id].MakePosDiff(ref x.posItem);
    //             data.list[i] = x;
    //         }
    //         if(x.HasFlag(MessageBit.Rotation))
    //         {
    //             _preStates[x.id].MakeRotationDiff(ref x.rotationItem);
    //             data.list[i] = x;
    //         }
    //     }
    // }

    // private int GetParamIndex(string v)
    // {
    //     if(_allStringParams == null) _allStringParams = new Dictionary<string, int>();
        
    //     if(_allStringParams.TryGetValue(v, out var index))
    //     {
    //         return index;
    //     }

    //     _allStringParams.Add(v, _allStringParams.Count);
    //     return _allStringParams.Count - 1;
    // }

    internal void Destroy()
    {
        SaveToDisk();
        _writer = null;
    }

    internal void OnUpdate(bool isLogicFrame, bool forceSaveThisFrame)
    {
        if (!forceSaveThisFrame)
        {
            if (isLogicFrame)
            {
                _timer++;
            }

            if (_timer < _saveInterval)
            {
                return;
            }

            if (isLogicFrame && _delaySaveFrame < 3)
            {
                _delaySaveFrame++;
                return;
            }
        }

        _delaySaveFrame = 0;
        _timer = 0;
        SaveToDisk();
    }

    void SaveToDisk()
    {
        if(_writer == null || _writer.Length == 0) return;

        using (var stream = new FileStream(_path, FileMode.Append))
        {
            stream.Write(_writer.Data, 0, _writer.Length);
            _writer.Reset();
        }
    }

    
    void SeveFrameCountToDisk(int frame)
    {
        using (var stream = new FileStream(_path, FileMode.Append))
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(frame);
            writer.Dispose();
        }

        // write To synmal file
        if(_allStringParams != null)
        {
            using (var stream = new FileStream(_path + "_symbol", FileMode.Create))
            {
                BinaryWriter writer = new BinaryWriter(stream);
                var list = new string[_allStringParams.Count];
                foreach(var x in _allStringParams)
                {
                    list[x.Value] = x.Key;
                }
                foreach(var x in list)
                {
                    writer.Write(x);
                }
                writer.Dispose();
            }
        }
    }

    public string BattleWinSaveAndCompress()
    {
        if(_writer == null) return "";

        // int totalFrame = BattleCore.BattleControllerManager.GetBattleController<BattleDataController>().FrameCount;
        // if(totalFrame != _totalFrame)
        // {
        //     _writer.Put(new PlaybackMessageItem(){frame = (ushort)totalFrame, playbackBit = PlaybackBit.GameEnd});
        // }

        SaveToDisk();
        SeveFrameCountToDisk(_totalFrame);

        _writer = null; // 不能写入了。
        var destPath = _path + CompressFormat;
        if(!CompressFileToZip(destPath, _path, UseGzip))
        {
            return "";
        }

        return destPath;
    }

    // 战斗失败把存档删掉。 目前需求
    public void BattleFailedRemoveSave()
    {
        File.Delete(_path);
        _writer = null; // 不能再往里面写了。
    }

    bool CompressFileToZip(string fileName, string srcPath, bool useGzip = false)
    {
        try
        {
            if (!File.Exists(srcPath))
            {
                return false;
            }

            File.Delete(fileName);
            using (var compressedFileStream = new FileStream(fileName, FileMode.CreateNew))
            {
                if (useGzip)
                {
                    using (var originalFileStream = new FileStream(srcPath, FileMode.Open)) {
                        
                        using (var compressor = new GZipStream(compressedFileStream, CompressionMode.Compress)) {
                            originalFileStream.CopyTo(compressor);
                        }
                    }
                }
                else
                {
                    //Create an archive and store the stream in memory.
                    using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false)) {
                        //Create a zip entry for each attachment
                        var zipEntry = zipArchive.CreateEntry("proto", System.IO.Compression.CompressionLevel.Optimal);

                        //Get the stream of the attachment
                        using (var originalFileStream = new FileStream(srcPath, FileMode.Open))
                        using (var zipEntryStream = zipEntry.Open()) {
                            //Copy the attachment stream to the zip entry stream
                            originalFileStream.CopyTo(zipEntryStream);
                        }
                    }
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"压缩存档报错：{srcPath}\n{e.Message} \n{e.StackTrace}");
            return false;
        }
        finally
        {
            File.Delete(srcPath);
        }

        return true;
    }

    // internal void SetLastFrame(int frame)
    // {
    //     _lastFrameIfNeed = frame;
    // }
}