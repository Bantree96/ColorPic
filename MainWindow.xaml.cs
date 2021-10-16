using ColorPic.ViewModels;
using System.Windows;

namespace ColorPic
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 필드 
        private MainViewModel _mainViewModel;
        #endregion

        public MainWindow(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            InitializeComponent();

            // 바인딩을 위한 권한
            this.DataContext = _mainViewModel;
        }
    }
}
