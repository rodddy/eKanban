using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplication.Shared
{
    public class TestPageViewModel : INotifyPropertyChanged
    {
        public TestPageViewModel()
        {
            Series = new List<SeriesData>();

            Yellow_Flags = new ObservableCollection<StatusClass>();
            Red_Flags = new ObservableCollection<StatusClass>();

            Yellow_Flags.Add(new StatusClass() { Category = "Line 1", Number = 66 });
            Yellow_Flags.Add(new StatusClass() { Category = "Line 2", Number = 23 });
            Yellow_Flags.Add(new StatusClass() { Category = "Line 3", Number = 12 });
            Yellow_Flags.Add(new StatusClass() { Category = "Line 4", Number = 94 });
            Yellow_Flags.Add(new StatusClass() { Category = "Line 5", Number = 45 });
            Yellow_Flags.Add(new StatusClass() { Category = "Line 6", Number = 29 });
            Yellow_Flags.Add(new StatusClass() { Category = "Line 7", Number = 12 });
            Yellow_Flags.Add(new StatusClass() { Category = "Line 8", Number = 94 });
            Yellow_Flags.Add(new StatusClass() { Category = "Line 9", Number = 45 });
            Yellow_Flags.Add(new StatusClass() { Category = "Line 10", Number = 29 });

            Red_Flags.Add(new StatusClass() { Category = "Line 1", Number = 34 });
            Red_Flags.Add(new StatusClass() { Category = "Line 2", Number = 23 });
            Red_Flags.Add(new StatusClass() { Category = "Line 3", Number = 15 });
            Red_Flags.Add(new StatusClass() { Category = "Line 4", Number = 66 });
            Red_Flags.Add(new StatusClass() { Category = "Line 5", Number = 56 });
            Red_Flags.Add(new StatusClass() { Category = "Line 6", Number = 45 });
            Red_Flags.Add(new StatusClass() { Category = "Line 7", Number = 15 });
            Red_Flags.Add(new StatusClass() { Category = "Line 8", Number = 66 });
            Red_Flags.Add(new StatusClass() { Category = "Line 9", Number = 56 });
            Red_Flags.Add(new StatusClass() { Category = "Line 10", Number = 34 });

            Series.Add(new SeriesData() { DisplayName = "等待", Items = Yellow_Flags });
            Series.Add(new SeriesData() { DisplayName = "故障", Items = Red_Flags });
        }

        public List<SeriesData> Series
        {
            get;
            set;
        }

        public ObservableCollection<StatusClass> Yellow_Flags
        {
            get;
            set;
        }

        public ObservableCollection<StatusClass> Red_Flags
        {
            get;
            set;
        }

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
