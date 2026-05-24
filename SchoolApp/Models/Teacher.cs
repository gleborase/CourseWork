using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolApp.Models
{
    public class Teacher
    {
        // Ключ — уникальный номер учителя
        public int Id { get; set; }

        // ФИО учителя
        public string FullName { get; set; } = "";

        // Какой предмет ведёт
        public string Subject { get; set; } = "";

        // Номер кабинета
        public int ClassroomNumber { get; set; }
    }
}
