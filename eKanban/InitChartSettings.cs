using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace eKanban
{
    public partial class MainWindow : Window
    {


    }
    public class MainViewModel
    {
        private  ObservableCollection<Population> _populations = new ObservableCollection<Population>();
        public ObservableCollection<Population> Populations
        {
            get
            {
                return _populations;
            }
        }

        public MainViewModel()
        {
            _populations.Add(new Population() { Name = "Line 1", Count = 1340 });
            _populations.Add(new Population() { Name = "Line 2", Count = 1220 });
            _populations.Add(new Population() { Name = "Line 3", Count = 309 });
            _populations.Add(new Population() { Name = "Line 4", Count = 240 });
            _populations.Add(new Population() { Name = "Line 5", Count = 195 });
            _populations.Add(new Population() { Name = "Line 6", Count = 174 });
            _populations.Add(new Population() { Name = "Line 7", Count = 158 });
            _populations.Add(new Population() { Name = "Line 8", Count = 158 });
            _populations.Add(new Population() { Name = "Line 9", Count = 158 });
            _populations.Add(new Population() { Name = "Line 10", Count = 158 });
        }
    }


    public class Population : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private int _count = 0;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                NotifyPropertyChanged("Count");
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
