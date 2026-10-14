using System;
using System.Globalization;

namespace Lab6App
{
    
    public class Employee
    {
        public string Name { get; set; }
        public decimal Salary { get; set; }

        public Employee(string name, decimal salary)
        {
            Name = name;
            Salary = salary;
        }

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"[Employee] Ім'я: {Name}, Зарплата: {Salary:C}");
        }

        
        public string GetRole()
        {
            return "Рядовий співробітник";
        }
    }

    
    public class Manager : Employee
    {
        public string Department { get; set; }

        public Manager(string name, decimal salary, string department)
            : base(name, salary)
        {
            Department = department;
        }

        
        public override void DisplayInfo()
        {
            Console.WriteLine($"[Manager] Ім'я: {Name}, Зарплата: {Salary:C}, Відділ: {Department}");
        }

        public void ManageTeam()
        {
            Console.WriteLine($"{Name} керує відділом {Department}.");
        }

        
        public new string GetRole()
        {
            return "Менеджер";
        }
    }

    
    public class Director : Employee
    {
        public decimal Bonus { get; set; }

        public Director(string name, decimal salary, decimal bonus)
            : base(name, salary)
        {
            Bonus = bonus;
        }

        
        public override void DisplayInfo()
        {
            Console.WriteLine($"[Director] Ім'я: {Name}, Зарплата: {Salary:C}, Бонус: {Bonus:C}");
        }

        public void LeadCompany()
        {
            Console.WriteLine($"{Name} керує всією компанією.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

            Console.WriteLine("=== 1. Демонстрація поліморфізму (virtual / override) ===");
            Employee[] employees = new Employee[]
            {
                new Employee("Іван", 20000),
                new Manager("Ольга", 45000, "IT-розробка"),
                new Director("Олександр", 100000, 25000)
            };

            foreach (var emp in employees)
            {
                emp.DisplayInfo();
            }

            Console.WriteLine("\n=== 2. Виклик унікальних методів ===");
            Manager mgr = new Manager("Олена", 50000, "Маркетинг");
            Director dir = new Director("Сергій", 120000, 30000);

            mgr.ManageTeam();
            dir.LeadCompany();

            Console.WriteLine("\n=== 3. Демонстрація різниці між override та new ===");
            Manager managerObj = new Manager("Анна", 40000, "Продажі");
            Employee empRef = managerObj; 

            Console.WriteLine($"Виклик через посилання Manager:  {managerObj.GetRole()}");
            Console.WriteLine($"Виклик через посилання Employee: {empRef.GetRole()}");
        }
    }
}