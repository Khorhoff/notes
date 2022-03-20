using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Notes.Models
{
    [Serializable]
    public class StickyNote
    {
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }
        [XmlElement(ElementName = "Title")]
        public string NoteTitle { get; set; }
        public string LastModifiedDate { get; set; }
        public int IdCategory { get; set; } = 0;
        public string Text { get; set; }

        public StickyNote()
        {

        }

        public StickyNote(string title, string text, int idCategory)
        {
            NoteTitle = title;
            Text = text;
            IdCategory = idCategory;
            LastModifiedDate = DateTime.Now.ToShortDateString();
        }
    }
}
