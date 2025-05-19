using System.Collections;
using System.Collections.Generic;
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

        _battleLogic.Initialize();
    }
}
