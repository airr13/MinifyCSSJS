using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MinifyCSSJS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Minifying CSS....");
            new MinifyCSS().Minify();
            Console.WriteLine(Environment.NewLine);

            Console.WriteLine("Minifying JS....");
            new MinifyJS().Minify();
            Console.WriteLine(Environment.NewLine);

            //Console.WriteLine("Enter 1 To Restore Default Config File.");
            //Console.WriteLine("Enter anything else To Skip.");
            //var entry = Console.ReadLine();
            //try
            //{
            //    var entry_int = Convert.ToInt32(entry);
            //    if (entry_int == 1)
            //        Console.WriteLine("Default Config File Has Been Restored...");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Skipping...");
            //}
            //Console.WriteLine(Environment.NewLine);

            Console.WriteLine("Pess Any Key To Exist....");
            Console.ReadLine();
        }

        //public static restoreDefaultConfigFile()
        //{
        //    var defaultConfigText = 
        //        @"<?xml version=""1.0"" encoding=""utf-8"" ?>" +
        //        @"<configuration>" +
        //        @"  <configSections></configSections>" +
        //        @"  <appSettings>" +
        //        @"      <add key=""CSS_Target_File_Name"" value=""C:\Users\dev1\Desktop\Test\op\minifiedCSS.css"" />" +
        //        @"      <add key=""CSS_File_1"" value=""C:\Users\dev1\Desktop\Test\CSS\one.css"" />" +
        //        @"      <add key=""CSS_File_2"" value=""C:\Users\dev1\Desktop\Test\CSS\test\two.css"" />" +
        //        @"      <add key=""JS_Target_File_Name"" value=""C:\Users\dev1\Desktop\Test\op\minifiedJS.js"" />" +
        //        @"      <add key=""JS_File_1"" value=""C:\Users\dev1\Desktop\Test\JS\one.js"" />" +
        //        @"      <add key=""JS_File_2"" value=""C:\Users\dev1\Desktop\Test\JS\test\two.js"" />" +
        //        @"  </appSettings>" +
        //        @"</configuration>";
        //}
    }
}
