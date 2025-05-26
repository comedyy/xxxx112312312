public class BattleDataController : IBattleController
{
    public BattleStartMessage battleStartMessage;

    public BattleDataController(BattleStartMessage battleStartMessage)
    {
        this.battleStartMessage = battleStartMessage;
    }
}