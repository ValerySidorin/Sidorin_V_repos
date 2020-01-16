using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexProgect
{
    class URL : INotifyPropertyChanged // Класс для использования ObservableCollection
    {
        private string url;
        public string Url
        {
            get { return url; }
            set
            {
                if (value != url)
                {
                    url = value;
                    Notify("Url");
                }
            }
        }

        public URL(string url)
        {
            Url = url;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
