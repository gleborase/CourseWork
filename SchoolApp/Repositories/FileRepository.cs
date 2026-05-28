using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace SchoolApp.Repositories
{
    public class FileRepository<T> where T : class
    {
        private readonly string _filePath;

        public FileRepository(string fileName)
        {
            // Создаём папку Data рядом с exe-файлом, если её нет
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            _filePath = Path.Combine(folder, fileName);
        }

        // Чтение всех объектов из JSON-файла
        public List<T> GetAll()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<T>();

                string json = File.ReadAllText(_filePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<T>();

                return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка чтения файла {_filePath}: {ex.Message}");
            }
        }

        // Сохранение всех объектов в JSON-файл
        public void SaveAll(List<T> items)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(items, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения в файл {_filePath}: {ex.Message}");
            }
        }

        // Получить новый ID (максимальный + 1)
        public int GetNewId()
        {
            var items = GetAll();
            if (items.Count == 0) return 1;

            var property = typeof(T).GetProperty("Id");
            if (property == null) return 1;

            return items.Max(item => (int)property.GetValue(item)) + 1;
        }
    }
}
