﻿using Common.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ricochet {
    internal class PendingRequests {
        private ConcurrentDictionary<int, SignaledResponse> requests = new ConcurrentDictionary<int, SignaledResponse>();
        private readonly ILog l = LogManager.GetCurrentClassLogger();

        internal Response Get(int ticket) {
            var sr = requests[ticket];

            int remainingTime = (int)(Client.HardQueryTimeout - sr.SW.ElapsedMilliseconds);
            if (remainingTime > Client.HardQueryTimeout) {
                remainingTime = 0;
            }
            if (remainingTime < 0) {
                remainingTime = 0;
            }

            Response res;
            bool canProceed = sr.WaitUntil(remainingTime);
            if (!canProceed) { // if timeout...
                l.InfoFormat("Hard timeout reached");
                res = Response.Timeout(ticket);
            } else {
                res = sr.Response;
            }

            Delete(ticket);

            return res;
        }

        internal void Set(int dispatch, Response response) {
            SignaledResponse sr;
            if (requests.TryGetValue(dispatch, out sr)) {
                sr.Response = response;
                sr.Set();
            }
        }

        internal void Add(Query query) {
            requests[query.Dispatch] = new SignaledResponse(query.SW);
        }

        internal void Delete(int ticket) {
            SignaledResponse junk;
            requests.TryRemove(ticket, out junk);
        }
    }
}
