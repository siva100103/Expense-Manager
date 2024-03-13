using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationPart_2
{
    public class Expense
    {
        public int Id { get; set; }

        public string Category { get; set; }

        public int Amount { get; set; }

        public DateTime Date { get; set; }

        public string Notes { get; set; } = "";

    }
}
