using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Notes.Interface;
using Notes.Models;
using Notes.Models.SubModels;
using Notes.Repositories;
using Xamarin.Forms;

namespace Notes.Services.SubService
{
    public class StickyNoteService : IDataReader, IDataChanger
    {
        private readonly string filePath = DependencyService.Get<IFileService>().GetFilePath("notes.xml");
        private readonly XmlSerializer formatter = new XmlSerializer(typeof(StickyNoteRepository));

        public StickyNoteRepository Repository { get; set; } = new StickyNoteRepository();

        public StickyNoteService()
        {

        }

        public StickyNoteService(StickyNoteRepository repository)
        {
            Repository = repository;
        }

        public void AddToXmlRoot(object obj)
        {
            if (obj is StickyNote note)
            {
                note.Id = Repository.StickyNotes.Count >= 1 ? Repository.StickyNotes.Max(n => n.Id) + 1 : 1;
                Repository.StickyNotes.Add(note);
            }
            SaveToFile();
        }

        public void DeleteFromXmlRoot(int id) 
        {
            foreach (var note in Repository.StickyNotes)
                if (note.Id == id)
                {
                    Repository.StickyNotes.Remove(note);
                    break;
                }
            SaveToFile();
        }

        public void ChangeElementOnRoot(int id, object obj)
        {
            if (obj is StickyNote newNote)
                foreach (var note in Repository.StickyNotes)
                    if (note.Id == id)
                    {
                        newNote.Id = id;
                        newNote.LastModifiedDate = DateTime.Now.ToShortDateString();
                        Repository.StickyNotes.Remove(note);
                        Repository.StickyNotes.Add(newNote);
                        break;
                    }
            Repository.StickyNotes = (from n in Repository.StickyNotes
                                     orderby n.Id
                                     select n).ToList();
            SaveToFile();
        }

        public void ReadFromFile() 
        {
            if (File.Exists(filePath))
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    Repository = (StickyNoteRepository)formatter.Deserialize(fs);
        }

        public void SaveToFile()
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
