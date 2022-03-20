using Notes.Models.SubModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Notes.Models
{
    [Serializable]
    public class Purse
    {
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public int IdCategory { get; set; }
        [XmlArray("Notes"),
        XmlArrayItem("Transaction", typeof(BudgetNote)),
        XmlArrayItem("Remittance", typeof(RemittanceNote))]
        public List<BudgetNote> BudgetNotes { get; set; } = new List<BudgetNote>();

        public Purse()
        {

        }

        public Purse(string name, string description, int idCategory)
        {
            Name = name;
            Description = description;
            Amount = 0;
            IdCategory = idCategory;
        }
    }
}
