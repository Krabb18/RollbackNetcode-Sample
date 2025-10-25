using Backdash;
using Backdash.Core;
using RollbackSandbox;
using System;
using System.Diagnostics;

Args gameArgs = new();

Console.WriteLine("Hi");

var session = CreateNetcodeSession(gameArgs);

TestGame testGame = new(session);


using var game = new RollbackSandbox.Game1(ref session);
game.Run();

session.Dispose();
static INetcodeSession<GameInput> CreateNetcodeSession(Args args) 
{
    var port = args.Port;

    var builder = RollbackNetcode
        .WithInputType<GameInput>()
        .WithPlayerCount(args.PlayerCount)
        .WithPort(args.Port)
        .WithInputDelayFrames(2)
        .WithInitialRandomSeed(42)
        .WithLogLevel(LogLevel.Information)
        .WithPackageStats()
        .ConfigureProtocol(options =>
        {
            options.NumberOfSyncRoundTrips = 10;
            // p.NetworkLatency = TimeSpan.FromMilliseconds(300);
            // p.DelayStrategy = Backdash.Network.DelayStrategy.Constant;
            // options.DisconnectTimeoutEnabled = false;
        });

    // parse console arguments checking if it is a spectator
    if (args.IsForSpectate(out var hostEndpoint))
        builder
            .WithFileLogWriter($"logs/log_game_spectator_{port}.log", append: false)
            .ForSpectator(hostEndpoint);

    // not a spectator, creating a `remote` game session
    else if (args.IsForPlay(out var players))
        builder
            // Write logs in a file with player number
            .WithFileLogWriter($"logs/log_game_player_{port}.log", append: false)
            .WithPlayers(players)
            .ForRemote();

    else
        throw new InvalidOperationException("Invalid CLI arguments");

    return builder.Build();
}
