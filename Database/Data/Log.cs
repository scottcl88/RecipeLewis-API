using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class NgxLog
    {
        [JsonProperty("level")]
        public LoggerLevel Level { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("lineNumber")]
        public string LineNumber { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("additional")]
        public object[] Additional { get; set; }
    }

    public enum LoggerLevel
    {
        TRACE = 0,
        DEBUG = 1,
        INFO = 2,
        LOG = 3,
        WARN = 4,
        ERROR = 5,
        FATAL = 6,
        OFF = 7
    }

    public class Log : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogID { get; set; }

        public virtual User User { get; set; }
        public LoggerLevel Level { get; set; }
        public DateTime Timestamp { get; set; }
        public string FileName { get; set; }
        public string LineNumber { get; set; }
        public string Message { get; set; }
        public string Additional { get; set; }
    }
}