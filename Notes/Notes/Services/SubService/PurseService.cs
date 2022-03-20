using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Notes.Interface;
using Notes.Models;
using Notes.Models.SubModels;
using Notes.Repositories;
using Xamarin.Forms;

namespace Notes.Services.SubService
{
    public class PurseService : IDataReader, IDataChanger
    {
        private readonly string filePath = DependencyService.Get<IFileService>().GetFilePath("budget.xml");
        private readonly XmlSerializer formatter = new XmlSerializer(typeof(PurseRepository));

        public PurseRepository Repository { get; set; } = new PurseRepository();

        public PurseService()
        {

        }

        public PurseService(PurseRepository repository)
        {
            Repository = repository;
        }

        public void AddToXmlRoot(object obj)
        {
            if (obj is Purse purse)
            {
                purse.Id = Repository.Purses.Count >= 1 ? Repository.Purses.Max(n => n.Id) + 1 : 1;
                Repository.Purses.Add(purse);
            }
            SaveToFile();
        }

        public void DeleteFromXmlRoot(int id)
        {
            foreach (var purse in Repository.Purses)
                if (purse.Id == id)
                {
                    Repository.Purses.Remove(purse);
                    break;
                }
            SaveToFile();
        }

        public void ChangeElementOnRoot(int id, object obj)
        {
            if (obj is Purse newPurse)
            {
                foreach (var purse in Repository.Purses)
                    if (purse.Id == id)
                    {
                        newPurse.Id = id;
                        Repository.Purses.Remove(purse);
                        Repository.Purses.Add(newPurse);
                        break;
                    }
                Repository.Purses = (from p in Repository.Purses
                                     orderby p.Id
                                     select p).ToList();
            }
            else if(obj is BudgetNote newNote)
            {
                foreach (var purse in Repository.Purses)
                    if (purse.Id == id)
                    {
                        purse.BudgetNotes.Add(newNote);
                        purse.Amount += newNote.Amount;
                        break;
                    }
                if (newNote is RemittanceNote newRemitt)
                    foreach (var purse in Repository.Purses)
                        if (purse.Id == newRemitt.IdSecondPurse)
                        {
                            purse.Amount -= newRemitt.Amount;
                            break;
                        }
                if (newNote.ChangeType == "Transaction" && newNote.Amount < 0)
                {
                    bool isMonthlyE = false, isCategoryE = false;
                    foreach (var monthlyE in Repository.Expenses)
                    {
                        if (monthlyE.MonthNumber == newNote.NoteDateTime.Month && monthlyE.Year == newNote.NoteDateTime.Year)
                        {
                            isMonthlyE = true;
                            foreach (var categoryE in monthlyE.CategorieExpenses)
                            {
                                if (categoryE.IdCategory == newNote.IdCategory)
                                {
                                    isCategoryE = true;
                                    categoryE.Amount += (newNote.Amount * -1);
                                    break;
                                }
                            }
                            if (!isCategoryE)
                            {
                                MonthlyExpenseByCategory categoryE = new MonthlyExpenseByCategory(-newNote.Amount, newNote.IdCategory);
                                monthlyE.CategorieExpenses.Add(categoryE);
                            }
                            break;
                        }
                    }
                    if (!isMonthlyE)
                    {
                        MonthlyExpenseByCategory categoryE = new MonthlyExpenseByCategory(-newNote.Amount, newNote.IdCategory);
                        MonthlyExpenses monthlyE = new MonthlyExpenses(newNote.NoteDateTime.Month, newNote.NoteDateTime.Year);
                        monthlyE.CategorieExpenses.Add(categoryE);
                        Repository.Expenses.Add(monthlyE);
                    }
                }
            }
            SaveToFile();
        }

        public void ReadFromFile()
        {
            if (File.Exists(filePath))
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    Repository = (PurseRepository)formatter.Deserialize(fs);
        }

        private void SaveToFile()
        {
            if (Repository.Expenses.Count < 1)
                Repository.Expenses.Add(new MonthlyExpenses(1, 2000) { CategorieExpenses = new ExpenseData { new MonthlyExpenseByCategory(1, 1) } });
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                formatter.Serialize(fs, Repository);
        }

        public object GetRepository()
        {
            return Repository;
        }
    }
}
