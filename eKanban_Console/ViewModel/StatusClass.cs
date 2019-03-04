using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eKanban_Console
{
    public class SeriesData
    {
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public ObservableCollection<StatusClass> Items { get; set; }
    }

    public class StatusClass
    {
        public string Category { get; set; }

        private int _number = 0;
        public int Number 
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
                //if (PropertyChanged != null)
                //{
                //    this.PropertyChanged(this, new PropertyChangedEventArgs("Number"));
                //}
            }

        }

        //public event PropertyChangedEventHandler PropertyChanged;
    }
}
