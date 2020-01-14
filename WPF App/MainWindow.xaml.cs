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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Organization org = new Organization("Organization");

        public Department chosenDep { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            org.Fill();

            cbDepartments.ItemsSource = org.GetDepartments;
            chosenDep = org.departments[0];
            cbDepartments.Text = chosenDep.Name;
            lvEmployees.ItemsSource = chosenDep.employees;
            
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text != "" || txtSurname.Text != "")
            {
                chosenDep.Add(new Employee(txtName.Text.ToString(), txtSurname.Text.ToString()));
                txtName.Clear();
                txtSurname.Clear();
            }
            else return;
        }


        private void cbDepartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var dep in org.departments)
                if (dep.Name == cbDepartments.SelectedItem.ToString())
                    chosenDep = dep;

            lvEmployees.ItemsSource = chosenDep.employees;
        }

        private void btnAddDepartment_Click(object sender, RoutedEventArgs e)
        {
            DepartmentWindow dw = new DepartmentWindow();

            dw.lbDepartments.ItemsSource = org.GetDepartments;

            if (dw.ShowDialog() == true)
            {
                org.Add(dw.dep);
                chosenDep = dw.dep;
                cbDepartments.ItemsSource = org.GetDepartments;
                cbDepartments.Text = chosenDep.Name;
            }
        }

        private void btnChangeDepartment_Click(object sender, RoutedEventArgs e)
        {
            EmployeeChangingDepartment ecd = new EmployeeChangingDepartment();
            for (int i = 0; i < chosenDep.employees.Count; i++)
            {
                if (chosenDep.employees[i].ToString() == lvEmployees.SelectedItem.ToString())
                {
                    ecd.emp = chosenDep.employees[i];
                    chosenDep.Remove(chosenDep.employees[i]);
                }
            }
            ecd.organization = org;
            ecd.cbDepartments.ItemsSource = ecd.organization.GetDepartments;
            ecd.lblEmployeeToChange.Content = ecd.emp.ToString();

            if (ecd.ShowDialog() == true)
            {
                chosenDep = ecd.chosenOne;
                cbDepartments.Text = chosenDep.Name.ToString();
                lvEmployees.ItemsSource = chosenDep.employees;
            }
        }

        private void btnFire_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < chosenDep.employees.Count; i++)
            {
                if (lvEmployees.SelectedValue != null)
                {

                    if (chosenDep.employees[i].ToString() == lvEmployees.SelectedValue.ToString())
                    {
                        if ((chosenDep.employees[i].FirstName.ToLower() == "vladimir" && chosenDep.employees[i].LastName.ToLower() == "putin") || (chosenDep.employees[i].FirstName.ToLower() == "владимир" && chosenDep.employees[i].LastName.ToLower() == "путин"))
                        {
                            MessageBox.Show("Ошибка! Плоти нологи!");
                            Close();
                        }
                        else chosenDep.Remove(chosenDep.employees[i]);
                    }
                }
                else return;
            }
        }
    }
}
