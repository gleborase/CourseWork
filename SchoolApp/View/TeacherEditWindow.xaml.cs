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
    /// Логика взаимодействия для TeacherEditWindow.xaml
    /// </summary>
    public partial class TeacherEditWindow : Window
    {
        public Teacher Teacher { get; private set; }

        public TeacherEditWindow(Teacher teacher)
        {
            InitializeComponent();

            if (teacher == null)
            {
                // Режим добавления
                Teacher = new Teacher();
            }
            else
            {
                // Режим редактирования — заполняем поля
                Teacher = new Teacher
                {
                    Id = teacher.Id,
                    FullName = teacher.FullName,
                    Subject = teacher.Subject,
                    ClassroomNumber = teacher.ClassroomNumber
                };

                FullNameBox.Text = Teacher.FullName;
                SubjectBox.Text = Teacher.Subject;
                ClassroomBox.Text = Teacher.ClassroomNumber.ToString();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("Введите ФИО!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(SubjectBox.Text))
            {
                MessageBox.Show("Введите предмет!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(ClassroomBox.Text, out int classroom))
            {
                MessageBox.Show("Введите номер кабинета числом!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Сохраняем данные
            Teacher.FullName = FullNameBox.Text.Trim();
            Teacher.Subject = SubjectBox.Text.Trim();
            Teacher.ClassroomNumber = classroom;

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
