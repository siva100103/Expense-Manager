using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace EvaluationPart_2
{
    public partial class ExpenseManager : Form
    {
        List<Expense> expenses = new List<Expense>();
        List<Category> categories = new List<Category>();
        string con;
        MySqlConnection Connection;

        Dictionary<string, int> MainBudgetControl = new Dictionary<string, int>();
        Dictionary<string, int> SpendedBudget = new Dictionary<string, int>();
        private DateTime MaxDate=DateTime.Now;
        private DateTime MinDate=DateTime.Now;

        public ExpenseManager()
        {
            InitializeComponent();
            dataGridView1.ReadOnly = dataGridView2.ReadOnly = dataGridView3.ReadOnly = dataGridView4.ReadOnly = true;
            con = "server=localhost;port=3306;uid=root;pwd=Suriya@123;database=expensedatabase";
            Connection = new MySqlConnection(con);
            Connection.Open();
            // For Expense Page
            Edt.Columns.Add("Id", typeof(int));
            Edt.Columns.Add("Category", typeof(string));
            Edt.Columns.Add("Amount", typeof(int));
            Edt.Columns.Add("Date", typeof(string));
            Edt.Columns.Add("Notes", typeof(string));
            Edt.Columns.Add(" ", typeof(Image));
            Edt.Columns.Add("  ", typeof(Image));
            dataGridView1.DataSource = Edt;
            dataGridView1.Columns[0].Visible  = false;
            ExpensePanel.BackColor = Color.AliceBlue;


            //For CategoryPage
            Cdt.Columns.Add("Id", typeof(int));
            Cdt.Columns.Add("Name", typeof(string));
            Cdt.Columns.Add(" ", typeof(Image));
            Cdt.Columns.Add("  ", typeof(Image));
            dataGridView2.DataSource = Cdt;


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
            dataGridView4.ReadOnly = false;
            dataGridView4.Columns[2].Visible = false;
            dataGridView4.Columns[0].ReadOnly = true;

            ExpenseTableUpdate();
            FilterTableUpdate();
            CategoryTableUpdate();
            BudgetTableUpdate();
        }

        // For Db Operations

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            MySqlDataReader reader = new MySqlCommand("Select * From Expenses",Connection).ExecuteReader();
            while (reader.Read())
            {
                Expense ex = new Expense
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Amount = Convert.ToInt32(reader["Amount"]),
                    Category = reader["Category"].ToString(),
                    Date = (DateTime)reader["Date"],
                    Notes = reader["Notes"].ToString()
                };
                expenses.Add(ex);
            }
            reader.Close();
           
            reader = new MySqlCommand("Select * From Categories",Connection).ExecuteReader();
            while (reader.Read())
            {
                Category c = new Category
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString()
                };
                comboBox1.Items.Add(c.Name);
                categories.Add(c);
            }
            reader.Close();

            reader = new MySqlCommand("Select * From MainBudgetControl",Connection).ExecuteReader();
            while (reader.Read())
            {
                MainBudgetControl.Add(reader["Period"].ToString(), Convert.ToInt32(reader["Budget"]));
            }
            reader.Close();
            expenses.ForEach((ex) =>
            {
                string s =$"{ex.Category},{ex.Date.Month.ToString()},{ex.Date.Year.ToString()}";
                if (SpendedBudget.ContainsKey(s)) SpendedBudget[s] += ex.Amount;
                else SpendedBudget.Add(s, ex.Amount);
            });
            ExpenseCount = expenses.Count+1;
           if (categories.Count != 0) CategoryCount = categories[categories.Count - 1].Id + 1;
            ExpenseTableUpdate();
            CategoryTableUpdate();
            BudgetTableUpdate();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MySqlCommand command = new MySqlCommand("Delete from Expenses",Connection);
            command.ExecuteNonQuery();
            int count = 1;
            expenses.Sort((ex, ex1) => ex.Amount.CompareTo(ex1.Amount));
            categories.Sort((c, c1) => c.Id.CompareTo(c1.Id));
            expenses.ForEach((ex) =>
            {
                ex.Id = count++;
                command = new MySqlCommand("Insert into Expenses values(@Id,@Category,@Date,@Amount,@Notes)",Connection);
                command.Parameters.AddWithValue("@Id", ex.Id);
                command.Parameters.AddWithValue("@Category", ex.Category);
                command.Parameters.AddWithValue("@Date", ex.Date);
                command.Parameters.AddWithValue("@Amount", ex.Amount);
                command.Parameters.AddWithValue("@Notes", ex.Notes);
                command.ExecuteNonQuery();
            });
            command = new MySqlCommand("Delete From Categories", Connection);
            command.ExecuteNonQuery();
            categories.ForEach((c) =>
            {
                command = new MySqlCommand("Insert into Categories Values(@Id,@Name)", Connection);
                command.Parameters.AddWithValue("@Id", c.Id);
                command.Parameters.AddWithValue("@Name", c.Name);
                command.ExecuteNonQuery();
            });
            command = new MySqlCommand("Delete From MainBudgetControl",Connection);
            command.ExecuteNonQuery();
            foreach(var a in MainBudgetControl)
            {
                command =new MySqlCommand("Insert into MainBudgetControl Values(@Period,@Budget)",Connection);
                command.Parameters.AddWithValue("@Period",a.Key);
                command.Parameters.AddWithValue("@Budget",a.Value);
                command.ExecuteNonQuery();
            }
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
                fm.Close();
                BudgetUpdaterAndChecker(amount,c.Name,date);
                
                Edt.Rows.Add(ExpenseCount++, c.Name, amount, date.ToShortDateString() + " " + date.ToShortTimeString(), note, Properties.Resources.icons8_remove_25, Properties.Resources.icons8_edit_24);
                Sdt.Rows.Add(ExpenseCount - 1, c.Name, amount, date.ToShortDateString() + " " + date.ToShortTimeString(), note);
            };
           
            fm.Show();
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
                    ExpenseTableUpdate();
                    FilterTableUpdate();
                }
            }
            
        }

        private void ExpenseTableUpdate()
        {
            Edt.Rows.Clear();
            foreach (var a in expenses) Edt.Rows.Add(a.Id, a.Category, a.Amount, a.Date.ToShortDateString() + " " + a.Date.ToShortTimeString(), a.Notes, Properties.Resources.icons8_remove_25, Properties.Resources.icons8_edit_24);
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

        private void CategoryDeleterAndUpdater(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex == 2)
            {
                string OldCategoryName = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString();
                CategoryEditForm ce = new CategoryEditForm(categories, OldCategoryName, expenses,SpendedBudget);
                ce.Show();
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
            AddBudgetForm fm = new AddBudgetForm(categories,MainBudgetControl);
            fm.Show();
            fm.FormClosed += (se, ev) =>
            {
                BudgetTableUpdate();
                if (MainBudgetControl.Count == categories.Count)
                {
                    AddBudgetButton.Visible = false;
                }
            };
        }

        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if(e.RowIndex!=-1 && e.ColumnIndex == 3)
            {
                DialogResult res = MessageBox.Show("Do You Want to remove the Budget", "Confirmation", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    DataGridViewCell cell = dataGridView4.Rows[e.RowIndex].Cells[0];
                    string s = cell.Value.ToString();
                    MainBudgetControl.Remove(s);
                    BudgetTableUpdate();
                }
            }
        }

        private void BudgetTableUpdate()
        {
            Bdt.Rows.Clear();
            foreach (var a in MainBudgetControl)
            {
               
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
            cb.FormClosed += (ob, cv) => BudgetTableUpdate();
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
