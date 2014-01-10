using System;
using System.Windows.Input;
using Civilization.Models;
using MvvmSupport;
using SharpDX.Toolkit;

namespace Civilization.ViewModels
{
    public class GuiViewModel : ObservableObject
    {
        private Scene game = new Scene();

        private int civsCount;
        private int simulationSpeed;
        private string toggleStartEnd;
        private readonly ICommand startEndCommandCommand;
        private string togglePauseResume;
        private readonly ICommand pauseResumeCommand;
        private readonly ICommand resetCommand;
        private bool isNotRunning;
        private bool isPaused;

        public Scene Game
        {
            get { return game; }
            set
            {
                game = value;
                RaisePropertyChanged("Game");
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

        public string ToggleStartEnd
        {
            get { return toggleStartEnd; }
            set
            {
                toggleStartEnd = value;
                RaisePropertyChanged("ToggleStartEnd");
            }
        }

        public ICommand StartEndCommand
        {
            get { return startEndCommandCommand; }
        }

        public String TogglePauseResume
        {
            get { return togglePauseResume; }
            set
            {
                togglePauseResume = value;
                RaisePropertyChanged("TogglePauseResume");
            }
        }

        public ICommand PauseResumeCommand
        {
            get { return pauseResumeCommand; }
        }

        public ICommand ResetCommand
        {
            get { return resetCommand; }
        }

        public bool IsNotRunning
        {
            get { return isNotRunning; }
            set
            {
                isNotRunning = value;
                RaisePropertyChanged("IsNotRunning");
                RaisePropertyChanged("IsRunning");
            }
        }
        public bool IsRunning
        {
            get { return !isNotRunning; }
            set
            {
                isNotRunning = !value;
                RaisePropertyChanged("IsNotRunning");
                RaisePropertyChanged("IsRunning");
            }
        }

        public GuiViewModel()
        {
            isNotRunning = true;
            TogglePauseResume = "Pause";
            SimulationSpeed = 50;
            CivsCount = 2;
            startEndCommandCommand = new RelayCommand<object>(delegate
            {
                if (toggleStartEnd.Equals("Start"))
                {
                    IsRunning = true;
                    MainModel.Instance.Start(civsCount);
                    game.Start();
                }
                else
                {
                    IsRunning = false;
                    MainModel.Instance.Reset();
                }
            });
            pauseResumeCommand = new RelayCommand<object>(delegate
            {
                if (togglePauseResume.Equals("Pause"))
                {
                    TogglePauseResume = "Resume";
                    game.Stop();
                    isPaused = true;
                }
                else
                {
                    TogglePauseResume = "Pause";
                    game.Start();
                    isPaused = false;
                }
            });
            resetCommand = new RelayCommand<object>(delegate
            {
                IsRunning = false;
                MainModel.Instance.Reset();
            });
        }
    }
}