using System;
using System.Collections.Generic;
using System.IO;

namespace Logger
{
    class Program
    {
        static void Main(string[] args)
        {
            Log logger = new Log(Directory.GetCurrentDirectory());
            var ex = new Exception();
            Dictionary<object, object> dictionary = new Dictionary<object, object>();
            dictionary.Add(1, "1st property");
            dictionary.Add(2, "2nd property");

            logger.Fatal("Fatal error");
            logger.Fatal("Fatal error with ex", ex);

            logger.Error("Error");
            logger.Error("Error with ex", ex);
            logger.Error(ex);
            logger.ErrorUnique("Error Unique with ex", ex);
            logger.ErrorUnique("Error Unique with ex", ex);

            logger.Warning("Warning");
            logger.Warning("Warning with ex", ex);
            logger.WarningUnique("Warning Unique");
            logger.WarningUnique("Warning Unique");

            logger.Info("Info");
            logger.Info("Info with ex", ex);
            logger.Info("InfoFormat {0}, {1}", "fisrt parameter", "second parameter");

            logger.Debug("Debug");
            logger.Debug("Debug with ex", ex);
            logger.DebugFormat("DebugFormat {0}, {1}", "fisrt parameter", "second parameter");

            logger.SystemInfo("System Info with properties", dictionary);
        }
    }
}
