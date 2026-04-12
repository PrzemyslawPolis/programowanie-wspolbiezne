using PresentationViewModel;
using System.Windows;

namespace PresentationView
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Random random = new Random();
            InitializeComponent();
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            viewModel.Start(random.Next(5, 10));
        }

        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                viewModel.Dispose();
            base.OnClosed(e);
        }
    }
}