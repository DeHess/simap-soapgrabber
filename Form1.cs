using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WindowsFormsApp1.ch.simap.www;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        SoapServerService client = new SoapServerService();
        bool idGatheringFinished = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        static void WriteIDsToCsv(List<Int64> list, string filePath) {
            try {
                using (StreamWriter sw = new StreamWriter(filePath)) {
                    foreach (var item in list) {
                        sw.WriteLine(item);
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private void grabIDList(object sender, EventArgs e) {
            idGatheringFinished = false;
            List<Int64> idList = new List<Int64>();

            int startYear = 2000;
            DateTime endDate = DateTime.Now.Date;

            //Iterate over years between 2000 and endYear

            int year = startYear;
            
            while (!idGatheringFinished) {
                //Iterate over Months in each year
                for (int month = 1; month <= 12; month++) {
                    
                    //Start of the month
                    DateTime startOfTimespan = new DateTime(year, month, 1);
                
                    //End of the Month
                    DateTime endOfTimeSpan = startOfTimespan.AddMonths(1).AddDays(-1);

                    //End Reached
                    if (endOfTimeSpan >= endDate) {
                        endOfTimeSpan = endDate;
                        idGatheringFinished = true;
                    }

                    string startDateFormatted = startOfTimespan.ToString("dd.MM.yyyy");
                    string endDateFormmatted = endOfTimeSpan.ToString("dd.MM.yyyy");

                    string searchXML = "<search pageNo=\"1\" recordsPerPage=\"-1\">" +
            "<field name=\"STAT_TM_1\"><value>" + startDateFormatted + "</value></field><field name=\"STAT_TM_2\"><value>" + endDateFormmatted + "</value></field></search>";
                    long?[] monthNoticeIds = client.getSearchNoticeList(searchXML);

                    //Less than a thousand notices in current month
                    if (monthNoticeIds.Length < 1000) {
                        Console.WriteLine(startOfTimespan + " - " + endDateFormmatted + " -- " + monthNoticeIds.Length + " Notices");
                        foreach (long id in monthNoticeIds) {
                            idList.Add(id);
                        }
                    }
                        

                    //A thousand or more notices in current month:
                    else {
                        Console.WriteLine("More than 1000 Notices in month " + month + " of " + year + "! Switching to Daily Mode");
                        int daysInMonth = DateTime.DaysInMonth(year, month);
                        for (int day = 1; day <= daysInMonth; day++)
                        {

                            DateTime currentDate = new DateTime(year, month, day);
                            string currentDateFormatted = currentDate.ToString("dd.MM.yyyy");
                            string daySearchXML = "<search pageNo=\"1\" recordsPerPage=\"-1\">" +
                "<field name=\"STAT_TM_1\"><value>" + currentDateFormatted + "</value></field></search>";

                            long?[] dayNoticeIds = client.getSearchNoticeList(daySearchXML);
                            Console.WriteLine(currentDateFormatted + " -- " + dayNoticeIds.Length + " Notices");
                            if (dayNoticeIds.Length > 1000) {
                                MessageBox.Show("Data Loss due to more than 1000 Notices during the day of" + currentDateFormatted + "!!!!");
                            }
                            foreach (long id in dayNoticeIds)
                            {
                                idList.Add(id);
                            }


                            if (currentDate == endDate)
                            {
                                idGatheringFinished = true;
                                break;
                            }

                        }

                    }



                    if (idGatheringFinished) { break; }
                    
                if (idGatheringFinished) { break; }
                }
                
                //Update Year
                year++;
            }


            //Show Results!!!
            MessageBox.Show("Length of idList: " + idList.Count);

            //Write to File
            string filePath = "idList.csv";
            WriteIDsToCsv(idList, filePath);
            Console.WriteLine("Data has been written to the CSV file.");
        }
    }
}
