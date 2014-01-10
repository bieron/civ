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
        private int civsCount;
        private int simulationSpeed;
        private readonly ICommand startStopCommand;
        private bool isNotRunning;

        public Scene Game
        {
            get { return game; }
            set
            {
                game = value;
                RaisePropertyChanged("Game");
            }
        }

        public String ToggleStartStop
        {
            get { return toggleStartStop; }
            set
            {
                toggleStartStop = value;
                RaisePropertyChanged("ToggleStartStop");
            }
        }

        public int CivsCount
        {
            get { return civsCount; }
            set
            {
                civsCount = value;
                RaisePropertyChanged("CivsCount");
            }
        }

        public int SimulationSpeed
        {
            get { return simulationSpeed; }
            set
            {
                simulationSpeed = value;
                RaisePropertyChanged("SimulationSpeed");
            }
        }

        public ICommand StartStop
        {
            get { return startStopCommand; }
        }

        public bool IsNotRunning
        {
            get { return isNotRunning; }
            set
            {
                isNotRunning = value;
                RaisePropertyChanged("IsNotRunning");
            }
        }

        public GuiViewModel()
        {
            isNotRunning = true;
            ToggleStartStop = "Start";
            SimulationSpeed = 50;
            CivsCount = 2;
            startStopCommand = new RelayCommand<object>(delegate
            {
                if (toggleStartStop.Equals("Start"))
                {
                    ToggleStartStop = "Stop";
                    game.Start();
                    IsNotRunning = false;
                }
                else
                {
                    ToggleStartStop = "Start";
                    game.Stop();
                    IsNotRunning = true;
                }

            });
        }
    }
}