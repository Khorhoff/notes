using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Notes.Droid;
using Notes.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace Notes.Droid
{
    public class FileService : IFileService
    {
        public string GetRootPath()
        {
            return Application.Context.GetExternalFilesDir(null).ToString();
        }

        public string GetFilePath(string fileName)
        {
            return Path.Combine(GetRootPath(), fileName);
        }
    }
}