using Notes.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Repositories
{
    public class DateNotesRepository
    {
        public List<CalendarNote> Notes { get; set; }
        public DateTime DateOfNotes { get; set; }

        public DateNotesRepository()
        {

        }
    }
}
