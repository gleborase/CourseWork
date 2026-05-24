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
using SchoolApp.Models;

namespace SchoolApp.View
{
    /// <summary>
    /// Логика взаимодействия для StudentEditWindow.xaml
    /// </summary>
    public partial class StudentEditWindow : Window
    {
        public Student Student { get; private set; }

        public StudentEditWindow(Student student)
        {
            InitializeComponent();

            if (student == null)
            {
                Student = new Student();
            }
            else
            {
                Student = new Student
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    ClassName = student.ClassName
                };

                FullNameBox.Text = Student.FullName;
                ClassBox.Text = Student.ClassName;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("Введите ФИО!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ClassBox.Text))
            {
                MessageBox.Show("Введите класс!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Student.FullName = FullNameBox.Text.Trim();
            Student.ClassName = ClassBox.Text.Trim();

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
