using GematriaForTruth.API.DataStructures;
using GematriaForTruth.API.Factory;
using GematriaToolsForTruth.API.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GematriaToolsForTruth.API.Factory.DateSpanFactory;

namespace GematriaToolsForTruth.API.Factory
{
    public class DateSpanFactory
    {
        public enum EndStartDateSetting { 
        INCLUDE_AND_NOT_INCLUDE,
        INCLUDE_END_ONLY,
        EXCLUDE_END_ONLY
        }

        public enum FuturePastCalculatedDateOptions
        {
            FUTURE_AND_PAST,
            FUTURE_ONLY,
            PAST_ONLY
        }

        private static DateSpanFactory dateSpanFactory = null;
        public bool IgnoreErrors {  get; set; }

        public delegate void DateSpanCreationStatusUpdate(string status);

        private DateSpanFactory()
        {

        }

        public static DateSpanFactory Instance()
        {
            if(dateSpanFactory == null)
                dateSpanFactory = new DateSpanFactory();

            return dateSpanFactory;
        }


       public List<CalculatedDateSpan> GetDateSpansGivenCalanderValues(DateTime givenDate, 
                                                           int years, 
                                                           int months, 
                                                           int weeks, 
                                                           int days,
                                                           FuturePastCalculatedDateOptions futurePastCalculatedDateOptions,
                                                           EndStartDateSetting endStartDateSetting,
                                                           DateSpanCreationStatusUpdate CreationStatusUpdate = null,
                                                           bool includeOnlyFutureDates = false,
                                                           DateSpan onlyGenerateSpansBetweenDates = null,
                                                           int maxPastYears = -1,
                                                           int maxFutureYears = -1)

        {
            List<CalculatedDateSpan> calculatedDates = new List<CalculatedDateSpan>();

            if (futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.FUTURE_AND_PAST || futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.FUTURE_ONLY)
            {
                if(CreationStatusUpdate != null)
                    CreationStatusUpdate.Invoke("Generating Future Calculated Date Spans");

                if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE || endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY) //both end start with end and start without
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, years, months, weeks, days, true, false, IgnoreErrors);

                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);

                    calculatedDate = AddSingleDate(givenDate, years, months, weeks, days, true, true, IgnoreErrors);

