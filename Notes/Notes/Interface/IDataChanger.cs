using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Notes.Interface
{
    public interface IDataChanger
    {
        void AddToXmlRoot(object obj);
        void ChangeElementOnRoot(int id, object obj);
        void DeleteFromXmlRoot(int id);
    }
}
