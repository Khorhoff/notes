using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Interface
{
    public interface IFileService
    {
        string GetFilePath(string fileName);
    }
}
