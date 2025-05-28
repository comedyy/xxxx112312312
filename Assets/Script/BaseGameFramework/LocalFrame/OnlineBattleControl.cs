using System;
using System.Collections.Generic;
using LiteNetLib.Utils;

public class OnlineBattleControl : ILocalFrame
{
    float totalTime;
    float preFrameSeconds;
    InputCache _inputCache;
    IClientGameSocket _clientGameSocket;
    OnlineBattleNetHelper _onlineBattleNetHelper;
    IPutMessage _putMessage;

    public OnlineBattleControl(InputCache inputCache, IClientGameSocket clientGameSocket, IPutMessage putMessage)
    {
        _putMessage = putMessage;
        _inputCache = inputCache;
        _clientGameSocket = clientGameSocket;
        _onlineBattleNetHelper = new OnlineBattleNetHelper(_clientGameSocket, _putMessage);
    }

    public void Dispose()
    {
        _onlineBattleNetHelper.OnDispose();
    }

    public void SetBattleEnd()
    {
        _clientGameSocket.SendMessage(new FinishRoomMsg()
        {
            stageValue = 999
        });
    }

    public void Update()
    {
        var deltaTime = MathF.Min(UnityEngine.Time.deltaTime, ComFrameCount.DELTA_TIME);
        totalTime += deltaTime;
        if (totalTime - preFrameSeconds < ComFrameCount.DELTA_TIME) // 还未能upate
        {
            return;
        }

        preFrameSeconds += ComFrameCount.DELTA_TIME;

        AddLocalFrame();
    }

    private void AddLocalFrame()
    {
        if (!_onlineBattleNetHelper.IsInReconnectState)
        {
            var item = _inputCache.FetchItem();
            if (item != null)
            {
                
                _clientGameSocket.SendMessage(new UserFrameInputMsg()
                {
                    input = item.Value
                });
            }
        }
    }
    













}
