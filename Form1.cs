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
using HtmlAgilityPack;
using System.Text.RegularExpressions;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form {

        private const int NOTICELISTLIMIT = 1000;
        SoapServerService client = new SoapServerService();
        bool idGatheringFinished = false;
        List<Int64> idList = new List<Int64>();

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {

        }



        private void GrabIDList(object sender, EventArgs e) {
            idGatheringFinished = false;
            idList = new List<Int64>();

            int startYear = 2000;
            DateTime endDate = DateTime.Now.Date;

            //Iterate over years between startyear and endYear
            int year = startYear;
            while (!idGatheringFinished) {
                for (int month = 1; month <= 12; month++) {
                    
                    long?[] monthNoticeIds = GetMonthlyIds(year, month, endDate);
                    if (monthNoticeIds.Length < NOTICELISTLIMIT) {
                        AddMonthlyIDsToList(year, month, monthNoticeIds);
                    }

                    if (monthNoticeIds.Length >= NOTICELISTLIMIT) {
                        int daysInMonth = DateTime.DaysInMonth(year, month);
                        for (int day = 1; day <= daysInMonth; day++)
                        {
                            
                            long?[] dayNoticeIds = GetDayNoticeIds(year, month, day, endDate);
                                    
                            if (dayNoticeIds.Length > NOTICELISTLIMIT) {
                                //This does not ever occur (6.9.2023)
                                MessageBox.Show("DATA LOSS: There were more than 1000 Notices during the day of" + day + "." + month + "." + year + "!!!!");
                            }
                            AddDayIDsToList(year, month, day, dayNoticeIds);

                            if (idGatheringFinished) { break;}
                        }
                    }
                    if (idGatheringFinished) { break; }
                }
                year++;
            }
            WriteIDsToCsv(idList, "idList.csv");
            Console.WriteLine("Data has been written to the CSV file.");
            MessageBox.Show("All Notice ID's from 01/01/2000 to " + endDate.Day + "/" + endDate.Month + "/" + endDate.Year + " have been written to idList.csv");
        }


        private long?[] GetDayNoticeIds(int year, int month, int day, DateTime endDate) {
            
            DateTime currentDate = new DateTime(year, month, day);
            if (currentDate == endDate) { 
                idGatheringFinished = true;
            }

            string currentDateFormatted = currentDate.ToString("dd.MM.yyyy");
            string daySearchXML = "<search pageNo=\"1\" recordsPerPage=\"-1\">" +
                "<field name=\"STAT_TM_1\"><value>" + currentDateFormatted + "</value></field></search>";

            return client.getSearchNoticeList(daySearchXML);
        }


        private void AddDayIDsToList(int year, int month, int day, long?[] dayNoticeIds) {
            Console.WriteLine("Daily Mode: " + day + "." + month + "." + year + " - " + dayNoticeIds.Length + " Notices");
            foreach (long id in dayNoticeIds) {
                idList.Add(id);
            }

        }


        private void AddMonthlyIDsToList(int year, int month, long?[] monthNoticeIds) {
            Console.WriteLine("Monthly Mode: " + month + "." + year + " - " + monthNoticeIds.Length + " Notices");
            foreach (long id in monthNoticeIds) {
                idList.Add(id);
            }
        }


        private long?[] GetMonthlyIds(int year, int month, DateTime endDate) {

            DateTime startOfMonth = new DateTime(year, month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            if (endOfMonth >= endDate) {
                endOfMonth = endDate;
                idGatheringFinished = true;
            }

            string startDateFormatted = startOfMonth.ToString("dd.MM.yyyy");
            string endDateFormmatted = endOfMonth.ToString("dd.MM.yyyy");

            string searchXML = "<search pageNo=\"1\" recordsPerPage=\"-1\">" +
                "<field name=\"STAT_TM_1\"><value>" + startDateFormatted + "</value></field><field name=\"STAT_TM_2\"><value>" + endDateFormmatted + "</value></field></search>";
            return client.getSearchNoticeList(searchXML);
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

        private void GrabNoticesFromIDList(object sender, EventArgs e) {
            string idListFile = "idList.csv";

            if (!File.Exists(idListFile)) {
                MessageBox.Show("No IDList has been grabbed from the Webserver. Grab the ID List first ;)");
                return;
            }

            idList = File.ReadAllLines(idListFile)
                .Select(line => long.Parse(line.Trim()))
                .ToList();

            foreach (int id in idList) {
                string notice = client.getNoticeHtml(idList[id]);
                Console.WriteLine(notice);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(notice);



            }
        }
    }
}
