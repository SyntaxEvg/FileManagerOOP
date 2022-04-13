using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileManagerEmpty
{
    internal static class RegexHelp
    {
       
        readonly static string PatternLS = @".:\\+[#'.А-Яа-яA-Za-z0-9\\ ]* -p[ 0-9]{0,9999}";
        readonly static string PatternAll = @".:\\+['.А-Яа-яA-Za-z0-9\\ ]* .:\\+['.А-Яа-яA-Za-z0-9\\ ]*";
        readonly static string PatternNoPagging = @".:\\+['.А-Яа-яA-Za-z0-9\\ ]*";

        public readonly static Regex PatternPaggingComp = new Regex(PatternLS, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public readonly static Regex PatternAllComand = new Regex(PatternAll, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public readonly static Regex NoPagging = new Regex(PatternNoPagging, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
