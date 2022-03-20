using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Notes.Models
{
    [Serializable]
    public class CalendarNote
    {
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }
        [XmlElement(ElementName = "Title")]
        public string NoteTitle { get; set; }
        public string Text { get; set; }
        public DateTime NoteDate { get; set; }
        public int IdCategory { get; set; }

        public CalendarNote()
        {

        }

        public CalendarNote(string title, string text, DateTime noteDate, int idCategory)
        {
            NoteTitle = title;
            Text = text;
            NoteDate = noteDate.Date;
            IdCategory = idCategory;
        }
    }
}
