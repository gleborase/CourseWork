using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolApp.Models
{
    public class Student
    {
        // Ключ — уникальный номер ученика
        public int Id { get; set; }

        // ФИО ученика
        public string FullName { get; set; } = "";

        // Класс, например "10А"
        public string ClassName { get; set; } = "";
    }
}
