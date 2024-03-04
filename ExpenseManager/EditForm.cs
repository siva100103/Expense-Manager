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
    public partial class EditForm : Form
    {
        Expense expense;
        public event EventHandler EditFinished;
        Dictionary<string, int> SpendingBudget;
        public EditForm(Expense ex,List<Category> categories,Dictionary<string,int> spen)
        {
            expense = ex;
            InitializeComponent();
            SpendingBudget = spen;
            comboBox1.DataSource = categories;
            comboBox1.DisplayMember = "Name";
            comboBox1.Text = ex.Category;
            comboBox1.SelectedItem = ex.Category;
            richTextBox1.Text = ex.Notes;
            numericUpDown1.Value = ex.Amount;
            dateTimePicker1.Value = ex.Date;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(var a in SpendingBudget.ToArray())
            {
                string[] s = a.Key.Split(',');
                if(s[0].Equals(expense.Category) && s[1].Equals(expense.Date.Month.ToString()) && s[2].Equals(expense.Date.Year.ToString()))
                {
                    SpendingBudget[a.Key] -= expense.Amount;
                }
            }

            expense.Amount = (int)numericUpDown1.Value;
            expense.Date = dateTimePicker1.Value;
            expense.Notes = richTextBox1.Text;
            expense.Category = ((Category)comboBox1.SelectedItem).Name;

            string str = expense.Category + "," + expense.Date.Month + "," + expense.Date.Year;

            if (SpendingBudget.ContainsKey(str)) SpendingBudget[str] += expense.Amount;
            else SpendingBudget.Add(str, expense.Amount);
            
            EditFinished.Invoke(sender,e);
            
        }
    }
}
