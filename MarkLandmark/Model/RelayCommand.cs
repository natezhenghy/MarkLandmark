using System;
using System.Windows.Input;

namespace MarkLandmark
{
    public class RelayCommand : ICommand
    {
        private Action action;
        private Action<bool> onPreviousFolder;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action a)
        {
            action = a;
        }

        public RelayCommand(Action<bool> onPreviousFolder)
        {
            this.onPreviousFolder = onPreviousFolder;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private Action<T> action;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<T> a)
        {
            action = a;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action((T)parameter);
        }
    }
}
