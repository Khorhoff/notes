using Notes.Models;
using Notes.Models.SubModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Notes.Repositories
{
    [XmlRoot]
    public class PurseRepository
    {
        public List<Purse> Purses { get; set; } = new List<Purse>();
        public List<MonthlyExpenses> Expenses { get; set; } = new List<MonthlyExpenses>();
    }
}
