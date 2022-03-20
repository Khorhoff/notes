using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Interface
{
    public interface IDataReader
    {
        void ReadFromFile();
        object GetRepository();
    }
}
