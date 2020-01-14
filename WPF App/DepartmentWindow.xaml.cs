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
    /// Логика взаимодействия для DepartmentWindow.xaml
    /// </summary>
    public partial class DepartmentWindow : Window
    {
        public Department dep { get; set; }
        public DepartmentWindow()
        {
            InitializeComponent();
        }

        private void btnAddDepartment_Click(object sender, RoutedEventArgs e)
        {
            dep = new Department(txtAddDepartment.Text);
            DialogResult = true;
            Close();
        }
    }
}
