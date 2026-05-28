using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SchoolApp.Models;
using SchoolApp.Repositories;

namespace SchoolApp.View
{
    public partial class AnalyticsWindow : Window
    {
        private List<Student> _students;
        private List<Teacher> _teachers;
        private List<Performance> _performances;

        public AnalyticsWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var studentRepo = new FileRepository<Student>("students.json");
                var teacherRepo = new FileRepository<Teacher>("teachers.json");
                var perfRepo = new FileRepository<Performance>("performances.json");

                _students = studentRepo.GetAll();
                _teachers = teacherRepo.GetAll();
                _performances = perfRepo.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void QueryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = QueryComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string query = selectedItem.Content.ToString();

            if (query == "Успеваемость по заданному предмету")
            {
                SubjectPanel.Visibility = Visibility.Visible;
            }
            else
            {
                SubjectPanel.Visibility = Visibility.Collapsed;
                ExecuteQuery(query, "");
            }
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SubjectTextBox.Text))
            {
                MessageBox.Show("Введите название предмета!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ExecuteQuery("Успеваемость по заданному предмету", SubjectTextBox.Text.Trim());
        }

        private void ExecuteQuery(string query, string subject)
        {
            try
            {
                ResultGrid.Visibility = Visibility.Collapsed;
                ResultText.Visibility = Visibility.Collapsed;

                switch (query)
                {
                    case "Успеваемость по заданному предмету":
                        ShowPerformanceBySubject(subject);
                        break;

                    case "Количество неуспевающих учеников по всем классам":
                        ShowFailingStudents();
                        break;

                    case "У какого учителя самая низкая успеваемость":
                        ShowTeacherWithLowestPerformance();
                        break;

                    case "Средняя оценка по всем предметам в каждом классе":
                        ShowAverageByClass();
                        break;

                    case "Класс с самой высокой успеваемостью":
                        ShowClassWithBestPerformance();
                        break;

                    case "Класс с самой низкой успеваемостью":
                        ShowClassWithWorstPerformance();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 1. Успеваемость по заданному предмету
        private void ShowPerformanceBySubject(string subject)
        {
            var result = from perf in _performances
                         join student in _students on perf.StudentId equals student.Id
                         where perf.Grades.ContainsKey(subject)
                         select new
                         {
                             Ученик = student.FullName,
                             Класс = student.ClassName,
                             Предмет = subject,
                             Оценка = perf.Grades[subject]
                         };

            var list = result.OrderBy(r => r.Класс).ThenBy(r => r.Ученик).ToList();

            if (list.Count == 0)
            {
                ResultText.Text = $"Нет данных по предмету \"{subject}\"";
                ResultText.Visibility = Visibility.Visible;
                return;
            }

            ResultGrid.ItemsSource = list;
            ResultGrid.Visibility = Visibility.Visible;
        }

        // 2. Количество неуспевающих учеников по всем классам
        private void ShowFailingStudents()
        {
            var result = from perf in _performances
                         join student in _students on perf.StudentId equals student.Id
                         where perf.Grades.Values.Any(g => g == 2)
                         group student by student.ClassName into g
                         select new
                         {
                             Класс = g.Key,
                             КоличествоДвоечников = g.Count()
                         };

            var list = result.OrderBy(r => r.Класс).ToList();

            if (list.Count == 0)
            {
                ResultText.Text = "Неуспевающих учеников нет!";
                ResultText.Visibility = Visibility.Visible;
                return;
            }

            ResultGrid.ItemsSource = list;
            ResultGrid.Visibility = Visibility.Visible;
        }

        // 3. Учитель с самой низкой успеваемостью
        private void ShowTeacherWithLowestPerformance()
        {
            var result = from teacher in _teachers
                         let grades = (from perf in _performances
                                       where perf.Grades.ContainsKey(teacher.Subject)
                                       select perf.Grades[teacher.Subject]).ToList()
                         where grades.Any()
                         select new
                         {
                             Учитель = teacher.FullName,
                             Предмет = teacher.Subject,
                             СредняяОценка = Math.Round(grades.Average(), 2)
                         };

            var list = result.OrderBy(r => r.СредняяОценка).ToList();

            if (list.Count == 0)
            {
                ResultText.Text = "Нет данных об успеваемости!";
                ResultText.Visibility = Visibility.Visible;
                return;
            }

            var worst = list.First();

            ResultText.Text = $"Учитель с самой низкой успеваемостью:\n\n" +
                              $"👨‍🏫 {worst.Учитель}\n" +
                              $"📖 Предмет: {worst.Предмет}\n" +
                              $"📊 Средняя оценка: {worst.СредняяОценка}";
            ResultText.Visibility = Visibility.Visible;

            ResultGrid.ItemsSource = list;
            ResultGrid.Visibility = Visibility.Visible;
        }

        // 4. Средняя оценка по всем предметам в каждом классе
        private void ShowAverageByClass()
        {
            var result = from perf in _performances
                         join student in _students on perf.StudentId equals student.Id
                         let allGrades = perf.Grades.Values
                         where allGrades.Any()
                         group new { student, allGrades } by student.ClassName into g
                         select new
                         {
                             Класс = g.Key,
                             СредняяОценка = Math.Round(
                                 g.SelectMany(x => x.allGrades).Average(), 2)
                         };

            var list = result.OrderBy(r => r.Класс).ToList();

            if (list.Count == 0)
            {
                ResultText.Text = "Нет данных об успеваемости!";
                ResultText.Visibility = Visibility.Visible;
                return;
            }

            ResultGrid.ItemsSource = list;
            ResultGrid.Visibility = Visibility.Visible;
        }

        // 5. Класс с самой высокой успеваемостью
        private void ShowClassWithBestPerformance()
        {
            var classAverages = from perf in _performances
                                join student in _students on perf.StudentId equals student.Id
                                let allGrades = perf.Grades.Values
                                where allGrades.Any()
                                group new { student, allGrades } by student.ClassName into g
                                select new
                                {
                                    Класс = g.Key,
                                    СредняяОценка = Math.Round(
                                        g.SelectMany(x => x.allGrades).Average(), 2)
                                };

            var best = classAverages.OrderByDescending(c => c.СредняяОценка).FirstOrDefault();

            if (best == null)
            {
                ResultText.Text = "Нет данных об успеваемости!";
                ResultText.Visibility = Visibility.Visible;
                return;
            }

            ResultText.Text = $"Класс с самой высокой успеваемостью:\n\n" +
                              $"🏆 {best.Класс}\n" +
                              $"📊 Средняя оценка: {best.СредняяОценка}";
            ResultText.Visibility = Visibility.Visible;

            ResultGrid.ItemsSource = classAverages.OrderByDescending(c => c.СредняяОценка).ToList();
            ResultGrid.Visibility = Visibility.Visible;
        }

        // 6. Класс с самой низкой успеваемостью
        private void ShowClassWithWorstPerformance()
        {
            var classAverages = from perf in _performances
                                join student in _students on perf.StudentId equals student.Id
                                let allGrades = perf.Grades.Values
                                where allGrades.Any()
                                group new { student, allGrades } by student.ClassName into g
                                select new
                                {
                                    Класс = g.Key,
                                    СредняяОценка = Math.Round(
                                        g.SelectMany(x => x.allGrades).Average(), 2)
                                };

            var worst = classAverages.OrderBy(c => c.СредняяОценка).FirstOrDefault();

            if (worst == null)
            {
                ResultText.Text = "Нет данных об успеваемости!";
                ResultText.Visibility = Visibility.Visible;
                return;
            }

            ResultText.Text = $"Класс с самой низкой успеваемостью:\n\n" +
                              $"⚠️ {worst.Класс}\n" +
                              $"📊 Средняя оценка: {worst.СредняяОценка}";
            ResultText.Visibility = Visibility.Visible;

            ResultGrid.ItemsSource = classAverages.OrderBy(c => c.СредняяОценка).ToList();
            ResultGrid.Visibility = Visibility.Visible;
        }
    }
}