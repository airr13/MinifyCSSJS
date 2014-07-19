using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MinifyCSSJS
{
    class Helper
    {
        public static String[] GetJSFilePathCollection()
        {
            string[] repositoryUrls = ConfigurationSettings.AppSettings.AllKeys
                .Where(key => key.StartsWith("JS_File_"))
                .Select(key => ConfigurationSettings.AppSettings[key])
                .OrderBy(key => ConfigurationSettings.AppSettings[key]).ToArray();
            return repositoryUrls;
        }

        public static String[] GetCSSFilePathCollection()
        {
            string[] repositoryUrls = ConfigurationSettings.AppSettings.AllKeys
                .Where(key => key.StartsWith("CSS_File_"))
                .Select(key => ConfigurationSettings.AppSettings[key])
                .OrderBy(key => ConfigurationSettings.AppSettings[key]).ToArray();
            return repositoryUrls;
        }
    }
}
