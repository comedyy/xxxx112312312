using System;

public class BattleControlFactory
{
    internal static ILocalFrame Create(BattleType battleType, LocalFrame localFrame)
    {
        if (battleType == BattleType.Client)
        {
            return new ClientBattleControl(localFrame);
        }
        else if (battleType == BattleType.Replay)
        {
            var playbackReader = BattleControllerMgr.Instance.GetController<PlaybackController>().Reader;
            return new ReplayBattleControl(localFrame, playbackReader);
        }
        else
        {
            throw new ArgumentException($"Unsupported BattleType: {battleType}");
        }
    }
}