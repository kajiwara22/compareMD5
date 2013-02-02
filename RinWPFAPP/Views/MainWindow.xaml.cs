using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RinWPFAPP.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void sourceDirBox_Drop_1(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null)
            {
                (this.DataContext as ViewModels.MainWindowViewModel).SourceDir = files[0];
            }
        }

        private void sourceDirBox_PreviewDragOver_1(object sender, DragEventArgs e)
        {
            e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
        }

        private void targetDirBox_Drop_1(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null)
            {
                (this.DataContext as ViewModels.MainWindowViewModel).TargetDir = files[0];
            }
            
        }
    }
}
