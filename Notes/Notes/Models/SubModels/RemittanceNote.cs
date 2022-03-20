using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Models.SubModels
{
    [Serializable]
    public class RemittanceNote : BudgetNote
    {
        public int IdSecondPurse { get; set; }

        public RemittanceNote()
        {

        }

        public RemittanceNote(double amount, int idCategory, int idSecondPurse) : base(amount, idCategory)
        {
            ChangeType = "Remittance";
            IdSecondPurse = idSecondPurse;
        }
    }
}
