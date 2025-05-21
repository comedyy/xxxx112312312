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
        game = new OneBattle();
        game.Initialize();
    }

    private void OnDestroy()
    {
        game.Dispose();
        game = null;
    }

    // List<Vector3> allPoint = new List<Vector3>();
    // void Update()
    // {
    //     instanceDrawer.Update1(allPoint);
    // }
}
