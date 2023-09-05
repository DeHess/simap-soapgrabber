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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        static void WriteListToCsv(List<Int64> list, string filePath) {
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

        private void button1_Click(object sender, EventArgs e)
        {
            
            List<Int64> idList = new List<Int64>();

            int startYear = 2000;

            DateTime endDate = DateTime.Now.Date;

            //Iterate over years between 2000 and endYear
            bool finished = false;
            int year = startYear;
            while (!finished) {
                //Iterate over Months in each year
                for (int month = 1; month <= 12; month++) {
                    int daysInMonth = DateTime.DaysInMonth(year, month);
                    
                    //Iterate over each Day in each Month
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        DateTime date = new DateTime(year, month, day);
                        string formattedDate = date.ToString("dd.MM.yyyy");
                        
                        if (date == endDate) {
                            finished = true;
                        }
   
                        Console.WriteLine(date);

                        //Get Notice ID's of the current day:
                        try
                        {
                            string searchXML = "<search pageNo=\"1\" recordsPerPage=\"-1\">" +
                  "<field name=\"STAT_TM_1\"><value>"+ formattedDate +"</value></field></search>";
                            long?[] noticeIds = client.getSearchNoticeList(searchXML);
                            foreach (long id in noticeIds)
                            {
                                idList.Add(id);
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }


                        if (finished) { break; }
                    }
                    if (finished) { break; }
                }
                
                //Update Year
                year++;
            }


            //Show Results!!!
            MessageBox.Show("Length of idList: " + idList.Count);

            //Write to File
            string filePath = "idList.csv";

        }
    }
}
