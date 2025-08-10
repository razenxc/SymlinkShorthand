using Avalonia.Controls;
using SymlinkShorthand.ViewModels;

namespace SymlinkShorthand.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MainWindowViewModel.TopLevel = this;
            InitializeComponent();
        }
    }
}