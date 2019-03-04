using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eKanban_Console
{
    public class StatusDataModel
    {


        public StatusDataModel()
        {
            YR_Series = new List<SeriesData>();
            YR_Flags = new ObservableCollection<StatusClass>();

            YR_Flags.Add(new StatusClass() { Category = "等待", Number = 66 });
            YR_Flags.Add(new StatusClass() { Category = "警告", Number = 23 });
            YR_Series.Add(new SeriesData() { DisplayName = "SMT 9线", Description = "SMT 9线", Items = YR_Flags });

            YR_Flags = new ObservableCollection<StatusClass>();
            YR_Flags.Add(new StatusClass() { Category = "等待", Number = 35 });
            YR_Flags.Add(new StatusClass() { Category = "警告", Number = 63 });
            YR_Series.Add(new SeriesData() { DisplayName = "SMT 10线", Description = "SMT 10线", Items = YR_Flags });

            YR_Flags = new ObservableCollection<StatusClass>();
            YR_Flags.Add(new StatusClass() { Category = "等待", Number = 51 });
            YR_Flags.Add(new StatusClass() { Category = "警告", Number = 54 });
            YR_Series.Add(new SeriesData() { DisplayName = "SMT 11线", Description = "SMT 11线", Items = YR_Flags });

            YR_Flags = new ObservableCollection<StatusClass>();
            YR_Flags.Add(new StatusClass() { Category = "等待", Number = 86 });
            YR_Flags.Add(new StatusClass() { Category = "警告", Number = 68 });
            YR_Series.Add(new SeriesData() { DisplayName = "SMT 12线", Description = "SMT 12线", Items = YR_Flags });

            YR_Flags = new ObservableCollection<StatusClass>();
            YR_Flags.Add(new StatusClass() { Category = "等待", Number = 45 });
            YR_Flags.Add(new StatusClass() { Category = "警告", Number = 73 });
            YR_Series.Add(new SeriesData() { DisplayName = "SMT 13线", Description = "SMT 13线", Items = YR_Flags });

            YR_Flags = new ObservableCollection<StatusClass>();
            YR_Flags.Add(new StatusClass() { Category = "等待", Number = 75 });
            YR_Flags.Add(new StatusClass() { Category = "警告", Number = 71 });
            YR_Series.Add(new SeriesData() { DisplayName = "SMT 14线", Description = "SMT 14线", Items = YR_Flags });

            YR_Flags = new ObservableCollection<StatusClass>();
            YR_Flags.Add(new StatusClass() { Category = "等待", Number = 43 });
            YR_Flags.Add(new StatusClass() { Category = "警告", Number = 29 });
            YR_Series.Add(new SeriesData() { DisplayName = "SMT 15线", Description = "SMT 15线", Items = YR_Flags });

            YR_Flags.ElementAt(0).Number = 75; YR_Flags.ElementAt(1).Number = 71;
            YR_Series.Add(new SeriesData() { DisplayName = "SMT 16线", Description = "SMT 16线", Items = YR_Flags });

            YR_Flags.ElementAt(0).Number = 75; YR_Flags.ElementAt(1).Number = 71;
            YR_Series.Add(new SeriesData() { DisplayName = "SMT 17线", Description = "SMT 17线", Items = YR_Flags });

            YR_Flags.ElementAt(0).Number = 75; YR_Flags.ElementAt(1).Number = 71;
            YR_Series.Add(new SeriesData() { DisplayName = "SMT 22线", Description = "SMT 22线", Items = YR_Flags });

            Activation_Series = new List<SeriesData>();
            Activation_Flags = new ObservableCollection<StatusClass>();

            Activation_Flags.Add(new StatusClass() { Category = "稼动率", Number = 34 });
            Activation_Series.Add(new SeriesData() { DisplayName = "SMT 9线", Description = "SMT 9线", Items = Activation_Flags });

            Activation_Flags.ElementAt(0).Number = 67;
            Activation_Series.Add(new SeriesData() { DisplayName = "SMT 10线", Description = "SMT 10线", Items = Activation_Flags });

            Activation_Flags.ElementAt(0).Number = 77;
            Activation_Series.Add(new SeriesData() { DisplayName = "SMT 11线", Description = "SMT 11线", Items = Activation_Flags });

            Activation_Flags.ElementAt(0).Number = 45;
            Activation_Series.Add(new SeriesData() { DisplayName = "SMT 12线", Description = "SMT 12线", Items = Activation_Flags });

            Activation_Flags.ElementAt(0).Number = 96;
            Activation_Series.Add(new SeriesData() { DisplayName = "SMT 13线", Description = "SMT 13线", Items = Activation_Flags });

            Activation_Flags.ElementAt(0).Number = 59;
            Activation_Series.Add(new SeriesData() { DisplayName = "SMT 14线", Description = "SMT 14线", Items = Activation_Flags });

            Activation_Flags.ElementAt(0).Number = 73;
            Activation_Series.Add(new SeriesData() { DisplayName = "SMT 15线", Description = "SMT 15线", Items = Activation_Flags });

            Activation_Flags.ElementAt(0).Number = 81;
            Activation_Series.Add(new SeriesData() { DisplayName = "SMT 16线", Description = "SMT 16线", Items = Activation_Flags });

            Activation_Flags.ElementAt(0).Number = 72;
            Activation_Series.Add(new SeriesData() { DisplayName = "SMT 17线", Description = "SMT 17线", Items = Activation_Flags });

            Activation_Flags.ElementAt(0).Number = 69;
            Activation_Series.Add(new SeriesData() { DisplayName = "SMT 22线", Description = "SMT 22线", Items = Activation_Flags });

        }

        public List<SeriesData> Activation_Series
        {
            get;
            set;
        }
        public List<SeriesData> YR_Series
        {
            get;
            set;
        }

        public ObservableCollection<StatusClass> Activation_Flags
        {
            get;
            set;
        }

        public ObservableCollection<StatusClass> YR_Flags
        {
            get;
            set;
        }

        //private void NotifyPropertyChanged(string property)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
        //    }
        //}
        //public event PropertyChangedEventHandler PropertyChanged;
    }
}
