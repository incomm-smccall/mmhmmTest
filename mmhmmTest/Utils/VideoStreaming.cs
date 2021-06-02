using ImageProcessor;
using ImageProcessor.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using timer = System.Timers;
using FFMpegCore;

namespace mmhmmTest.Utils
{
    public sealed class VideoStreaming : IDisposable
    {
        private System.Drawing.Bitmap _lastFrame;
        private Task _viewTask;
        private CancellationTokenSource _cancelTokenSource;
        private readonly Image _imageRendering;
        private readonly int _frameWidth;
        private readonly int _frameHeight;
        private VideoWriter outputVideo;

        public int CameraIndex { get; private set; }
        public byte[] LastImageFrame { get; private set; }

        public VideoStreaming(Image imageToRender, int frameWidth, int frameHeight, int cameraId)
        {
            _imageRendering = imageToRender;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            CameraIndex = cameraId;
        }

        /// <summary>
        /// Will capture a single image and return it to be saved.
        /// </summary>
        /// <returns></returns>
        public BitmapSource CaptureImage()
        {
            var imgCapture = new VideoCapture();
            BitmapSource lastBitmapImage = null;
            if (!imgCapture.Open(CameraIndex))
            {
                throw new Exception("Could not connect to selected camera");
            }
            using (var img = new Mat())
            {
                imgCapture.Read(img);
                if (!img.Empty())
                {
                    _lastFrame = BitmapConverter.ToBitmap(img);
                    lastBitmapImage = _lastFrame.ToBitmapSource();
                    lastBitmapImage.Freeze();
                }
            }
            imgCapture?.Dispose();
            return lastBitmapImage;
        }

        public async Task StartPreview()
        {
            // Need to prevent two tasks from streaming
            if (_viewTask != null && !_viewTask.IsCompleted) return;

            outputVideo = new VideoWriter("video.mp4", FourCC.AVC, 29, new OpenCvSharp.Size(400, 300));

            // Controlling the number of request allowed concurrently
            var initSemaphore = new SemaphoreSlim(0, 1);
            _cancelTokenSource = new CancellationTokenSource();
            _viewTask = Task.Run(async () =>
            {
                try
                {
                    var imgCapture = new VideoCapture();
                    if (!imgCapture.Open(CameraIndex))
                    {
                        throw new Exception("Could not connect to selected camera");
                    }

                    using (var img = new Mat())
                    {
                        while (!_cancelTokenSource.IsCancellationRequested)
                        {
                            imgCapture.Read(img);
                            if (!img.Empty())
                            {
                                if (initSemaphore != null)
                                    initSemaphore.Release();
                                _lastFrame = BitmapConverter.ToBitmap(img);
                                var lastBitmapImage = _lastFrame.ToBitmapSource();
                                lastBitmapImage.Freeze();
                                _imageRendering.Dispatcher.Invoke(() => _imageRendering.Source = lastBitmapImage);
                                outputVideo.Write(img);  // Suppose to write the images to file for recording.
                            }
                            // Allows 30 FPS
                            await Task.Delay(33);
                        }
                    }
                    imgCapture?.Dispose();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (initSemaphore != null)
                        initSemaphore.Release();
                }
            }, _cancelTokenSource.Token);

            await initSemaphore.WaitAsync();
            initSemaphore.Dispose();
            initSemaphore = null;

            if (_viewTask.IsFaulted)
                await _viewTask;
        }

        /// <summary>
        /// Stop Preview will pause continued streaming from the camera.
        /// I also used this to debug and test recording.
        /// </summary>
        /// <returns></returns>
        public async Task StopPreview()
        {
            if (_cancelTokenSource.IsCancellationRequested) return;

            if (!_viewTask.IsCompleted)
            {
                _cancelTokenSource.Cancel();
                await _viewTask;
            }

            if (_lastFrame != null)
            {
                using (var imgFactor = new ImageFactory())
                using (var stream = new MemoryStream())
                {
                    imgFactor.Load(_lastFrame)
                        .Resize(new ResizeLayer(
                            new System.Drawing.Size(_frameWidth, _frameHeight),
                            ResizeMode.Crop, AnchorPosition.Center)).Save(stream);
                    LastImageFrame = stream.ToArray();
                }
                OutputRecording();
            }
            else
            {
                LastImageFrame = null;
            }
        }

        /// <summary>
        /// Suppose to write the created 'video.mp4' file to the output path.
        /// The 'video.mp4' file is never created.
        /// </summary>
        public void OutputRecording()
        {
            string outputPath = $"output_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.mp4";
            FFMpeg.ReplaceAudio("video.mp4", "", outputPath, true);
        }

        /// <summary>
        /// Was going to use a timer for recording.
        /// 'video.mp4' was not being created.
        /// </summary>
        /// <param name="startTimer"></param>
        public void StartRecord(bool startTimer)
        {
            outputVideo = new VideoWriter("video.mp4", FourCC.AVC, 29, new OpenCvSharp.Size(400, 300));
            var recordTimer = new timer.Timer(15);
            recordTimer.Enabled = startTimer;
            recordTimer.Elapsed += recordImageElapsed;
            recordTimer.AutoReset = true;
        }

        private void recordImageElapsed(object sender, timer.ElapsedEventArgs e)
        {
            BitmapSource lastBitmapImage = null;
            using (var img = new Mat())
            {
                var imgCapture = new VideoCapture();
                imgCapture.Read(img);
                if (!img.Empty())
                {
                     _lastFrame = BitmapConverter.ToBitmap(img);
                    lastBitmapImage = _lastFrame.ToBitmapSource();
                    lastBitmapImage.Freeze();
                    _imageRendering.Dispatcher.Invoke(() => _imageRendering.Source = lastBitmapImage);
                    outputVideo.Write(img);
                }
                imgCapture?.Dispose();
            }
        }

        public void Dispose()
        {
            _cancelTokenSource?.Cancel();
            _lastFrame?.Dispose();
        }
    }
}
