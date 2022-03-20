using Notes.Models;
using Notes.Repositories;
using Notes.Services;
using Notes.Services.SubService;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StickyNotePage : ContentPage
    {
        readonly DataReadService Reader;
        StickyNoteRepository Repository;
        public string SortParam = "Standart";

        public StickyNotePage()
        {
            InitializeComponent();
            Reader = new DataReadService(new StickyNoteService());
            UpdateData();
        }

        public void UpdateData()
        {
            NoteList.Children.Clear();
            Reader.ReadFromFile();
            Repository = (StickyNoteRepository)Reader.GetRepository();
            switch (SortParam)
            {
                case "Title":
                    Repository.StickyNotes = (from n in Repository.StickyNotes
                                              orderby n.NoteTitle
                                              select n).ToList();
                    break;
                case "Date":
                    Repository.StickyNotes = (from n in Repository.StickyNotes
                                              orderby n.LastModifiedDate descending
                                              select n).ToList();
                    break;
                case "Category":
                    Repository.StickyNotes = (from n in Repository.StickyNotes
                                              orderby n.IdCategory
                                              select n).ToList();
                    break;
                default:
                    break;
            }
            if (Repository.StickyNotes.Count < 1)
            {
                NoteList.Children.Add(new Label()
                {
                    Text = "Нет заметок",
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                });
            }
            else
            {
                foreach (StickyNote note in Repository.StickyNotes)
                {
                    Frame frame = new Frame()
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        BorderColor = Color.Blue,
                        BackgroundColor = Color.Transparent,
                        Margin = 10,
                        Padding = 1
                    };

                    AbsoluteLayout layout = new AbsoluteLayout()
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        BackgroundColor = Settings.StickyNoteCategories[note.IdCategory].CategoryColor,
                        Margin = 0,
                        Padding = 0
                    };
                    frame.Content = layout;

                    Label info = new Label()
                    {
                        Text = $"{note.LastModifiedDate},  {Settings.StickyNoteCategories[note.IdCategory].Name}",
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
                            GoToNote(note, frame);
                        })
                    });

                    NoteList.Children.Add(frame);
                }
            }
        }

        public async void GoToNote(StickyNote note, Frame frame)
        {
            var notePage = new AnyStickyNotePage(note, Repository, "ReadPage");
            notePage.ChangeRepository += ChangeRepositoryGo;
            frame.IsEnabled = false;
            await Navigation.PushModalAsync(notePage);
            frame.IsEnabled = true;
        }

        private void ChangeRepositoryGo(object sender, EventArgs e)
        {
            (sender as AnyStickyNotePage).ChangeRepository -= ChangeRepositoryGo;
            UpdateData();
        }

        private async void AddNote_Clicked(object sender, EventArgs e)
        {
            var notePage = new AnyStickyNotePage(new StickyNote(), Repository, "AddPage");
            notePage.ChangeRepository += ChangeRepositoryGo;
            AddNote.IsEnabled = false;
            await Navigation.PushModalAsync(notePage);
            AddNote.IsEnabled = true;
        }

        private async void SortButton_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet("Выберите параметр сортировки", "Отмена", 
                "Начальный вид", "Название", "Дата последнего изменения", "Категория");
            switch (result)
            {
                case "Начальный вид":
                    SortParam = "Standart";
                    UpdateData();
                    break;
                case "Название":
                    SortParam = "Title";
                    UpdateData();
                    break;
                case "Дата последнего изменения":
                    SortParam = "Date";
                    UpdateData();
                    break;
                case "Категория":
                    SortParam = "Category";
                    UpdateData();
                    break;
                default:
                    break;
            }
        }
    }
}