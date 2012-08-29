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
using SideScroller.Components.Physics;
using System.Xml;


namespace SideScroller.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Camera2D Camera { get; protected set; }
        public UserControlledSprite Player { get; protected set; }
        public List<Sprite> spriteList { get; set; }
        private SpriteBatch spriteBatch;
        private List<Sprite> removals = new List<Sprite>();
        public SpriteFont debugFont, gui1Font;

        private int lastScroll = 0;
        public List<Texture2D> platforms = new List<Texture2D>();
        private int curPlatform = 0;

        public List<Sprite> liveSprites = new List<Sprite>();
        private int curLiveSprite = 0;

        private Texture2D halfGUI, quarterGUI, healthBarBg, healthBar, colaSolo;
        public ContentManager contentManager { get; set; }
        public SoundBank soundBank { get; set; }
        private WaveBank waveBank;
        private AudioEngine audioEngine;

        public Cue currentSong;

        private LevelManager levelManager;

        private enum EditMode
        {
            Platform,
            Sprite
        }

        private List<Sprite> restoreList;

        private EditMode editMode = EditMode.Platform;

        public SpriteManager() : base(new Game()) { }

        public SpriteManager(Game game, Camera2D camera, UserControlledSprite player)
            : base(game)
        {
            this.Camera = camera;
            this.spriteList = new List<Sprite>();
            this.Player = player;
            this.Player.Manager = this;
            this.contentManager = new ContentManager(game);
            spriteBatch = new SpriteBatch(
                Game.GraphicsDevice);
            this.levelManager = new LevelManager(this);
            this.restoreList = new List<Sprite>();
        }

        public void LoadLevel(int index)
        {
            levelManager.CurLevelIndex = index;
        }

        public Sprite SpriteFromXML(XmlNode node)
        {
            Sprite s = null;
            switch (node.Name)
            {
                case "ZombieSprite":
                    s = getSpriteAdd(liveSprites[0]);
                    break;
                case "Platform":
                    s = new PlatformSprite(platforms[0], Vector2.Zero, new Point(1, 1), 0, 0, Vector2.Zero, this);
                    break;
                case "ZombieCola":
                    s = getSpriteAdd(liveSprites[1]);
                    break;
                case "HealthPack":
                    s = getSpriteAdd(liveSprites[2]);
                    break;
            }

            ((CanSerialize)s).Read((XmlElement)node);
            return s;
        }

        public void addSprite(Sprite sprite)
        {
            spriteList.Add(sprite);
        }

        public void ClearSprites()
        {
            this.spriteList.Clear();
        }

        public void removeSprite(Sprite sprite)
        {
            removals.Add(sprite);
        }

        public Boolean isBlocked(Rectangle r)
        {
            foreach (Sprite s in checkCollisions(r))
            {
                if (s.Blocking)
                    return true;
            }
            return false;
        }

        public List<Sprite> checkCollisions(Rectangle r)
        {
            List<Sprite> list = new List<Sprite>();
            foreach (Sprite sprite in spriteList)
            {
                if (sprite.CanCollide)
                {
                    if (sprite.Bounds.Intersects(Camera.Window))
                    {

                        Rectangle sprB = sprite.Bounds;
                        sprB.Width -= sprite.CollisionOffset * 2;
                        sprB.Height -= sprite.CollisionOffset * 2;
                        sprB.X += sprite.CollisionOffset;
                        sprB.Y -= sprite.CollisionOffset;
                        if (sprB.Intersects(r))
                        {
                            list.Add(sprite);
                        }
                    }
                }
            }
            return list;
        }


        public List<Sprite> checkCollisions(Sprite s)
        {
            Rectangle sprB = s.Bounds;
            sprB.Width -= s.CollisionOffset * 2;
            sprB.Height -= s.CollisionOffset * 2;
            sprB.X += s.CollisionOffset;
            sprB.Y -= s.CollisionOffset;
            List<Sprite> list = this.checkCollisions(sprB);
            list.Remove(s);
            return list;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            debugFont = Game.Content.Load<SpriteFont>(@"Fonts/debug_info");
            gui1Font = Game.Content.Load<SpriteFont>(@"Fonts/gui_1");
            halfGUI = Game.Content.Load<Texture2D>(@"Images/half_gui_box");
            quarterGUI = Game.Content.Load<Texture2D>(@"Images/quarter_gui_box");
            healthBar = Game.Content.Load<Texture2D>(@"Images/healthbar");
            healthBarBg = Game.Content.Load<Texture2D>(@"Images/healthbar_bg");
            colaSolo = contentManager.GetTexture("cola_solo");
            ZombieSprite.healthBarEnemy = Game.Content.Load<Texture2D>(@"Images/healthbar_enemy");

            this.platforms.Add(contentManager.GetPlatformTexture(1, 1, 1));
            this.platforms.Add(contentManager.GetPlatformTexture(1, 5, 1));
            this.platforms.Add(contentManager.GetPlatformTexture(1, 10, 5));

            this.liveSprites.Add(new ZombieSprite(contentManager.GetZombieTexture(1), Vector2.Zero, new Point(64, 78), 0, 200, new Vector2(3, 0), Player.LayerDepth, this));
            this.liveSprites.Add(new ZombieColaSprite(contentManager.GetTexture("cola"), Vector2.Zero, this));
            this.liveSprites.Add(new HealthPackSprite(contentManager.GetTexture("health_pack"), Vector2.Zero, this));

            audioEngine = new AudioEngine(@"Content\Sounds\Sounds.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Sounds\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Sounds\Sound Bank.xsb");

            this.currentSong = soundBank.GetCue("level1");
            base.Initialize();
            this.levelManager.LoadAll();
            this.LoadLevel(0);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            //if (!currentSong.IsPlaying)
              //  currentSong.Play();
            Camera.Update();
            Point windowSize = new Point(Camera.Window.Width, Camera.Window.Height);
            Player.Update(gameTime, windowSize);
            foreach (Sprite s in spriteList)
            {
                if (s.Bounds.Intersects(Camera.Window))
                    s.Update(gameTime, windowSize);
            }

            foreach (Sprite s in removals)
            {
                spriteList.Remove(s);
                if (((SideScroller)this.Game).GameState == SideScroller.GameStates.GameEditMode)
                    restoreList.Add(s);
            }
            removals.Clear();

            if (((SideScroller)Game).GameState == SideScroller.GameStates.GameEditMode)
                DoEdits();

            lastScroll = Mouse.GetState().ScrollWheelValue;
            base.Update(gameTime);
        }

        private void RestoreSprites()
        {
            foreach (Sprite s in restoreList) {
                if (s is HealthSprite)
                    ((HealthSprite)s).Heal();
                this.spriteList.Add(s);
            }
            restoreList.Clear();
        }

        public void PlayerDied()
        {
            
            ((HealthSprite)Player).Heal();
            foreach (Sprite s in spriteList)
            {
                s.Position = s.StartPosition;
            }
            this.RestoreSprites();
            Player.Position = Player.StartPosition;
        }
            

        private void DoEdits()
        {
            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();
            Point camOffset = new Point(ms.X + Camera.Window.X, ms.Y + Camera.Window.Y);

            if ((ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl)) && ks.IsKeyDown(Keys.H))
                ((HealthSprite)Player).Heal();

            if ((ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl)) && ks.IsKeyDown(Keys.R))
            {
                this.PlayerDied();
            }

            if ((ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl)) && ks.IsKeyDown(Keys.S))
            {
                this.RestoreSprites();
                levelManager.Save();
            }

            if ((ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl)) && ks.IsKeyDown(Keys.P))
                editMode = EditMode.Platform;

            if ((ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl)) && ks.IsKeyDown(Keys.O))
                editMode = EditMode.Sprite;


            int scrollDiff = -((ms.ScrollWheelValue - lastScroll) / 120);

            if (scrollDiff != 0)
            {
                if (editMode == EditMode.Platform)
                {
                    if (scrollDiff + curPlatform < 0)
                        curPlatform = 0;
                    else
                    {
                        if (scrollDiff + curPlatform >= platforms.Count)
                            curPlatform = platforms.Count - 1;
                        else
                            curPlatform += scrollDiff;
                    }
                }
                else if (editMode == EditMode.Sprite)
                {
                    if (scrollDiff + curLiveSprite < 0)
                        curLiveSprite = 0;
                    else
                    {
                        if (scrollDiff + curLiveSprite >= liveSprites.Count)
                            curLiveSprite = liveSprites.Count - 1;
                        else
                            curLiveSprite += scrollDiff;
                    }
                }

            }

            if (ms.RightButton == ButtonState.Pressed)
            {

                if (ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl))
                {
                    Player.Position = new Vector2(camOffset.X, camOffset.Y);
                }
                else if (ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift))
                {
                    foreach (Sprite s in this.checkCollisions(new Rectangle(camOffset.X, camOffset.Y, 2, 2)))
                    {
                        this.removals.Add(s);
                    }
                }
                else
                {
                    switch (editMode)
                    {
                        case EditMode.Platform:
                            Texture2D platform = this.platforms[curPlatform];
                            Point offset = camOffset;
                            offset.X = (int)Math.Floor((double)offset.X / (double)platform.Width) * platform.Width;
                            offset.Y = (int)Math.Floor((double)offset.Y / (double)platform.Height) * platform.Height;
                            bool create = true;
                            foreach (Sprite s in this.checkCollisions(new Rectangle(offset.X, offset.Y, platform.Width, platform.Height)))
                            {
                                create = false;
                                break;
                            }

                            if (create)
                                this.addSprite(new PlatformSprite(platform,
                                new Vector2(offset.X, offset.Y), new Point(platform.Width, platform.Height), 0, 0, Vector2.Zero, this));
                            break;
                        case EditMode.Sprite:
                            Point sOffset = camOffset;
                            Point size = this.liveSprites[curLiveSprite].FrameDims;
                            create = true;
                            foreach (Sprite s in this.checkCollisions(new Rectangle(sOffset.X, sOffset.Y, size.X, size.Y)))
                            {
                                create = false;
                                break;
                            }

                            if (create)
                            {
                                this.addSprite(getSpriteAdd(this.liveSprites[curLiveSprite]));
                            }
                            break;
                    }

                }
            }
        }


        private Sprite getSpriteAdd(Sprite spriteAdd)
        {
            MouseState ms = Mouse.GetState();
            Point camOffset = new Point(ms.X + Camera.Window.X, ms.Y + Camera.Window.Y);
            Texture2D sTex = spriteAdd.TextureImage;
            Point sOffset = camOffset;

            Sprite s = null;
            if (spriteAdd is ZombieSprite)
                s = new ZombieSprite(sTex, new Vector2(sOffset.X, sOffset.Y), spriteAdd.FrameDims, spriteAdd.CollisionOffset,
                    spriteAdd.Animation.FrameDelay, spriteAdd.Speed, spriteAdd.LayerDepth, this);
            else if (spriteAdd is ZombieColaSprite)
                s = new ZombieColaSprite(sTex, new Vector2(sOffset.X, sOffset.Y), this);
            else if (spriteAdd is HealthPackSprite)
                s = new HealthPackSprite(sTex, new Vector2(sOffset.X, sOffset.Y), this);


            return s;
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            SideScroller.GameStates state = ((SideScroller)this.Game).GameState;
            switch (state)
            {
                case SideScroller.GameStates.Game:
                    DrawGame(gameTime);
                    DrawGui();
                    break;
                case SideScroller.GameStates.GameEditMode:
                    DrawGame(gameTime);
                    DrawGui();
                    DrawDebug(gameTime);
                    break;

            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public virtual void DrawGame(GameTime gameTime)
        {
            Player.Draw(gameTime, this.spriteBatch, Camera, new Vector2(1, 1));
            foreach (Sprite s in spriteList)
            {
                s.Draw(gameTime, this.spriteBatch, Camera, new Vector2(1, 1));
            }
        }

        public virtual void DrawDebug(GameTime gameTime)
        {
            try
            {
                spriteBatch.DrawString(debugFont, "FPS: " + (1000 / gameTime.ElapsedGameTime.Milliseconds), Vector2.Zero, Color.Red);
                spriteBatch.DrawString(debugFont, "Sprite Count: " + spriteList.Count, new Vector2(0, 18), Color.Red);
                spriteBatch.DrawString(debugFont, "Direction: " + Player.Direction.ToString(), new Vector2(0, 36), Color.Red);
                spriteBatch.DrawString(debugFont, "PlayerPos: " + Player.Position.ToString(), new Vector2(0, 54), Color.Red);
                switch (editMode)
                {
                    case EditMode.Platform:
                        spriteBatch.DrawString(debugFont, "Platform: " + platforms[curPlatform].Name, new Vector2(0, 72), Color.Red);
                        break;
                    case EditMode.Sprite:
                        spriteBatch.DrawString(debugFont, "Sprite: " + liveSprites[curLiveSprite].TextureImage.Name, new Vector2(0, 72), Color.Red);
                        break;
                }

                spriteBatch.DrawString(debugFont, "Level: " + levelManager.CurLevel.Name, new Vector2(0, 90), Color.Red);
                
            }
            catch
            { }
        }

        public virtual void DrawGui()
        {
            int y = Camera.Window.Height - quarterGUI.Bounds.Height;
            int x = 0;
            float health = ((float)((HealthSprite)Player).CurHealth) / ((float)((HealthSprite)Player).MaxHealth);
            spriteBatch.Draw(quarterGUI, new Vector2(x, y), Color.White);
            x += quarterGUI.Bounds.Width;
            spriteBatch.Draw(halfGUI, new Rectangle(x, y, halfGUI.Width, halfGUI.Height), new Rectangle(0, 0, halfGUI.Width, halfGUI.Height),
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.11f);
            spriteBatch.DrawString(gui1Font, "Health:", new Vector2(x + 10, y + 15), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(healthBarBg, new Rectangle(x + 90, y + 17, healthBarBg.Width, healthBarBg.Height), new Rectangle(0, 0, healthBarBg.Width, healthBarBg.Height),
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(healthBar, new Rectangle(x + 91, y + 17, (int)(((float)healthBar.Width) * health), healthBar.Height), new Rectangle(0, 0, (int)(((float)healthBar.Width) * health), healthBar.Height),
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.09f);
            x += halfGUI.Bounds.Width;

            spriteBatch.Draw(quarterGUI, new Rectangle(x, y, quarterGUI.Width, quarterGUI.Height), new Rectangle(0, 0, quarterGUI.Width, quarterGUI.Height),
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.11f);
            spriteBatch.Draw(quarterGUI, new Rectangle(x, 0, quarterGUI.Width, quarterGUI.Height), new Rectangle(0, 0, quarterGUI.Width, quarterGUI.Height),
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.11f);
            spriteBatch.Draw(colaSolo, new Rectangle(x + 15, y + 15, colaSolo.Width, colaSolo.Height), new Rectangle(0, 0, colaSolo.Width, colaSolo.Height),
                Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.09f);
            spriteBatch.DrawString(gui1Font, "XP: " + Player.TotalXP, new Vector2(x + 20, 15), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(gui1Font, " x " + Player.TotalCola.ToString(), new Vector2(x + 40, y + 15), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);


        }

        protected override void LoadContent()
        {

        }
    }
}
