using System.Windows;

namespace Autologin.Views
{
    /// <summary>
    /// Interaction logic for MiniView.xaml
    /// </summary>
    public partial class MiniView : Window
    {
        public MiniView()
        {
            if (Application.Current is App a)
            {
                DataContext = a.ActivityContext;
            }

            InitializeComponent();
        }
    }
}