                    if (calculatedDate != null)
                         calculatedDates.Add(calculatedDate);
                }
                if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE || endStartDateSetting == EndStartDateSetting.EXCLUDE_END_ONLY)
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, years, months, weeks, days, true, false, IgnoreErrors);

                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }
            }

            if (futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.FUTURE_AND_PAST || futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.PAST_ONLY)
            {
                if (CreationStatusUpdate != null)
                    CreationStatusUpdate.Invoke("Generating Past Calculated Date Spans");

                if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE || endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, years, months, weeks, days, false, false, IgnoreErrors);

                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);

                    calculatedDate = AddSingleDate(givenDate, years, months, weeks, days, false, true, IgnoreErrors);

                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }
                if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE || endStartDateSetting == EndStartDateSetting.EXCLUDE_END_ONLY)
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, years, months, weeks, days, false, false, IgnoreErrors);

                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }
            }
            
            if (CreationStatusUpdate != null)
                CreationStatusUpdate.Invoke("Checking/Apply Specific Settings and Options");

            calculatedDates = TrimCalculatedDateSpans(calculatedDates, includeOnlyFutureDates, onlyGenerateSpansBetweenDates, maxPastYears, maxFutureYears);

            return calculatedDates;
       }

        public List<CalculatedDateSpan> GetDateSpansGivenValue(DateTime givenDate,
                                                           int numberForCalValues,
                                                           bool includeAllNumbersDivByTen,
                                                           FuturePastCalculatedDateOptions futurePastCalculatedDateOptions,
                                                           EndStartDateSetting endStartDateSetting,
                                                           DateSpanCreationStatusUpdate CreationStatusUpdate = null,
                                                           bool includeOnlyFutureDates = false,
                                                           DateSpan onlyGenerateSpansBetweenDates = null,
                                                           int maxPastYears = -1,
                                                           int maxFutureYears = -1)

        {
            List<CalculatedDateSpan> calculatedDates = new List<CalculatedDateSpan>();

            //add
            if (futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.FUTURE_AND_PAST || futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.FUTURE_ONLY)
            {
                if (CreationStatusUpdate != null)
                    CreationStatusUpdate.Invoke("Generating Future Calculated Date Spans");

                if (numberForCalValues == -1)
                {
                    for (int i = 9; i > 0; i--)
                    {
                        int number = i * 111111;
                        calculatedDates.AddRange(CalculateAndFill(givenDate, number, true, true, endStartDateSetting).ToArray());
                    }
                }
                else
                {
                    calculatedDates.AddRange(CalculateAndFill(givenDate, numberForCalValues, true, includeAllNumbersDivByTen, endStartDateSetting).ToArray());
                }
            }

            //subtract
            if (futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.FUTURE_AND_PAST || futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.PAST_ONLY)
            {
                if (CreationStatusUpdate != null)
                    CreationStatusUpdate.Invoke("Generating Past Calculated Date Spans");

                if (numberForCalValues == -1)
                {
                    for (int i = 9; i > 0; i--)
                    {
                        int number = i * 1111111;
                        calculatedDates.AddRange(CalculateAndFill(givenDate, number, false, true, endStartDateSetting).ToArray());
                    }
                }
                else
                {
                    calculatedDates.AddRange(CalculateAndFill(givenDate, numberForCalValues, false, includeAllNumbersDivByTen, endStartDateSetting).ToArray());
                }
            }

            if (CreationStatusUpdate != null)
                CreationStatusUpdate.Invoke("Checking/Apply Specific Settings and Options");

            calculatedDates = TrimCalculatedDateSpans(calculatedDates, includeOnlyFutureDates, onlyGenerateSpansBetweenDates, maxPastYears, maxFutureYears);

            return calculatedDates;
        }

        public List<CalculatedDateSpan> GetDateSpansGivenValues(List<DateTime> givenDates,
                                                         List<long> values,
                                                         FuturePastCalculatedDateOptions futurePastCalculatedDateOptions,
                                                         EndStartDateSetting endStartDateSetting,
                                                         bool generateAllPossibleCombinations,
                                                         DateSpanCreationStatusUpdate CreationStatusUpdate = null,
                                                         DateSpan onlyGenerateSpansBetweenDates = null,
                                                         bool includeOnlyFutureDates = false,
                                                         int maxPastYears = -1,
                                                         int maxFutureYears = -1)

        {
            List<CalculatedDateSpan> calculatedDates = new List<CalculatedDateSpan>();

            if (generateAllPossibleCombinations)
            {
                int currYears = 0;
                int currMonths = 0;
                int currWeeks = 0;
                int currDays = 0;

                //add 0
                values.Add(0);

                int totalEvents = givenDates.Count;
                int totalValueMatches = values.Count;
                int currEvent = 1;
                string eventProgress = "Event(" + currEvent + " of " + totalEvents + ") ";

                foreach (DateTime givenDate in givenDates)
                {
                    foreach (int matchValueYears in values)
                    {
                        currYears++;
                        string yearsProgress = "Year(" + currYears + " of " + totalValueMatches + ") ";

                        foreach (int matchValueMonths in values)
                        {
                            currMonths++;
                            string monthProgress = "Month(" + currMonths + " of " + totalValueMatches + ") ";

                            foreach (int matchValueWeeks in values)
                            {
                                currWeeks++;
                                string weeksProgress = "Week(" + currWeeks + " of " + totalValueMatches + ") ";
                                if (CreationStatusUpdate != null)
                                    CreationStatusUpdate.Invoke("Progress " + eventProgress + yearsProgress + monthProgress + weeksProgress);

                                foreach (int matchValueDays in values)
                                {
                                    currDays++;
                                    //add

                                    if (futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.FUTURE_AND_PAST || futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.FUTURE_ONLY)
                                    {
                                        if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                                        {
                                            Parallel.Invoke(
                                            () =>
                                            {
                                                CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, matchValueYears, matchValueMonths, matchValueWeeks, matchValueDays, true, true, true);
                                                if (calculatedDate != null)
                                                    calculatedDates.Add(calculatedDate);
                                            });
                                            Parallel.Invoke(
                                            () =>
                                            {
                                                CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, matchValueYears, matchValueMonths, matchValueWeeks, matchValueDays, true, false, true);
                                                if (calculatedDate != null)
                                                    calculatedDates.Add(calculatedDate);
                                            });
                                        }
                                        else if (endStartDateSetting == EndStartDateSetting.EXCLUDE_END_ONLY)
                                        {
                                            Parallel.Invoke(
                                            () =>
                                            {
                                                CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, matchValueYears, matchValueMonths, matchValueWeeks, matchValueDays, true, false, true);
                                                if (calculatedDate != null)
                                                    calculatedDates.Add(calculatedDate);
                                            });
                                        }
                                        else
                                        {
                                            Parallel.Invoke(
                                            () =>
                                            {
                                                CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, matchValueYears, matchValueMonths, matchValueWeeks, matchValueDays, true, true, true);
                                                if (calculatedDate != null)
                                                    calculatedDates.Add(calculatedDate);
                                            });

                                        }
                                    }

                                    //subtract
                                    if (futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.FUTURE_AND_PAST || futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.PAST_ONLY)
                                    {
                                        if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                                        {
                                            Parallel.Invoke(
                                             () =>
                                             {
                                                 CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, matchValueYears, matchValueMonths, matchValueWeeks, matchValueDays, false, true, true);
                                                 if (calculatedDate != null)
                                                     calculatedDates.Add(calculatedDate);
                                             });

                                            Parallel.Invoke(
                                            () =>
                                            {
                                                CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, matchValueYears, matchValueMonths, matchValueWeeks, matchValueDays, false, false, true);
                                                if (calculatedDate != null)
                                                    calculatedDates.Add(calculatedDate);
                                            });
                                        }
                                        else if (endStartDateSetting == EndStartDateSetting.EXCLUDE_END_ONLY)
                                        {
                                            Parallel.Invoke(
                                            () =>
                                            {
                                                CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, matchValueYears, matchValueMonths, matchValueWeeks, matchValueDays, false, false, true);
                                                if (calculatedDate != null)
                                                    calculatedDates.Add(calculatedDate);
                                            });
                                        }
                                        else
                                        {
                                            Parallel.Invoke(
                                                 () =>
                                                 {
                                                     CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, matchValueYears, matchValueMonths, matchValueWeeks, matchValueDays, false, true, true);
                                                     if (calculatedDate != null)
                                                         calculatedDates.Add(calculatedDate);
                                                 });
                                        }
                                    }
                                }
                                currDays = 0;
                            }
                            currWeeks = 0;
                        }
                        currMonths = 0;
                    }
                    currYears = 0;
                }
            }
            else
            {
                foreach (int value in values)
                {
                    foreach (DateTime givenDate in givenDates)
                    {
                        //add
                        if (futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.FUTURE_AND_PAST || futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.FUTURE_ONLY)
                        {
                            if (CreationStatusUpdate != null)
                                CreationStatusUpdate.Invoke("Generating Future Calculated Date Spans");

                            calculatedDates.AddRange(CalculateAndFill(givenDate, value, true, false, endStartDateSetting).ToArray());
                        }

                        //subtract
                        if (futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.FUTURE_AND_PAST || futurePastCalculatedDateOptions == FuturePastCalculatedDateOptions.PAST_ONLY)
                        {
                            if (CreationStatusUpdate != null)
                                CreationStatusUpdate.Invoke("Generating Past Calculated Date Spans");

                            calculatedDates.AddRange(CalculateAndFill(givenDate, value, false, false, endStartDateSetting));
                        }
                    }
                }
            }

            if (CreationStatusUpdate != null)
                CreationStatusUpdate.Invoke("Checking/Apply Specific Settings and Options");

            calculatedDates = TrimCalculatedDateSpans(calculatedDates, includeOnlyFutureDates, onlyGenerateSpansBetweenDates, maxPastYears, maxFutureYears);

            return calculatedDates;
        }

        public string ToSentence(CalculatedDateSpan calculatedDateSpan, bool includeNotes = false)
        {
            string SentenceString = "";

            string phrase = calculatedDateSpan.StartDate.ToString("M/d/yyyy") + " occurs exactly ";

            List<string> calDataList = new List<string>();
            if (calculatedDateSpan.Years != 0)
                calDataList.Add(((calculatedDateSpan.Years < 0) ? (calculatedDateSpan.Years * -1) : calculatedDateSpan.Years) + " Years");

            if (calculatedDateSpan.Months != 0)
                calDataList.Add(((calculatedDateSpan.Months < 0) ? (calculatedDateSpan.Months * -1) : calculatedDateSpan.Months) + " Months");

            if (calculatedDateSpan.Weeks != 0)
                calDataList.Add(((calculatedDateSpan.Weeks < 0) ? (calculatedDateSpan.Weeks * -1) : calculatedDateSpan.Weeks) + " Weeks");

            if (calculatedDateSpan.Days != 0)
                calDataList.Add(((calculatedDateSpan.Days < 0) ? (calculatedDateSpan.Days * -1) : calculatedDateSpan.Days) + " Days");

            if (calDataList.Count == 1)
            {
                phrase += calDataList[0];
            }
            else if (calDataList.Count == 2)
            {
                phrase += calDataList[0] + " and " + calDataList[1];
            }
            else if (calDataList.Count > 2)
            {
                for (int i = 0; i < calDataList.Count; i++)
                {
                    if (i == 0)
                        phrase += calDataList[i];
                    else if (i == calDataList.Count - 1)
                        phrase += ", and " + calDataList[i];
                    else
                        phrase += ", " + calDataList[i];
                }
            }

            if (calculatedDateSpan.StartDate > calculatedDateSpan.EndDate)
            {
                phrase += " before the Date " + calculatedDateSpan.EndDate.ToString("M/d/yyyy") + ".";

                if (calculatedDateSpan.IncludesEndDate)
                {
                    phrase += " (Include End Date)";
                }
            }
            else
            {
                phrase += " after the Date " + calculatedDateSpan.EndDate.ToString("M/d/yyyy") + ".";
                if (calculatedDateSpan.IncludesEndDate)
                {
                    phrase += " (Include Start Date)";
                }
            }

            List<string> notes = new List<string>();
            //notes
            if (includeNotes)
            {
                phrase += Environment.NewLine;

                if (calculatedDateSpan.Years != 0)
                {
                    MatchValue matchValue = GematriaFactory.Instance().GetMatchValue(calculatedDateSpan.Years < 0 ? calculatedDateSpan.Years * -1 : calculatedDateSpan.Years);
                    if (matchValue != null)
                    {
                        notes.Add("Years(" + matchValue.Comment + ")");
                    }
                }

                if (calculatedDateSpan.Months != 0)
                {
                    MatchValue matchValue = GematriaFactory.Instance().GetMatchValue(calculatedDateSpan.Months < 0 ? calculatedDateSpan.Months * -1 : calculatedDateSpan.Months);
                    if (matchValue != null)
                    {
                        notes.Add("Months(" + matchValue.Comment + ")");
                    }
                }

                if (calculatedDateSpan.Weeks != 0)
                {
                    MatchValue matchValue = GematriaFactory.Instance().GetMatchValue(calculatedDateSpan.Weeks < 0 ? calculatedDateSpan.Weeks * -1 : calculatedDateSpan.Weeks);
                    if (matchValue != null)
                    {
                        notes.Add("Weeks(" + matchValue.Comment + ")");
                    }
                }

                if (calculatedDateSpan.Days != 0)
                {
                    MatchValue matchValue = GematriaFactory.Instance().GetMatchValue(calculatedDateSpan.Days < 0 ? calculatedDateSpan.Days * -1 : calculatedDateSpan.Days);
                    if (matchValue != null)
                    {
                        notes.Add("Days(" + matchValue.Comment + ")");
                    }
                }

                if (notes.Count == 1)
                {
                    phrase += "Note: " + notes[0];
                }
                else if (notes.Count == 2)
                {
                    phrase += " Notes: " + notes[0] + " and " + notes[1];
                }
                else
                {
                    for (int i = 0; i < notes.Count; i++)
                    {
                        if (i == 0)
                            phrase += "  Notes: " + notes[0];
                        else if (i == notes.Count - 1)
                            phrase += ", and " + notes[i];
                        else
                            phrase += ", " + notes[i];
                    }
                }
            }

            if (phrase.Length > 0)
            {
                SentenceString += phrase;
            }
            return SentenceString;
        }

        private List<CalculatedDateSpan> CalculateAndFill(DateTime givenDate, 
                                                          int number, 
                                                          bool isAddition, 
                                                          bool isDivideBy10,
                                                          EndStartDateSetting endStartDateSetting)
        {
            List<CalculatedDateSpan> createdDatates = new List<CalculatedDateSpan>();

            int dividethisNumberby10 = -1;
            do
            {
                if (dividethisNumberby10 != -1)
                    number = dividethisNumberby10;

                createdDatates.AddRange(CalculateAndFillYears(givenDate, number, isDivideBy10, isAddition, endStartDateSetting).ToArray());
                createdDatates.AddRange(CalculateAndFillMonths(givenDate, number, isDivideBy10, isAddition, endStartDateSetting).ToArray());
                createdDatates.AddRange(CalculateAndFillWeeks(givenDate, number, isDivideBy10, isAddition, endStartDateSetting).ToArray());
                createdDatates.AddRange(CalculateAndFillDays(givenDate, number, isDivideBy10, isAddition, endStartDateSetting).ToArray());

                if (isDivideBy10)
                {
                    dividethisNumberby10 = number / 10;
                    if (dividethisNumberby10 < 1)
                        dividethisNumberby10 = -1;
                }
            } while (dividethisNumberby10 != -1);

            return createdDatates;
        }

        private List<CalculatedDateSpan> CalculateAndFillYears(DateTime givenDate, 
                                                              int number,
                                                              bool divBy10, 
                                                              bool isAdd,
                                                              EndStartDateSetting endStartDateSetting,
                                                              int setYears = -1, 
                                                              int setMonths = -1, 
                                                              int setWeeks = -1, 
                                                              int setDays = -1)
        {
            List<CalculatedDateSpan> calculatedDates = new List<CalculatedDateSpan>();

            if (setYears == -1)
                setYears = number;

            if (setMonths == -1)
                setMonths = number;

            if (setWeeks == -1)
                setWeeks = number;

            if (setDays == -1)
                setDays = number;

            if (isAdd == false)
            {
                if (setYears > 0)
                    setYears *= -1;

                if (setMonths > 0)
                    setMonths *= -1;

                if (setWeeks > 0)
                    setWeeks *= -1;

                if (setDays > 0)
                    setDays *= -1;
            }

            int dividethisNumberby10 = -1;
            if (divBy10)
            {
                dividethisNumberby10 = number / 10;
                if (dividethisNumberby10 < 1)
                    dividethisNumberby10 = -1;
            }

            DateTime startTemp = givenDate;
            try
            {
                //years
                startTemp = startTemp.AddYears(setYears);
                if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, 0, 0, 0, isAdd, true, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }
                else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, 0, 0, 0, isAdd, false, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);

                    calculatedDate = AddSingleDate(givenDate, setYears, 0, 0, 0, isAdd, true, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }
                else
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, 0, 0, 0, isAdd, false, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }

                try
                {
                    //years and months
                    startTemp = startTemp.AddMonths(setMonths);
                    if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                    {
                        CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, setMonths, 0, 0, isAdd, true, IgnoreErrors);
                        if (calculatedDate != null)
                            calculatedDates.Add(calculatedDate);
                    }
                    else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                    {
                        CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, setMonths, 0, 0, isAdd, false, IgnoreErrors);
                        if (calculatedDate != null)
                            calculatedDates.Add(calculatedDate);

                        calculatedDate = AddSingleDate(givenDate, setYears, setMonths, 0, 0, isAdd, true, IgnoreErrors);
                        if (calculatedDate != null)
                            calculatedDates.Add(calculatedDate);
                    }
                    else
                    {
                        CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, setMonths, 0, 0, isAdd, false, IgnoreErrors);
                        if (calculatedDate != null)
                            calculatedDates.Add(calculatedDate);
                    }

                    try
                    {
                        //years, month and weeks
                        startTemp = startTemp.AddDays(setWeeks * 7);
                        if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                        {
                            CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, setMonths, setWeeks, 0, isAdd, true, IgnoreErrors);
                            if (calculatedDate != null)
                                calculatedDates.Add(calculatedDate);
                        }
                        else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                        {
                            CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, setMonths, setWeeks, 0, isAdd, false, IgnoreErrors);
                            if (calculatedDate != null)
                                calculatedDates.Add(calculatedDate);

                            calculatedDate = AddSingleDate(givenDate, setYears, setMonths, setWeeks, 0, isAdd, true, IgnoreErrors);
                            if (calculatedDate != null)
                                calculatedDates.Add(calculatedDate);
                        }
                        else
                        {
                            CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, setMonths, setWeeks, 0, isAdd, false, IgnoreErrors);
                            if (calculatedDate != null)
                                calculatedDates.Add(calculatedDate);
                        }

                        try
                        {
                            //years, months, weeks, and days
                            startTemp = startTemp.AddDays(setDays);
                            if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                            {
                                CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, setMonths, setWeeks, setDays, isAdd, true, IgnoreErrors);
                                if (calculatedDate != null)
                                    calculatedDates.Add(calculatedDate);
                            }
                            else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                            {
                                CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, setMonths, setWeeks, setDays, isAdd, false, IgnoreErrors);
                                if (calculatedDate != null)
                                    calculatedDates.Add(calculatedDate);

                                calculatedDate = AddSingleDate(givenDate, setYears, setMonths, setWeeks, setDays, isAdd, true, IgnoreErrors);
                                if (calculatedDate != null)
                                    calculatedDates.Add(calculatedDate);
                            }
                            else
                            {
                                CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, setMonths, setWeeks, setDays, isAdd, false, IgnoreErrors);
                                if (calculatedDate != null)
                                    calculatedDates.Add(calculatedDate);
                            }

                            try
                            {
                                //years, and weeks
                                startTemp = givenDate;
                                startTemp = startTemp.AddYears(setYears);
                                startTemp = startTemp.AddDays(setWeeks * 7);
                                if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                                {
                                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, 0, setWeeks, 0, isAdd, true, IgnoreErrors);
                                    if (calculatedDate != null)
                                        calculatedDates.Add(calculatedDate);
                                }
                                else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                                {
                                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, 0, setWeeks, 0, isAdd, false, IgnoreErrors);
                                    if (calculatedDate != null)
                                        calculatedDates.Add(calculatedDate);

                                    calculatedDate = AddSingleDate(givenDate, setYears, 0, setWeeks, 0, isAdd, true, IgnoreErrors);
                                    if (calculatedDate != null)
                                        calculatedDates.Add(calculatedDate);
                                }
                                else
                                {
                                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, 0, setWeeks, 0, isAdd, false, IgnoreErrors);
                                    if (calculatedDate != null)
                                        calculatedDates.Add(calculatedDate);
                                }

                                try
                                {
                                    //years, weeks, and days
                                    startTemp = startTemp.AddDays(setDays);
                                    if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                                    {
                                        CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, 0, setWeeks, setDays, isAdd, true, IgnoreErrors);
                                        if (calculatedDate != null)
                                            calculatedDates.Add(calculatedDate);
                                    }
                                    else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                                    {
                                        CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, 0, setWeeks, setDays, isAdd, false, IgnoreErrors);
                                        if (calculatedDate != null)
                                            calculatedDates.Add(calculatedDate);

                                        calculatedDate = AddSingleDate(givenDate, setYears, 0, setWeeks, setDays, isAdd, true, IgnoreErrors);
                                        if (calculatedDate != null)
                                            calculatedDates.Add(calculatedDate);
                                    }
                                    else
                                    {
                                        CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, 0, setWeeks, setDays, isAdd, false, IgnoreErrors);
                                        if (calculatedDate != null)
                                            calculatedDates.Add(calculatedDate);
                                    }

                                    try
                                    {
                                        //years, and days
                                        startTemp = givenDate;
                                        startTemp = startTemp.AddYears(setYears);
                                        startTemp = startTemp.AddDays(setDays);
                                        if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                                        {
                                            CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, 0, 0, setDays, isAdd, true, IgnoreErrors);
                                            if (calculatedDate != null)
                                                calculatedDates.Add(calculatedDate);
                                        }
                                        else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                                        {
                                            CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, 0, 0, setDays, isAdd, false, IgnoreErrors);
                                            if (calculatedDate != null)
                                                calculatedDates.Add(calculatedDate);

                                            calculatedDate = AddSingleDate(givenDate, setYears, 0, 0, setDays, isAdd, true, IgnoreErrors);
                                            if (calculatedDate != null)
                                                calculatedDates.Add(calculatedDate);
                                        }
                                        else
                                        {
                                            CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, setYears, 0, 0, setDays, isAdd, false, IgnoreErrors);
                                            if (calculatedDate != null)
                                                calculatedDates.Add(calculatedDate);
                                        }
                                    }
                                    catch { }
                                }
                                catch { }
                            }
                            catch { }
                        }
                        catch { }
                    }
                    catch { }
                }
                catch { }
            }
            catch { }

            if (dividethisNumberby10 != -1)
                calculatedDates.AddRange(CalculateAndFillYears(givenDate, dividethisNumberby10, divBy10, isAdd, endStartDateSetting, setYears).ToArray());

            return calculatedDates;
        }

        private List<CalculatedDateSpan> CalculateAndFillMonths(DateTime givenDate, 
                                                                int number,                                                              
                                                                bool divBy10, 
                                                                bool isAdd,
                                                                EndStartDateSetting endStartDateSetting,
                                                                int setMonths = -1, 
                                                                int setWeeks = -1, 
                                                                int setDays = -1)
        {
            List<CalculatedDateSpan> calculatedDates = new List<CalculatedDateSpan>();

            if (setMonths == -1)
                setMonths = number;

            if (setWeeks == -1)
                setWeeks = number;

            if (setDays == -1)
                setDays = number;

            if (isAdd == false)
            {
                if (setMonths > 0)
                    setMonths *= -1;

                if (setWeeks > 0)
                    setWeeks *= -1;

                if (setDays > 0)
                    setDays *= -1;
            }

            int dividethisNumberby10 = -1;
            if (divBy10)
            {
                dividethisNumberby10 = number / 10;
                if (dividethisNumberby10 < 1)
                    dividethisNumberby10 = -1;
            }

            DateTime startTemp = givenDate;

            //months
            try
            {
                startTemp = startTemp.AddMonths(setMonths);
                if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, setMonths, 0, 0, isAdd, true, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }
                else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, setMonths, 0, 0, isAdd, false, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);

                    calculatedDate = AddSingleDate(givenDate, 0, setMonths, 0, 0, isAdd, true, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }
                else
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, setMonths, 0, 0, isAdd, false, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }

                try
                {
                    //month and weeks
                    startTemp = startTemp.AddDays(setWeeks * 7);
                    if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                    {
                        CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, setMonths, setWeeks, 0, isAdd, true, IgnoreErrors);
                        if (calculatedDate != null)
                            calculatedDates.Add(calculatedDate);
                    }
                    else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                    {
                        CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, setMonths, setWeeks, 0, isAdd, false, IgnoreErrors);
                        if (calculatedDate != null)
                            calculatedDates.Add(calculatedDate);

                        calculatedDate = AddSingleDate(givenDate, 0, setMonths, setWeeks, 0, isAdd, true, IgnoreErrors);
                        if (calculatedDate != null)
                            calculatedDates.Add(calculatedDate);
                    }
                    else
                    {
                        CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, setMonths, setWeeks, 0, isAdd, false, IgnoreErrors);
                        if (calculatedDate != null)
                            calculatedDates.Add(calculatedDate);
                    }

                    try
                    {
                        // months, weeks, and days
                        startTemp = startTemp.AddDays(setDays);
                        if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                        {
                            CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, setMonths, setWeeks, setDays, isAdd, true, IgnoreErrors);
                            if (calculatedDate != null)
                                calculatedDates.Add(calculatedDate);
                        }
                        else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                        {
                            CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, setMonths, setWeeks, setDays, isAdd, false, IgnoreErrors);
                            if (calculatedDate != null)
                                calculatedDates.Add(calculatedDate);

                            calculatedDate = AddSingleDate(givenDate, 0, setMonths, setWeeks, setDays, isAdd, true, IgnoreErrors);
                            if (calculatedDate != null)
                                calculatedDates.Add(calculatedDate);
                        }
                        else
                        {
                            CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, setMonths, setWeeks, setDays, isAdd, false, IgnoreErrors);
                            if (calculatedDate != null)
                                calculatedDates.Add(calculatedDate);
                        }
                        try
                        {
                            //month, and days
                            startTemp = givenDate;
                            startTemp = startTemp.AddMonths(setMonths);
                            startTemp = startTemp.AddDays(setDays);
                            if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                            {
                                CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, setMonths, 0, setDays, isAdd, true, IgnoreErrors);
                                if (calculatedDate != null)
                                    calculatedDates.Add(calculatedDate);
                            }
                            else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                            {
                                CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, setMonths, 0, setDays, isAdd, false, IgnoreErrors);
                                if (calculatedDate != null)
                                    calculatedDates.Add(calculatedDate);

                                calculatedDate = AddSingleDate(givenDate, 0, setMonths, 0, setDays, isAdd, true, IgnoreErrors);
                                if (calculatedDate != null)
                                    calculatedDates.Add(calculatedDate);
                            }
                            else
                            {
                                CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, setMonths, 0, setDays, isAdd, false, IgnoreErrors);
                                if (calculatedDate != null)
                                    calculatedDates.Add(calculatedDate);
                            }
                        }
                        catch { }
                    }
                    catch { }
                }
                catch { }
            }
            catch { }

            if (dividethisNumberby10 != -1)
                calculatedDates.AddRange(CalculateAndFillMonths(givenDate, dividethisNumberby10, divBy10, isAdd, endStartDateSetting, setMonths).ToArray());

            return calculatedDates;
        }

        private List<CalculatedDateSpan> CalculateAndFillWeeks(DateTime givenDate,
                                                               int number, 
                                                               bool divBy10, 
                                                               bool isAdd,
                                                               EndStartDateSetting endStartDateSetting,
                                                               int setWeeks = -1, 
                                                               int setDays = -1)
        {
            List<CalculatedDateSpan> calculatedDates = new List<CalculatedDateSpan>();

            if (setWeeks == -1)
                setWeeks = number;

            if (setDays == -1)
                setDays = number;

            if (isAdd == false)
            {
                if (setWeeks > 0)
                    setWeeks *= -1;

                if (setDays > 0)
                    setDays *= -1;
            }

            int dividethisNumberby10 = -1;
            if (divBy10)
            {
                dividethisNumberby10 = number / 10;
                if (dividethisNumberby10 < 1)
                    dividethisNumberby10 = -1;
            }

            DateTime startTemp = givenDate;

            try
            {
                //weeks
                startTemp = startTemp.AddDays(setWeeks * 7);
                if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, 0, setWeeks, 0, isAdd, true, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }
                else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, 0, setWeeks, 0, isAdd, false, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);

                    calculatedDate = AddSingleDate(givenDate, 0, 0, setWeeks, 0, isAdd, true, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }
                else
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, 0, setWeeks, 0, isAdd, false, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }

                try
                {
                    //  weeks, and days
                    startTemp = startTemp.AddDays(setDays);
                    if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                    {
                        CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, 0, setWeeks, setDays, isAdd, true, IgnoreErrors);
                        if (calculatedDate != null)
                            calculatedDates.Add(calculatedDate);
                    }
                    else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                    {
                        CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, 0, setWeeks, setDays, isAdd, false, IgnoreErrors);
                        if (calculatedDate != null)
                            calculatedDates.Add(calculatedDate);

                        calculatedDate = AddSingleDate(givenDate, 0, 0, setWeeks, setDays, isAdd, true, IgnoreErrors);
                        if (calculatedDate != null)
                            calculatedDates.Add(calculatedDate);
                    }
                    else
                    {
                        CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, 0, setWeeks, setDays, isAdd, false, IgnoreErrors);
                        if (calculatedDate != null)
                            calculatedDates.Add(calculatedDate);
                    }
                }
                catch { }
            }
            catch { }

            if (dividethisNumberby10 != -1)
                calculatedDates.AddRange(CalculateAndFillWeeks(givenDate, dividethisNumberby10, divBy10, isAdd, endStartDateSetting, setWeeks).ToArray());

            return calculatedDates;
        }

        private List<CalculatedDateSpan> CalculateAndFillDays(DateTime givenDate, 
                                                             int number, 
                                                             bool divBy10,
                                                             bool isAdd,
                                                             EndStartDateSetting endStartDateSetting,
                                                             int setDays = -1)
        {
            List<CalculatedDateSpan> calculatedDates = new List<CalculatedDateSpan>();

            if (setDays == -1)
                setDays = number;

            if (isAdd == false)
            {
                if (setDays > 0)
                    setDays *= -1;
            }

            int dividethisNumberby10 = -1;
            if (divBy10)
            {
                dividethisNumberby10 = number / 10;
                if (dividethisNumberby10 < 1)
                    dividethisNumberby10 = -1;
            }

            DateTime startTemp = givenDate;

            try
            {
                //days
                startTemp = startTemp.AddDays(setDays);
                if (endStartDateSetting == EndStartDateSetting.INCLUDE_END_ONLY)
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, 0, 0, setDays, isAdd, true, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }
                else if (endStartDateSetting == EndStartDateSetting.INCLUDE_AND_NOT_INCLUDE)
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, 0, 0, setDays, isAdd, false, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);

                    calculatedDate = AddSingleDate(givenDate, 0, 0, 0, setDays, isAdd, true, IgnoreErrors);
                    if (calculatedDate != null)
                        calculatedDates.Add(calculatedDate);
                }
                else
                {
                    CalculatedDateSpan calculatedDate = AddSingleDate(givenDate, 0, 0, 0, setDays, isAdd, false, IgnoreErrors);
                }
            }
            catch { }

            if (dividethisNumberby10 != -1)
                calculatedDates.AddRange(CalculateAndFillDays(givenDate, dividethisNumberby10, divBy10, isAdd, endStartDateSetting, setDays).ToArray());

            return calculatedDates;
        }

        private CalculatedDateSpan AddSingleDate(DateTime givenDate,
                                                 int years,
                                                 int months,
                                                 int weeks,
                                                 int days,
                                                 bool isFutureSpan,
                                                 bool isUseEndDate,
                                                 bool ignoreErrors)
        {
            if (isFutureSpan == false)
            {
                if (years > 0)
                    years *= -1;

                if (months > 0)
                    months *= -1;

                if (weeks >= 0)
                    weeks *= -1;

                if (days > 0)
                    days *= -1;
            }
            DateTime startDate = givenDate;
            try
            {
                if (years != 0)
                    startDate = startDate.AddYears(years);
            }
            catch
            {
                string calculateTypeString = isFutureSpan ? "Add" : "Subtract";
                if (ignoreErrors == false)
                {
                    CalculatedDateSpan calculatedDateSpan = new CalculatedDateSpan();
                    calculatedDateSpan.ErrorString = "Unable to " + calculateTypeString + " " + years + " years to date.  Out of date bounds.";
                    return calculatedDateSpan;
                }
                return null;
            }

            try
            {
                if (months != 0)
                    startDate = startDate.AddMonths(months);
            }
            catch
            {
                string calculateTypeString = isFutureSpan ? "Add" : "Subtract";
                if (ignoreErrors == false)
                {
                    CalculatedDateSpan calculatedDateSpan = new CalculatedDateSpan();
                    calculatedDateSpan.ErrorString = "Unable to " + calculateTypeString + " " + months + " months to date.  Out of date bounds.";
                    return calculatedDateSpan;
                }
                return null;
            }

            try
            {
                if (weeks != 0)
                    startDate = startDate.AddDays(weeks * 7);
            }
            catch
            {
                string calculateTypeString = isFutureSpan ? "Add" : "Subtract";
                if (ignoreErrors == false)
                {
                    CalculatedDateSpan calculatedDateSpan = new CalculatedDateSpan();
                    calculatedDateSpan.ErrorString = "Unable to " + calculateTypeString + " " + weeks + " weeks to date.  Out of date bounds.";
                    return calculatedDateSpan;
                }
                return null;
            }

            try
            {
                if (days != 0)
                    startDate = startDate.AddDays(days);
            }
            catch
            {
                string calculateTypeString = isFutureSpan ? "Add" : "Subtract";
                if (ignoreErrors == false)
                {
                    CalculatedDateSpan calculatedDateSpan = new CalculatedDateSpan();
                    calculatedDateSpan.ErrorString = "Unable to " + calculateTypeString + " " + " days to date.  Out of date bounds: " + days;
                    return calculatedDateSpan;
                }
                return null;
            }

            CalculatedDateSpan calculatedDate = CreateDateInstance(givenDate, startDate, isFutureSpan, years, months, weeks, days, isUseEndDate);

            return calculatedDate;
        }

        private CalculatedDateSpan CreateDateInstance(DateTime dateGiven,
                                                     DateTime newDate,
                                                     bool isfutureDate,
                                                     int years,
                                                     int months,
                                                     int weeks,
                                                     int days,
                                                     bool includesEnd)  
        {
            if (years == 0 && months == 0 && weeks == 0 && days == 0)
                return null;

            if (includesEnd)
            {
                if (isfutureDate)
                {
                    newDate = newDate.AddDays(-1);
                }
                else
                {
                    newDate = newDate.AddDays(1);
                }
            }
            CalculatedDateSpan calculatedDateSpan = new CalculatedDateSpan(dateGiven, newDate, years, months, weeks, days, includesEnd);
            return calculatedDateSpan;
        }

        private List<CalculatedDateSpan> TrimCalculatedDateSpans(List<CalculatedDateSpan> calculatedDates,
                                                   bool includeOnlyFutureDates = false,
                                                   DateSpan onlyGenerateSpansBetweenDates = null,
                                                   int maxPastYears = -1,
                                                   int maxFutureYears = -1)
        {
            if (onlyGenerateSpansBetweenDates != null)
            {
                for (int i = calculatedDates.Count - 1; i > -1; i--)
                {
                    if (calculatedDates[i].EndDate < calculatedDates[i].StartDate)
                    {
                        if (calculatedDates[i].EndDate < onlyGenerateSpansBetweenDates.StartDate || calculatedDates[i].StartDate > onlyGenerateSpansBetweenDates.EndDate)
                        {
                            calculatedDates.RemoveAt(i);
                        }
                    }
                    else
                    {
                        if (calculatedDates[i].EndDate > onlyGenerateSpansBetweenDates.EndDate || calculatedDates[i].StartDate < onlyGenerateSpansBetweenDates.StartDate)
                        {
                            calculatedDates.RemoveAt(i);
                        }
                    }
                }
            }

            if (maxFutureYears > 0)
            {
                int limitYearsBy = maxFutureYears;
                DateTime now = DateTime.Now;
                try
                {
                    now = now.AddYears(limitYearsBy);

                    for (int i = calculatedDates.Count - 1; i > -1; i--)
                    {
                        //only future dates?
                        if (includeOnlyFutureDates)
                            if (calculatedDates[i].EndDate < now)
                                calculatedDates.RemoveAt(i);


                        if (calculatedDates[i].EndDate > now)
                            calculatedDates.RemoveAt(i);
                    }
                }
                catch
                { }
            }

            if (maxPastYears > 0)
            {
                DateTime now = DateTime.Now;
                int limitYearsBy = maxPastYears;

                try
                {
                    now = now.AddYears(limitYearsBy * -1);

                    for (int i = calculatedDates.Count - 1; i > -1; i--)
                    {
                        if (calculatedDates[i].EndDate < now)
                            calculatedDates.RemoveAt(i);
                    }
                }
                catch
                { }
            }
            return calculatedDates;
        }
    }
}
