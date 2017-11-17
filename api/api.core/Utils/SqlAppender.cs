using log4net.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace api.core.Utils
{
    public class SqlAppender : log4net.Appender.AppenderSkeleton
    {
        private readonly SqlConnection _conn;

        public SqlAppender()
        {
            _conn = new SqlConnection(Configuration.DbConnection);
            _conn.Open();
        }
        ~SqlAppender()
        {
            _conn.Dispose();
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var txt = "INSERT INTO Log (Date, Thread, Level, Logger, Message, Exception) " +
                "VALUES(@date, @thread, @level, @logger, @message, @exception)";

            using (var cmd = new SqlCommand(txt, _conn))
            {
                cmd.Parameters.AddWithValue("@date", loggingEvent.TimeStamp);
                cmd.Parameters.AddWithValue("@thread", loggingEvent.ThreadName);
                cmd.Parameters.AddWithValue("@level", loggingEvent.Level.Name);
                cmd.Parameters.AddWithValue("@logger", loggingEvent.LoggerName);
                cmd.Parameters.AddWithValue("@message", loggingEvent.RenderedMessage);
                var ex = loggingEvent.GetExceptionString();
                if (!string.IsNullOrWhiteSpace(ex))
                {
                    var idx = ex.Length > 2000 ? 2000 : ex.Length;
                    ex = ex.Substring(0, idx);
                }
                cmd.Parameters.AddWithValue("@exception", ex);
                cmd.ExecuteNonQuery();
            }
        }
    }

}
