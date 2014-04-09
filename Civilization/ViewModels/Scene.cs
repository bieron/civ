//#define DESIGN_MODE
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
        private Texture2D terrainTexture, desTexture, defTexture;
        //private VertexInputLayout inputLayout;
        private bool runSimulation = false;
        private bool bPaintBorders = true;
        private bool bPaintTerritory = true;
        private bool bPaintCapitals = true;
        private string usedTextureString = "Teren";

        public Scene()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";
        }

        public string UsedTextureString
        {
            set { usedTextureString = value; }
        }

        public void SetPaintBorders(bool val)
        {
            bPaintBorders = val;
        }

        public void SetPaintTerritory(bool val)
        {
            bPaintTerritory = val;
        }

        public void SetPaintCapitals(bool val)
        {
            bPaintCapitals = val;
        }

        public void RunSimulation()
        {
            runSimulation = true;
        }

        public void StopSimulation()
        {
            runSimulation = false;
        }
        
#if (DESIGN_MODE != true)
        protected override void LoadContent()
        {
            terrainTexture = Texture2D.Load(GraphicsDevice, @"..\..\Resources\" + MainModel.Instance.GameBoard.MapTitle + @"\terrain.bmp");
            desTexture = Texture2D.Load(GraphicsDevice, @"..\..\Resources\" + MainModel.Instance.GameBoard.MapTitle + @"\desirability.bmp");
            defTexture = Texture2D.Load(GraphicsDevice, @"..\..\Resources\" + MainModel.Instance.GameBoard.MapTitle + @"\defensibility.bmp");
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (!runSimulation)
                return;

            for (int i=0;i<MainModel.Instance.DrawSpeed;i++)
                MainModel.Instance.GameBoard.Tick();

            Image img;
            if(usedTextureString=="Teren")
                img = terrainTexture.GetDataAsImage();
            else if (usedTextureString == "Desirability")
                img = desTexture.GetDataAsImage();
            else
                img = defTexture.GetDataAsImage();
            PaintCountries(img);
            if(bPaintCapitals)
                PaintCapitals(img);

            if (texture != null)
                texture.Dispose();
            texture = Texture2D.New(GraphicsDevice, img);
            img.Dispose();
            // Handle base.Update
            base.Update(gameTime);
        }

        private void PaintCountries(Image img)
        {
            Cell[][] cells = MainModel.Instance.GameBoard.Cells;
            Color red = new Color(255,0,0);
            Color blue = new Color(0,0,255);
            for (int i = 0; i < img.PixelBuffer[0].Width; i++)
            {
                for (int j = 0; j < img.PixelBuffer[0].Height; j++)
                {
                    if (!cells[i][j].IsReachable || cells[i][j].Owner == null) continue;
                    bool bInternal = true;
                    foreach (Cell theCell in cells[i][j].Neighbors)
                        if (theCell.Owner != cells[i][j].Owner || !theCell.IsReachable)
                        {
                            bInternal = false;
                            break;
                        }
                    if (bPaintTerritory && (bInternal || !bPaintBorders)) {
                        //img.PixelBuffer[0].SetPixel(i, j, cells[i][j].Owner.Color);
                           img.PixelBuffer[0].SetPixel(i, j, cells[i][j].IsStable? blue : cells[i][j].Owner.Color);

                    }  else if (bPaintBorders && !bInternal)
                        img.PixelBuffer[0].SetPixel(i, j, red);
                }
            }
        }

        protected void PaintCapitals(Image img)
        {
            foreach (Civ empire in MainModel.Instance.Civilizations)
            {
                int x = empire.Capital.X;
                int y = empire.Capital.Y;
                for(int i = x-3; i <= x+3; i++)
                    for(int j = y-3; j <= y+3; j++)
                        if(IsInBounds(img, i, j)) 
                        {
                            if(i > x-2 && i < x+2 && j > y-2 && j < y+2)
                                img.PixelBuffer[0].SetPixel(i, j, new Color(255, 255, 255));
                            else
                                img.PixelBuffer[0].SetPixel(i, j, new Color(0, 0, 255));
                        }
                img.PixelBuffer[0].SetPixel(x, y, new Color(0, 0, 255));
            }
        }

        protected bool IsInBounds(Image img, int i, int j)
        {
            if (i < 0 || j < 0 || i > img.PixelBuffer[0].Width - 1 || j > img.PixelBuffer[0].Height - 1)
                return false;
            return true;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!runSimulation)
                return;

            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Handle base.Draw
            base.Draw(gameTime);
        }
#endif
    }
}
