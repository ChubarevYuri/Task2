using System;
using System.Drawing;
using System.Windows;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Task2
{
    internal class Camera
    {

        private VideoCaptureDevice FinalFrame;
        private readonly MainWindowVM mainWindowVM;

        public Camera(MainWindowVM mainWindowVM)
        {
            this.mainWindowVM = mainWindowVM;
            Application.Current.Exit += new ExitEventHandler(FinalFrame_Stop);
            FilterInfoCollection CaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FinalFrame = new VideoCaptureDevice(CaptureDevices[0].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.Start();
        }
        
        readonly Grayscale filter = new Grayscale(0.333, 0.333, 0.333);

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Получаем изображение с веб-камеры
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            // Делаем изображение серым
            image = filter.Apply(image);
            // Передаём изображение, игнорируем если произойдут ошибки
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mainWindowVM.Image = image;
                });
            }
            catch { }
        }

        public void FinalFrame_Stop(object sender, EventArgs eventArgs)
        {
            FinalFrame?.SignalToStop();
        }

    }
}
