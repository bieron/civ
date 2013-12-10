using System;
using System.Windows.Input;
using MvvmSupport;
using SharpDX.Toolkit;

namespace Civilization.ViewModels
{
    public class GuiViewModel : ObservableObject
    {
        private readonly ICommand _sendCommand = new RelayCommand<object>(delegate { Console.WriteLine("przycisk"); });
        private Game _game = new Scene();

        public ICommand Send
        {
            get { return _sendCommand; }
        }

        public Game Game
        {
            get { return _game; }
            set
            {
                _game = value;
                RaisePropertyChanged("Game");
            }
        }

        public string Text
        {
            get { return "cos"; }
        }
    }
}