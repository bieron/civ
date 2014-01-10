using System;
using System.Windows.Input;
using MvvmSupport;
using SharpDX.Toolkit;

namespace Civilization.ViewModels
{
    public class GuiViewModel : ObservableObject
    {
        private Scene game = new Scene();

        private string toggleStartStop;
        private readonly ICommand startStopCommand;

        public String ToggleStartStop
        {
            get { return toggleStartStop; }
            set
            {
                toggleStartStop = value;
                RaisePropertyChanged("ToggleStartStop");
            }
        }

        public ICommand StartStop
        {
            get { return startStopCommand; }
        }

        public Scene Game
        {
            get { return game; }
            set
            {
                game = value;
                RaisePropertyChanged("Game");
            }
        }

        public GuiViewModel()
        {
            toggleStartStop = "Start";
            startStopCommand = new RelayCommand<object>(delegate
            {
                if (toggleStartStop.Equals("Start"))
                {
                    ToggleStartStop = "Stop";
                    game.Start();
                }
                else
                {
                    ToggleStartStop = "Start";
                    game.Stop();
                }

            });
        }
    }
}