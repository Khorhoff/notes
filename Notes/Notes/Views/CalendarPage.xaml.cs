using Notes.Models;
using Notes.Repositories;
using Notes.Services;
using Notes.Services.SubService;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Plugin.Calendar.Models;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarPage : ContentPage
    {
        readonly DataReadService Reader;
        public List<DateNotesRepository> NotesGroupByDate = new List<DateNotesRepository>();
        public CalendarNoteRepository Repository;
        public EventCollection Events { get; set; } = new EventCollection();
        public string SortParam = "Standart";
        public bool NoteListActive = false;

        public CalendarPage()
        {
            InitializeComponent();
            Reader = new DataReadService(new CalendarNoteService());
            Calendar.DayTappedCommand = new Command(() =>
            {
                UpdateNoteList(Calendar.SelectedDates);
            });
            UpdateCalendar();
        }

        public void UpdateCalendar()
        {
            Reader.ReadFromFile();
            Repository = (CalendarNoteRepository)Reader.GetRepository();
            if (Repository.CalendarNotes.Count >= 1)
            {
                NotesGroupByDate = (from n in Repository.CalendarNotes
                                    group n by new
                                    {
                                        n.NoteDate,
                                    } into dn
                                    select new DateNotesRepository()
                                    {
                                        DateOfNotes = dn.Key.NoteDate,
                                        Notes = dn.ToList(),
                                    }).ToList();
                Events = new EventCollection();
                foreach (var dn in NotesGroupByDate)
                {
                    Events.Add(dn.DateOfNotes, dn.Notes);
                }
            }
            Calendar.Events = Events;
            if (NoteListActive)
            {
                UpdateNoteList(Calendar.SelectedDates);
            }
        }

        public void UpdateNoteList(List<DateTime> currentDates)
        {
            List<CalendarNote> notesToList = new List<CalendarNote>();
            List<CalendarNote> notesInCalendar = new List<CalendarNote>();
            foreach (var date in currentDates)
            {
                foreach (var dateNote in NotesGroupByDate)
                {
                    if (dateNote.DateOfNotes.ToShortDateString() == date.ToShortDateString())
                    {
                        notesInCalendar = dateNote.Notes;
                    }
                }
                foreach (var note in notesInCalendar)
                {
                    notesToList.Add(note);
                }
            }

            switch (SortParam)
            {
                case "Title":
                    notesToList = (from n in notesToList
                                   orderby n.NoteTitle
                                   select n).ToList();
                    break;
                case "Category":
                    notesToList = (from n in notesToList
                                   orderby n.IdCategory
                                   select n).ToList();
                    break;
                default:
                    break;
            }

            NoteList.Children.Clear();
            if (notesToList.Count < 1)
            {
                NoteListActive = false;
                Label label = new Label()
                {
                    Text = "Нет заметок",
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                NoteList.Children.Add(label);
            }
            else
            {
                NoteListActive = true;
                foreach (var note in notesToList)
                {
                    Frame frame = new Frame()
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        BorderColor = Color.Red,
                        BackgroundColor = Color.Transparent,
                        Margin = 10,
                        Padding = 1
                    };

                    AbsoluteLayout layout = new AbsoluteLayout()
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        BackgroundColor = Settings.CalendarNoteCategories[note.IdCategory].CategoryColor,
                        Margin = 0,
                        Padding = 0
                    };
                    frame.Content = layout;

                    Label info = new Label()
                    {
                        Text = $"{Settings.CalendarNoteCategories[note.IdCategory].Name}",
                        HorizontalTextAlignment = TextAlignment.Start
                    };
                    AbsoluteLayout.SetLayoutBounds(info, new Rectangle(0.05, 0.95, 0.9, 0.25));
                    AbsoluteLayout.SetLayoutFlags(info, AbsoluteLayoutFlags.All);
                    Label title = new Label()
                    {
                        Text = note.NoteTitle,
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Start
                    };
                    AbsoluteLayout.SetLayoutBounds(title, new Rectangle(0.05, 0.05, 0.9, 0.25));
                    AbsoluteLayout.SetLayoutFlags(title, AbsoluteLayoutFlags.All);

                    layout.Children.Add(info);
                    layout.Children.Add(title);

                    frame.GestureRecognizers.Add(new TapGestureRecognizer()
                    {
                        Command = new Command(() =>
                        {
                            GoToNote(note);
                        })
                    });

                    NoteList.Children.Add(frame);
                }
            }
        }

        public async void GoToNote(CalendarNote note)
        {
            var notePage = new AnyCalendarNotePage(note, Repository, "ReadPage");
            notePage.ChangeRepository += ChangeRepositoryGo;
            await Navigation.PushModalAsync(notePage);
        }

        private async void SortButton_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet("Выберите параметр сортировки", "Отмена",
                "Начальный вид", "Название", "Категория");
            switch (result)
            {
                case "Начальный вид":
                    SortParam = "Standart";
                    if (NoteListActive)
                        UpdateNoteList(Calendar.SelectedDates);
                    break;
                case "Название":
                    SortParam = "Title";
                    if (NoteListActive)
                        UpdateNoteList(Calendar.SelectedDates);
                    break;
                case "Категория":
                    SortParam = "Category";
                    if (NoteListActive)
                        UpdateNoteList(Calendar.SelectedDates);
                    break;
                default:
                    break;
            }
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            var notePage = new AnyCalendarNotePage(new CalendarNote(), Repository, "AddPage");
            notePage.ChangeRepository += ChangeRepositoryGo;
            await Navigation.PushModalAsync(notePage);
        }

        private void ChangeRepositoryGo(object sender, EventArgs e)
        {
            (sender as AnyCalendarNotePage).ChangeRepository -= ChangeRepositoryGo;
            UpdateCalendar();
        }
    }
}