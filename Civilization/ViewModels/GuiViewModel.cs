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
        private readonly ICommand startEndCommand;
        private string togglePauseResume;
        private readonly ICommand pauseResumeCommand;
        private readonly ICommand resetCommand;
        private bool isNotRunning;
        private bool isNotPaused;

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
            get { return startEndCommand; }
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

        public bool IsNotPaused
        {
            get { return isNotPaused; }
            set
            {
                isNotPaused = value;
                RaisePropertyChanged("IsNotPaused");
            }
        }

        public GuiViewModel()
        {
            isNotRunning = true;
            isNotPaused = true;
            SimulationSpeed = 50;
            CivsCount = 2;

            TogglePauseResume = "Pause";
            ToggleStartEnd = "Start";

            startEndCommand = new RelayCommand<object>(StartEnd);
            pauseResumeCommand = new RelayCommand<object>(PauseResume);
            resetCommand = new RelayCommand<object>(Reset);
        }

        private void StartEnd(object obj)
        {
            if (toggleStartEnd.Equals("Start"))
            {
                ToggleStartEnd = "Stop";
                IsRunning = true;
                MainModel.Instance.Start(civsCount);
                game.RunSimulation();
            }
            else
            {
                ToggleStartEnd = "Start";
                Reset(obj);
            }
        }

        private void PauseResume(object obj)
        {
            if (togglePauseResume.Equals("Pause"))
            {
                IsNotPaused = false;
                TogglePauseResume = "Resume";
                game.StopSimulation();
            }
            else
            {
                IsNotPaused = true;
                TogglePauseResume = "Pause";
                game.RunSimulation();
            }
        }

        private void Reset(object obj)
        {
            IsRunning = false;
            game.StopSimulation();
            MainModel.Instance.Reset();
        }
    }
}