using System;

public class BattleControlFactory
{
    internal static ILocalFrame Create(int idController, BattleType battleType, IPutMessage putMessage, out InputCache inputCache)
    {
        inputCache = null;
        if (battleType == BattleType.Client)
        {
            inputCache = new InputCache(idController);
            return new ClientBattleControl(inputCache, putMessage);
        }
        else if (battleType == BattleType.Replay)
        {
            var playbackReader = BattleControllerMgr.Instance.GetController<PlaybackController>().Reader;
            return new PlaybackBattleControl(playbackReader, putMessage);
        }
        else if (battleType == BattleType.ContinueBattle)
        {
            inputCache = new InputCache(idController);
            var playbackReader = BattleControllerMgr.Instance.GetController<PlaybackController>().Reader;
            return new ContinueBattleControl(inputCache, playbackReader, putMessage);
        }
        else if (battleType == BattleType.OnlineBattle)
        {
            inputCache = new InputCache(idController);
            IClientGameSocket clientGameSocket = ClientBattleRoomMgr.Instance().GetSocket();
            return new OnlineBattleControl(inputCache, clientGameSocket, putMessage);
        }
        else
        {
            throw new ArgumentException($"Unsupported BattleType: {battleType}");
        }
    }
}