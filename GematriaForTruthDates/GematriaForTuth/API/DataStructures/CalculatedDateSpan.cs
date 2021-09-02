using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GematriaToolsForTruth.API.DataStructures
{
    [Serializable]
    public class CalculatedDateSpan : DateSpan
    {  
        public int Years { get; set; }
        public int Months { get; set; }
        public int Weeks { get; set; }
        public int Days { get; set; }
        public bool IncludesEndDate { get; set; }
        public string ErrorString { get; set; }

        public CalculatedDateSpan(DateTime dateGiven, DateTime dateCalculated, int years, int months, int weeks, int days, bool includesEndDate) : base(dateGiven, dateCalculated)
        {
            Years = years;
            Months = months;
            Weeks = weeks;
            Days = days;
            IncludesEndDate = includesEndDate;
            ErrorString = string.Empty;
        }

        public CalculatedDateSpan()
        {
            ErrorString = string.Empty;
        }

    }
}
