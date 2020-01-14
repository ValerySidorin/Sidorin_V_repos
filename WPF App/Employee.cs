using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    public class Employee : INotifyPropertyChanged
    {
        private string firstname;
        private string lastname;
        public string FirstName 
        { 
            get 
            { 
                return firstname; 
            }
            set
            {
                if (value != firstname)
                {
                    firstname = value;
                    Notify("FirstName");
                }
            }
        }
        public string LastName
        {
            get
            {
                return lastname;
            }
            set
            {
                if (value != lastname)
                {
                    lastname = value;
                    Notify("LastName");
                }
            }
        }

        public Employee()
        {

        }
        public Employee(string firstname, string lastname)
        {
            FirstName = firstname;
            LastName = lastname;
        }

        public Employee(string FIO)
        {
            string[] fio = FIO.Split(' ');
            FirstName = fio[0];
            LastName = fio[1];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        //public string GetFullName
        //{
        //    get { return FirstName + " " + LastName; }
        //}

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
