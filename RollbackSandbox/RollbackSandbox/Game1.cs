using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Backdash;
using Backdash.Core;
using Backdash.Serialization;
using System;
using System.Net;
using System.Diagnostics;


namespace RollbackSandbox
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
    

        SpriteFont font1;
        Vector2 fontPos;

        TestGame game;

        Texture2D spriteTexture;

        public Game1(ref INetcodeSession<GameInput> session)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            game = new TestGame(session);
            session.SetHandler(game);
            session.Start();

            Console.WriteLine("Started");

            IsMouseVisible = true;

        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font1 = Content.Load<SpriteFont>("MyMenuFont");
            Viewport viewport = GraphicsDevice.Viewport;

            fontPos = new Vector2(viewport.Width / 2, viewport.Height / 2);

            spriteTexture = Content.Load<Texture2D>("onion");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            game.UpdateGame();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            string text = "Hello";
            if (game.nonGameState.IsRunning) { text = "Conntected"; }
            else { text = "Not Conntected (Nicht schlimm :))"; }

            
            Vector2 FontOrigin = font1.MeasureString(text) / 2;
            _spriteBatch.DrawString(font1, text, fontPos, Color.LightGreen,
            0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            game.DrawGame(spriteTexture, _spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            spriteTexture.Dispose();
            base.Dispose(disposing);
        }
    }
}
