using Notes.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Services
{
    public class DataReadService
    {
        private IDataReader dataReader;

        public DataReadService(IDataReader anyReader)
        {
            dataReader = anyReader;
        }

        public void ReadFromFile()
        {
            dataReader.ReadFromFile();
        }

        public object GetRepository()
        {
            return dataReader.GetRepository();
        }
    }
}
