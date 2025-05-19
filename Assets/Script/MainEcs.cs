using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEcs : MonoBehaviour
{
    public OneBattle game;
    // Start is called before the first frame update
    void Start()
    {
        game = new OneBattle();
        game.Initialize();
    }
}
