using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolApp.Models
{
    public class Performance
    {
        // Ключ — уникальный номер записи
        public int Id { get; set; }

        // ID ученика, к которому относится успеваемость
        public int StudentId { get; set; }

        // Словарь: название предмета → оценка (2–5)
        // Например: { "Математика": 4, "Физика": 3, "Русский": 5 }
        // Количество предметов можно легко менять
        public Dictionary<string, int> Grades { get; set; } = new Dictionary<string, int>();
    }
}
