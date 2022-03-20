using Notes.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Notes.Repositories
{
    [XmlRoot]
    public class CalendarNoteRepository
    {
        public List<CalendarNote> CalendarNotes { get; set; } = new List<CalendarNote>();
    }
}
