using mmhmmTest.Model;
using mmhmmTest.Utils;
using OpenCvSharp;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using winForms = System.Windows.Forms;

namespace mmhmmTest.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        private VideoStreaming _videoStreaming;
        public MainView()
        {
            InitializeComponent();
            btnCapture.IsEnabled = false;
            btnRecord.IsEnabled = false;
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCameras.SelectedItem != null)
            {
                if (btnPreview.Content.ToString() == "Start")
                {
                    btnCapture.IsEnabled = true;
                    btnRecord.IsEnabled = true;
                    StartVideoPreview();
                }
                if (btnPreview.Content.ToString() == "Stop")
                {
                    btnPreview.IsEnabled = false;
                    btnRecord.IsEnabled = false;
                    StopVideoPreview();
                }
            }
            else
            {
                TxtMessageBlock.Text = "Please select a camera...";
                TxtMessageBlock.Foreground = Brushes.Red;
                return;
            }
        }

        private async void StopVideoPreview()
        {
            btnPreview.Content = "Start";
            TxtMessageBlock.Text = "Video preview has been stopped";
            await _videoStreaming.StopPreview();
        }

        private async void StartVideoPreview()
        {
            GetVideoStreaming();
            try
            {
                await _videoStreaming.StartPreview();
                btnPreview.Content = "Stop";
                TxtMessageBlock.Text = "Video preview has started";
                TxtMessageBlock.Foreground = Brushes.Black;
            }
            catch (Exception ex)
            {
                btnPreview.Content = "Start";
                TxtMessageBlock.Text = $"ERROR: {ex.Message}";
                TxtMessageBlock.Foreground = Brushes.Red;
            }
        }

        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            TxtMessageBlock.Text = "Starting recording...";

            btnRecord.Content = "Stop";
            _videoStreaming.StartRecord(true);
            TxtMessageBlock.Text = "Recording...";
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            GetVideoStreaming();
            try
            {
                BitmapSource picture = _videoStreaming.CaptureImage();
                winForms.SaveFileDialog picDialog = new winForms.SaveFileDialog();
                picDialog.Filter = "JPG Files (*.jpg)|*.jpg";
                if (picDialog.ShowDialog() == winForms.DialogResult.OK)
                {
                    using (var imgageStream = new FileStream(picDialog.FileName, FileMode.Create))
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(picture));
                        encoder.Save(imgageStream);
                    }
                }
                _videoStreaming = null;
                StartVideoPreview();
                TxtMessageBlock.Text = "Video preview has been restarted";
                TxtMessageBlock.Foreground = Brushes.Black;

            }
            catch (Exception ex)
            {
                TxtMessageBlock.Text = $"ERROR: {ex.Message}";
                TxtMessageBlock.Foreground = Brushes.Red;
            }
        }

        private void GetVideoStreaming()
        {
            var selectedCameraID = (cmbCameras.SelectedItem as CameraModel).CameraID;
            if (_videoStreaming == null || _videoStreaming.CameraIndex != selectedCameraID)
            {
                _videoStreaming?.Dispose();
                _videoStreaming = new VideoStreaming(VideoPreview, 400, 300, cmbCameras.SelectedIndex);
            }
        }
    }
}
