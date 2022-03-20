using Notes.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Services
{
    public class DataChangeService
    {
        private IDataChanger dataChanger;

        public DataChangeService(IDataChanger anyChanger)
        {
            dataChanger = anyChanger;
        }

        public void AddToXmlRoot(object obj)
        {
            dataChanger.AddToXmlRoot(obj);
        }

        public void ChangeElementOnRoot(int id, object obj)
        {
            dataChanger.ChangeElementOnRoot(id, obj);
        }

        public void DeleteFromXmlRoot(int id)
        {
            dataChanger.DeleteFromXmlRoot(id);
        }
    }
}
