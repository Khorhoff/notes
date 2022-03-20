using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Notes.Models.SubModels
{
    [Serializable]
    public class BudgetNote
    {
        public string ChangeType { get; set; }
        public double Amount { get; set; }
        public DateTime NoteDateTime { get; set; }
        public int IdCategory { get; set; }

        public BudgetNote()
        {

        }

        public BudgetNote(double amount, int idCategory)
        {
            ChangeType = "Transaction";
            Amount = amount;
            NoteDateTime = DateTime.Now;
            IdCategory = idCategory;
        }
    }
}
