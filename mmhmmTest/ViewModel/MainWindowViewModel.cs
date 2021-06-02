using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace mmhmmTest.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand MenuExit { get; set; }

        private ObservableCollection<ViewModelBase> _userspaces;
        public ObservableCollection<ViewModelBase> Userspaces
        {
            get
            {
                if (_userspaces == null)
                {
                    _userspaces = new ObservableCollection<ViewModelBase>();
                }
                return _userspaces;
            }
        }

        public MainWindowViewModel()
        {
            MenuExit = new RelayCommand(ExitMmhmm);
            ShowMainView();
        }

        private void ShowMainView()
        {
            Userspaces.Clear();
            MainViewModel mainSpace = new MainViewModel(Userspaces);
            _userspaces.Add(mainSpace);
            SetActiveWorkspace(mainSpace);
        }

        private void ExitMmhmm(object obj)
        {
            Application.Current.Shutdown();
        }


        public void SetActiveWorkspace(ViewModelBase userspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(_userspaces);
            collectionView?.MoveCurrentTo(userspace);
        }
    }
}
