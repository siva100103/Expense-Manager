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
    public partial class AddCategoryForm : Form
    {
        List<Category> categories;
        private int count;
        public event EventHandler<string> AddFinished;
        public AddCategoryForm(List<Category> categorie,int Count)
        {
            InitializeComponent();
            categories = categorie;
            count = Count;
        }

        private void AddButtonClick(object sender,EventArgs e)
        {
            string s = richTextBox2.Text.Trim();
            if (s.Equals(""))
            {
                MessageBox.Show("Enter a Valid Category:");
                return;
            }
            foreach(var a in categories)
            {
                if (a.Name.ToLower().Equals(s.ToLower()))
                {
                    MessageBox.Show("category Already Exist");
                    return;
                }
            }
            Category c = new Category
            {
                Id = count,
                Name = s
            };
            categories.Add(c);
            AddFinished?.Invoke(this, s);
        }
    }
}
