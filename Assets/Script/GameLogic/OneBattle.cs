using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Entities;
using UnityEngine;

public class OneBattle
{
    BattleLogic _battleLogic;
    public OneBattle()
    {
    }
    public void Initialize(BattleType client)
    {
        // Initialize the battle logic here
        Debug.Log("Battle Initialized");

        _battleLogic = new BattleLogic();
        if (client == BattleType.Client)
        {
            _battleLogic.StartBattle(new BattleStartMessage() { seed = 12345678, guid = Guid.NewGuid().ToString() }, BattleType.Client);
        }
        else if (client == BattleType.ContinueBattle)
        {
            _battleLogic.StartContinueBattle(GetDefaultPlaybackGUID());
        }
        else
        {
            _battleLogic.StartReplay(GetDefaultPlaybackGUIDZip());
        }

        // Add my system to the battle logic
        World world = World.DefaultGameObjectInjectionWorld;
        world.GetOrCreateSystemManaged<PreRvoSystemGroup>().AddSystemToUpdateList(world.GetOrCreateSystem<InitAddAgentsSystem>());

        world.GetOrCreateSystemManaged<RvoCustomAgentPropertySystemGroup>()
            .AddSystemToUpdateList(world.GetOrCreateSystem<ChangeRvoParamSystem>());

        world.GetOrCreateSystemManaged<AfterRvoSystemGroup>().AddSystemToUpdateList(world.GetOrCreateSystem<TimeoutFinishGameSystem>());
    }


    public void Dispose()
    {
        // Dispose of the battle logic here
        Debug.Log("Battle Disposed");
        _battleLogic.Dispose();
    }

    private string GetDefaultPlaybackGUIDZip()
    {
        DirectoryInfo d = new DirectoryInfo(PlaybackController.PlaybackPath);

        if (!d.Exists) return "";

        List<FileInfo> files = d.GetFiles().ToList();

        files.RemoveAll(m => !m.Name.EndsWith(PlaybackWriter.CompressFormat));

        if (files.Count == 0) return "";

        files.Sort((m, n) => { return n.CreationTime.CompareTo(m.CreationTime); });

        var guid = files[0].Name.Replace("wsa_playback_", "").Replace(PlaybackWriter.CompressFormat, "");
        return guid;
    }
    
    
    private static string GetDefaultPlaybackGUID()
    {
        DirectoryInfo d = new DirectoryInfo(PlaybackController.PlaybackPath);
        
        if(!d.Exists) return "";

        List<FileInfo> files = d.GetFiles().ToList(); 
        files.RemoveAll(m=>m.Name.EndsWith(PlaybackWriter.CompressFormat));
        files.RemoveAll(m=>m.Name.EndsWith("_symbol"));

        if(files.Count == 0) return "";

        files.Sort((m, n)=>{return n.CreationTime.CompareTo(m.CreationTime);});
        
        var guid = files[0].Name.Replace("wsa_playback_", "").Replace(PlaybackWriter.CompressFormat, "");
        return guid;
    }
}