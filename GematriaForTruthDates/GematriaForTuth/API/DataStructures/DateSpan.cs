using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GematriaToolsForTruth.API.DataStructures
{
    [Serializable]
    public class DateSpan
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate {  get; set; }

        public DateSpan(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public DateSpan()
        { }
    }
}
