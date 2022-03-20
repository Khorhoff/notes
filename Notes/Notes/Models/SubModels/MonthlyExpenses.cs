using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Notes.Models.SubModels
{
    [Serializable]
    public class MonthlyExpenses
    {
        public int MonthNumber { get; set; }
        public int Year { get; set; }
        [XmlArray("CategorieExpenses")]
        public ExpenseData CategorieExpenses { get; set; } = new ExpenseData();

        public MonthlyExpenses()
        {
            
        }

        public MonthlyExpenses(int month, int year)
        {
            MonthNumber = month;
            Year = year;
        }
    }
}
