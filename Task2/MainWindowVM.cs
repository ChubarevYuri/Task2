using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Task2
{
    internal class MainWindowVM : INotifyPropertyChanged
    {

        public MainWindowVM()
        {
            try
            {
                image = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\ТПИlogo.png"));
            }
            catch { }
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

        private BitmapSource image;
        /// <summary>
        /// связка с image - текущий кадр
        /// </summary>
        public BitmapSource Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Image"));
            }
        }

        /// <summary>
        /// Сохранение изображения
        /// </summary>
        public void SaveImage()
        {
            if (image == null) { return; }
            int num = Properties.Settings.Default.Num;
            if (Directory.Exists(folderToSave)) 
            {
                
                FileAttributes attr = File.GetAttributes(FolderToSave);
                //Проверка что путь является папкой
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    /*
                    //перебор всех png файлов для выбора следующего номера
                    foreach (string file in Directory.EnumerateFiles(FolderToSave, "*.png", SearchOption.AllDirectories))
                    {
                        string a = file.Substring(file.LastIndexOf('\\')+1);
                        a = a.Substring(0, a.Length - 4);
                        int fileNum = 0;
                        Int32.TryParse(a, out fileNum);
                        if (fileNum > num) { num = fileNum; }
                    }
                    */
                }
                else
                {
                    return;
                }
            }
            Properties.Settings.Default.Num++;
            Properties.Settings.Default.Save();
            Bitmap bitmap = BitmapConvert(Image);
            bitmap.Save(string.Format("{0}.png", num), ImageFormat.Png);
        }

        /// <summary>
        /// из BitmapSource в Bitmap
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Bitmap BitmapConvert(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
                source.PixelWidth,
                source.PixelHeight,
                PixelFormat.Format32bppArgb);
            BitmapData data = bmp.LockBits(
              new Rectangle(System.Drawing.Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
