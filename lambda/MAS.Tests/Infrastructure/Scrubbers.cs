﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MAS.Tests.Infrastructure
{
    public static class Scrubbers
    {
        public static string ScrubHashFromJavascriptFileName(string str)
        {
            return Regex.Replace(str, "(src=\\\".*.)(.[a-z0-9]{8}.)(js\\\")", "$1.$3"); //unescaped regex is: src=\".*.([a-z0-9]{8}.)js
        }

        public static string ScrubLastModifiedDate(string str)
        {
            return Regex.Replace(str, "\\\"lastModifiedDate\\\":\\\"([0-9\\-TZ:\\.]+)\\\"", "\"lastModifiedDate\":\"scrubbed by ScrubLastModifiedDate\""); //unescaped regex is: \"lastModifiedDate\":\"([0-9\-TZ:\.]+)\"
        }
    }
}