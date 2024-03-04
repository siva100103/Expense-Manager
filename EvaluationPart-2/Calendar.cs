using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EvaluationPart_2
{
    public partial class Calendar : UserControl
    {
        private int widgetHeight;
        private int labelHeight;
        private int labelWidth;
        Point CurrentLocation;
        List<Label> labels = new List<Label>();
        string[] months = new string[] { "Jan", "Feb", "Mar", "Apr", "May","Jun", "July", "Aug", "Sep", "Oct", "Nov", "Dec"};
        int year = 2024;
        int month = 02;
        public Calendar()
        {
            InitializeComponent();
            labelHeight = panel2.Height / 7;
            labelWidth = panel2.Width / 7;
            comboBox1.Text = "Feb";
            MinimumSize = new Size(300, 200);
            widgetHeight = Width / 5;
            comboBox1.Width = widgetHeight;
            numericUpDown1.Width = widgetHeight;
            button1.Width = widgetHeight;
            panel1.Height = Height / 5;
            panel2.Height = (Height / 5) * 4;
            //Height += Height % 7;
            CurrentLocation = new Point(0, 0);
            foreach(var a in months)
            {
                comboBox1.Items.Add(a);
            }
            comboBox1.SelectedItem = "Feb";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            int dt = DateTime.DaysInMonth(2024,2);
            for(int i = 0; i < 7; i++)
            {
                CurrentLocation.X = 0;
                for(int j = 0; j < 7; j++)
                {
                    Label l = new Label { Height = labelHeight, Width = labelWidth, Location = CurrentLocation ,BorderStyle=BorderStyle.FixedSingle,AutoSize=false,Font=new Font(FontFamily.GenericSansSerif,12)};
                    
                    if (i == 0 && j == 0)
                    {
                        l.Text = "Sun";
                        
                    }
                    if (i == 0 && j == 1) l.Text = "Mon";
                    if (i == 0 && j == 2) l.Text = "Tue";
                    if (i == 0 && j == 3) l.Text = "Wed";
                    if (i == 0 && j == 4) l.Text = "Thur";
                    if (i == 0 && j == 5) l.Text = "Fri";
                    if (i == 0 && j == 6) l.Text = "Sat";

                    CurrentLocation.X += labelWidth;
                    panel2.Controls.Add(l);
                    labels.Add(l);
                }
                CurrentLocation.Y += labelHeight;
            }
            DateNow(year, month);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            labelHeight = panel2.Height / 7;
            labelWidth = panel2.Width / 7;
            CurrentLocation = new Point(0, 0);
            int count = 0;
            foreach (var a in labels)
            {
                count++;
                a.Height = labelHeight;
                a.Width = labelWidth;
                a.Location = CurrentLocation;
                CurrentLocation.X += labelWidth;
                if (count == 7)
                {
                    count = 0;
                    CurrentLocation.X = 0;
                    CurrentLocation.Y += labelHeight;
                }
            }
            widgetHeight = Width / 5;
            comboBox1.Width = numericUpDown1.Width = button1.Width = widgetHeight;
            panel1.Height = Height / 5;
            panel2.Height = (Height / 5) * 4;
            comboBox1.Location= new Point((Width / 10), panel1.Height / 4);
            numericUpDown1.Location = new Point(2*(Width / 10)+Width/5, panel1.Height / 4);
            button1.Location= new Point(3*(Width / 10)+(2*Width/5), panel1.Height / 4);

        }

        private void DateNow(int year,int month)
        {

            if (year < 1)
            {
                numericUpDown1.Value = 1;
                return;
            }
            DateTime dt = new DateTime(year, month, 1);
            var t = DateTime.Now;
                
            int totDay = DateTime.DaysInMonth(year,month);
            int day =6+ (int)dt.DayOfWeek;
            int count = 1;
            int tot = 7;
            foreach(var a in labels)
            {
                if(tot--<=0) 
                a.Text = "";
                if (a.BackColor == Color.Aqua) a.BackColor = BackColor;
            }
            foreach(var a in labels)
            {
                if (day-- < 0)
                {
                    if(year==t.Year && month==t.Month && count == t.Day) { a.BackColor = Color.Aqua; }
                    a.Text = count.ToString();
                    count++;
                    totDay--;
                }
                if (totDay <= 0) break;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            month = Array.IndexOf(months,comboBox1.SelectedItem.ToString())+1;
            year = (int)numericUpDown1.Value;
            DateNow(year, month);
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            month = Array.IndexOf(months,comboBox1.SelectedItem.ToString())+1;
            year = (int)numericUpDown1.Value;
            DateNow(year, month);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            DateNow(2024, 02);
            DateTime t = DateTime.Now;
            comboBox1.Text = months[t.Month-1];
            numericUpDown1.Value = t.Year;
        }
    }
}
