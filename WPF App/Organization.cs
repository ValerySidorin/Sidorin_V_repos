using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    public class Organization
    {
        public List<Department> departments = new List<Department>();
        public string Name { get; set; }

        public Organization(string Name)
        {
            this.Name = Name;
        }

        public void Add(Department department)
        {
            departments.Add(department);
        }

        public List<string> GetDepartments
        {
            get { return departments.Select(dep => dep.Name).ToList(); }
        }

        public void Fill()
        {
            Department head = new Department("Head");
            Department marketing = new Department("Marketing");
            Department sales = new Department("Sales");
            Department logistics = new Department("Logistics");

            head.Add(new Employee("John Snow"));

            marketing.Add(new Employee("Rick Grimes"));
            marketing.Add(new Employee("Walter White"));

            sales.Add(new Employee("Ragnar Lodbrock"));
            sales.Add(new Employee("Jesus Christ"));
            sales.Add(new Employee("John Price"));

            logistics.Add(new Employee("Iosif Stalin"));
            logistics.Add(new Employee("Donald Thrump"));
            logistics.Add(new Employee("Conor McGregor"));
            logistics.Add(new Employee("Khabib Nurmagomedov"));

            Add(head);
            Add(marketing);
            Add(sales);
            Add(logistics);
        }
    }
}
