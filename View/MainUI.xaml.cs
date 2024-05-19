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
using WPF_RevitAPI_04.ViewModel;

namespace WPF_RevitAPI_04.View
{
    /// <summary>
    /// Interaction logic for MainUI.xaml
    /// </summary>
    public partial class MainUI : Window
    {
        public MainUI()
        {
            DataContext = new MainUIViewModel();
            InitializeComponent();
        }

        //private void Excel_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    string h = this.Excel.Text;
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    string h = this.Excel.Text;
        //}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
