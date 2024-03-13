using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EvaluationPart_2
{
    public partial class AddCustomBudget : Form
    {
        List<Category> categories;
        Dictionary<string, int> MainBudgetControl;
        string[] arr = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        public AddCustomBudget(List<Category> categories,Dictionary<string,int> di)
        {
            InitializeComponent();
            this.categories = categories;
            MainBudgetControl = di;
            comboBox1.DataSource = this.categories;
            comboBox1.DisplayMember = "Name";
            
            for(int i = 0; i < 12; i++)
            {
                comboBox2.Items.Add(arr[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int n = Array.IndexOf(arr, arr[comboBox2.SelectedIndex]) + 1;
                string s = ((Category)comboBox1.SelectedItem).Name + "," + n.ToString() + "," + numericUpDown2.Value;
                MainBudgetControl[s] = (int)numericUpDown1.Value;
                Close();
            }
            catch
            {
                MessageBox.Show("Please Fill All the fields");
            }

        }
    }
}
