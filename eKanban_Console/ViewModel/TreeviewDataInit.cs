using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace eKanban_Console
{
    public class TreeviewDataInit
    {
        private static TreeviewDataInit dataInit;

        public static TreeviewDataInit Instance
        {
            get
            {
                if (dataInit == null)
                    dataInit = new TreeviewDataInit();
                return dataInit;
            }
        }
        private TreeviewDataInit()
        {
            OrgList = new ObservableCollection<OrgModel>()
            {
                new OrgModel(){
                            IsGrouping=true,
                            DisplayName="五车间",
                            Children=new ObservableCollection<OrgModel>()
                            {
                                new OrgModel(){
                                    IsGrouping=false,
                                    Name="SMT 09线"
                                },
                                new OrgModel(){
                                    IsGrouping=false,
                                    Name="SMT10线"

                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT11线"

                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT12线"
                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT13线"
                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT14线",

                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT15线"
                                },

                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT16线"
                                },

                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT17线"
                                },

                                new OrgModel(){
                                    IsGrouping =false,
                                    SurName="刘",
                                    Name="SMT22线"
                                },
                            }

                },

                new OrgModel(){
                            IsGrouping=true,
                            DisplayName="六车间",
                            Children=new ObservableCollection<OrgModel>()
                            {
                                new OrgModel(){
                                    IsGrouping=false,
                                    Name="SMT 09线"
                                },
                                new OrgModel(){
                                    IsGrouping=false,
                                    Name="SMT10线"

                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT11线"

                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT12线"
                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT13线"
                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT14线",

                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT15线"
                                },

                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT16线"
                                },

                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT17线"
                                },

                                new OrgModel(){
                                    IsGrouping =false,
                                    SurName="刘",
                                    Name="SMT22线"
                                },
                            }

                        },

                new OrgModel(){
                            IsGrouping=true,
                            DisplayName="七车间",
                            Children=new ObservableCollection<OrgModel>()
                            {
                                new OrgModel(){
                                    IsGrouping=false,
                                    Name="SMT 09线"
                                },
                                new OrgModel(){
                                    IsGrouping=false,
                                    Name="SMT10线"

                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT11线"

                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT12线"
                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT13线"
                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT14线",

                                },
                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT15线"
                                },

                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT16线"
                                },

                                new OrgModel(){
                                    IsGrouping =false,
                                    Name="SMT17线"
                                },

                                new OrgModel(){
                                    IsGrouping =false,
                                    SurName="刘",
                                    Name="SMT22线"
                                },
                            }

                }

            };
        }
        public ObservableCollection<OrgModel> OrgList { get; set; }

    }
}
