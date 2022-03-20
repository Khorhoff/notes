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
    public partial class AllPursesPage : ContentPage
    {
        readonly DataReadService Reader;
        PurseRepository Repository;
        public string SortParam = "Standart";

        public AllPursesPage()
        {
            InitializeComponent();
            Reader = new DataReadService(new PurseService());
            UpdateData();
        }

        public void UpdateData()
        {
            PurseList.Children.Clear();
            Reader.ReadFromFile();
            Repository = (PurseRepository)Reader.GetRepository();
            switch (SortParam)
            {
                case "Name":
                    Repository.Purses = (from n in Repository.Purses
                                         orderby n.Name
                                              select n).ToList();
                    break;
                case "Amount":
                    Repository.Purses = (from n in Repository.Purses
                                         orderby n.Amount descending
                                              select n).ToList();
                    break;
                default:
                    break;
            }
            if (Repository.Purses.Count < 1)
            {
                PurseList.Children.Add(new Label()
                {
                    Text = "Нет кошельков",
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                });
            }
            else
            {
                foreach (Purse purse in Repository.Purses)
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
                        BackgroundColor = Settings.PurseCategory[purse.IdCategory].CategoryColor,
                        Margin = 0,
                        Padding = 0
                    };
                    frame.Content = layout;

                    Label info = new Label()
                    {
                        Text = $"{purse.Amount} p.  {Settings.PurseCategory[purse.IdCategory].Name}",
                        FontSize = 16,
                        HorizontalTextAlignment = TextAlignment.Start
                    };
                    AbsoluteLayout.SetLayoutBounds(info, new Rectangle(0.05, 0.95, 0.9, 0.25));
                    AbsoluteLayout.SetLayoutFlags(info, AbsoluteLayoutFlags.All);
                    Label name = new Label()
                    {
                        Text = purse.Name,
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Start
                    };
                    AbsoluteLayout.SetLayoutBounds(name, new Rectangle(0.05, 0.05, 0.9, 0.25));
                    AbsoluteLayout.SetLayoutFlags(name, AbsoluteLayoutFlags.All);

                    layout.Children.Add(info);
                    layout.Children.Add(name);

                    frame.GestureRecognizers.Add(new TapGestureRecognizer()
                    {
                        Command = new Command(() =>
                        {
                            GoToPurse(purse);
                        })
                    });

                    PurseList.Children.Add(frame);
                }
            }
        }

        public async void GoToPurse(Purse purse)
        {
            Reader.ReadFromFile();
            var notePage = new AnyPursePage(purse, (PurseRepository)Reader.GetRepository());
            notePage.ChangeRepository += ChangeRepositoryGoToDelete;
            await Navigation.PushModalAsync(notePage);
        }

        private void ChangeRepositoryGoToDelete(object sender, EventArgs e)
        {
            (sender as AnyPursePage).ChangeRepository -= ChangeRepositoryGoToDelete;
            UpdateData();
        }

        private async void SortButton_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet("Выберите параметр сортировки", "Отмена", 
                "Начальный вид", "Название", "Количество средств");
            switch (result)
            {
                case "Начальный вид":
                    SortParam = "Standart";
                    UpdateData();
                    break;
                case "Название":
                    SortParam = "Name";
                    UpdateData();
                    break;
                case "Количество средств":
                    SortParam = "Amount";
                    UpdateData();
                    break;
                default:
                    break;
            }
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            var notePage = new AddPursePage(Repository);
            notePage.ChangeRepository += ChangeRepositoryGoToAdd;
            await Navigation.PushModalAsync(notePage);
        }

        private void ChangeRepositoryGoToAdd(object sender, EventArgs e)
        {
            (sender as AddPursePage).ChangeRepository -= ChangeRepositoryGoToAdd;
            UpdateData();
        }

        private void UpdateButton_Clicked(object sender, EventArgs e)
        {
            UpdateData();
        }
    }
}