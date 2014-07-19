using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace MinifyCSSJS
{
    public class MinifyJS
    {
        const int EOF = -1;

        StreamReader sr;
        StringWriter sw;
        int theA;
        int theB;
        int theLookahead = EOF;


        public void Minify()
        {
            try
            {
                var filePathCollection = Helper.GetJSFilePathCollection();
                string strCompressed = "";
                StreamReader st = null;
                var targetFilePath = ConfigurationSettings.AppSettings["JS_Target_File_Name"];
                long totalFileSize = 0;
                long compressedFileSize = 0;
                FileInfo fi = null;

                if (String.IsNullOrEmpty(targetFilePath))
                {
                    Console.WriteLine("WARNING: JS_Target_File_Name Key in Config File is Empty or Does Not Exist. Skipping JS Minification...");
                    return;
                }

                if (filePathCollection.Length == 0)
                {
                    Console.WriteLine("WARNING: No JS File Described in Config File. Skipping CSS Minification...");
                    return;
                }

                foreach (string filePath in filePathCollection)
                {
                    if (File.Exists(filePath))
                    {
                        strCompressed += RunMinifier(filePath);
                        Console.WriteLine("SUCCESS: Compressing JS FIle - {0}", filePath);
                        fi = new FileInfo(filePath);
                        totalFileSize += fi.Length;
                    }
                    else
                    {
                        Console.WriteLine("WARNING: Skipping Invalid JS File Path - {0}", filePath);
                    }
                }
                strCompressed = strCompressed.StartsWith("\n") ? strCompressed.Substring(1, strCompressed.Length - 2) : strCompressed;

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
                        Console.WriteLine("WARNING: Minified JS file - {0} is Empty", targetFilePath);
                    }
                    else
                    {
                        Console.WriteLine("SUCCESS: Compressed & Minified JS FIle - {0}", targetFilePath);
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

        public string RunMinifier(string filePath) //removed the out file path
        {
            using (sr = new StreamReader(filePath))
            {
                using (sw = new StringWriter())  //used to be a StreamWriter
                {
                    jsmin();
                    return sw.ToString(); // return the minified string
                }
            }
        }

        /* jsmin -- Copy the input to the output, deleting the characters which are
                insignificant to JavaScript. Comments will be removed. Tabs will be
                replaced with spaces. Carriage returns will be replaced with linefeeds.
                Most spaces and linefeeds will be removed.
        */
        void jsmin()
        {
            theA = '\n';
            action(3);
            while (theA != EOF)
            {
                switch (theA)
                {
                    case ' ':
                        {
                            if (isAlphanum(theB))
                            {
                                action(1);
                            }
                            else
                            {
                                action(2);
                            }
                            break;
                        }
                    case '\n':
                        {
                            switch (theB)
                            {
                                case '{':
                                case '[':
                                case '(':
                                case '+':
                                case '-':
                                    {
                                        action(1);
                                        break;
                                    }
                                case ' ':
                                    {
                                        action(3);
                                        break;
                                    }
                                default:
                                    {
                                        if (isAlphanum(theB))
                                        {
                                            action(1);
                                        }
                                        else
                                        {
                                            action(2);
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            switch (theB)
                            {
                                case ' ':
                                    {
                                        if (isAlphanum(theA))
                                        {
                                            action(1);
                                            break;
                                        }
                                        action(3);
                                        break;
                                    }
                                case '\n':
                                    {
                                        switch (theA)
                                        {
                                            case '}':
                                            case ']':
                                            case ')':
                                            case '+':
                                            case '-':
                                            case '"':
                                            case '\'':
                                                {
                                                    action(1);
                                                    break;
                                                }
                                            default:
                                                {
                                                    if (isAlphanum(theA))
                                                    {
                                                        action(1);
                                                    }
                                                    else
                                                    {
                                                        action(3);
                                                    }
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        action(1);
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
        }
        /* action -- do something! What you do is determined by the argument:
                1   Output A. Copy B to A. Get the next B.
                2   Copy B to A. Get the next B. (Delete A).
                3   Get the next B. (Delete B).
           action treats a string as a single character. Wow!
           action recognizes a regular expression if it is preceded by ( or , or =.
        */
        void action(int d)
        {
            if (d <= 1)
            {
                put(theA);
            }
            if (d <= 2)
            {
                theA = theB;
                if (theA == '\'' || theA == '"')
                {
                    for (; ; )
                    {
                        put(theA);
                        theA = get();
                        if (theA == theB)
                        {
                            break;
                        }
                        if (theA <= '\n')
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}\n", theA));
                        }
                        if (theA == '\\')
                        {
                            put(theA);
                            theA = get();
                        }
                    }
                }
            }
            if (d <= 3)
            {
                theB = next();
                if (theB == '/' && (theA == '(' || theA == ',' || theA == '=' ||
                                    theA == '[' || theA == '!' || theA == ':' ||
                                    theA == '&' || theA == '|' || theA == '?' ||
                                    theA == '{' || theA == '}' || theA == ';' ||
                                    theA == '\n'))
                {
                    put(theA);
                    put(theB);
                    for (; ; )
                    {
                        theA = get();
                        if (theA == '/')
                        {
                            break;
                        }
                        else if (theA == '\\')
                        {
                            put(theA);
                            theA = get();
                        }
                        else if (theA <= '\n')
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.\n", theA));
                        }
                        put(theA);
                    }
                    theB = next();
                }
            }
        }
        /* next -- get the next character, excluding comments. peek() is used to see
                if a '/' is followed by a '/' or '*'.
        */
        int next()
        {
            int c = get();
            if (c == '/')
            {
                switch (peek())
                {
                    case '/':
                        {
                            for (; ; )
                            {
                                c = get();
                                if (c <= '\n')
                                {
                                    return c;
                                }
                            }
                        }
                    case '*':
                        {
                            get();
                            for (; ; )
                            {
                                switch (get())
                                {
                                    case '*':
                                        {
                                            if (peek() == '/')
                                            {
                                                get();
                                                return ' ';
                                            }
                                            break;
                                        }
                                    case EOF:
                                        {
                                            throw new Exception("Error: JSMIN Unterminated comment.\n");
                                        }
                                }
                            }
                        }
                    default:
                        {
                            return c;
                        }
                }
            }
            return c;
        }
        /* peek -- get the next character without getting it.
        */
        int peek()
        {
            theLookahead = get();
            return theLookahead;
        }
        /* get -- return the next character from stdin. Watch out for lookahead. If
                the character is a control character, translate it to a space or
                linefeed.
        */
        int get()
        {
            int c = theLookahead;
            theLookahead = EOF;
            if (c == EOF)
            {
                c = sr.Read();
            }
            if (c >= ' ' || c == '\n' || c == EOF)
            {
                return c;
            }
            if (c == '\r')
            {
                return '\n';
            }
            return ' ';
        }
        void put(int c)
        {
            sw.Write((char)c);
        }
        /* isAlphanum -- return true if the character is a letter, digit, underscore,
                dollar sign, or non-ASCII character.
        */
        bool isAlphanum(int c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' ||
                c > 126);
        }
    }
}