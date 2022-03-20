using Notes.Models;
using Notes.Repositories;
using Notes.Services;
using Notes.Services.SubService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnyCalendarNotePage : ContentPage
    {
        CalendarNote calendarNote;
        string PageType;
        DataChangeService DataChanger;
        public event EventHandler<EventArgs> ChangeRepository;

        public AnyCalendarNotePage(CalendarNote anyNote, CalendarNoteRepository repository, string pageType)
        {
            PageType = pageType;
            calendarNote = anyNote;
            DataChanger = new DataChangeService(new CalendarNoteService(repository));
            InitializeComponent();
            switch (PageType)
            {
                case "AddPage":
                    NoteTitle.Text = "";
                    NoteTitle.IsEnabled = true;
                    NoteDate.Date = DateTime.Now;
                    NoteDate.IsEnabled = true;
                    Category.Text = Settings.CalendarNoteCategories[0].Name;
                    Category.BackgroundColor = Settings.CalendarNoteCategories[0].CategoryColor;
                    RedactButton.IsVisible = false;
                    RedactButton.IsEnabled = false;
                    BackButton.IsVisible = false;
                    BackButton.IsEnabled = false;
                    NoteText.Text = "";
                    NoteText.IsEnabled = true;
                    break;
                case "ReadPage":
                    NoteTitle.Text = anyNote.NoteTitle;
                    NoteTitle.IsEnabled = false;
                    NoteDate.Date = anyNote.NoteDate;
                    NoteDate.IsEnabled = false;
                    Category.Text = Settings.CalendarNoteCategories[anyNote.IdCategory].Name;
                    Category.BackgroundColor = Settings.CalendarNoteCategories[anyNote.IdCategory].CategoryColor;
                    SettingButton.IsVisible = false;
                    SettingButton.IsEnabled = false;
                    SaveButton.IsVisible = false;
                    SaveButton.IsEnabled = false;
                    NoteText.Text = anyNote.Text;
                    NoteText.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            ChangeRepository?.Invoke(this, EventArgs.Empty);
            await Navigation.PopModalAsync();
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            int IdCategory = 0;
            for (int i = 0; i < Settings.CalendarNoteCategories.Count(); i++)
                if (Settings.CalendarNoteCategories[i].Name == Category.Text)
                    IdCategory = i;

            if (PageType == "AddPage")
            {
                if (NoteTitle.Text == "" && NoteText.Text == "")
                {
                    ChangeRepository?.Invoke(this, EventArgs.Empty);
                    await Navigation.PopModalAsync();
                }
                else
                {
                    if (NoteTitle.Text == "")
                        await DisplayAlert("Ошибка", "Заполните поле имени", "Ok");
                    else
                    {
                        DataChanger.AddToXmlRoot(new CalendarNote(NoteTitle.Text, NoteText.Text, NoteDate.Date, IdCategory));
                        ChangeRepository?.Invoke(this, EventArgs.Empty);
                        await Navigation.PopModalAsync();
                    }
                }
            }
            else if (PageType == "RedactPage")
            {
                if (NoteTitle.Text == calendarNote.NoteTitle && NoteText.Text == calendarNote.Text && 
                    IdCategory == calendarNote.IdCategory && NoteDate.Date == calendarNote.NoteDate)
                {
                    ChangeRepository?.Invoke(this, EventArgs.Empty);
                    await Navigation.PopModalAsync();
                }
                else
                {
                    if (NoteTitle.Text == "")
                        await DisplayAlert("Ошибка", "Заполните поле имени", "Ok");
                    else
                    {
                        RedactButton.IsVisible = true;
                        RedactButton.IsEnabled = true;
                        BackButton.IsVisible = true;
                        BackButton.IsEnabled = true;

                        PageType = "ReadPage";

                        NoteTitle.IsEnabled = false;
                        SettingButton.IsVisible = false;
                        SettingButton.IsEnabled = false;
                        SaveButton.IsVisible = false;
                        SaveButton.IsEnabled = false;
                        NoteText.IsEnabled = false;
                        NoteDate.IsEnabled = false;

                        CalendarNote newNote = new CalendarNote(NoteTitle.Text, NoteText.Text, NoteDate.Date, IdCategory);
                        DataChanger.ChangeElementOnRoot(calendarNote.Id, newNote);
                    }
                }
            }
        }

        private async void SettingButton_Clicked(object sender, EventArgs e)
        {
            string[] CategoryNames = new string[4];
            int i = 0;
            foreach (var categ in Settings.CalendarNoteCategories)
                if (categ.Name != "Общее")
                {
                    CategoryNames[i] = categ.Name;
                    i++;
                }

            string result = await DisplayActionSheet("Выберите категорию заметки", "Отмена",
               "Общее", CategoryNames);
            foreach (var categ in Settings.CalendarNoteCategories)
            {
                if (categ.Name == result)
                {
                    Category.Text = categ.Name;
                    Category.BackgroundColor = categ.CategoryColor;
                }
            }
        }

        private void RedactButton_Clicked(object sender, EventArgs e)
        {
            RedactButton.IsVisible = false;
            RedactButton.IsEnabled = false;
            BackButton.IsVisible = false;
            BackButton.IsEnabled = false;

            PageType = "RedactPage";

            NoteTitle.IsEnabled = true;
            DeleteButton.IsVisible = true;
            DeleteButton.IsEnabled = true;
            SettingButton.IsVisible = true;
            SettingButton.IsEnabled = true;
            SaveButton.IsVisible = true;
            SaveButton.IsEnabled = true;
            NoteText.IsEnabled = true;
            NoteDate.IsEnabled = true;
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            if (PageType == "AddPage")
            {
                ChangeRepository?.Invoke(this, EventArgs.Empty);
                await Navigation.PopModalAsync();
            }
            else if (PageType == "RedactPage")
            {
                RedactButton.IsVisible = true;
                RedactButton.IsEnabled = true;
                BackButton.IsVisible = true;
                BackButton.IsEnabled = true;

                PageType = "ReadPage";

                NoteTitle.IsEnabled = false;
                SettingButton.IsVisible = false;
                SettingButton.IsEnabled = false;
                SaveButton.IsVisible = false;
                SaveButton.IsEnabled = false;
                NoteText.IsEnabled = false;
                NoteDate.IsEnabled = false;
            }
            else if (PageType == "ReadPage")
            {
                if (await DisplayAlert("Предупреждение", "Вы действительно хотите удалить заметку?", "Да", "Нет") == true)
                {
                    DataChanger.DeleteFromXmlRoot(calendarNote.Id);
                    ChangeRepository?.Invoke(this, EventArgs.Empty);
                    await Navigation.PopModalAsync();
                }
            }
        }
    }
}