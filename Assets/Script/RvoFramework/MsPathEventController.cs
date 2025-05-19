using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle.CommonLib
{
    public class MsPathEventController
    {
        static List<GameEventBattleRvoOpt> gameEventBattleRvoOpts = new List<GameEventBattleRvoOpt>();
        static List<GameEventBattleRvoOptSetPos> gameEventBattleRvoOptSetPoss = new List<GameEventBattleRvoOptSetPos>();

        public static void AddRvoOpt(GameEventBattleRvoOpt gameEventBattleRvoOpt)
        {
            gameEventBattleRvoOpts.Add(gameEventBattleRvoOpt);
        }

        public static void AddSetPosEvent(GameEventBattleRvoOptSetPos gameEventBattleRvoOpt)
        {
            gameEventBattleRvoOptSetPoss.Add(gameEventBattleRvoOpt);
        }

        public static void InputEvent()
        {
            foreach(var e in gameEventBattleRvoOpts)
            {
                if (e is GameEventBattleRvoOpt @event)
                {
                    if(@event.opt == GameEventBattleRvoOpt.Opt.AddObstacle)
                    {
                        MSPathSystem.AddObstacleCS(@event.points);
                    }
                    else if(@event.opt == GameEventBattleRvoOpt.Opt.ProcessObstacle)
                    {
                        MSPathSystem.ProcessObstaclesCS();
                    }
                    else if(@event.opt == GameEventBattleRvoOpt.Opt.RemoveObstacle)
                    {
                        MSPathSystem.ClearObstacleCS();
                    }
                    else if(@event.opt == GameEventBattleRvoOpt.Opt.RemoveAgent)
                    {
                        MSPathSystem.RemoveAgentCS(@event.param, @event.param1);
                    }
                    else if(@event.opt == GameEventBattleRvoOpt.Opt.SetAgentRadius)
                    {
                        MSPathSystem.SetAgentRadiusCS(@event.param, @event.param1, @event.points[0]);
                    }
                }
            }
            gameEventBattleRvoOpts.Clear();

            foreach(var @event in gameEventBattleRvoOptSetPoss)
            {
                MSPathSystem.SetAgentPositionCS(@event.type, @event.index, @event.x, @event.z);
            }
            gameEventBattleRvoOptSetPoss.Clear();
        }
    }
}

