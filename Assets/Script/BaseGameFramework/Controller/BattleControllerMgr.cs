using System;
using System.Collections.Generic;

public class BattleControllerMgr
{
    private static BattleControllerMgr _instance;
    Dictionary<Type, IBattleController> _dictionary = new Dictionary<Type, IBattleController>();

    public static BattleControllerMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BattleControllerMgr();
            }
            return _instance;
        }
    }

    private BattleControllerMgr()
    {
        // Initialize any necessary components or systems here
    }

    public void StartBattle()
    {
    }

    public void StopBattle()
    {
        _dictionary.Clear();
    }

    public void AddController<T>(T controller) where T : IBattleController
    {
        if (!_dictionary.ContainsKey(typeof(T)))
        {
            _dictionary.Add(typeof(T), controller);
        }
        else
        {
            throw new Exception($"Controller of type {typeof(T)} already exists.");
        }
    }

    public T GetController<T>() where T : IBattleController
    {
        if (_dictionary.TryGetValue(typeof(T), out var controller))
        {
            return (T)controller;
        }
        else
        {
            throw new Exception($"Controller of type {typeof(T)} not found.");
        }
    }
}