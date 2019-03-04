﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace eKanban_Console
{
    public class OrgModel
    {
        public bool IsGrouping { get; set; }
        public ObservableCollection<OrgModel> Children { get; set; }
        public string DisplayName { get; set; }
        public string SurName { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public int Count { get; set; }
    }
}
