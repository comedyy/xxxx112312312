using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class OneBattle
{
    BattleLogic _battleLogic;
    public OneBattle()
    {
        _battleLogic = new BattleLogic();
    }
    public void Initialize()
    {
        // Initialize the battle logic here
        Debug.Log("Battle Initialized");

        _battleLogic.Initialize(new BattleStartMessage() { seed = 12345678, guid = Guid.NewGuid().ToString() });

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
}