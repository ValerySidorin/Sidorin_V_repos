using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyApp
{
    /// <summary>
    /// Логика взаимодействия для EmployeeChangingDepartment.xaml
    /// </summary>
    public partial class EmployeeChangingDepartment : Window
    {
        public Employee emp { get; set; }

        public Organization organization { get; set; }

        public Department chosenOne { get; set; }
        public EmployeeChangingDepartment()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            chosenOne.Add(emp);
            DialogResult = true;
            Close();
        }

        private void cbDepartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var dep in organization.departments)
            {
                if (dep.Name == cbDepartments.SelectedItem.ToString())
                    chosenOne = dep;
            }
        }
    }
}
