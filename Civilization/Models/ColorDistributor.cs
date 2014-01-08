using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civilization.Models
{
    class ColorDistributor
    {
        private ISet<Color> colors;

        public ColorDistributor()
        {
            InitializeColors();
        }

        private void InitializeColors()
        {
            colors = new HashSet<Color>();
            colors.Add(new Color(0, 204, 255));
            colors.Add(new Color(255, 255, 0));
            colors.Add(new Color(153, 77, 204));
            colors.Add(new Color(104, 0, 0));
            colors.Add(new Color(0, 51, 255));
            colors.Add(new Color(32, 128, 32));
            colors.Add(new Color(32, 255, 32));
            colors.Add(new Color(139, 101, 8));
            colors.Add(new Color(238, 130, 238));
            colors.Add(new Color(238, 173, 14));
            colors.Add(new Color(78, 84, 82));
        }

        public Color GetNextColor()
        {
            if (colors.Count == 0)
                return new Color(MainModel.Instance.Random.NextFloat(0.0f, 1.0f), MainModel.Instance.Random.NextFloat(0.0f, 1.0f), MainModel.Instance.Random.NextFloat(0.0f, 1.0f));
            Color color = colors.First();
            colors.Remove(color);
            return color;
        }

        public void AddColor(Color color)
        {
            colors.Add(color);
        }
    }
}
