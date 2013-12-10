using System;

using SharpDX;
using SharpDX.Toolkit;

namespace Civilization.ViewModels
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;
    using Civilization.Models;

    /// <summary>
    /// Simple MiniCube application using SharpDX.Toolkit.
    /// The purpose of this application is to show a rotating cube using <see cref="BasicEffect"/>.
    /// </summary>
    public class Scene : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private Texture2D texture;
        private Texture2D savedTexture;
        private VertexInputLayout inputLayout;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scene" /> class.
        /// </summary>
        public Scene()
        {
            // Creates a graphics manager. This is mandatory.
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
            Window.Title = "MiniCube demo";

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            MainModel.getInstance().getBoard().tick();
            Image img = savedTexture.GetDataAsImage();
            Cell[][] cells = MainModel.getInstance().getBoard().getCells();
            for (int i = 0; i < img.PixelBuffer[0].Width; i++)
            {
                //System.Console.WriteLine("Starting column " + i);
                for (int j = 0; j < img.PixelBuffer[0].Height; j++)
                {
                    if (cells[i][j].isReachable()) {
                        if (cells[i][j].getOwner() != null) {
                            bool bInternal = true;
                            foreach (Cell theCell in cells[i][j].getNeighbours())
                                if (theCell.getOwner() != cells[i][j].getOwner()||!theCell.isReachable()) {
                                    bInternal = false;
                                    break;
                                }
                            if(bInternal)
                                img.PixelBuffer[0].SetPixel<Color>(i, j, cells[i][j].getOwner().getColor());
                            else
                                img.PixelBuffer[0].SetPixel<Color>(i, j, new Color(255,0,0));

                        }
                    }
                }
            }
            if(texture!=null)
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
