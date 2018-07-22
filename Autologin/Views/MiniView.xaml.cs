namespace Autologin.Views
{
    #region Includes
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    #endregion

    /// <summary>
    /// Interaction logic for MiniView.xaml
    /// </summary>
    public partial class MiniView : Window
    {
        public MiniView()
        {
            if(Application.Current is App a)
            {
                DataContext = a.ActivityContext;
            }
            
            InitializeComponent();
        }
    }
}
