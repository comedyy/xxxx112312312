using System;

public class BattleControlFactory
{
    internal static ILocalFrame Create(BattleType battleType, LocalFrame localFrame, out InputCache inputCache)
    {
        inputCache = null;
        if (battleType == BattleType.Client)
        {
            inputCache = new InputCache(0);
            return new ClientBattleControl(localFrame, inputCache);
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