using Backdash;
using Backdash.Serialization.Numerics;
using Backdash.Serialization;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;


namespace RollbackSandbox
{
    public class TestGame : INetcodeSessionHandler
    {
        readonly INetcodeSession<GameInput> session;

        public readonly NonGameState nonGameState;

        GameState currentGameState;
        GameLogic gameLogic;

        public TestGame(INetcodeSession<GameInput> session)
        {
            this.session = session;
            gameLogic = new GameLogic();
            currentGameState = new GameState();

            if (this.session.IsRemote())
            {
                if(!this.session.TryGetLocalPlayer(out var localPlayer)) 
                {
                    throw new InvalidOperationException("Local player not found :(");
                }

                if(!this.session.TryGetRemotePlayer(out var remote)) 
                {
                    throw new InvalidOperationException("Remote player not found :(");
                }

                nonGameState = new()
                {
                    LocalPlayer = localPlayer,
                    RemotePlayer = remote,
                    SessionInfo = session,
                };
            }
        }

        GameInput GetKeyboardInput() 
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.A)) { return GameInput.Left; }
            else if (state.IsKeyDown(Keys.D)) { return GameInput.Right; }
            else if (state.IsKeyDown(Keys.W)) { return GameInput.Up; }
            else if (state.IsKeyDown(Keys.S)) { return GameInput.Down; }

            return GameInput.None;
        }

        public void UpdateGame() 
        {
            session.BeginFrame();
            UpdateState();
        }

        void UpdateState() 
        {
            if (nonGameState.LocalPlayer is { } localPlayer) 
            {
                var localInput = GetKeyboardInput();

                var result = session.AddLocalInput(localPlayer, localInput);
                if(result is not ResultCode.Ok)
                {
                    //Log($"UNABLE TO ADD INPUT: {result}");
                    nonGameState.LastError = @$"{result} {DateTime.Now:mm\:ss\.fff}";
                    return;
                }
            }

            nonGameState.Checksum = session.CurrentChecksum;
            session.SynchronizeInputs();
            //session.SetRandomSeed() //Erstmal testen wozu das nötig ist

            var (input1, input2) = (session.GetInput(0), session.GetInput(1));
            gameLogic.Update(ref currentGameState, input1, input2);

            session.AdvanceFrame();
        }

        public void DrawGame(Texture2D texture, SpriteBatch spriteBatch) 
        {
            //Hier dann den current state zeichnen
            spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Vector2(currentGameState.Position1.X, currentGameState.Position1.Y), Microsoft.Xna.Framework.Color.White);
            spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Vector2(currentGameState.Position2.X, currentGameState.Position2.Y), Microsoft.Xna.Framework.Color.White);
        }

        public void OnSessionStart() 
        {
            /* ... */
            nonGameState.IsRunning = true;
            nonGameState.RemotePlayerStatus = PlayerStatus.Running;
        }
        public void OnSessionClose() 
        {
            /* ... */
            nonGameState.IsRunning = false;
            nonGameState.RemotePlayerStatus = PlayerStatus.Disconnected;
        }
        public void SaveState(in Frame frame, ref readonly BinaryBufferWriter writer) 
        {
            /* ... */
            writer.Write(currentGameState.Position1);
            writer.Write(currentGameState.Position2);

            writer.Write(currentGameState.Score1);
            writer.Write(currentGameState.Score2);

            writer.Write(currentGameState.Target.X);
            writer.Write(currentGameState.Target.Y);
        }
        public void LoadState(in Frame frame, ref readonly BinaryBufferReader reader) 
        {
            /* ... */
            currentGameState.Position1 = reader.ReadVector2();
            currentGameState.Position2 = reader.ReadVector2();
            currentGameState.Score1 = reader.ReadInt32();
            currentGameState.Score2 = reader.ReadInt32();
            currentGameState.Target = reader.ReadVector2();
        }
        public void AdvanceFrame() 
        {
            /* ... */
            nonGameState.Checksum = session.CurrentChecksum;
            //session.SetRandomSeed(currentState.RandomSeed);
            session.SynchronizeInputs();
            var (input1, input2) = (session.GetInput(0), session.GetInput(1));
            gameLogic.Update(ref currentGameState, input1, input2);
            session.AdvanceFrame();
        }
        public void TimeSync(FrameSpan framesAhead) 
        {
            /* ... */
            Debug.WriteLine("> Syncing...");
        }
        public void OnPeerEvent(NetcodePlayer player, PeerEventInfo evt) 
        {
            if (player.IsSpectator())
                return;
            switch (evt.Type)
            {
                case PeerEvent.Connected:
                    nonGameState.RemotePlayerStatus = PlayerStatus.Synchronizing;
                    break;
                case PeerEvent.Synchronizing:
                    nonGameState.SyncProgress =
                        evt.Synchronizing.CurrentStep / (float)evt.Synchronizing.TotalSteps;
                    break;
                case PeerEvent.Synchronized:
                    nonGameState.SyncProgress = 1;
                    break;
                case PeerEvent.ConnectionInterrupted:
                    nonGameState.RemotePlayerStatus = PlayerStatus.Waiting;
                    nonGameState.LostConnectionTime = DateTime.UtcNow;
                    nonGameState.DisconnectTimeout = evt.ConnectionInterrupted.DisconnectTimeout;
                    break;
                case PeerEvent.ConnectionResumed:
                    nonGameState.RemotePlayerStatus = PlayerStatus.Running;
                    break;
                case PeerEvent.Disconnected:
                    nonGameState.RemotePlayerStatus = PlayerStatus.Disconnected;
                    nonGameState.IsRunning = false;
                    break;
            }
        }
    }
}
