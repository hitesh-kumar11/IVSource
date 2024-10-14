using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IVSource.Models
{
    public class Subscriber
    {
        public IvsVisaSubscribeToUs SubscriptionForm { get; set; }
        public IvsVisaPages DynamicContent { get; set; }
    }
}
