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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SchoolApp.Models;
using SchoolApp.Repositories;

namespace SchoolApp.View
{
    /// <summary>
    /// Логика взаимодействия для PerformanceWindow.xaml
    /// </summary>
    public partial class PerformanceWindow : Window
    {
        private FileRepository<Performance> _perfRepository;
        private FileRepository<Student> _studentRepository;
        private FileRepository<Teacher> _teacherRepository;

        private List<Performance> _performances;
        private List<Student> _students;
        private List<Teacher> _teachers;

        // Для таблицы оценок
        public ObservableCollection<GradeRow> GradesList { get; set; } = new();

        public PerformanceWindow()
        {
            InitializeComponent();

            _perfRepository = new FileRepository<Performance>("performances.json");
            _studentRepository = new FileRepository<Student>("students.json");
            _teacherRepository = new FileRepository<Teacher>("teachers.json");

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _students = _studentRepository.GetAll();
                _teachers = _teacherRepository.GetAll();
                _performances = _perfRepository.GetAll();

                // Заполняем выпадающий список учеников
                StudentComboBox.ItemsSource = null;
                StudentComboBox.ItemsSource = _students;
                StudentComboBox.DisplayMemberPath = "FullName";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // При выборе ученика — загружаем его оценки
        private void StudentComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Student selectedStudent = StudentComboBox.SelectedItem as Student;
            if (selectedStudent == null)
            {
                GradesList.Clear();
                GradesGrid.ItemsSource = null;
                return;
            }

            LoadGrades(selectedStudent.Id);
        }

        // Загрузка оценок для конкретного ученика
        private void LoadGrades(int studentId)
        {
            GradesList.Clear();

            // Получаем список всех предметов (из учителей)
            var subjects = _teachers.Select(t => t.Subject).Distinct().OrderBy(s => s).ToList();

            // Ищем успеваемость этого ученика
            var perf = _performances.FirstOrDefault(p => p.StudentId == studentId);

            // Заполняем таблицу: предмет — оценка
            foreach (var subject in subjects)
            {
                int grade = 0; // 0 — нет оценки

                if (perf != null && perf.Grades.ContainsKey(subject))
                {
                    grade = perf.Grades[subject];
                }

                GradesList.Add(new GradeRow
                {
                    Subject = subject,
                    Grade = grade
                });
            }

            GradesGrid.ItemsSource = GradesList;
        }

        // Сохранение оценок
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Student selectedStudent = StudentComboBox.SelectedItem as Student;
            if (selectedStudent == null)
            {
                MessageBox.Show("Выберите ученика!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка оценок (2–5 или 0)
            foreach (var row in GradesList)
            {
                if (row.Grade != 0 && (row.Grade < 2 || row.Grade > 5))
                {
                    MessageBox.Show($"Оценка по предмету '{row.Subject}' должна быть от 2 до 5 или 0!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            try
            {
                // Ищем существующую запись или создаём новую
                var perf = _performances.FirstOrDefault(p => p.StudentId == selectedStudent.Id);

                if (perf == null)
                {
                    perf = new Performance
                    {
                        Id = _perfRepository.GetNewId(),
                        StudentId = selectedStudent.Id
                    };
                    _performances.Add(perf);
                }

                // Обновляем оценки
                perf.Grades.Clear();
                foreach (var row in GradesList)
                {
                    if (row.Grade > 0) // Сохраняем только заданные оценки
                    {
                        perf.Grades[row.Subject] = row.Grade;
                    }
                }

                _perfRepository.SaveAll(_performances);

                MessageBox.Show("Успеваемость сохранена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем отображение
                LoadGrades(selectedStudent.Id);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Удаление успеваемости ученика
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Student selectedStudent = StudentComboBox.SelectedItem as Student;
            if (selectedStudent == null)
            {
                MessageBox.Show("Выберите ученика!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить успеваемость ученика {selectedStudent.FullName}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var perf = _performances.FirstOrDefault(p => p.StudentId == selectedStudent.Id);
                if (perf != null)
                {
                    _performances.Remove(perf);
                    _perfRepository.SaveAll(_performances);
                    LoadGrades(selectedStudent.Id);

                    MessageBox.Show("Успеваемость удалена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // Обновление
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            if (StudentComboBox.SelectedItem != null)
            {
                Student selected = StudentComboBox.SelectedItem as Student;
                LoadGrades(selected.Id);
            }
        }
    }

    // Вспомогательный класс для строки таблицы оценок
    public class GradeRow : INotifyPropertyChanged
    {
        public string Subject { get; set; } = "";

        private int _grade;
        public int Grade
        {
            get => _grade;
            set
            {
                _grade = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
