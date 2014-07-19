using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MinifyCSSJS
{
    public class MinifyCSS
    {
        public void Minify()
        {
            try
            {
                var filePathCollection = Helper.GetCSSFilePathCollection();
                string strCompressed = "";
                StreamReader st = null;
                var targetFilePath = ConfigurationSettings.AppSettings["CSS_Target_File_Name"];
                long totalFileSize = 0;
                long compressedFileSize = 0;
                FileInfo fi = null;

                if (String.IsNullOrEmpty(targetFilePath))
                {
                    Console.WriteLine("WARNING: CSS_Target_File_Name Key in Config File is Empty or Does Not Exist. Skipping CSS Minification...");
                    return;
                }

                if (filePathCollection.Length==0)
                {
                    Console.WriteLine("WARNING: No CSS File Described in Config File. Skipping CSS Minification...");
                    return;
                }

                foreach (string filePath in filePathCollection)
                {
                    if (File.Exists(filePath))
                    {
                        st = new StreamReader(filePath);
                        var str = st.ReadToEnd();
                        strCompressed += RunMinifier(str);
                        st.Close();
                        Console.WriteLine("SUCCESS: Compressing CSS FIle - {0}", filePath);
                        fi = new FileInfo(filePath);
                        totalFileSize += fi.Length;
                    }
                    else
                    {
                        Console.WriteLine("WARNING: Skipping Invalid CSS File Path - {0}", filePath);
                    }
                }

                if (!File.Exists(targetFilePath))
                {
                    FileStream fs = File.Create(targetFilePath);
                    fs.Close();
                }
                using (StreamWriter writer = new StreamWriter(targetFilePath))
                {
                    writer.Write(strCompressed);
                    if (String.IsNullOrEmpty(strCompressed))
                    {
                        Console.WriteLine("WARNING: Minified CSS file - {0} is Empty", targetFilePath);
                    }
                    else
                    {
                        Console.WriteLine("SUCCESS: Compressed & Minified CSS FIle - {0}", targetFilePath);
                    }
                    fi = new FileInfo(targetFilePath);
                    compressedFileSize = fi.Length;
                    Console.WriteLine("Total Css File Size    - {0} Bytes", totalFileSize);
                    Console.WriteLine("Minified Css File Size - {0} Bytes", compressedFileSize);
                    Console.WriteLine("Compression Ratio      - {0} %", ((totalFileSize - compressedFileSize) * 100) / totalFileSize);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }
        }

        public static string RunMinifier(string body)
        {
            body = Regex.Replace(body, @"[a-zA-Z]+#", "#");
            body = Regex.Replace(body, @"[\n\r]+\s*", string.Empty);
            body = Regex.Replace(body, @"\s+", " ");
            body = Regex.Replace(body, @"\s?([:,;{}])\s?", "$1");
            body = body.Replace(";}", "}");
            body = Regex.Replace(body, @"([\s:]0)(px|pt|%|em)", "$1");

            // Remove comments from CSS
            body = Regex.Replace(body, @"/\*[\d\D]*?\*/", string.Empty);
            return body;
        }
    }
}
