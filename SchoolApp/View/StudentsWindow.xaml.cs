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
using SchoolApp.Repositories;

namespace SchoolApp.View
{
    /// <summary>
    /// Логика взаимодействия для StudentsWindow.xaml
    /// </summary>
    public partial class StudentsWindow : Window
    {
        private FileRepository<Student> _repository;
        private List<Student> _students;

        public StudentsWindow()
        {
            InitializeComponent();
            _repository = new FileRepository<Student>("students.json");
            LoadStudents();
        }

        private void LoadStudents()
        {
            try
            {
                _students = _repository.GetAll();
                StudentsGrid.ItemsSource = null;
                StudentsGrid.ItemsSource = _students;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            StudentEditWindow editWindow = new StudentEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                Student newStudent = editWindow.Student;
                newStudent.Id = _repository.GetNewId();
                _students.Add(newStudent);
                _repository.SaveAll(_students);
                LoadStudents();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Student selected = StudentsGrid.SelectedItem as Student;
            if (selected == null)
            {
                MessageBox.Show("Выберите ученика для редактирования!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            StudentEditWindow editWindow = new StudentEditWindow(selected);
            if (editWindow.ShowDialog() == true)
            {
                Student updated = editWindow.Student;
                Student existing = _students.FirstOrDefault(s => s.Id == updated.Id);
                if (existing != null)
                {
                    existing.FullName = updated.FullName;
                    existing.ClassName = updated.ClassName;
                }
                _repository.SaveAll(_students);
                LoadStudents();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Student selected = StudentsGrid.SelectedItem as Student;
            if (selected == null)
            {
                MessageBox.Show("Выберите ученика для удаления!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Удалить ученика {selected.FullName}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _students.Remove(selected);
                _repository.SaveAll(_students);
                LoadStudents();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadStudents();
        }
    }
}
