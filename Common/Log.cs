using System;

namespace Common
{
    public class Log
    {
        public string Level { get; set; }
        public string GameName {get; set; }
        public string UserName {get; set; }
        public DateTime Timestamp {get; set;}
        public string Message { get; set; }

        public Log()
        {
        }
    }
}