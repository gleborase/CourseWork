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

namespace SchoolApp.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TeachersButton_Click(object sender, RoutedEventArgs e)
        {
            TeachersWindow teachersWindow = new TeachersWindow();
            teachersWindow.ShowDialog();
        }

        private void StudentsButton_Click(object sender, RoutedEventArgs e)
        {
            StudentsWindow studentsWindow = new StudentsWindow();
            studentsWindow.ShowDialog();
        }

        private void PerformanceButton_Click(object sender, RoutedEventArgs e)
        {
            PerformanceWindow performanceWindow = new PerformanceWindow();
            performanceWindow.ShowDialog();
        }

        private void AnalyticsButton_Click(object sender, RoutedEventArgs e)
        {
            AnalyticsWindow analyticsWindow = new AnalyticsWindow();
            analyticsWindow.ShowDialog();
        }
    }
}