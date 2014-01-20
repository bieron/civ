using System;
using System.Windows.Input;
using Civilization.Models;
using MvvmSupport;

namespace Civilization.ViewModels
{
    public class GuiViewModel : ObservableObject
    {
        private Scene game = new Scene();

        private int civsCount;
        private string toggleStartEnd;
        private readonly ICommand startEndCommand;
        private string togglePauseResume;
        private readonly ICommand pauseResumeCommand;
        private readonly ICommand resetCommand;
        private bool isNotRunning;
        private bool isNotPaused;
        private readonly ICommand capitalsCommand;
        private readonly ICommand bordersCommand;
        private readonly ICommand territoryCommand;
        private readonly ICommand splitsCommand;
        private bool paintCapitals = true;
        private bool paintBorders = true;
        private bool paintTerritory = true;
        private bool doSplits = true;

        public bool PaintCapitals
        {
            get { return paintCapitals; }
            set
            {
                paintCapitals = value;
                RaisePropertyChanged("PaintCapitals");
            }
        }

        public bool PaintBorders
        {
            get { return paintBorders; }
            set
            {
                paintBorders = value;
                RaisePropertyChanged("PaintBorders");
            }
        }

        public bool PaintTerritory
        {
            get { return paintTerritory; }
            set
            {
                paintTerritory = value;
                RaisePropertyChanged("PaintTerritory");
            }
        }

        public bool DoSplits
        {
            get { return doSplits; }
            set
            {
                doSplits = value;
                RaisePropertyChanged("DoSplits");
            }
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

        public int CivsCount
        {
            get { return civsCount; }
            set
            {
                civsCount = value;
                RaisePropertyChanged("CivsCount");
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

        public int ActiveCivsCount
        {
            get { return MainModel.Instance.AliveCivilizationsCount; }
        }

        public long TicksCount
        {
            get { return MainModel.Instance.TicksCount; }
        }

        private void GvmTickEvent(object sender, EventArgs args)
        {
            RaisePropertyChanged("TicksCount");
            RaisePropertyChanged("ActiveCivsCount");
        }

        public GuiViewModel()
        {
            isNotRunning = true;
            isNotPaused = true;
            CivsCount = 2;

            TogglePauseResume = "Pause";
            ToggleStartEnd = "Start";

            startEndCommand = new RelayCommand<object>(StartEnd);
            pauseResumeCommand = new RelayCommand<object>(PauseResume);
            resetCommand = new RelayCommand<object>(Reset);
            capitalsCommand = new RelayCommand<object>(ToggleCapitals);
            bordersCommand = new RelayCommand<object>(ToggleBorders);
            territoryCommand = new RelayCommand<object>(ToggleTerritory);
            splitsCommand = new RelayCommand<object>(ToggleSplits);
            MainModel.Instance.TickEvent += GvmTickEvent;
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

        public ICommand CapitalsCommand
        {
            get { return capitalsCommand; }
        }

        private void ToggleCapitals(object obj)
        {
            game.SetPaintCapitals(paintCapitals);
        }

        public ICommand BordersCommand
        {
            get { return bordersCommand; }
        }

        private void ToggleBorders(object obj)
        {
            game.SetPaintBorders(paintBorders);
        }

        public ICommand TerritoryCommand
        {
            get { return territoryCommand; }
        }

        private void ToggleTerritory(object obj)
        {
            game.SetPaintTerritory(paintTerritory);
        }

        public ICommand SplitsCommand
        {
            get { return splitsCommand; }
        }

        private void ToggleSplits(object obj)
        {
            MainModel.Instance.DoSplits = doSplits;
        }
    }
}