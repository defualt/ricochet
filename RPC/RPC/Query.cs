﻿using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPC {
    /// <summary>
    /// The actual package that is sent to a server representing an RPC call.
    /// </summary>
    internal class Query : Message {
        static private int ticketNumber = 0;
        // static string cache = null;
        internal Stopwatch SW = Stopwatch.StartNew();

        /// <summary>
        /// Name of function to which this query should be passed.
        /// </summary>
        public string Handler { get; set; }

        internal static Query CreateQuery<T>(string handler, T data) {
            return new Query {
                Handler = handler,
                Dispatch = Interlocked.Increment(ref ticketNumber),
                // Dispatch = 1,
                MessageType = typeof(T),
                MessageData = JsonSerializer.SerializeToString(data)
            };
        }
        // serializer doesn't use most specific type?
        internal string Serialize() {
            //if (cache == null) {
            //    cache = JsonSerializer.SerializeToString(this);
            //}
            //return cache;
            //string ret = Handler + "|" + Dispatch + "|" + MessageType + "|" + MessageData;
            //return ret;
            return JsonSerializer.SerializeToString(this);
        }
    }

}
