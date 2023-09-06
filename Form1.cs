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
        private const int NOTICELISTLIMIT = 1000;
        SoapServerService client = new SoapServerService();
        bool idGatheringFinished = false;
        List<Int64> idList = new List<Int64>();

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
            idList = new List<Int64>();

            int startYear = 2000;
            DateTime endDate = DateTime.Now.Date;

            //Iterate over years between 2000 and endYear
            int year = startYear;
            while (!idGatheringFinished) {
                for (int month = 1; month <= 12; month++) {
                    
                    long?[] monthNoticeIds = getMonthlyIds(year, month, endDate);
                    if (monthNoticeIds.Length < NOTICELISTLIMIT) {
                        addMonthlyIDsToList(year, month, monthNoticeIds);
                    }

                    if (monthNoticeIds.Length >= NOTICELISTLIMIT) {
                        int daysInMonth = DateTime.DaysInMonth(year, month);
                        for (int day = 1; day <= daysInMonth; day++)
                        {
                            DateTime currentDate = new DateTime(year, month, day);
                            string currentDateFormatted = currentDate.ToString("dd.MM.yyyy");
                            string daySearchXML = "<search pageNo=\"1\" recordsPerPage=\"-1\">" +
                "<field name=\"STAT_TM_1\"><value>" + currentDateFormatted + "</value></field></search>";

                            long?[] dayNoticeIds = client.getSearchNoticeList(daySearchXML);
                            Console.WriteLine("Daily Mode: " + day + "." + month + "." + year + " - " + dayNoticeIds.Length + " Notices");
                            if (dayNoticeIds.Length > 1000) {
                                MessageBox.Show("DATA LOSS: There were more than 1000 Notices during the day of" + currentDateFormatted + "!!!!");
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

        private void addMonthlyIDsToList(int year, int month, long?[] monthNoticeIds)
        {
            Console.WriteLine("Monthly Mode: " + month + "." + year + " - " + monthNoticeIds.Length + " Notices");
            foreach (long id in monthNoticeIds)
            {
                idList.Add(id);
            }
        }

        private long?[] getMonthlyIds(int year, int month, DateTime endDate) {

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
    }
}
