using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Notes.Interface;
using Notes.Models;
using Notes.Repositories;
using Xamarin.Forms;

namespace Notes.Services.SubService
{
    public class CalendarNoteService : IDataReader, IDataChanger
    {
        private readonly string filePath = DependencyService.Get<IFileService>().GetFilePath("calendar.xml");
        private readonly XmlSerializer formatter = new XmlSerializer(typeof(CalendarNoteRepository));

        public CalendarNoteRepository Repository { get; set; } = new CalendarNoteRepository();

        public CalendarNoteService()
        {

        }

        public CalendarNoteService(CalendarNoteRepository repository)
        {
            Repository = repository;
        }

        public void AddToXmlRoot(object obj)
        {
            if (obj is CalendarNote note)
            {
                note.Id = Repository.CalendarNotes.Count >= 1 ? Repository.CalendarNotes.Max(n => n.Id) + 1 : 1;
                Repository.CalendarNotes.Add(note);
            }
            SaveToFile();
        }

        public void DeleteFromXmlRoot(int id)
        {
            foreach (var note in Repository.CalendarNotes)
                if (note.Id == id)
                {
                    Repository.CalendarNotes.Remove(note);
                    break;
                }
            SaveToFile();
        }

        public void ChangeElementOnRoot(int id, object obj)
        {
            if (obj is CalendarNote newNote)
                foreach (var note in Repository.CalendarNotes)
                    if (note.Id == id)
                    {
                        newNote.Id = id;
                        Repository.CalendarNotes.Remove(note);
                        Repository.CalendarNotes.Add(newNote);
                        break;
                    }
            Repository.CalendarNotes = (from c in Repository.CalendarNotes
                                        orderby c.Id
                                        select c).ToList();
            SaveToFile();
        }

        public void ReadFromFile()
        {
            if (File.Exists(filePath))
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    Repository = (CalendarNoteRepository)formatter.Deserialize(fs);
        }

        private void SaveToFile()
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                formatter.Serialize(fs, Repository);
        }

        public object GetRepository()
        {
            return Repository;
        }
    }
}
