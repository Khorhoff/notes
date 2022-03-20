using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Repositories;
using Notes.Services;
using Notes.Services.SubService;
using Notes.Models.SubModels;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BudgetPage : ContentPage
    {
        readonly DataReadService Reader;
        public PurseRepository Repository;
        public DateTime DateOfChart;
        private readonly string[] MonthNames = new string[] { "Январь", "Февраль", "Март", "Апрель", "Мая",
        "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };

        public BudgetPage()
        {
            InitializeComponent();
            Reader = new DataReadService(new PurseService());
            DateOfChart = DateTime.Now;
            UpdateChart();
        }

        public void UpdateChart()
        {
            Reader.ReadFromFile();
            Repository = (PurseRepository)Reader.GetRepository();
            ExpenseData ChartData = new ExpenseData();
            DataMonth.Text = $"{MonthNames[DateOfChart.Month - 1]} {DateOfChart.Year}";
            foreach (var monthE in Repository.Expenses)
                if (monthE.Year == DateOfChart.Year && monthE.MonthNumber == DateOfChart.Month)
                {
                    ChartData = monthE.CategorieExpenses;
                    break;
                }
            StatisticsChart.ItemsSource = ToChartData(ChartData);
            if (ChartData.Count < 1)
                IsData.IsVisible = true;
            else
                IsData.IsVisible = false;
        }

        public ChartData ToChartData(ExpenseData data)
        {
            ChartData chartCategories = new ChartData();
            foreach (var categoryE in data)
            {
                chartCategories.Add(new ChartCategory(categoryE.Amount, categoryE.IdCategory));
            }

            return chartCategories;
        }

        private void DecreaseMonth_Clicked(object sender, EventArgs e)
        {
            DateOfChart = DateOfChart.AddMonths(-1);
            UpdateChart();
        }

        private void IncreaseMonth_Clicked(object sender, EventArgs e)
        {
            DateOfChart = DateOfChart.AddMonths(1);
            UpdateChart();
        }

        private void RefreshButton_Clicked(object sender, EventArgs e)
        {
            DateOfChart = DateTime.Now;
            UpdateChart();
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            var notePage = new BudgetNotePage(Repository);
            notePage.ChangeRepository += ChangeRepositoryGo;
            await Navigation.PushModalAsync(notePage);
        }

        private void ChangeRepositoryGo(object sender, EventArgs e)
        {
            (sender as BudgetNotePage).ChangeRepository -= ChangeRepositoryGo;
            UpdateChart();
        }
    }
}