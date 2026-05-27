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
    /// Логика взаимодействия для TeachersWindow.xaml
    /// </summary>
    public partial class TeachersWindow : Window
    {
        private FileRepository<Teacher> _repository;
        private List<Teacher> _teachers;

        public TeachersWindow()
        {
            InitializeComponent();
            _repository = new FileRepository<Teacher>("teachers.json");
            LoadTeachers();
        }

        // Загрузка учителей в таблицу
        private void LoadTeachers()
        {
            try
            {
                _teachers = _repository.GetAll();
                TeachersGrid.ItemsSource = null;
                TeachersGrid.ItemsSource = _teachers;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Добавление учителя
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            TeacherEditWindow editWindow = new TeacherEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                Teacher newTeacher = editWindow.Teacher;
                newTeacher.Id = _repository.GetNewId();
                _teachers.Add(newTeacher);
                _repository.SaveAll(_teachers);
                LoadTeachers();
            }
        }

        // Редактирование учителя
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Teacher selected = TeachersGrid.SelectedItem as Teacher;
            if (selected == null)
            {
                MessageBox.Show("Выберите учителя для редактирования!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TeacherEditWindow editWindow = new TeacherEditWindow(selected);
            if (editWindow.ShowDialog() == true)
            {
                Teacher updated = editWindow.Teacher;
                Teacher existing = _teachers.FirstOrDefault(t => t.Id == updated.Id);
                if (existing != null)
                {
                    existing.FullName = updated.FullName;
                    existing.Subject = updated.Subject;
                    existing.ClassroomNumber = updated.ClassroomNumber;
                }
                _repository.SaveAll(_teachers);
                LoadTeachers();
            }
        }

        // Удаление учителя
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Teacher selected = TeachersGrid.SelectedItem as Teacher;
            if (selected == null)
            {
                MessageBox.Show("Выберите учителя для удаления!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Удалить учителя {selected.FullName}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _teachers.Remove(selected);
                _repository.SaveAll(_teachers);
                LoadTeachers();
            }
        }

        // Обновление таблицы
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadTeachers();
        }

        private void TeachersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
