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
    public partial class BudgetNotePage : ContentPage
    {
        DataChangeService DataChanger;
        PurseRepository Repository;
        Purse SelectedPurse, SecondPurse;
        double MaxSumm;
        string SelectedType = "";
        string[] CategoryNames = new string[3];
        public event EventHandler<EventArgs> ChangeRepository;

        public BudgetNotePage(PurseRepository repository)
        {
            DataChanger = new DataChangeService(new PurseService(repository));
            Repository = repository;
            InitializeComponent();
            NoteDateTime.Text = DateTime.Now.ToShortDateString();
            Category.Text = Settings.PurseNoteCategories[0].Name;
            Category.BackgroundColor = Settings.PurseNoteCategories[0].CategoryColor;
            int i = 0;
            foreach (var categ in Settings.PurseNoteCategories)
                if (categ.Name != "Общее" && categ.Name != "Доход" && categ.Name != "Перевод")
                {
                    CategoryNames[i] = categ.Name;
                    i++;
                }


            foreach (var purse in repository.Purses)
            {
                PursePicker.Items.Add(purse.Name);
            }
        }

        private void PursePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var purse in Repository.Purses)
                if (purse.Name == PursePicker.SelectedItem.ToString())
                {
                    MaxSumm = purse.Amount;
                    AvailableAmount.Text = $"{Math.Round(MaxSumm, 2)} p.";
                    SelectedPurse = purse;
                    Summ.IsVisible = true;
                    Amount.IsVisible = true;
                    NoteTypePicker.IsVisible = true;
                    NoteTypePicker.Items.Clear();
                    NoteTypePicker.Items.Add("Доход");
                    if (MaxSumm > 0)
                        NoteTypePicker.Items.Add("Расход");
                    if (Repository.Purses.Count > 1 && MaxSumm > 0)
                        NoteTypePicker.Items.Add("Перевод");
                    break;
                }
        }

        private void NoteTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedType = NoteTypePicker.SelectedItem.ToString();
            switch (SelectedType)
            {
                case "Перевод":
                    SecondPursePicker.IsVisible = true;
                    SettingButton.IsVisible = false;
                    Category.Text = Settings.PurseNoteCategories[2].Name;
                    Category.BackgroundColor = Settings.PurseNoteCategories[2].CategoryColor;
                    SecondPursePicker.Items.Clear();
                    foreach (var purse in Repository.Purses)
                        if (purse != SelectedPurse)
                            SecondPursePicker.Items.Add(purse.Name);
                    break;
                case "Расход":
                    SecondPursePicker.IsVisible = false;
                    SecondAvailableAmount.IsVisible = false;
                    SettingButton.IsVisible = true;
                    Category.Text = Settings.PurseNoteCategories[0].Name;
                    Category.BackgroundColor = Settings.PurseNoteCategories[0].CategoryColor;
                    break;
                case "Доход":
                    SecondPursePicker.IsVisible = false;
                    SecondAvailableAmount.IsVisible = false;
                    SettingButton.IsVisible = false;
                    Category.Text = Settings.PurseNoteCategories[1].Name;
                    Category.BackgroundColor = Settings.PurseNoteCategories[1].CategoryColor;
                    break;
                default:
                    break;
            }
        }

        private void SecondPursePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var purse in Repository.Purses)
                if (purse.Name == SecondPursePicker.SelectedItem.ToString())
                {
                    SecondAvailableAmount.Text = $"{Math.Round(purse.Amount, 2)} p.";
                    SecondPurse = purse;
                    SecondAvailableAmount.IsVisible = true;
                }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            int IdCategory = 0;
            double summ = 0;
            for (int i = 0; i < Settings.PurseNoteCategories.Count(); i++)
                if (Settings.PurseNoteCategories[i].Name == Category.Text)
                    IdCategory = i;

            if (SelectedType == "Доход")
            {
                if (PursePicker.SelectedItem != null && Amount.Text != "" && double.TryParse(Amount.Text, out summ) &&
                summ > 0)
                {
                    DataChanger.ChangeElementOnRoot(SelectedPurse.Id, new BudgetNote(summ, IdCategory));
                    ChangeRepository?.Invoke(this, EventArgs.Empty);
                    await Navigation.PopModalAsync();
                }
                else
                    await DisplayAlert("Ошибка", "Заполните правильно все поля", "Ok");
            }
            else if (SelectedType == "Расход")
            {
                if (PursePicker.SelectedItem != null && Amount.Text != "" && double.TryParse(Amount.Text, out summ) &&
                summ > 0 && summ <= MaxSumm)
                {
                    DataChanger.ChangeElementOnRoot(SelectedPurse.Id, new BudgetNote(-summ, IdCategory));
                    ChangeRepository?.Invoke(this, EventArgs.Empty);
                    await Navigation.PopModalAsync();
                }
                else
                    await DisplayAlert("Ошибка", "Заполните правильно все поля", "Ok");
            }
            else if (SelectedType == "Перевод")
            {
                if (PursePicker.SelectedItem != null && Amount.Text != "" && double.TryParse(Amount.Text, out summ) &&
                summ > 0 && SecondPursePicker.SelectedItem != null && summ <= MaxSumm)
                {
                    DataChanger.ChangeElementOnRoot(SelectedPurse.Id, new RemittanceNote(-summ, IdCategory, SecondPurse.Id));
                    ChangeRepository?.Invoke(this, EventArgs.Empty);
                    await Navigation.PopModalAsync();
                }
                else
                    await DisplayAlert("Ошибка", "Заполните правильно все поля", "Ok");
            }
            else
            {
                await DisplayAlert("Ошибка", "Заполните правильно все поля", "Ok");
            }
        }

        private async void SettingButton_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet("Выберите категорию изменения", "Отмена",
                "Общее", CategoryNames);
            foreach (var categ in Settings.PurseNoteCategories)
            {
                if (categ.Name == result)
                {
                    Category.Text = categ.Name;
                    Category.BackgroundColor = categ.CategoryColor;
                }
            }
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            ChangeRepository?.Invoke(this, EventArgs.Empty);
            await Navigation.PopModalAsync();
        }
    }
}