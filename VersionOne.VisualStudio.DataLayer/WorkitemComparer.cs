using System;
using System.Collections.Generic;
using VersionOne.VisualStudio.DataLayer.Entities;

namespace VersionOne.VisualStudio.DataLayer {
    internal class WorkitemComparer : IComparer<Workitem> {
        private readonly string TestToken;
        private readonly string TaskToken;

        public WorkitemComparer (string testToken, string taskToken) {
            TaskToken = taskToken;
            TestToken = testToken;
        }

        public int Compare (Workitem w1, Workitem w2) {
            if (ReferenceEquals(w1, w2)) {
                return 0;
            }    
        
            if(w1.TypePrefix == TestToken && w2.TypePrefix == TaskToken) {
                return -1;
            }

            if(w1.TypePrefix == TaskToken && w2.TypePrefix == TestToken) {
                return 1;
            }

            if (w2.IsVirtual && w1.IsVirtual) {
                return -1;
            }

            if (w2.IsVirtual) {
                return -1;
            }

            if (w1.IsVirtual) {
                return 1;
            }

            try {
                int order1;
                int order2;
                int.TryParse(w1.GetProperty(Entity.OrderProperty).ToString(), out order1);
                int.TryParse(w2.GetProperty(Entity.OrderProperty).ToString(), out order2);
                return order1.CompareTo(order2);
            } catch(ArgumentException) {
                return -1;
            }
        }
    }
}