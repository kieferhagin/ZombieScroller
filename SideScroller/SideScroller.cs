using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SideScroller.Components;

namespace SideScroller
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SideScroller : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteManager spriteManager;

        public static Point EXPECTED_SIZE = new Point(1024, 768);
        public enum GameStates
        {
            MainMenu,
            Game,
            GameEditMode
        }

        public GameStates GameState { get; set; }

        public SideScroller()
        {
            this.GameState = GameStates.GameEditMode;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = EXPECTED_SIZE.X;
            graphics.PreferredBackBufferHeight = EXPECTED_SIZE.Y;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            UserControlledSprite player =
                new UserControlledSprite(
                    Content.Load<Texture2D>(@"images/soldier_machinegun"),
                    Content.Load<Texture2D>(@"images/bullet"),
                    Vector2.Zero,
                    new Point(48, 64),
                    0,
                    75,
                    new Vector2(5, 5), 0, null
                    );

            spriteManager = new SpriteManager(this, new PanningCamera(player, new Point(Window.ClientBounds.Width, Window.ClientBounds.Height)), player);
            spriteManager.Initialize();
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            

            

            BackgroundSprite bg = new BackgroundSprite(Content.Load<Texture2D>(@"Images/test_bg"),
                new Point(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), spriteManager.Player, spriteManager);
            bg.Animated = false;

            //PlatformSprite platform = new PlatformSprite(Content.Load<Texture2D>(@"Images/platform_1"), 
            //    new Vector2(500, 736), new Point(32, 32), 0, new Point(1, 1), 0, Vector2.Zero, spriteManager);
            
            
            spriteManager.addSprite(bg);
            //spriteManager.addSprite(platform);
            this.Components.Add(spriteManager);

            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
