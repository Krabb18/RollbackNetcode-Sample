﻿using System;
using System.Numerics;
using Backdash;
using Backdash.Serialization.Numerics;


//Das hier sind die encodeten inputs siehe: https://delta3.studio/Backdash/docs/developer_guide.html
[Flags]
public enum GameInput
{
    None = 0,
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,
}

public class GameState
{
    public Vector2 Position1;
    public Vector2 Position2;
    public int Score1;
    public int Score2;
    public Vector2 Target;
    public uint RandomSeed;
}

public class NonGameState
{
    public required NetcodePlayer? LocalPlayer;
    public required NetcodePlayer RemotePlayer;
    public required INetcodeSessionInfo SessionInfo;
    public bool IsRunning;
    public float SyncProgress;
    public string LastError = "";
    public uint Checksum;
    public PlayerStatus RemotePlayerStatus;
    public DateTime LostConnectionTime;
    public TimeSpan DisconnectTimeout;
}

public enum PlayerStatus
{
    Connecting = 0,
    Synchronizing,
    Running,
    Waiting,
    Disconnected,
}
