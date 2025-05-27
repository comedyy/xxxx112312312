using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEcs : MonoBehaviour
{
    public OneBattle game;

    public InstanceDrawer instanceDrawer;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnDestroy()
    {
        game.Dispose();
        game = null;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Start Battle"))
        {
            StartBattle(BattleType.Client);
        }
        if(GUI.Button(new Rect(10, 50, 100, 30), "Replay Battle"))
        {
            StartBattle(BattleType.Replay);
        }
    }

    private void StartBattle(BattleType client)
    {
        game = new OneBattle();
        game.Initialize(client);
    }

    // List<Vector3> allPoint = new List<Vector3>();
    // void Update()
    // {
    //     instanceDrawer.Update1(allPoint);
    // }
}
