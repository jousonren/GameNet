using System;

public class TestReq : Request {
    public string msg;
    public TestReq(string message) {
        msg = message;
    }

    public override int GetProtocol() {
        return NetProtocols.Test;
    }
}

public class TestResp : Resp {
    public string msg;
    public override int GetProtocol() {
        return NetProtocols.Test;
    }
}


public class LoginReq : Request {
    public int id;

    public LoginReq(int userId) {
        id = userId;
    }

    public override int GetProtocol() {
        return NetProtocols.Login;
    }
}
public class LoginResp : Resp {
    public int result;
    public override int GetProtocol() {
        return NetProtocols.Login;
    }
}

public class EnterRoomReq : Request {
    public override int GetProtocol() {
        return NetProtocols.EnterRoom;
    }
}
public class EnterRoomResp : Resp {
    public int result;
    public int camp;
    public override int GetProtocol() {
        return NetProtocols.EnterRoom;
    }
}

public class KickOutResp : Resp {
    public string reason;
    public override int GetProtocol() {
        return NetProtocols.KickOut;
    }
}

public class PingPongReq : Request {
    public override int GetProtocol() {
        return NetProtocols.PingPong;
    }
}
public class PingPongResp : Resp {
    public override int GetProtocol() {
        return NetProtocols.PingPong;
    }
}

public class ReadyReq : Request {
    public string chariotsMsg;
    public override int GetProtocol() {
        return NetProtocols.Ready;
    }
}
public class ReadyResp : Resp {
    public int result;
    public override int GetProtocol() {
        return NetProtocols.Ready;
    }
}
[Serializable]
public class ChariotMsg {
    public int id;
    public string msg;
    public int pos;
    public int camp;
}
public class BeginGameResp : Resp {
    public string roomMsg;
    public ChariotMsg[] chariots;
    public override int GetProtocol() {
        return NetProtocols.BeginGame;
    }
}

public class CreatBattlefieldReq : Request {
    public override int GetProtocol() {
        return NetProtocols.CreatBattlefield;
    }
}
public class CreatBattlefieldResp : Resp {
    public int result;
    public override int GetProtocol() {
        return NetProtocols.CreatBattlefield;
    }
}

public class StartFightingResp : Resp {
    public override int GetProtocol() {
        return NetProtocols.StartFighting;
    }
}
[Serializable]
public class WeaponMsg {
    public float gunX;
    public float baseY;
}
public class SyncChariotMsgReq : Request {
    public float x;
    public float y;
    public float z;
    public float rx;
    public float ry;
    public float rz;
    public WeaponMsg[] weaponMsg;

    public override int GetProtocol() {
        return NetProtocols.SyncChariotMsg;
    }
}
public class SyncChariotMsgResp : Resp {
    public int id;
    public float x;
    public float y;
    public float z;
    public float rx;
    public float ry;
    public float rz;
    public WeaponMsg[] weaponMsg;
    public override int GetProtocol() {
        return NetProtocols.SyncChariotMsg;
    }
}

public class FireMsgReq : Request {
    public int weaponId;
    public float x;
    public float y;
    public float z;
    public float rx;
    public float ry;
    public float rz;

    public override int GetProtocol() {
        return NetProtocols.FireMsg;
    }
}
public class FireMsgResp : Resp {
    public int playerId;
    public int weaponId;
    public float x;
    public float y;
    public float z;
    public float rx;
    public float ry;
    public float rz;
    public override int GetProtocol() {
        return NetProtocols.FireMsg;
    }
}

public class HitMsgReq : Request {
    public int targetId;
    public int damage;
    public int weaponId;
    public override int GetProtocol() {
        return NetProtocols.HitMsg;
    }
}
public class HitMsgResp : Resp {
    public int attackerId;
    public int targetId;
    public int damage;
    public int weaponId;
    public override int GetProtocol() {
        return NetProtocols.HitMsg;
    }
}

public class GameOverResp : Resp {
    public int winner;
    public override int GetProtocol() {
        return NetProtocols.GameOver;
    }
}

public class ExitGameReq : Request {
    public override int GetProtocol() {
        return NetProtocols.ExitGame;
    }
}
public class ExitGameResp : Resp {
    public int id;
    public override int GetProtocol() {
        return NetProtocols.ExitGame;
    }
}

public class ExitRoomReq : Request {
    public override int GetProtocol() {
        return NetProtocols.ExitRoom;
    }
}
public class ExitRoomResp : Resp {
    public int id;
    public override int GetProtocol() {
        return NetProtocols.ExitRoom;
    }
}
public class PlayerDieReq : Request {
    public override int GetProtocol() {
        return NetProtocols.PlayerDie;
    }
}
public class PlayerDieResp : Resp {
    public int id;
    public override int GetProtocol() {
        return NetProtocols.PlayerDie;
    }
}