using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "abcabc";
            Regex r = new Regex("a");
            var result =r.Replace(str, "1",1);
        }
    }
}
