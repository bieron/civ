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
                    if (cells[i][j].IsReachable) {
                        if (cells[i][j].Owner != null)
                        {
                            bool bInternal = true;
                            foreach (Cell theCell in cells[i][j].Neighbors)
                                if (theCell.Owner != cells[i][j].Owner || !theCell.IsReachable)
                                {
                                    bInternal = false;
                                    break;
                                }
                            if (bInternal)
                                img.PixelBuffer[0].SetPixel<Color>(i, j, cells[i][j].Owner.Color);
                            else
                                img.PixelBuffer[0].SetPixel<Color>(i, j, new Color(255, 0, 0));
                        }

                    }
                }
            }
            paintCapitals(img);
            if (texture != null)
                texture.Dispose();
            texture = Texture2D.New(GraphicsDevice, img);
            img.Dispose();
            // Handle base.Update
            base.Update(gameTime);
        }

        protected void paintCapitals(Image img)
        {
            foreach (Civ empire in MainModel.Instance.Civilizations)
            {
                int x = empire.Capital.X;
                int y = empire.Capital.Y;
                for(int i=x-3;i<=x+3;i++)
                    for(int j=y-3;j<=y+3;j++)
                        if(isInBounds(img, i, j)) 
                        {
                            if(i>x-2&&i<x+2&&j>y-2&&j<y+2)
                                img.PixelBuffer[0].SetPixel<Color>(i, j, new Color(255, 255, 255));
                            else
                                img.PixelBuffer[0].SetPixel<Color>(i, j, new Color(0, 0, 255));
                        }
                img.PixelBuffer[0].SetPixel<Color>(x, y, new Color(0, 0, 255));
            }
        }

        protected bool isInBounds(Image img, int i, int j)
        {
            if (i < 0 || j < 0 || i > img.PixelBuffer[0].Width - 1 || i > img.PixelBuffer[0].Height - 1)
                return false;
            return true;
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Deferred, null);
            spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            //spriteBatch.Draw(texture, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.End();

            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
