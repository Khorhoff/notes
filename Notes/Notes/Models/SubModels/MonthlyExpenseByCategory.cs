using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Models.SubModels
{
    [Serializable]
    public class MonthlyExpenseByCategory
    {
        public double Amount { get; set; }
        public int IdCategory { get; set; }

        public MonthlyExpenseByCategory()
        {

        }

        public MonthlyExpenseByCategory(double amount, int idCategory)
        {
            Amount = amount;
            IdCategory = idCategory;
        }
    }
}
