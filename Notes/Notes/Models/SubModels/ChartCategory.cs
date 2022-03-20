using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Notes.Models.SubModels
{
    public class ChartCategory
    {
        public double Amount { get; set; }
        public string LegendLabel { get; set; }
        public int IdCategory { get; set; }
        public string CategoryName { get; set; }
        public Color CategoryColor { get; set; }

        public ChartCategory()
        {

        }

        public ChartCategory(double amount, int idCategory)
        {
            Amount = amount;
            LegendLabel = $"{amount} p.";
            IdCategory = idCategory;
            CategoryName = Settings.PurseNoteCategories[idCategory].Name;
            CategoryColor = Settings.PurseNoteCategories[idCategory].CategoryColor;
        }
    }
}
