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
   
    public partial class AddExpenseForm : Form
    {
        public delegate void GetExpense(Category category, int amount, DateTime date, string note);
        public event GetExpense GetData;
        public AddExpenseForm(List<Category> categories)
        {
            InitializeComponent();
            comboBox1.DataSource = categories;
            comboBox1.DisplayMember = "Name";
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetData?.Invoke((Category)comboBox1.SelectedItem, (int)numericUpDown1.Value,dateTimePicker1.Value, richTextBox1.Text);
        }
    }
}
