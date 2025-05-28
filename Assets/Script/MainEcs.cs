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
        if (GUI.Button(new Rect(1000, 310, 200, 100), "Start Battle"))
        {
            StartBattle(BattleType.Client);
        }
        if (GUI.Button(new Rect(1000, 450, 200, 100), "Replay Battle"))
        {
            StartBattle(BattleType.Replay);
        }
        if (GUI.Button(new Rect(1000, 550, 200, 100), "Continue Battle"))
        {
            StartBattle(BattleType.ContinueBattle);
        }
        if (GUI.Button(new Rect(1000, 650, 200, 100), "Online Battle"))
        {
            var roomGUI = gameObject.AddComponent<RoomGUI>();
            roomGUI.OnBattleStart = (msg) =>
            {
                StartBattle(BattleType.OnlineBattle, msg);
                Destroy(roomGUI);
            };
        }
    }

    private void StartBattle(BattleType client, BattleStartMessage battleStartMessage = default)
    {
        game = new OneBattle();
        game.Initialize(client, battleStartMessage);
    }

    // List<Vector3> allPoint = new List<Vector3>();
    // void Update()
    // {
    //     instanceDrawer.Update1(allPoint);
    // }
}
