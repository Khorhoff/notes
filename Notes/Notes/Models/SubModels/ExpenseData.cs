using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Notes.Models.SubModels
{
    [Serializable]
    public class ExpenseData : ObservableCollection<MonthlyExpenseByCategory>
    {
        public ExpenseData()
        {
            
        }
    }
}
