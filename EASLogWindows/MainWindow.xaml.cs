using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Text;


namespace EASlog.Dialogs
{
	public partial class OpenFileDialogEAS : Window
	{
		public OpenFileDialogEAS()
		{
			InitializeComponent();
			txtEditor.Text = "Select Which Log File to open by clicking above...";
		}

		public void btnOpenFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if(openFileDialog.ShowDialog() == true)
				//txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
				//tester(openFileDialog.FileName);
                //txtEditor.Text = $"{openFileDialog.FileName}";
				processlog(openFileDialog.FileName);
		}

		public void processlog(string filename)
		{
			String? line = "";
			string[] dateSplit;
			string[] firstDate;
			System.Collections.Generic.List<int> weekStartsList = new System.Collections.Generic.List<int>();
			int firstDayOfTheWeek;
			int currentDayOfTheWeek = 0;
			System.Collections.Generic.List<string> TransmitLogList = new System.Collections.Generic.List<string>();
			System.Collections.Generic.List<string> RecieveLogList = new System.Collections.Generic.List<string>();
			string currentLogEntry = "";
			string entryType = "";
			bool loopBreak = true;
			string message = "";
			int[] weekStarts;

			try
			{
				txtEditor.Text = $"File opened, {filename}... \n\n";
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
					loopBreak = true;
					//txtEditor.Text += $"{line}\n";
					// Extract all EAS Transmissions for later display.
					if(line.Contains("Transmit log"))
					{
						entryType = "Transmit";
						//DEBUG txtEditor.Text += "Transmit Found\n";
						currentLogEntry += line.Split(" ")[0];
						currentLogEntry += "\t";
					}
					else if(line.Contains("Receive log"))
					{
						entryType = "Recieve";
						//DEBUG txtEditor.Text += "Recieve Found\n";
						currentLogEntry += line.Split(" ")[0];
						currentLogEntry += "\t";
					}
					else 
					{
						//txtEditor.Text += $"{line}\n";
						do
						{
							line = sr.ReadLine();
							//DEBUG if(line == null) {Console.WriteLine("NULL LINE");}
							if(line == null || line.Contains("====")) {loopBreak = false;}

							////  CONVERT THIS TO A LOOP BREAK VARIABLE CHECK FOR ==== AND NULL


							if(loopBreak) {    
								if(line.Contains("EVENT:") || line.Contains("EAS MESSAGE from:") || (line.Contains("CAP MESSAGE from: ") && entryType == "Recieve"))
								{
									message = line.Split(": ")[1];
									currentLogEntry += message;
									currentLogEntry += "\t\t";
									if(message.Contains("IPAWS")) 
									{
										currentLogEntry += "\t\t\t"; 
									}
								}
								if(line.Contains("SUCCESSFUL DELIVERIES:"))
								{
									currentLogEntry += "Successfully Delivered";
								}
								if(line.Contains("FAILED DELIVERIES:"))
								{
									currentLogEntry += "Delivery Failed";
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
							currentLogEntry = "";
							
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
				txtEditor.Text += "----------------------------------------\nMonthly Tests\n----------------------------------------\n\n";
				foreach(string i in RecieveLogList.ToArray())
				{
					if(i.Contains("Monthly")) {
					sw.WriteLine(i);
					txtEditor.Text += $"{i}\n";
					}
				}

				sw.WriteLine();
				sw.WriteLine("----------------------------------------");
				sw.WriteLine("Recieve Dates");
				sw.WriteLine("----------------------------------------");
				sw.WriteLine();
				txtEditor.Text += "----------------------------------------\nRecieve Dates\n----------------------------------------\n\n";
				sw.WriteLine($"Week {currentWeek}: ");
				txtEditor.Text += $"Week {currentWeek}: \n";
				foreach(string i in RecieveLogList.ToArray())
				{
					if(!(i.Contains("Monthly"))) {
					if(int.Parse(i.Split("/")[1]) >= weekStarts[currentWeek]) 
					{ 
						currentWeek +=1;
						sw.WriteLine(); 
						sw.WriteLine($"Week {currentWeek}: ");
						txtEditor.Text += $"\n\nWeek {currentWeek}: \n";
					}
					sw.WriteLine(i);
					txtEditor.Text += $"{i}\n";
					}
				}

				sw.WriteLine();
				sw.WriteLine("----------------------------------------");
				sw.WriteLine("Transmit Dates");
				sw.WriteLine("----------------------------------------");
				sw.WriteLine();
				txtEditor.Text += "----------------------------------------\nTransmit Dates\n----------------------------------------\n\n";
				foreach(string i in TransmitLogList.ToArray())
				{
					sw.WriteLine(i);
					txtEditor.Text += $"{i}\n";
				}   

				//close the file
				sw.Flush();
				sw.Close();
				
				txtEditor.Text += "\n\n .....Done";
				
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


	}
}