using System;
using System.Reflection;

namespace MarkLandmark
{
    public static class Log
    {
        public enum EventName
        {
            Unknown = -1,
            Launch,
            Close,
            OpenDataset,
            OpenImage,
            Save,
            MoveLandmarkStart,
            MoveLandmarkEnd,
        }

        private static readonly log4net.ILog my_log
            = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void LogInfo(EventName name, String msg)
        {
            my_log.Info(name.ToString() + " " + msg);
        }

        public static void LogError(String msg, Exception ex)
        {
            my_log.Error(msg, ex);
        }
    }
}
