﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public static class Regions {
        public static string MainView => nameof(MainView);
        public static string Content => nameof(Content);
        public static string Ribbon => nameof(Ribbon);
        public static string Navigation => nameof(Navigation);
        public static string Settings => nameof(Settings);
        public static string Branch => nameof(Branch);
    }

    public static class Modules {
        public static string NoneContent => nameof(NoneContent);
        public static string MainView => nameof(MainView);
        public static string RepositoriesViewRibbon => nameof(RepositoriesViewRibbon);
        public static string RepositoriesViewContent => nameof(RepositoriesViewContent);
        public static string BranchView => nameof(BranchView);
    }
}
