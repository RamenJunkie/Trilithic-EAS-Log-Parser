using System.IO;
using System;
using System.Text;

static void processlog(string filename)
{
    String? line = "";
    string[] dateSplit;
    string[] firstDate;
    List<int> weekStartsList = new List<int>();
    int firstDayOfTheWeek;
    int currentDayOfTheWeek = 0;
    List<string> TransmitLogList = new List<string>();
    List<string> RecieveLogList = new List<string>();
    string currentLogEntry = "";
    string entryType = "";
    bool loopBreak = true;
    string message = "";
    int[] weekStarts;

    try
    {
        //Pass the file path and file name to the StreamReader constructor
        StreamReader sr = new StreamReader(filename);

        //Read until the first actual entry
        do{
            line = sr.ReadLine();
        } while(!(line.Contains(">>>=")));
        
        line = sr.ReadLine();
        // Figure out day of the week for the first actual entry
        dateSplit = line.Split(" ");
        firstDate = dateSplit[0].Split("/");
        //DEBUG Console.WriteLine(firstDate[2]);
        DateTime dateValue = new DateTime(int.Parse(firstDate[2]), int.Parse(firstDate[0]), int.Parse(firstDate[1]));
        firstDayOfTheWeek = (int) dateValue.DayOfWeek;

        // Create an array of dates for the first day of each week.
        currentDayOfTheWeek = int.Parse(firstDate[1])-firstDayOfTheWeek;
        do{
        weekStartsList.Add(currentDayOfTheWeek);
        currentDayOfTheWeek += 7;
        }
        while(currentDayOfTheWeek < 38);

        weekStarts = weekStartsList.ToArray();
        // DEBUG
        // Console.WriteLine(weekStarts[0]);
        // Console.WriteLine(weekStarts[1]);
        // Console.WriteLine(weekStarts[2]);


        //Continue to read until you reach end of file
        do
        {   
            // Extract all EAS Transmissions for later display.
            if(line.Contains("Transmit log"))
            {
                entryType = "Transmit";
            }
            else if(line.Contains("Receive log"))
            {
                entryType = "Recieve";
            }
            currentLogEntry = "";
            currentLogEntry += line.Split(" ")[0];
            currentLogEntry += "\t";
            loopBreak = true;

            do
            {
                line = sr.ReadLine();
                //DEBUG if(line == null) {Console.WriteLine("NULL LINE");}
                if(line == null || line.Contains("====")) {loopBreak = false;}

                ////  CONVERT THIS TO A LOOP BREAK VARIABLE CHECK FOR ==== AND NULL


                if(line != null) {    
                    if(line.Contains("EVENT: ") || line.Contains("EAS MESSAGE from:") || (line.Contains("CAP MESSAGE from: ") && entryType == "Recieve"))
                    {
                        message = line.Split(": ")[1];
                        currentLogEntry += message;
                        currentLogEntry += "\t\t";
                        if(message.Contains("IPAWS")) 
                        {
                            currentLogEntry += "\t\t\t"; 
                        }
                    }
                }
            } while(loopBreak);

            if(currentLogEntry != "") 
            {
                if(entryType == "Recieve")
                {
                    RecieveLogList.Add((currentLogEntry)); 
                }
                else if(entryType == "Transmit")
                {
                    TransmitLogList.Add((currentLogEntry)); 
                }
                
            }
            
            //DEBUG Console.WriteLine(currentLogEntry);
            line = sr.ReadLine();
            //DEBUG Console.WriteLine($"TEST - {line}");
            //DEBUG if(line == null) { Console.WriteLine("Error"); }

        } while (line != null);

        //close the file
        sr.Close();

        /// Output Sequence
        /// 

            //Open the File
        StreamWriter sw = new StreamWriter($"{filename}-Processed.txt", true, Encoding.ASCII);


        int currentWeek = 1;
        // Output the data
        sw.WriteLine("----------------------------------------");
        sw.WriteLine("Monthly Tests");
        sw.WriteLine("----------------------------------------");
        sw.WriteLine();
        foreach(string i in RecieveLogList.ToArray())
        {
            if(i.Contains("Monthly")) {
            sw.WriteLine(i);
            }
        }

        sw.WriteLine();
        sw.WriteLine("----------------------------------------");
        sw.WriteLine("Recieve Dates");
        sw.WriteLine("----------------------------------------");
        sw.WriteLine();
        sw.WriteLine($"Week {currentWeek}: ");
        foreach(string i in RecieveLogList.ToArray())
        {
            if(!(i.Contains("Monthly"))) {
            if(int.Parse(i.Split("/")[1]) >= weekStarts[currentWeek]) 
            { 
                currentWeek +=1;
                sw.WriteLine(); 
                sw.WriteLine($"Week {currentWeek}: ");
            }
            sw.WriteLine(i);
            }
        }

        sw.WriteLine();
        sw.WriteLine("----------------------------------------");
        sw.WriteLine("Transmit Dates");
        sw.WriteLine("----------------------------------------");
        sw.WriteLine();
        foreach(string i in TransmitLogList.ToArray())
        {
            sw.WriteLine(i);
        }   

        //close the file
        sw.Close();


    }
    catch(Exception e)
    {
        Console.WriteLine("Exception: " + e.Message);
    }
    finally
    {
        Console.WriteLine("Done Reading.");
    }
}


//processlog("EAS001_DCTRILDCW14_2022_10.txt");
if(args[0].Contains(".txt"))
{
    //DEBUG Console.WriteLine(args[0]);
    processlog(args[0]);
}
else
{
    Console.WriteLine("No File Supplied.");
}


