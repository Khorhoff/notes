using Notes.Models;
using Notes.Models.SubModels;
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
    public partial class AnyStickyNotePage : ContentPage
    {
        StickyNote stickyNote;
        public string PageType;
        public DataChangeService DataChanger;
        public event EventHandler<EventArgs> ChangeRepository;

        public AnyStickyNotePage(StickyNote anyNote, StickyNoteRepository repository, string pageType)
        {
            PageType = pageType;
            stickyNote = anyNote;
            DataChanger = new DataChangeService(new StickyNoteService(repository));
            InitializeComponent();
            switch (PageType)
            {
                case "AddPage":
                    NoteTitle.Text = "";
                    NoteTitle.IsEnabled = true;
                    LatsModifiedDate.Text = DateTime.Now.ToShortDateString();
                    Category.Text = Settings.StickyNoteCategories[0].Name;
                    Category.BackgroundColor = Settings.StickyNoteCategories[0].CategoryColor;
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
                    LatsModifiedDate.Text = anyNote.LastModifiedDate;
                    Category.Text = Settings.StickyNoteCategories[anyNote.IdCategory].Name;
                    Category.BackgroundColor = Settings.StickyNoteCategories[anyNote.IdCategory].CategoryColor;
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

        private async void SettingButton_Clicked(object sender, EventArgs e)
        {
            string[] CategoryNames = new string[4];
            int i = 0;
            foreach (var categ in Settings.StickyNoteCategories)
                if (categ.Name != "Общее")
                {
                    CategoryNames[i] = categ.Name;
                    i++;
                }

            string result = await DisplayActionSheet("Выберите категорию заметки", "Отмена",
               "Общее", CategoryNames);
            foreach (var categ in Settings.StickyNoteCategories)
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
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            int IdCategory = 0;
            for (int i = 0; i < Settings.StickyNoteCategories.Count(); i++)
                if (Settings.StickyNoteCategories[i].Name == Category.Text)
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
                        await DisplayAlert("Предупреждение", "Заполните поле имени", "Ok");
                    else
                    {
                        DataChanger.AddToXmlRoot(new StickyNote(NoteTitle.Text, NoteText.Text, IdCategory));
                        ChangeRepository?.Invoke(this, EventArgs.Empty);
                        await Navigation.PopModalAsync();
                    }
                }
            }
            else if (PageType == "RedactPage")
            {
                if (NoteTitle.Text == stickyNote.NoteTitle && NoteText.Text == stickyNote.Text && IdCategory == stickyNote.IdCategory)
                {
                    ChangeRepository?.Invoke(this, EventArgs.Empty);
                    await Navigation.PopModalAsync();
                }
                else
                {
                    if (NoteTitle.Text == "")
                        await DisplayAlert("Предупреждение", "Заполните поле имени", "Ok");
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

                        StickyNote newNote = new StickyNote(NoteTitle.Text, NoteText.Text, IdCategory);
                        DataChanger.ChangeElementOnRoot(stickyNote.Id, newNote);
                    }
                }
            }
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            ChangeRepository?.Invoke(this, EventArgs.Empty);
            await Navigation.PopModalAsync();
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
            }
            else if (PageType == "ReadPage")
            {
                if (await DisplayAlert("Предупреждение", "Вы действительно хотите удалить заметку?", "Да", "Нет") == true)
                {
                    DataChanger.DeleteFromXmlRoot(stickyNote.Id);
                    ChangeRepository?.Invoke(this, EventArgs.Empty);
                    await Navigation.PopModalAsync();
                }
            }
        }
    }
}