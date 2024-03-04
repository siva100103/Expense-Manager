using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace EvaluationPart_2
{
    public partial class ExpenseManager : Form
    {
        List<Expense> expenses = new List<Expense>();
        List<Category> categories = new List<Category>();

        Dictionary<string, int> MainBudgetControl = new Dictionary<string, int>();
        Dictionary<string, int> SpendedBudget = new Dictionary<string, int>();
        private DateTime MaxDate;
        private DateTime MinDate;

        public ExpenseManager()
        {
            InitializeComponent();
            dataGridView1.ReadOnly = dataGridView2.ReadOnly = dataGridView3.ReadOnly = dataGridView4.ReadOnly = true;

           // For Expense Page
            Edt.Columns.Add("Id", typeof(int));
            Edt.Columns.Add("Category", typeof(string));
            Edt.Columns.Add("Amount", typeof(int));
            Edt.Columns.Add("Date", typeof(string));
            Edt.Columns.Add("Notes", typeof(string));
            Edt.Columns.Add(" ", typeof(Image));
            Edt.Columns.Add("  ", typeof(Image));

            categories.AddRange(new Category[] {
            new Category { Id = 1, Name = "Food" },
            new Category { Id = 2, Name = "Travel" },
            new Category { Id = 3, Name = "Others" }
            });

            dataGridView1.DataSource = Edt;
            dataGridView1.Columns[0].Visible = dataGridView1.Columns[5].Visible = dataGridView1.Columns[6].Visible = false;
            button3.Visible =button4.Visible= false;
            ExpensePanel.BackColor = Color.AliceBlue;


            //For CategoryPage
            Cdt.Columns.Add("Id", typeof(int));
            Cdt.Columns.Add("Name", typeof(string));
            Cdt.Columns.Add(" ", typeof(Image));
            Cdt.Columns.Add("  ", typeof(Image));
            dataGridView2.DataSource = Cdt;
            categories.ForEach( c => Cdt.Rows.Add(c.Id, c.Name, Properties.Resources.icons8_edit_24, Properties.Resources.icons8_remove_25));
            dataGridView2.Columns[2].Visible = dataGridView2.Columns[3].Visible = false;


            //For SortPage            
            Sdt.Columns.Add("Id", typeof(int));
            Sdt.Columns.Add("Category", typeof(string));
            Sdt.Columns.Add("Amount", typeof(int));
            Sdt.Columns.Add("Date", typeof(string));
            Sdt.Columns.Add("Notes", typeof(string));
            dataGridView3.DataSource = Sdt;
            comboBox1.SelectedItem = "All";
            MaxDate=MinDate= dateTimePicker2.Value=dateTimePicker1.Value = DateTime.Now;

            foreach (var a in categories) comboBox1.Items.Add(a.Name);

            dataGridView3.Columns[0].Visible = false;
            comboBox1.Text = "All";

            //For Sort Page
            Bdt.Columns.Add("Category",typeof(string));
            Bdt.Columns.Add("Limit", typeof(int));
            Bdt.Columns.Add(" ", typeof(Image));
            Bdt.Columns.Add("  ", typeof(Image));
            dataGridView4.DataSource = Bdt;
            dataGridView4.Columns[2].Visible = dataGridView4.Columns[3].Visible = false;

            ExpenseTableUpdate();
            FilterTableUpdate();
            CategoryTableUpdate();
            BudgetTableUpdate();
        }

        //For Tab Switching

        private int SelectedTab = 1;

        private void mouseEnter(object sender, EventArgs e)
        {
            Label l = sender as Label;
            if (l.Parent.Name.Equals("ExpensePanel"))
            {
                ExpensePanel.BackColor = Color.AliceBlue;
            }
            else if (l.Parent.Name.Equals("CategoriesPanel"))
            {
                CategoriesPanel.BackColor = Color.AliceBlue;
            }
            else if(l.Parent.Name.Equals("SortPanel"))
            {
                SortPanel.BackColor = Color.AliceBlue;
            }
            else
            {
                BudgetPanel.BackColor = Color.AliceBlue;
            }
        }

        private void mouseLeave(object sender, EventArgs e)
        {
            Label l = sender as Label;
            if (l.Parent.Name.Equals("ExpensePanel") && SelectedTab != 1)
            {
                ExpensePanel.BackColor = Color.FromArgb(192, 255, 255);
            }
            else if (l.Parent.Name.Equals("CategoriesPanel") && SelectedTab != 2)
            {
                CategoriesPanel.BackColor = Color.FromArgb(192, 255, 255);
            }
            else if (l.Parent.Name.Equals("SortPanel") && SelectedTab != 3)
            {
                SortPanel.BackColor = Color.FromArgb(192, 255, 255);
            }
            else if(l.Parent.Name.Equals("BudgetPanel") && SelectedTab != 4)
            {
                BudgetPanel.BackColor = Color.FromArgb(192, 255, 255);
            }
        }

        private void mouseClick(object sender, EventArgs e)
        {
            Label l = sender as Label;
            if (l.Parent.Name.Equals("ExpensePanel"))
            {
                ExpensePanel.BackColor = Color.AliceBlue;
                CategoriesPanel.BackColor = BudgetPanel.BackColor = SortPanel.BackColor = Color.FromArgb(192, 255, 255);
                tabControl1.SelectedTab = ExpensePage;
                SelectedTab = 1;
            }
            else if (l.Parent.Name.Equals("CategoriesPanel"))
            {
                CategoriesPanel.BackColor = Color.AliceBlue;
                ExpensePanel.BackColor = BudgetPanel.BackColor = SortPanel.BackColor = Color.FromArgb(192, 255, 255);
                SelectedTab = 2;
                tabControl1.SelectedTab = CategoriesPage;

            }
            else if(l.Parent.Name.Equals("SortPanel"))
            {
                SortPanel.BackColor = Color.AliceBlue;
                ExpensePanel.BackColor = BudgetPanel.BackColor = CategoriesPanel.BackColor = Color.FromArgb(192, 255, 255);
                tabControl1.SelectedTab = SortPage;
                SelectedTab = 3;
            }
            else
            {
                BudgetPanel.BackColor = Color.AliceBlue;
                ExpensePanel.BackColor = CategoriesPanel.BackColor =SortPanel.BackColor= Color.FromArgb(192, 255, 255);
                tabControl1.SelectedTab = BudgetPage;
                SelectedTab = 4;
            }

            dataGridView1.Columns[5].Visible = dataGridView1.Columns[6].Visible  = false;
            dataGridView2.Columns[2].Visible = dataGridView2.Columns[3].Visible = false;
        }



        //For Expense Page

        DataTable Edt = new DataTable();
        private int ExpenseCount = 1;

        private void MenuButtonClick(object sender, EventArgs e)
        {
            ContentPanel.Visible = !ContentPanel.Visible;
        }

        private void E_AddButtonClick(object sender, EventArgs e)
        {
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[6].Visible = false;
            AddExpenseForm fm = new AddExpenseForm(categories);
            fm.GetData += (c, amount, date, note) =>
            {
                Expense ex = new Expense
                {
                    Id = ExpenseCount,
                    Category = c.Name,
                    Amount = amount,
                    Notes = note,
                    Date = date,
                };
                if (date > MaxDate)
                {
                    MaxDate = date;
                    dateTimePicker2.Value = MaxDate;
                }
                if (date < MinDate)
                {
                    MinDate = date;
                    dateTimePicker1.Value = MinDate;
                }
                expenses.Add(ex);
                button3.Visible = true;
                button4.Visible = true;
                fm.Close();
                BudgetUpdaterAndChecker(amount,c.Name,date);
                
                Edt.Rows.Add(ExpenseCount++, c.Name, amount, date.ToShortDateString() + " " + date.ToShortTimeString(), note, Properties.Resources.icons8_remove_25, Properties.Resources.icons8_edit_24);
                Sdt.Rows.Add(ExpenseCount - 1, c.Name, amount, date.ToShortDateString() + " " + date.ToShortTimeString(), note);
            };
           
            fm.Show();
        }

        private void E_EditButtonClick(object sender, EventArgs e)
        {
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[6].Visible = true;
        }

        private void E_RemoveButtonClick(object sender, EventArgs e)
        {
            dataGridView1.Columns[5].Visible = true;
            dataGridView1.Columns[6].Visible = false;
        }

        private void ExpenseDeleterAndUpdater(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 6 && e.RowIndex != -1)
            {
                int id = Int32.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                Expense expense = expenses.Find((exp) => exp.Id == id);
                expenses.Remove(expense);
                EditForm ed = new EditForm(expense, categories,SpendedBudget);
                ed.Show();
                ed.EditFinished += (s, o) =>
                {
                    ed.Close();
                    ExpenseTableUpdate();
                    FilterTableUpdate();
                    MessageBox.Show("Expense Updated");
                };
                ed.FormClosed += (sen, ev) =>
                {
                    dataGridView1.Columns[6].Visible = false;
                };
                expenses.Add(expense);
            }
            else if (e.ColumnIndex == 5 && e.RowIndex != -1)
            {
                int id = Int32.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                DialogResult result = MessageBox.Show("Do You Really Want to Remove The Expense", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    foreach (var a in expenses.ToArray())
                    {
                        if (id == a.Id)
                        {
                            string s = a.Category + "," + a.Date.Month + "," + a.Date.Year;
                            SpendedBudget[s] -= a.Amount;
                            expenses.Remove(a);
                            
                        }
                    }
                   
                    if (expenses.Count == 0)
                    {
                        button3.Visible = false;
                        button4.Visible = false;
                    }
                    ExpenseTableUpdate();
                    FilterTableUpdate();
                }
                dataGridView1.Columns[5].Visible = false;
            }
            
        }

        private void ExpenseTableUpdate()
        {
            Edt.Rows.Clear();

            foreach (var a in expenses) Edt.Rows.Add(a.Id, a.Category, a.Amount, a.Date.ToShortDateString() + " " + a.Date.ToShortTimeString(), a.Notes, Properties.Resources.icons8_remove_25, Properties.Resources.icons8_edit_24);

            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[6].Visible = false;
        }

        private void BudgetUpdaterAndChecker(int amount,string category,DateTime date)
        {
            string s = category + "," + date.Month + "," + date.Year;
            
                if (SpendedBudget.ContainsKey(s)) SpendedBudget[s] += amount;
                else SpendedBudget.Add(s, amount);  

            if (MainBudgetControl.ContainsKey(category) && !MainBudgetControl.ContainsKey(s))
            {
                if (SpendedBudget[s] >= MainBudgetControl[category]) MessageBox.Show("Your Limit Reached!!!");
            }

            if (MainBudgetControl.ContainsKey(s))
            {
                if(SpendedBudget[s]>MainBudgetControl[s]) MessageBox.Show("Your Limit Reached!!!");
            }
        }


        //For Category Page

        DataTable Cdt = new DataTable();
        private int CategoryCount = 4;

        private void AddCategoryButtonClick(object sender, EventArgs e)
        {
            AddCategoryForm ac = new AddCategoryForm(categories, CategoryCount);
            dataGridView2.Columns[2].Visible = false;
            dataGridView2.Columns[3].Visible = false;
            ac.AddFinished += (ob, s) =>
            {
                ac.Close();
                Cdt.Rows.Add(CategoryCount++, s, Properties.Resources.icons8_edit_24, Properties.Resources.icons8_remove_25);
                Category ct = categories.Find(c => c.Id == 3);
                categories.Remove(ct);
                categories.Sort((cat, cat1) => cat.Name.CompareTo(cat1.Name));
                categories.Add(ct);
                comboBox1.Items.Add(s);
                //FilterTableUpdate();
                CategoryTableUpdate();
            };
            ac.Show();
        }

        private void EditCategoryButtonClick(object sender, EventArgs e)
        {
            dataGridView2.Columns[2].Visible = true;
            dataGridView2.Columns[3].Visible = false;
        }

        private void DeleteCategoryButtonClick(object sender, EventArgs e)
        {
            dataGridView2.Columns[2].Visible = false;
            dataGridView2.Columns[3].Visible = true;
        }

        private void CategoryDeleterAndUpdater(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex == 2)
            {
                string OldCategoryName = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString();
                CategoryEditForm ce = new CategoryEditForm(categories, OldCategoryName, expenses,SpendedBudget);
                ce.Show();
                ce.FormClosed += (se, ev) => dataGridView2.Columns[2].Visible = false;
                ce.CategoryUpdated += (ob, NewCategoryName) =>
                {
                    ExpenseTableUpdate();
                    comboBox1.Items.Remove(OldCategoryName);
                    comboBox1.Items.Add(NewCategoryName);
                    CategoryTableUpdate();
                    FilterTableUpdate();
                    ce.Close();
                    MessageBox.Show("Category Updated");
                    if (MainBudgetControl.ContainsKey(OldCategoryName))
                    {
                        MainBudgetControl.Add(NewCategoryName, MainBudgetControl[OldCategoryName]);
                        MainBudgetControl.Remove(OldCategoryName);
                    }
                    BudgetTableUpdate();
                };
            }
            else if (e.RowIndex != -1 && e.ColumnIndex == 3)
            {
                string CategoryName = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString();
                if (CategoryName.Equals("Others"))
                {
                    MessageBox.Show("You Cannot Delete This Category");
                    return;
                }
                DialogResult res = MessageBox.Show("Do you Want to Delete this category", "Confirmation", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {    
                    comboBox1.Items.Remove(CategoryName);
                    foreach (var a in categories.ToArray())
                    {
                        if (a.Name.Equals(CategoryName)) categories.Remove(a);
                    }
                    DialogResult result = MessageBox.Show("Do You Want To Remove The Expense in the Category", "Confirmation", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        foreach (var a in expenses.ToArray())
                        {
                            if (a.Category.Equals(CategoryName)) expenses.Remove(a);
                        }
                        dataGridView2.Columns[3].Visible = false;
                    }
                    else
                    {
                        foreach (var a in expenses.ToArray())
                        {
                            if (a.Category.Equals(CategoryName))
                            {
                                string s = "Others" +","+ a.Date.Month+","+a.Date.Year;
                                a.Category = "Others";
                                if (SpendedBudget.ContainsKey(s)) SpendedBudget[s] += a.Amount;
                                else SpendedBudget[s] = a.Amount;
                                SpendedBudget.Remove(CategoryName + "," + a.Date.Month + "," + a.Date.Year);
                            }
                        }
                    }
                    
                    if (categories.Count == 1)
                    {
                        DeleteCategoryButton.Visible = false;
                        EditCategoryButton.Visible = false;
                    }
                    MainBudgetControl.Remove(CategoryName);
                }
            }
            ExpenseTableUpdate();
            CategoryTableUpdate();
            FilterTableUpdate();
            BudgetTableUpdate();
        }

        private void CategoryTableUpdate()
        {
            Cdt.Rows.Clear();
            foreach (var a in categories)
            {
                Cdt.Rows.Add(a.Id, a.Name, Properties.Resources.icons8_edit_24, Properties.Resources.icons8_remove_25);
            }
        }


        //For Sort Page

        DataTable Sdt = new DataTable();

        private bool DateFallsInRange(DateTime from,DateTime to,DateTime current)
        {
            return ((current.Date >= from.Date && current.Date <= to.Date));
            
        }

        private void FilterTableUpdate()
        {
            string s = comboBox1.SelectedItem.ToString();
            DateTime from = dateTimePicker1.Value;
            DateTime to = dateTimePicker2.Value;
            Sdt.Rows.Clear();
            foreach (var a in expenses)
            {
                if ((a.Category.Equals(s) || s.Equals("All")) && DateFallsInRange(from, to, a.Date)) Sdt.Rows.Add(a.Id, a.Category, a.Amount, a.Date, a.Notes);
            }
        }

        private void RestartFilter(object sender,EventArgs e)
        {
            FilterTableUpdate();
            dataGridView3.Sort(dataGridView3.Columns[3],System.ComponentModel.ListSortDirection.Ascending);
        }

        //For Budget Page
        DataTable Bdt = new DataTable();
        private void AddBudgetButton_Click(object sender, EventArgs e)
        {
            dataGridView4.Columns[2].Visible = false;
            dataGridView4.Columns[3].Visible = false;
            AddBudgetForm fm = new AddBudgetForm(categories,MainBudgetControl);
            fm.Show();
            fm.FormClosed += (se, ev) =>
            {
                BudgetTableUpdate();
                if (MainBudgetControl.Count == categories.Count)
                {
                    AddBudgetButton.Visible = false;
                }
                DeleteBudgetButton.Visible = true;
                EditBudgetButton.Visible = true;
            };
        }

        private void EditBudgetButton_Click(object sender, EventArgs e)
        {
            dataGridView4.Columns[2].Visible = true;
            dataGridView4.Columns[3].Visible = false;
            dataGridView4.ReadOnly = false;
            dataGridView4.Columns[0].ReadOnly = true;
        }

        private void DeleteBudgetButton_Click(object sender, EventArgs e)
        {
            dataGridView4.Columns[2].Visible = false;
            dataGridView4.Columns[3].Visible = true;
            AddBudgetButton.Visible = true;     
        }

        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex!=-1 && e.ColumnIndex == 2)
            {
                dataGridView4.Columns[2].Visible = false;
            }
            else if(e.RowIndex!=-1 && e.ColumnIndex == 3)
            {
                DialogResult res = MessageBox.Show("Do You Want to remove the Budget", "Confirmation", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    DataGridViewCell cell = dataGridView4.Rows[e.RowIndex].Cells[0];
                    string s = cell.Value.ToString();
                    MainBudgetControl.Remove(s);
                    BudgetTableUpdate();
                    if (MainBudgetControl.Count == 0)
                    {
                        DeleteBudgetButton.Visible = EditBudgetButton.Visible = false;
                    }
                }
                dataGridView4.Columns[3].Visible = false;
            }
        }

        private void BudgetTableUpdate()
        {
            Bdt.Rows.Clear();
            foreach (var a in MainBudgetControl)
            {
                if (a.Key.Split(',').Length != 1) continue;
                Bdt.Rows.Add(a.Key, a.Value, Properties.Resources.icons8_edit_24, Properties.Resources.icons8_remove_25);
            }  
        }

        private void dataGridView4_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow cell = dataGridView4.Rows[e.RowIndex];
            string s = cell.Cells[0].Value.ToString();
            int n = (int)cell.Cells[1].Value;
            MainBudgetControl[s] = n;
            dataGridView4.ReadOnly = true;
            BudgetTableUpdate();
        }

        private void AddCustomBudgetButton_Click(object sender, EventArgs e)
        {
            AddCustomBudget cb = new AddCustomBudget(categories,MainBudgetControl);
            cb.Show();
        }
    }


    public class Expense
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; } = "";
    }
    
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
