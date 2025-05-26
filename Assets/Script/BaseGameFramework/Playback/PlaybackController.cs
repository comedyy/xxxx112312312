
using System;
using System.IO;
using System.IO.Compression;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

public enum PlaybackMode
{
    ReadPlaybackZip = 1 << 1,
    ReadPlayOnGoing = 1 << 2,
    Write = 1 << 3,
}

public class PlaybackController : IBattleController
{
    internal bool readError
    {
        get
        {
            if(Reader == null) return true;

            return Reader.NotMatchRecord != RecordErrorReason.None || Reader.IsUnSync;
        }
    }

    public PlaybackReader Reader{get; private set;}
    public PlaybackWriter Writer{get; private set;}

    #if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public static string PlaybackPath => Application.dataPath + "/../playback/";
    #else
    public static string PlaybackPath => Application.persistentDataPath + "/playback/";
    #endif

    [Preserve]
    public PlaybackController(){}

    public PlaybackController(string guid, PlaybackMode mode)
    {
        bool read = (mode & (PlaybackMode.ReadPlaybackZip | PlaybackMode.ReadPlayOnGoing)) > 0;
        bool write = (mode & PlaybackMode.Write) > 0;
        var folderPath = PlaybackPath;
        if(!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var path = GetPath(guid);
        if(read)
        {
            var isReadZip = (mode & PlaybackMode.ReadPlaybackZip) > 0;
            var isContinueGoing = (mode & PlaybackMode.ReadPlayOnGoing) > 0;

            byte[] binary;
            if (isReadZip)
            {
                binary = LoadBytesFromZipOnDisk(path + PlaybackWriter.CompressFormat, PlaybackWriter.UseGzip);
            }
            else
            {
                binary = File.ReadAllBytes(path);
            }

            if(binary == null)
            {
                return;
            }

            // symbol 

            byte[] symbol = null;
            var symbolPath = path + "_symbol";
            if(File.Exists(symbolPath))
            {
                symbol = File.ReadAllBytes(symbolPath);
            }

            Reader = new PlaybackReader(binary, symbol, !isContinueGoing);
        }
        
        if(write)
        {
            File.Delete(path);
            Writer = new PlaybackWriter(path, 100);
        }
    }

    byte[] LoadBytesFromZipOnDisk(string zipPath, bool useGz)
    {
        try
        {
            if (!File.Exists(zipPath))
            {
                Debug.LogError($"读取存档不存在 ：{zipPath}");
                return null;
            }
            
            using (var compressedFileStream = new FileStream(zipPath, FileMode.Open))
            {

                if (useGz)
                {
                    using (var decompressor = new GZipStream(compressedFileStream, CompressionMode.Decompress))
                    {
                        using (var memorySteam = new MemoryStream())
                        {
                            decompressor.CopyTo(memorySteam); 
                            return memorySteam.ToArray();
                        }
                    }
                }
                else
                {
                    //Create an archive and store the stream in memory.
                    using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Read, false)) {
                        //Create a zip entry for each attachment
                        var zipEntry = zipArchive.GetEntry("proto");

                        //Get the stream of the attachment
                        using (var memorySteam = new MemoryStream())
                        using (var zipEntryStream = zipEntry.Open()) {
                            //Copy the attachment stream to the zip entry stream
                            zipEntryStream.CopyTo(memorySteam);
                            return memorySteam.ToArray();
                        }
                    }
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"读取存档失败 ：{zipPath}\n{e.Message} \n{e.StackTrace}");
            return null;
        }
    }

    public static string GetPath(string guid)
    {
        return PlaybackPath + $"wsa_playback_{guid}";
    }

    public static string GetPathBackUp(string guid)
    {
        return PlaybackPath + $"wsa_playback_{guid}_backup";
    }

    //bbfd-34d-vxcvf 或者 11530102021.420304a5215f682214e32371006ba8b8f1
    public static string MakeZipName(string guid)
    {
        return $"wsa_playback_{guid}{PlaybackWriter.CompressFormat}";
    }

    public static string MakeGuidWithHeader(string localGuid, string remoteGuid, int configId)
    {
        // return $"{localGuid}|{remoteGuid}|{VersionUtil.GetVersionStr()}|{(int) Application.platform}|{configId}";
        return "";
    }

    public static void ResolveGuid(string localGuid, string remoteGuid, out string fileNameWithExt, out string fileNameNoExt, out string destPath, out bool useStorage)
    {
        useStorage = false;
        //如果平台录像,直接使用路径 //sample: https://statics.9458.com/storage/data_30days/20240401/03/11530102021.420304a5215f682214e32371006ba8b8f1.zip
        if (string.IsNullOrEmpty(localGuid) && !string.IsNullOrEmpty(remoteGuid))
        {
            var fileName = Path.GetFileNameWithoutExtension(remoteGuid);
            fileNameNoExt = fileName;
            fileNameWithExt = MakeZipName(fileName);
            destPath = PlaybackPath + fileNameWithExt;  
            useStorage = true;
        }
        else
        {
            if (!string.IsNullOrEmpty(remoteGuid))
            {
                useStorage = true;
            }
            
            fileNameWithExt = MakeZipName(localGuid);
            fileNameNoExt = localGuid;
            destPath = PlaybackPath + fileNameWithExt;  
        }
    }

    // {本地guid}|{平台guid}|1.7.0.0|7
    public static bool ResolveReplayHeader(string recordGuid, out string localGuid, out string remoteGuid, out string version, out string platform)
    {
        version = string.Empty;
        platform = string.Empty;
        localGuid = string.Empty;
        remoteGuid = string.Empty;
        
        var list = recordGuid.Split(new char[]{'|'});
        if(list.Length == 0)
        {
            Debug.LogError($"获取guid失败 {recordGuid}");
            return false;
        }
        
        localGuid = list[0];
        remoteGuid = list.Length > 1 ? list[1] : "";
        version = list.Length > 2 ? list[2] : "";
        platform = list.Length > 3 ? list[3] : "";
        return true;
    }

    public static void CheckGuidFileExist(string localFileName, string playFileName)
    {
        if (localFileName.Equals(playFileName)) return;
        
        var localFilePath = PlaybackPath + MakeZipName(localFileName);
        var playFilePath = PlaybackPath + MakeZipName(playFileName);
        if (!File.Exists(localFilePath)
        && File.Exists(playFilePath))
        {
            File.Copy(playFilePath, localFilePath);
        }
    }

    public void Destroy()
    {
        if(Reader != null) Reader.Destroy();
        if(Writer != null) Writer.Destroy();
    }

    internal string SaveAndCompress()
    {
        if(Writer == null) return "";

        return Writer.BattleWinSaveAndCompress();
    }

    internal static void DeleteOnGoing()
    {
        // PlayerPrefs.DeleteKey(LocalFrameClient.OnGoingBattleGUIDKey);

        // 查看是否还在 write，如果在，报错。
        var playbackController = BattleControllerMgr.Instance.GetController<PlaybackController>();
        if(!playbackController.Writer.EndWrite)
        {
            Debug.LogError("DeleteOnGoing File Error, writer is Working, Return!");
            return;
        }

        var guid = PlaybackReader._battleId;
        if(string.IsNullOrEmpty(guid)) return;
        
        var path = GetPath(guid);
        if(File.Exists(path))
        {
            File.Delete(path);
        }

        var pathBaked = GetPathBackUp(guid);
        if(File.Exists(pathBaked))
        {
            File.Delete(pathBaked);
        }
    }
}