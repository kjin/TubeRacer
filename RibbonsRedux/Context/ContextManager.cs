using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RibbonsRedux.Content;
using RibbonsRedux.Graphics;
using RibbonsRedux.Audio;
using RibbonsRedux.Input;
using RibbonsRedux.Storage;

namespace RibbonsRedux.Context
{
    /// <summary>
    /// The game engine. Provides top-level context control over the game.
    /// </summary>
    public class ContextManager
    {
        #region Fields
        //Graphics and Audio
        GraphicsDeviceManager graphics;
        ContentManager content;
        GraphicsDevice graphicsDevice;

        AssetManager assets;
        Canvas canvas;
        AudioPlayer audioPlayer;
        //Input Controller
        InputController inputController;
        //File Manager
        DataCenter fileManager;
        //Current running context
        public GameContext currentContext;
        float currentOverlayAlpha;
        float targetOverlayAlpha;
        bool exitGame;
        //async stuff
        bool asyncStarted;
        bool asyncFinished;
        #endregion

        #region Properties
        public bool Exit { get { return exitGame; } }
        #endregion

        public ContextManager(GraphicsDeviceManager graphics, ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.graphics = graphics;
            this.content = content;
            this.graphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Should be called in load content.
        /// </summary>
        public void Initialize(GameContext initialContext)
        {
            canvas = new Canvas(graphics, graphicsDevice);
#if DEBUG
            canvas.DebugMode = true;
#else
            canvas.DebugMode = false;
#endif
            // Initialize audio player.
            audioPlayer = new AudioPlayer();
            assets = new AssetManager();
            exitGame = false;

            //launch initialize asynchronously
            //ThreadPool.QueueUserWorkItem(new WaitCallback(InitializeNextContext));
            asyncFinished = true;
            /*Thread t = new Thread(new ThreadStart(InitializeNextContext));
            t.IsBackground = true;
            t.Start();*/

            // load all content
            assets.LoadContent(content, graphicsDevice);
            canvas.LoadContent(assets);
            inputController = new InputController(assets);
            fileManager = new DataCenter(assets);
            InitializeContextComponents(initialContext);
            currentContext = initialContext;

            inputController.Update();

            currentOverlayAlpha = 0;
        }

        void InitializeContextComponents(GameContext gameContext)
        {
            if (gameContext == null)
                return;
            gameContext.AssetManager = assets;
            gameContext.AudioPlayer = audioPlayer;
            gameContext.Canvas = canvas;
            gameContext.DataCenter = fileManager;
            gameContext.InputController = inputController;
            gameContext.Initialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            currentContext.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            // Get input from the player
            inputController.Update();
            // screencaps
            if (inputController.ScreenCap.JustPressed)
            {
                Draw(gameTime);
                Texture2D texture = new Texture2D(graphicsDevice, GraphicsConstants.VIEWPORT_WIDTH, GraphicsConstants.VIEWPORT_HEIGHT);
                Color[] data = new Color[texture.Width * texture.Height];
                graphicsDevice.GetBackBufferData<Color>(data);
                texture.SetData<Color>(data);
                Stream stream = File.OpenWrite("output.png");
                texture.SaveAsPng(stream, texture.Width, texture.Height);
                stream.Dispose();
                texture.Dispose();
            }

            if (currentContext.Exit || currentContext.NextContext != null)
            {
                targetOverlayAlpha = 1;
                if (currentOverlayAlpha == 1)
                {
                    //audioPlayer.StopSong();
                    if (currentContext.Exit)
                        exitGame = true;
                    else if (!asyncStarted)
                    {
                        asyncStarted = true;
                        asyncFinished = false;
                        InitializeContextComponents(currentContext.NextContext);
                        asyncFinished = true;
                    }
                    if (asyncStarted && asyncFinished)
                    {
                        //Thread.Sleep(1000);
                        asyncStarted = false;
                        targetOverlayAlpha = 0;
                        currentContext.Dispose();
                        currentContext = currentContext.NextContext;
                    }
                }
            }
            else
            {
                currentContext.Update(gameTime);
            }

            currentOverlayAlpha = MathHelper.Lerp(currentOverlayAlpha, targetOverlayAlpha, currentContext.FadeMultiplier);
            //Console.WriteLine(Math.Abs(currentOverlayAlpha - targetOverlayAlpha));
            if (Math.Abs(currentOverlayAlpha - targetOverlayAlpha) < 0.001f || inputController.Zoom.Pressed)
                currentOverlayAlpha = targetOverlayAlpha;
        }

        void InitializeNextContext()
        {
            while (true)
            {
                Thread.Sleep(100);
                if (asyncFinished) continue;
                Console.WriteLine("Context loading started...");
                InitializeContextComponents(currentContext.NextContext);
                asyncFinished = true;
                Console.WriteLine("Context loading finished.");
            }
        }

        /// <summary>
        /// This is called when the game should play sounds.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void PlayAudio(GameTime gameTime)
        {
            audioPlayer.Update(gameTime);
            currentContext.PlayAudio(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(ClearOptions.Stencil | ClearOptions.Target, currentContext.BackgroundColor, 0, 0);
            canvas.BeginDraw();
            currentContext.Draw(gameTime);
            canvas.DrawRectangle(Color.Transparent, new Color(0f, 0f, 0f, currentOverlayAlpha), 0, canvas.ActiveCamera.Dimensions / 2f, Anchor.Center, canvas.ActiveCamera.Dimensions, 0, false);
            if (currentOverlayAlpha == 1)
            {
                // These are things that are drawn when the game is loading
                //canvas.DrawString("LOADING", Color.White, canvas.Camera.Dimensions / 2, Anchor.Center);
            }
            canvas.EndDraw();
            //check if cameras exist that weren't popped
            if (canvas.ResetCameraStack())
                Console.WriteLine("Warning: Not all cameras were removed from the stack. Resetting...");
        }
    }
}
