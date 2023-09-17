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
using static System.Net.Mime.MediaTypeNames;


namespace WindowsFormsApp1
{
    public partial class Soapgrabber : Form {

        private const int RESULTSLIMIT = 1000;
        private const string NOTICECODE = "OB01";
        private const string AWARDCODE = "OB02";
        private const string NOTICEIDPATH = "NoticeIds.csv";
        private const string AWARDIDPATH = "AwardIds.csv";
        private const string NOTICESPATH = "Notices.csv";
        private const string AWARDSPATH = "Awards.csv";

        SoapServerService client = new SoapServerService();
        bool idGatheringFinished = false;
        List<Int64> noticeIdList = new List<Int64>();
        List<Int64> awardIdList = new List<Int64>();

        public Soapgrabber() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {   }

       
        private void GrabIDList(object sender, EventArgs e) {
            idGatheringFinished = false;
            noticeIdList = new List<Int64>();
            awardIdList = new List<Int64>();
            
            int startYear = 2023;
            DateTime endDate = DateTime.Now.Date;

            //Iterate over years between startyear and endYear
            int year = startYear;
            while (!idGatheringFinished) {
                for (int month = 1; month <= 12; month++) {
                    
                    long?[] monthNoticeIds = GetIdsOfMonth(year, month, endDate, NOTICECODE);
                    long?[] monthAwardIds = GetIdsOfMonth(year, month, endDate, AWARDCODE);
                    
                    if (monthNoticeIds.Length < RESULTSLIMIT) {
                        AddMonthIds(month, year, monthNoticeIds, noticeIdList, NOTICECODE);
                    }

                    if (monthAwardIds.Length < RESULTSLIMIT) {
                        AddMonthIds(month, year, monthAwardIds, awardIdList, AWARDCODE);
                    }

                    if (monthNoticeIds.Length >= RESULTSLIMIT) {
                        int daysInMonth = DateTime.DaysInMonth(year, month);
                        for (int day = 1; day <= daysInMonth; day++) {                    
                            long?[] dayNoticeIds = GetIdsOfDay(year, month, day, endDate, NOTICECODE);
                            AddDayIds(day, month, year, dayNoticeIds, noticeIdList, NOTICECODE);
                            if (idGatheringFinished) { break; }
                        }
                    }

                    if (monthAwardIds.Length >= RESULTSLIMIT) {
                        int daysInMonth = DateTime.DaysInMonth(year, month);
                        for (int day = 1; day <= daysInMonth; day++) {
                            long?[] dayAwardIds = GetIdsOfDay(year, month, day, endDate, AWARDCODE);
                            AddDayIds(day, month, year, dayAwardIds, awardIdList, AWARDCODE);
                            if (idGatheringFinished) { break; }
                        }
                    }
                    Console.WriteLine("===================================");
                    if (idGatheringFinished) { break; }
                }
                year++;
            }
            WriteIDsToCsv(noticeIdList, NOTICEIDPATH);
            WriteIDsToCsv(awardIdList, AWARDIDPATH);
            Console.WriteLine("Data has been written to respective CSV files.");
            MessageBox.Show("All Notice and Award ID's from " + startYear + " to " + endDate.Day + "/" + endDate.Month + "/" + endDate.Year + " have been written to " + NOTICEIDPATH + " and " + AWARDIDPATH);
        }

        private void AddDayIds(int day, int month, int year, long?[] dayIds, List<Int64> results, string code) {
            Console.WriteLine(code +  ", Daily Mode: " + day + "." + month + "." + year + " - " + dayIds.Length + " Notices/Awards");
            foreach (long id in dayIds) {
                results.Add(id);
            }
            
        }

        private void AddMonthIds(int month, int year, long?[] monthIds, List<Int64> results, string code) {
            Console.WriteLine(code + ", Monthly Mode: " + month + "." + year + " - " + monthIds.Length + " Notices/Awards");
            foreach (long id in monthIds) {
                results.Add(id);
            }
            
        }

