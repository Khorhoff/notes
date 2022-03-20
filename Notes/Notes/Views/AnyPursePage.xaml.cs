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
    public partial class AnyPursePage : ContentPage
    {
        Purse AnyPurse;
        PurseRepository Repository;
        DataChangeService DataChanger;
        public event EventHandler<EventArgs> ChangeRepository;

        public AnyPursePage(Purse purse, PurseRepository repository)
        {
            foreach (var anyPurse in repository.Purses)
                if (purse.Id == anyPurse.Id)
                {
                    AnyPurse = anyPurse;
                    break;
                }
            Repository = repository;
            DataChanger = new DataChangeService(new PurseService(repository));
            InitializeComponent();
            PurseName.Text = AnyPurse.Name;
            Amount.Text = $"{Math.Round(AnyPurse.Amount, 2)} р.";
            Category.Text = Settings.PurseCategory[AnyPurse.IdCategory].Name;
            Category.BackgroundColor = Settings.PurseCategory[AnyPurse.IdCategory].CategoryColor;
            Description.Text = AnyPurse.Description;
            UpdateData();
        }

        public void UpdateData()
        {
            NoteList.Children.Clear();
            string text = "";
            if (AnyPurse.BudgetNotes.Count < 1)
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
                AnyPurse.BudgetNotes = (from bn in AnyPurse.BudgetNotes
                                       orderby bn.NoteDateTime descending
                                       select bn).ToList();
                foreach (BudgetNote note in AnyPurse.BudgetNotes)
                {
                    if (note is RemittanceNote remitt)
                    {
                        Purse secondPurse = new Purse();
                        foreach (var purse in Repository.Purses)
                            if (purse.Id == remitt.IdSecondPurse)
                                secondPurse = purse;
                        if (secondPurse.Name == "")
                            secondPurse.Name = "Неизвестный кошелек";
                        text = $"Перевод на сумму {-remitt.Amount} р. на {secondPurse.Name}";
                    }
                    else if(note.Amount < 0)
                    {
                        text = $"Расход на сумму {-note.Amount} р.";
                    }
                    else if (note.Amount > 0)
                    {
                        text = $"Доход на сумму {note.Amount} р.";
                    }
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
                        BackgroundColor = Settings.PurseNoteCategories[note.IdCategory].CategoryColor,
                        Margin = 0,
                        Padding = 0
                    };
                    frame.Content = layout;

                    Label info = new Label()
                    {
                        Text = $"{note.NoteDateTime.ToShortDateString()},  {Settings.PurseNoteCategories[note.IdCategory].Name}",
                        HorizontalTextAlignment = TextAlignment.Start
                    };
                    AbsoluteLayout.SetLayoutBounds(info, new Rectangle(0.05, 0.95, 0.9, 0.25));
                    AbsoluteLayout.SetLayoutFlags(info, AbsoluteLayoutFlags.All);
                    Label title = new Label()
                    {
                        Text = text,
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Start
                    };
                    AbsoluteLayout.SetLayoutBounds(title, new Rectangle(0.05, 0.05, 0.9, 0.25));
                    AbsoluteLayout.SetLayoutFlags(title, AbsoluteLayoutFlags.All);

                    layout.Children.Add(info);
                    layout.Children.Add(title);

                    NoteList.Children.Add(frame);
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
            if (await DisplayAlert("Предупреждение", "Вы действительно хотите удалить кошелек?", "Да", "Нет") == true)
            {
                DataChanger.DeleteFromXmlRoot(AnyPurse.Id);
                ChangeRepository?.Invoke(this, EventArgs.Empty);
                await Navigation.PopModalAsync();
            }
        }
    }
}