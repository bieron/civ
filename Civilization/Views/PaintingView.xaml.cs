using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpDX.Toolkit;

namespace Civilization.Views
{
    /// <summary>
    /// Interaction logic for PaintingView.xaml
    /// </summary>
    public partial class PaintingView : UserControl
    {
        public static readonly DependencyProperty GameProperty = DependencyProperty
            .Register("Game", typeof (Game), typeof (PaintingView), new FrameworkPropertyMetadata(HandleGameChanged));

        private static void HandleGameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as PaintingView;
            if (view == null) return;

            var newGame = e.NewValue as Game;

            view.Content = null; // old element will be unloaded and will disable the game

            if (newGame != null)
            {
                // new element will reactivate the game on loading
                var element = new SharpDXElement
                {
                    SendResizeToGame = true,
                    SendResizeDelay = TimeSpan.FromSeconds(1),
                    LowPriorityRendering = false
                };
                view.Content = element;

                if (newGame.IsRunning)
                    newGame.Switch(element);
                else
                    newGame.Run(element);
            }
        }

        public PaintingView()
        {
            InitializeComponent();
        }

        //private Game game;

        public Game Game
        {
            get { return (Game) GetValue(GameProperty); }
            set { SetValue(GameProperty, value); }
        }
    }
}