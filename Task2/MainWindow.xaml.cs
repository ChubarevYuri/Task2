using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;


namespace Task2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        readonly MainWindowVM VM = new MainWindowVM();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = VM;
        }

        private void SavePathButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                dialog.SelectedPath = VM.FolderToSave;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SavePathButton.Content = dialog.SelectedPath;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VM.SaveImage();
        }
    }
}