        private long?[] GetIdsOfDay(int year, int month, int day, DateTime endDate, string code) {
            DateTime currentDate = new DateTime(year, month, day);
            if (currentDate == endDate) { 
                idGatheringFinished = true;
            }

            string currentDateFormatted = currentDate.ToString("dd.MM.yyyy");
            
            string daySearchXML = "<search pageNo=\"1\" recordsPerPage=\"-1\">" +
                "<field name=\"STAT_TM_1\"><value>" + currentDateFormatted + 
                "</value></field><field name=\"TYPE_CD_OB\"><value>" + code + "</value></field></search>";

            long?[] results = client.getSearchNoticeList(daySearchXML);

            if (results.Length > RESULTSLIMIT) {
                //DOES NOT EVER OCCUR (20.12.2023)
                MessageBox.Show("DATA LOSS, More than 1000 Notices/Awards during one single day, some Notices/Awards were lost."); 
            }

            return results;
        }

        private long?[] GetIdsOfMonth(int year, int month, DateTime endDate, string code) {
            DateTime startOfMonth = new DateTime(year, month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            if (endOfMonth >= endDate) {
                endOfMonth = endDate;
                idGatheringFinished = true;
            }
            string startDateFormatted = startOfMonth.ToString("dd.MM.yyyy");
            string endDateFormmatted = endOfMonth.ToString("dd.MM.yyyy");

            string monthSearchXML = "<search pageNo=\"1\" recordsPerPage=\"-1\">" +
                "<field name=\"STAT_TM_1\"><value>" + startDateFormatted + "</value></field><field name=\"STAT_TM_2\"><value>" 
                + endDateFormmatted + "</value></field><field name=\"TYPE_CD_OB\"><value>" + code +"</value></field></search>";

            return client.getSearchNoticeList(monthSearchXML);
        }



  
        private void GrabNoticesFromIDList(object sender, EventArgs e) {
           
            if (!File.Exists(NOTICEIDPATH) || !File.Exists(AWARDIDPATH)) {
                MessageBox.Show("List of Award and Notice Ids could not be found, Grab the Ids first ;)");
                return;
            }
            WriteAwardDataToCSV(AWARDIDPATH, AWARDSPATH);
            WriteNoticeDataToCSV(NOTICEIDPATH, NOTICESPATH);
            
        }

        
        private void WriteNoticeDataToCSV(string sourcePath, string resultPath) {
            noticeIdList = File.ReadAllLines(sourcePath)
                .Select(line => long.Parse(line.Trim()))
                .ToList();
            
            try {
                using (StreamWriter sw = new StreamWriter(resultPath)) {
                    foreach (long id in noticeIdList) {
                        string notice = client.getNoticeXml(id);
                        sw.WriteLine(notice);
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show("An error occurred: " + ex.Message);
            }            
        }

        private void WriteAwardDataToCSV(string sourcePath, string resultPath)
        {
            awardIdList = File.ReadAllLines(sourcePath)
                .Select(line => long.Parse(line.Trim()))
                .ToList();

            string t1 = client.getNoticeHtml(1305785);
            string t2 = client.getNoticeXml(1362465);
            
           
            try
            {
                using (StreamWriter sw = new StreamWriter(resultPath))
                {
                    foreach (long id in awardIdList)
                    {
                        string noticeHTML = client.getNoticeHtml(id);
                        string pattern = @"NOTICE_NR=(\d+)\b";
                        Match match = Regex.Match(noticeHTML, pattern);

                        if (match.Success)
                        {

                            string numberString = match.Groups[1].Value;

                            int number;



                            if (int.TryParse(numberString, out number))
                            {
                                Console.WriteLine("Extracted Notice Number: " + number);
                                Console.WriteLine("========================");
                            }


                            string noticeXML = client.getNoticeXml(id);
                            sw.WriteLine(noticeXML);
                        }
                        else {
                            Console.WriteLine("YO WTFFF: " + id);
                            Console.WriteLine("========================");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

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
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
