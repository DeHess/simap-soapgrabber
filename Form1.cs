using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.ch.simap.www;
using System.Text.RegularExpressions;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1 {
    public partial class Soapgrabber : Form {

        private const int RESULTSLIMIT = 1000;
        private const string NOTICECODE = "OB01";
        private const string AWARDCODE = "OB02";
        private const string DEFAULTPATH = "C:/Soapgrabber/";

      

        SoapServerService client = new SoapServerService();
        bool idGatheringFinished = false;
        List<Int64> noticeIdList = new List<Int64>();
        List<Int64> awardIdList = new List<Int64>();
        string outputDirectory = DEFAULTPATH;
        private string noticeIdPath = DEFAULTPATH + "\\NoticeIds.csv";
        private string awardIdPath = DEFAULTPATH + "\\AwardIds.csv";
        private string NoticesPath = DEFAULTPATH + "\\Notices.csv";
        private string AwardsPath = DEFAULTPATH + "\\Awards.csv";
        
        public Soapgrabber() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {   }
       
        private void GrabIDList(object sender, EventArgs e) {
            if (! Directory.Exists(outputDirectory)) {
                MessageBox.Show("The Output Directory does not exist. Aborting.");
                return;
            }
            consoleOutput.AppendText("==================================================\n");
            consoleOutput.AppendText("Gathering Notice and Award IDs\n");
            consoleOutput.AppendText("==================================================\n");

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
                    consoleOutput.AppendText("===================================\n");
                    if (idGatheringFinished) { break; }
                }
                year++;
            }
            WriteIDsToCsv(noticeIdList, noticeIdPath);
            WriteIDsToCsv(awardIdList, awardIdPath);
            consoleOutput.AppendText("Data has been written to respective CSV files.\n");
            MessageBox.Show("All Notice and Award ID's from " + startYear + " to " + 
                endDate.Day + "/" + endDate.Month + "/" + endDate.Year + " have been written to " + 
                noticeIdPath + " and " + awardIdPath);
        }

        private void AddDayIds(int day, int month, int year, long?[] dayIds, List<Int64> results, string code) {
            if (code == NOTICECODE) {
                consoleOutput.AppendText("Daily Mode: " + day + "." + month + "." + year + " - " + dayIds.Length + " Notices\n");
            }
            if (code == AWARDCODE) {
                consoleOutput.AppendText("Daily Mode: " + day + "." + month + "." + year + " - " + dayIds.Length + " Awards\n");
            }
            foreach (long id in dayIds) {
                results.Add(id);
            }
            
        }

        private void AddMonthIds(int month, int year, long?[] monthIds, List<Int64> results, string code) {
            if (code == NOTICECODE) {
                consoleOutput.AppendText("Monthly Mode: " + month + "." + year + " - " + monthIds.Length + " Notices\n");
            }
            if (code == AWARDCODE) {
                consoleOutput.AppendText("Monthly Mode: " + month + "." + year + " - " + monthIds.Length + " Awards\n");
            }
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
                //As of 20.12.2023, this does not ever occur. Furthemore, it is very unlikely that it ever will in the future.
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

        private void WriteIDsToCsv(List<Int64> list, string filePath) {
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





        private void GrabNoticesFromIDList(object sender, EventArgs e) {

            if (!Directory.Exists(outputDirectory)) {
                MessageBox.Show("The Output Directory does not exist. Aborting.");
                return;
            }

            if (!File.Exists(noticeIdPath) || !File.Exists(awardIdPath)) {
                MessageBox.Show("List of Award and Notice Ids could not be found, Grab the Ids first ;)");
                return;
            }
            WriteAwardDataToCSV(awardIdPath, AwardsPath);
            WriteNoticeDataToCSV(noticeIdPath, NoticesPath);
        }

        private void WriteNoticeDataToCSV(string sourcePath, string resultPath) {
            consoleOutput.AppendText("==================================================\n");
            consoleOutput.AppendText("Writing Notice Data To CSV\n");
            consoleOutput.AppendText("==================================================\n");
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
                MessageBox.Show("An error occurred while trying to write to a file: " + ex.Message);
            }            
        }

        private void WriteAwardDataToCSV(string sourcePath, string resultPath) {
            consoleOutput.AppendText("==================================================\n");
            consoleOutput.AppendText("Writing Award Data To CSV\n");
            consoleOutput.AppendText("==================================================\n");
            
            awardIdList = File.ReadAllLines(sourcePath)
                .Select(line => long.Parse(line.Trim()))
                .ToList();

            try {
                using (StreamWriter sw = new StreamWriter(resultPath)) {
                    foreach (long id in awardIdList) {
                        string awardXML = client.getNoticeXml(id);

                        string noticeHTML = client.getNoticeHtml(id);
                        string pattern = @"NOTICE_NR=(\d+)\b";
                        Match match = Regex.Match(noticeHTML, pattern);


                        if (match.Success) {
                            string numberString = match.Groups[1].Value;
                            int noticeNr;

                            int.TryParse(numberString, out noticeNr);
                                
                            consoleOutput.AppendText("Notice Number for Award "+ id + ": " + noticeNr + "\n");
                            consoleOutput.AppendText("========================\n");

                            awardXML = AddNoticeNrToXML(awardXML, noticeNr);

                            sw.WriteLine(awardXML);
                        }
                        else {
                            consoleOutput.AppendText("Notice Number not found for Award: " + id + "\n");
                            consoleOutput.AppendText("========================\n");
                            sw.WriteLine(awardXML);
                        }
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

        }

        private string AddNoticeNrToXML(string noticeXML, int noticeNr) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(noticeXML);
            XmlElement noticeIdElement = xmlDoc.CreateElement("NOTICE_ID");
            noticeIdElement.InnerText = noticeNr.ToString();

            XmlNode formNode = xmlDoc.SelectSingleNode("/FORM");
            formNode.AppendChild(noticeIdElement);
            return xmlDoc.OuterXml;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e) {

        }

        private void button3_Click(object sender, EventArgs e)   {
            string givenPath = maskedTextBox1.Text;
            if (!Directory.Exists(givenPath)) {
                MessageBox.Show("The given Directory does not exist.");
                return;
            }
            outputDirectory = givenPath;
            noticeIdPath = outputDirectory + "\\NoticeIds.csv";
            awardIdPath = outputDirectory + "\\AwardIds.csv";
            NoticesPath = outputDirectory + "\\Notices.csv";
            AwardsPath = outputDirectory + "\\Awards.csv";

            consoleOutput.AppendText("==================================================\n");
            consoleOutput.AppendText("Output Directory has been changed to: " + givenPath + "\n");
            consoleOutput.AppendText("==================================================\n");

            MessageBox.Show("Output Directory has been changed to: " + givenPath);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)    {

        }
    }
}
