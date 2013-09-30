using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionOne.VisualStudio.Tests
{
    public class TestConfiguration
    {
        public static string GetVariable(string key)
        {
            return GetVariable(key, string.Empty);
        }
        public static string GetVariable(string key, string harcoded)
        {
            try
            {
                return (ConfigurationManager.AppSettings[key] ?? Environment.GetEnvironmentVariable(key)) ?? harcoded;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
