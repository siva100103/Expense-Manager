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
    public partial class CategoryEditForm : Form
    {
        List<Category> categories;
        List<Expense> expenses;
        private string name;
        Dictionary<string, int> spendingBudget;
        public event EventHandler<string> CategoryUpdated;
        public CategoryEditForm(List<Category> cate,string Name,List<Expense> exp,Dictionary<string,int> spe)
        {
            InitializeComponent();
            categories = cate;
            expenses = exp;
            spendingBudget = spe;
            name = Name;
        }

        private void UpdateButtonClick(object sender, EventArgs e)
        {
            string s = richTextBox2.Text.Trim();
            if (s.Equals(""))
            {
                MessageBox.Show("Invalid Category Name");
                return;
            }
            foreach (var a in categories)
            {
                if (a.Name.ToLower().Equals(s.ToLower()))
                {
                    MessageBox.Show("Category Already Exists");
                    return;
                }
            }

            foreach (var a in spendingBudget.ToArray())
            {
                string[] arr = a.Key.Split(',');
                if (arr[0].Equals(name))
                {
                    string str = s + "," + arr[1] + "," + arr[2];
                    spendingBudget.Add(str, spendingBudget[a.Key]);
                    spendingBudget.Remove(a.Key);
                }
            }

            foreach (var a in categories)
            {
                if (a.Name.Equals(name)) a.Name = s;
            }
            foreach(var a in expenses)
            {
                if (a.Category.Equals(name)) a.Category = s;
            }
            CategoryUpdated.Invoke(sender,s);
        }
    }
}
