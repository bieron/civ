using System;
using SharpDX;
using SharpDX.Toolkit;

namespace Civilization.ViewModels
{
    using SharpDX.Toolkit.Graphics;
    using Civilization.Models;

    public class Scene : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private Texture2D texture;
        private Texture2D savedTexture;
        private VertexInputLayout inputLayout;

        public Scene()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            savedTexture = Texture2D.Load(GraphicsDevice, @"..\..\Resources\EgyptMap\terrain.bmp");
            base.LoadContent();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            MainModel.Instance.GameBoard.Tick();
            Image img = savedTexture.GetDataAsImage();
            Cell[][] cells = MainModel.Instance.GameBoard.Cells;
            for (int i = 0; i < img.PixelBuffer[0].Width; i++)
            {
                //System.Console.WriteLine("Starting column " + i);
                for (int j = 0; j < img.PixelBuffer[0].Height; j++)
                {
                    if (cells[i][j].IsReachable)
                    {
                        if (cells[i][j].Owner != null)
                            img.PixelBuffer[0].SetPixel<Color>(i, j, cells[i][j].Owner.Color);
                    }
                }
            }
            if (texture != null)
                texture.Dispose();
            texture = Texture2D.New(GraphicsDevice, img);
            img.Dispose();
            // Handle base.Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Deferred, null);
            spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}