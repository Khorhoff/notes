using Notes.Models;
using Notes.Models.SubModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Notes.Repositories
{
    [XmlRoot]
    public class StickyNoteRepository
    {
        public List<StickyNote> StickyNotes { get; set; } = new List<StickyNote>();
    }
}
