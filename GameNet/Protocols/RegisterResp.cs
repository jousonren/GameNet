public class RegisterResp {
    /// <summary>
    /// 注册所有的response
    /// </summary>
    public static void RegisterAll() {
        ProtoManager.Instance.AddProtocol<TestResp>(NetProtocols.Test);
        ProtoManager.Instance.AddProtocol<LoginResp>(NetProtocols.Login);
        ProtoManager.Instance.AddProtocol<EnterRoomResp>(NetProtocols.EnterRoom);
        ProtoManager.Instance.AddProtocol<KickOutResp>(NetProtocols.KickOut);
        ProtoManager.Instance.AddProtocol<PingPongResp>(NetProtocols.PingPong);
        ProtoManager.Instance.AddProtocol<ReadyResp>(NetProtocols.Ready);
        ProtoManager.Instance.AddProtocol<BeginGameResp>(NetProtocols.BeginGame);
        ProtoManager.Instance.AddProtocol<CreatBattlefieldResp>(NetProtocols.CreatBattlefield);
        ProtoManager.Instance.AddProtocol<StartFightingResp>(NetProtocols.StartFighting);
        ProtoManager.Instance.AddProtocol<SyncChariotMsgResp>(NetProtocols.SyncChariotMsg);
        ProtoManager.Instance.AddProtocol<FireMsgResp>(NetProtocols.FireMsg);
        ProtoManager.Instance.AddProtocol<HitMsgResp>(NetProtocols.HitMsg);
        ProtoManager.Instance.AddProtocol<GameOverResp>(NetProtocols.GameOver);
        ProtoManager.Instance.AddProtocol<ExitGameResp>(NetProtocols.ExitGame);
        ProtoManager.Instance.AddProtocol<ExitRoomResp>(NetProtocols.ExitRoom);
        ProtoManager.Instance.AddProtocol<PlayerDieResp>(NetProtocols.PlayerDie);
    }
}
