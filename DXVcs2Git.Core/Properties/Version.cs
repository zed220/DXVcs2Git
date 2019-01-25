﻿using System;

namespace DXVcs2Git.Core {
    public static class VersionInfo {
        public const string VersionString = "1.4.40"; // do not specify revision if 0
        public static readonly Version Version = new Version(VersionString); 
    }
}
