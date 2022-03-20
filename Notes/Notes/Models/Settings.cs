using Notes.Models.SubModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Notes.Models
{
    public static class Settings
    {
        static public Category[] StickyNoteCategories { get; set; } = new Category[] 
            { new Category("Общее",Color.LightGray), new Category("Важное",Color.IndianRed), 
                new Category("Интересное",Color.LightYellow), new Category("Полезное",Color.LightGreen)};

        static public Category[] CalendarNoteCategories { get; set; } = new Category[]
            { new Category("Общее",Color.LightGray), new Category("Важное",Color.IndianRed),
                new Category("Интересное",Color.LightYellow), new Category("Полезное",Color.LightGreen)};

        static public Category[] PurseNoteCategories { get; set; } = new Category[]
            { new Category("Общее",Color.LightGray),
                new Category("Доход",Color.LightBlue), new Category("Перевод",Color.LightCyan),
                new Category("Развлечения",Color.IndianRed), new Category("Питание",Color.LightYellow),
                new Category("Дом",Color.LightGreen)};

        static public Category[] PurseCategory { get; set; } = new Category[]
            { new Category("Наличные",Color.LightGray), new Category("Карта",Color.IndianRed),
                new Category("Электронный кошелек",Color.LightYellow)};
    }
}
