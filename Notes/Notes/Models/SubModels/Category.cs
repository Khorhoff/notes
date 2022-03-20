using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Notes.Models.SubModels
{
    public class Category
    {
        public string Name { get; set; }
        public Color CategoryColor { get; set; }

        public Category()
        {

        }

        public Category(string name, Color color)
        {
            Name = name;
            CategoryColor = color;
        }
    }
}
