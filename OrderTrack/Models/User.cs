using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTrack.Models
{
    public class User
    {
        public int CodeUser { get; set; }
        public string NameUser { get; set; }
        public string BarCode { get; set; }
        public string Login { get; set; }
        public string PassWord { get; set; }
        public eTypeUser TypeUser { get; set; }
    }
    public enum eTypeUser
    {
        /// <summary>
        /// Клієнт КСО
        /// </summary>
        Client = -100,
        /// <summary>
        /// Працівник без Прав.
        /// </summary>
        Employee = -20,
        /// <summary>
        /// Працівник з доступом до ТЗД
        /// </summary>
        EmployeeDCT = -15,
        /// <summary>
        /// Касир
        /// </summary>
        Сashier = -14,
        /// <summary>
        /// Адміністратор self-service checkout 
        /// </summary>
        AdminSSC = -13,
        /// <summary>
        /// Охоронець
        /// </summary>
        Guardian = -12,
        /// <summary>
        /// Адміністратор ТЗ
        /// </summary>
        AdminShop = -11,
        /// <summary>
        /// Адміністратор Дозвіл на всі операції
        /// </summary>
        Admin = 0
    }
}
