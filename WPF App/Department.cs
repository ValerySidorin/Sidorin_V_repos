using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    public class Department /*: INotifyPropertyChanged*/
    {
        public string Name { get; set; }
        
        public ObservableCollection<Employee> employees = new ObservableCollection<Employee>();

        public Department(string depname)
        {
            Name = depname;
        }
        public void Add(Employee emp)
        {
            employees.Add(emp);
        }

        public void Add(string firstname, string lastname)
        {
            employees.Add(new Employee() { FirstName = firstname, LastName = lastname });
        }

        public void Remove(Employee emp)
        {
            employees.Remove(emp);
        }

        public void Rasstrelyat(Employee emp)
        {
            employees.Remove(emp);
        }

        public List<string> GetEmployees
        {
            get { return employees.Select(emp => emp.FirstName + " " + emp.LastName).ToList(); }
        }
    }
}
