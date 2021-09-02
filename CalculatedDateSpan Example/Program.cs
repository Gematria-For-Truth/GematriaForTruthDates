using GematriaForTruth.API.Factory;
using GematriaToolsForTruth.API.DataStructures;
using GematriaToolsForTruth.API.Factory;
using GematriaToolsForTruth.API.Util;
using GematriaToolsForTruthAPI.GematriaForTruth.API.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CalculatedDateSpan_Example
{
class Program
    {
        static void Main()
        {
            bool exit = false;
            while (exit == false)
            {
                ///////////////////////////////////////////INITIALIZATION//////////////////////////////////////////////
                List<CalculatedDateSpan> calculatedDateSpans;

                //common vairable
                string[] futurePastCalculatedDateSelections = { "Yes_No", "Yes", "No" };
                string[] endStartDateSelections = { "Future_Past", "Future_Only", "Past_Only" };
                string[] outputFormatSelection = { "Screen", "TextFile" };
                //create the status callback delegate for progress on creation
                DateSpanFactory.DateSpanCreationStatusUpdate dateSpanCreationStatusUpdate = new DateSpanFactory.DateSpanCreationStatusUpdate(UpdateProgress);

                //calendar variables
                string[] calculationOptionsSelection = { "Calendar", "Number", "Repeating", "Numbers" };
                int years = 0;
                int months = 0;
                int weeks = 0;
                int days = 0;

                //number variables
                int baseNumber;
                bool divBy10;

                //numbers variables
                List<long> numbers;
                bool allPossibleCombinations = false;

                ///////////////////////////////////////////GATHER USER INPUT COMMON TO GENERATION FOR ALL TYPES//////////////////////////////////////////////

                //prompt the user for given date
                DateTime dateGiven = ConsoleWidgets.PromptDate();
            
                //prompt he user for the calculation option selection
                int calculationsSelection = ConsoleWidgets.PromptSelection("Select End Date(s) Calculation Option", calculationOptionsSelection, 0);

                ///////////////////////////////////////////GATHER USER INPUT SPECIFIC TO CALCULATION TYPE//////////////////////////////////////////////

                if (calculationsSelection == 0) //calendar
                {
                    //PromptFuturePastOption the user for the calendar numbers
                     years = ConsoleWidgets.PromptNumber("Years", 0);
                     months = ConsoleWidgets.PromptNumber("Months", 0);
                     weeks = ConsoleWidgets.PromptNumber("Weeks", 0);
                     days = ConsoleWidgets.PromptNumber("Days", 0);
                }
                else if(calculationsSelection == 1) //number
                {
                    //prompt the user for the base number
                    baseNumber = ConsoleWidgets.PromptNumber("Base Number");

                    //prompt the user for the Div10 Option
                    divBy10 = false;
                    if (GematriaUtils.AreAllSameDigitChars(baseNumber.ToString()))
                    {
                        //only applies when all digits are alike
                        ConsoleWidgets.YesNo yesNoReply = ConsoleWidgets.PromptYesNo("Create additional numbers by dividing the digits by 10", ConsoleWidgets.YesNo.NO);
                        if (yesNoReply == ConsoleWidgets.YesNo.YES)
                            divBy10 = true;
                    }
                }
                else if(calculationsSelection == 3) //numbers
                {
                    //prompt the user for a set of commas separated numbers to use
                    numbers = ConsoleWidgets.PromptNumbers();

                    //prompt the user for the all possible combinations option
                    ConsoleWidgets.YesNo yesNoAllCombo = ConsoleWidgets.PromptYesNo("Create all possible number combinations", ConsoleWidgets.YesNo.NO);
                    if (yesNoAllCombo == ConsoleWidgets.YesNo.YES)
                        allPossibleCombinations = true; 
                }

                ///////////////////////////////////////////GATHER MORE USER INPUT COMMON TO GENERATION FOR ALL TYPES//////////////////////////////////////////////

                //common widgets to all creation types
                //prompt the user for the past/future calculated date options
                DateSpanFactory.FuturePastCalculatedDateOptions futurePastCalculatedDateOptions =
                    (DateSpanFactory.FuturePastCalculatedDateOptions)ConsoleWidgets.PromptSelection("Enter Calculated Date Time Direction Option", futurePastCalculatedDateSelections, 0);

                //prompt the user for the Include start/end date options
                DateSpanFactory.EndStartDateSetting endStartDateSetting =
                   (DateSpanFactory.EndStartDateSetting)ConsoleWidgets.PromptSelection("Enter Calculated Date start/end date options", endStartDateSelections, 0);

                int limitFutureEndDateInYears = 0;
                bool limitToOnlyFutureDates = false; 

                //prompt the user if they want to limit future events in years from NOW date.
                if (futurePastCalculatedDateOptions == DateSpanFactory.FuturePastCalculatedDateOptions.FUTURE_AND_PAST || futurePastCalculatedDateOptions == DateSpanFactory.FuturePastCalculatedDateOptions.FUTURE_ONLY)
                {
                    limitFutureEndDateInYears = ConsoleWidgets.PromptNumber("Limit future end date(s) from NOW in years", 2);

                    //prompt the user if they want to limit Calculated Date to only future Dates from NOW
                    ConsoleWidgets.YesNo yesNoLimitToFuture = ConsoleWidgets.PromptYesNo("Limit Calculated Date(s) to only future Date(s)", ConsoleWidgets.YesNo.NO);
                    if (yesNoLimitToFuture == ConsoleWidgets.YesNo.YES)
                        limitToOnlyFutureDates = true;
                }

                int limitPastEndDateInYears = 0;
                //prompt the user if they want to limit past events in years from NOW date
                if (futurePastCalculatedDateOptions == DateSpanFactory.FuturePastCalculatedDateOptions.FUTURE_AND_PAST || futurePastCalculatedDateOptions == DateSpanFactory.FuturePastCalculatedDateOptions.PAST_ONLY)
                {
                    limitPastEndDateInYears = ConsoleWidgets.PromptNumber("Limit past end date(s) from NOW in years", 0);
                }

                //prompt the user if they want to limit ending dates within a given date span.
                DateSpan limitWithinDateSpan = null;
                ConsoleWidgets.YesNo yesNoDateSpan = ConsoleWidgets.PromptYesNo("Limit Calculated Date(s) to a given date span", ConsoleWidgets.YesNo.NO);
                if(yesNoDateSpan == ConsoleWidgets.YesNo.YES)
                {
                    limitWithinDateSpan = ConsoleWidgets.PromptDateSpan();
                }

                 ///////////////////////////////////////////HANDLE THE CALLS TO THE API FOR GENERATION FOR EACH CALCULATION TYPE//////////////////////////////////////////////

                if (calculationsSelection == 0) //calendar
                {
                    //call the API with the options we have gathered
                    calculatedDateSpans = DateSpanFactory.Instance().GetDateSpansGivenCalanderValues(dateGiven, 
                                                                                                     years, 
                                                                                                     months, 
                                                                                                     weeks, 
                                                                                                     days, 
                                                                                                     futurePastCalculatedDateOptions, 
                                                                                                     endStartDateSetting,
                                                                                                     dateSpanCreationStatusUpdate,
                                                                                                     limitToOnlyFutureDates,
                                                                                                     limitWithinDateSpan,
                                                                                                     limitPastEndDateInYears,
                                                                                                     limitFutureEndDateInYears);
                }
                else if(calculationsSelection == 1)
                {
                    //get and create the Calculated Span Dates
                    //call the API with the options we have gathered
                    calculatedDateSpans = DateSpanFactory.Instance().GetDateSpansGivenValue(dateGiven,
                                                                                            baseNumber,
                                                                                            divBy10,
                                                                                            futurePastCalculatedDateOptions,
                                                                                            endStartDateSetting,
                                                                                            dateSpanCreationStatusUpdate,
                                                                                            limitToOnlyFutureDates,
                                                                                            limitWithinDateSpan,
                                                                                            limitPastEndDateInYears,
                                                                                            limitFutureEndDateInYears);
                }
                else if (calculationsSelection == 2) //repeating
                {
                    //call the API with the options we have gathered
                    calculatedDateSpans = DateSpanFactory.Instance().GetDateSpansGivenValue(dateGiven, 
                                                                                            -1, 
                                                                                            false, 
                                                                                            futurePastCalculatedDateOptions, 
                                                                                            endStartDateSetting,
                                                                                            dateSpanCreationStatusUpdate,
                                                                                            limitToOnlyFutureDates,
                                                                                            limitWithinDateSpan,
                                                                                            limitPastEndDateInYears,
                                                                                            limitFutureEndDateInYears);
                }
                else if(calculationsSelection == 3) //numbers
                {
                    List<DateTime> givenDates = new List<DateTime>();
                    givenDates.Add(dateGiven);

                    //call the API with the options we have gathered
                    calculatedDateSpans = DateSpanFactory.Instance().GetDateSpansGivenValues(givenDates, 
                                                                                             numbers,
                                                                                             futurePastCalculatedDateOptions, 
                                                                                             endStartDateSetting, 
                                                                                             allPossibleCombinations, 
                                                                                             dateSpanCreationStatusUpdate,
                                                                                             limitWithinDateSpan,
                                                                                             limitToOnlyFutureDates,
                                                                                             limitPastEndDateInYears,
                                                                                             limitFutureEndDateInYears);
                    Console.WriteLine("");
                }

                ///////////////////////////////////////////GENERATE THE OUPUT//////////////////////////////////////////////

                //print the data
                bool pauseBetweenData = false;
                bool displayNotes = false;

                //display number of date spans
                Console.WriteLine(calculatedDateSpans.Count + " Calculated Date Spans Created.");

                //prompt the user if they was the output to go to the screen or a txt file.
                int outputFormatSelected = ConsoleWidgets.PromptSelection("Select output format", outputFormatSelection, 0);

                //prompt if the data should dispay extra information (notes)
                ConsoleWidgets.YesNo yesNo = ConsoleWidgets.PromptYesNo("Display Notes", ConsoleWidgets.YesNo.NO);
                if (yesNo == ConsoleWidgets.YesNo.YES)
                    displayNotes = true;

                if (outputFormatSelected == 0) //screen
                {
                    ///////////////////////////////////////////GENERATE THE OUPUT - SCREEN//////////////////////////////////////////////
                    
                    //prompt the user if they want t o puse between each date span
                    ConsoleWidgets.YesNo yesNoPause = ConsoleWidgets.PromptYesNo("Pause between date spans", ConsoleWidgets.YesNo.NO);
                    if (yesNoPause == ConsoleWidgets.YesNo.YES)
                        pauseBetweenData = true;

                    //loop through the created Calculated date spans and display them to the console
                    foreach (CalculatedDateSpan calculatedDateSpan in calculatedDateSpans)
                    {
                        //Convert the Calculated Date Span object to a string readable format.
                        string sentence = DateSpanFactory.Instance().ToSentence(calculatedDateSpan, displayNotes);
                       
                        if (sentence.Length > 0)
                        {
                            Console.WriteLine("");

                            Console.WriteLine(sentence);

                            if (pauseBetweenData)
                            {
                                ConsoleWidgets.PressKeyToContinue();
                            }
                        }
                    }
                    Console.WriteLine("");
                }
                else //text file
                {
                    ///////////////////////////////////////////GENERATE THE OUPUT - TEXT FILE//////////////////////////////////////////////
                    StringBuilder stringBuilder = new StringBuilder();
                    int totalCount = calculatedDateSpans.Count;
                    Console.WriteLine("");

                    //User Documnents directory + gematria for truth (version) direction + exports
                    string fileName = GematriaFactory.Instance().GetExportsDir() + "\\" + "Calculated_DateSpans_" + string.Format("yyyyMMdd_HHmmss.txt", DateTime.Now);

                    //generate the string for the txt file
                    for (int i = 0; i < calculatedDateSpans.Count; i++)
                    {
                        //display progress
                        UpdateProgress("Processing Calculated Date Span " + (i + 1) + " of " + totalCount);

                        //Convert the Calculated Date Span object to a string readable format.
                        string sentence = DateSpanFactory.Instance().ToSentence(calculatedDateSpans[i], displayNotes);
                        stringBuilder.Append(sentence + Environment.NewLine);
                        stringBuilder.Append(Environment.NewLine);
                    }
                    UpdateProgress("Processing Complete. Writting File: " + fileName);
                    
                    //write file
                    File.WriteAllText(fileName, stringBuilder.ToString());
                    UpdateProgress("Writing Complete. Opening File: " + fileName);
                   
                    //open the file in the editor
                    Process.Start(@fileName);
                    Console.WriteLine("");
                }

                exit = ConsoleWidgets.ContinueOrExit();
            }
        }

        ///////////////////////////////////////////FUNCTION DELEGATE TO HANDLE STATUS CALLBACKS FROM THE API DURING GENERATION//////////////////////////////////////////////
        public static void UpdateProgress(string progress)
        {
            Console.Write("\r" + progress);
        }
    }
}
