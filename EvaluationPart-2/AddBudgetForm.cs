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
    public partial class AddBudgetForm : Form
    {
        List<Category> categories;
        Dictionary<string, int> di;
        public AddBudgetForm(List<Category> ca,Dictionary<string,int> dic)
        {
            InitializeComponent();
            categories = ca;
            di = dic;
            foreach(var a in categories)
            {
                if (!di.ContainsKey(a.Name)) comboBox1.Items.Add(a.Name);
            }
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string s = comboBox1.SelectedItem.ToString();
                int n = (int)numericUpDown1.Value;
                di.Add(s, n);
                this.Close();
            }
            catch
            {
                MessageBox.Show("Enter the Category");
            }
            
        }
    }
}
