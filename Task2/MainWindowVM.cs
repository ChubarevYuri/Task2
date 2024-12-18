using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Task2
{
    internal class MainWindowVM : INotifyPropertyChanged
    {
        public readonly Camera camera;
        public MainWindowVM()
        {
            camera = new Camera(this);
        }

        private string folderToSave = Environment.CurrentDirectory;
        /// <summary>
        /// связка с label - папка для сохранения
        /// </summary>
        public string FolderToSave
        {
            get
            {
                return folderToSave;
            }
            set
            {
                folderToSave = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FolderToSave"));
            }
        }

        private Bitmap image;
        private DateTime lastScreen = new DateTime(0);

        public Bitmap Image
        {
            set
            {
                bool needRaiseFPS = false;
                lock (this)
                {
                    image = value;
                    DateTime factScreen = DateTime.Now;
                    if (lastScreen.Ticks > 0)
                    {
                        double seconds = (factScreen - lastScreen).TotalMilliseconds;
                        double fps_d = 1000 / seconds;
                        int fps = (int)Math.Round(fps_d);
                        this.fps = fps;
                        needRaiseFPS = true;
                    }
                    lastScreen = factScreen;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Size"));
                if (needRaiseFPS) 
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FPS"));
                }
            }
        }
        /// <summary>
        /// связка с image - текущий кадр
        /// </summary>
        public BitmapSource ImageSource
        {
            get
            {
                lock (this)
                {
                    if (image == null)
                    {
                        return null;
                    }
                    else
                    {
                        return BitmapSourceConvert(image);
                    }
                }
            }
        }
        /// <summary>
        /// связка с image - разрешение камеры
        /// </summary>
        public string Size
        {
            get
            {
                lock (this)
                {
                    if (image != null)
                    {
                        return string.Format("{0}x{1}", image.Width, image.Height);
                    }
                    else
                    {
                        return "???";
                    }
                }
            }
        }
        private int fps = 0;
        /// <summary>
        /// связка с image - частота кадров
        /// </summary>
        public string FPS
        {
            get
            {
                lock (this)
                {
                    if (fps > 0)
                    {
                        return fps.ToString();
                    }
                    return "???";
                }
            }
        }

        /// <summary>
        /// Сохранение изображения
        /// </summary>
        public void SaveImage()
        {
            lock (this)
            {
                if (image == null) { return; }
                if (Directory.Exists(folderToSave))
                {

                    FileAttributes attr = File.GetAttributes(FolderToSave);
                    // Проверка что путь является папкой
                    if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        return;
                    }
                }
                image.Save(string.Format("{0}\\{1}.bmp", FolderToSave, Properties.Settings.Default.Num), ImageFormat.Bmp);
                // Увеличим счётчик
                Properties.Settings.Default.Num++;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// из Bitmap в BitmapSource
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource BitmapSourceConvert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
            


            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Gray8, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);
            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
