using mmhmmTest.Model;
using mmhmmTest.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Timers;
using System.Windows;

namespace mmhmmTest.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        //public ICommand BtnPreview { get; set; }
        //public ICommand BtnRecord { get; set; }

        //private string _recordButtonName;
        //public string RecordButtonName
        //{
        //    get => _recordButtonName;
        //    set
        //    {
        //        if (_recordButtonName == value) return;
        //        _recordButtonName = value;
        //        OnPropertyChanged("RecordButtonName");
        //    }
        //}

        //private string _previewButtonName;
        //public string PreviewButtonName
        //{
        //    get => _previewButtonName;
        //    set
        //    {
        //        if (_previewButtonName == value) return;
        //        _previewButtonName = value;
        //        OnPropertyChanged("PreviewButtonName");
        //    }
        //}

        //private BitmapSource _videoPreview;
        //public BitmapSource VideoPreview
        //{
        //    get => _videoPreview;
        //    set
        //    {
        //        _videoPreview = value;
        //        OnPropertyChanged("VideoPreview");
        //    }
        //}

        //private BitmapSource _videoImage;
        //public BitmapSource VideoImage
        //{
        //    get => _videoImage;
        //    set
        //    {
        //        if (_videoImage == value) return;
        //        _videoImage = value;
        //        OnPropertyChanged("VideoImage");
        //    }
        //}

        private CameraModel _selectedCamera;
        public CameraModel SelectedCamera
        {
            get => _selectedCamera;
            set
            {
                if (_selectedCamera == value) return;
                _selectedCamera = value;
                OnPropertyChanged("SelectedCamera");
            }
        }

        private ObservableCollection<CameraModel> _cameraList;
        public ObservableCollection<CameraModel> CameraList
        {
            get => _cameraList;
            set
            {
                if (_cameraList == value) return;
                _cameraList = value;
                OnPropertyChanged("CameraList");
            }
        }

        //private VideoStreaming _videoStream;

        private ObservableCollection<ViewModelBase> _userspaces;
        public MainViewModel(ObservableCollection<ViewModelBase> userspaces)
        {
            _userspaces = userspaces;
            //PreviewButtonName = "Start";
            //RecordButtonName = "Record";
            //BtnPreview = new RelayCommand(PreviewVideo);
            //BtnRecord = new RelayCommand(RecordVideo);
            BuildCameraList();
        }

        //private void RecordVideo(object obj)
        //{
        //    throw new NotImplementedException();
        //}

        //private void PreviewVideo(object obj)
        //{
        //}

        private void BuildCameraList()
        {
            CameraList = new ObservableCollection<CameraModel>(CameraEnumerator.GetCameraList());
        }

        public void SetActiveWorkspace(ViewModelBase userspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(_userspaces);
            collectionView?.MoveCurrentTo(userspace);
        }
    }
}
