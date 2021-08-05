using Rhino;
using System;

namespace ITL.Energy2DExporter
{
    public static class Logger
    {
        public static void LogException(Exception e)
        {
            RhinoApp.WriteLine($"An error occured: \n {e.Message}. \n \n {e.StackTrace}.");
        }
    }
}
