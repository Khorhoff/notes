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
    public partial class AddPursePage : ContentPage
    {
        DataChangeService DataChanger;
        public event EventHandler<EventArgs> ChangeRepository;

        public AddPursePage(PurseRepository repository)
        {
            DataChanger = new DataChangeService(new PurseService(repository));
            InitializeComponent();
            Category.Text = Settings.PurseCategory[0].Name;
            Category.BackgroundColor = Settings.PurseCategory[0].CategoryColor;
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (PurseName.Text == "")
                await DisplayAlert("Ошибка", "Заполните поле названия", "Ok");
            else
            {
                int IdCategory = 0;
                for (int i = 0; i < Settings.PurseCategory.Count(); i++)
                    if (Settings.PurseCategory[i].Name == Category.Text)
                        IdCategory = i;
                DataChanger.AddToXmlRoot(new Purse(PurseName.Text, Description.Text, IdCategory));
                ChangeRepository?.Invoke(this, EventArgs.Empty);
                await Navigation.PopModalAsync();
            }
        }

        private async void SettingButton_Clicked(object sender, EventArgs e)
        {
            string[] CategoryNames = new string[3];
            int i = 0;
            foreach (var categ in Settings.PurseCategory)
            {
                CategoryNames[i] = categ.Name;
                i++;
            }

            string result = await DisplayActionSheet("Выберите тип кошелька", "Отмена",
               null, CategoryNames);
            foreach (var categ in Settings.PurseCategory)
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