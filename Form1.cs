using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.ch.simap.www;

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

        private void button1_Click(object sender, EventArgs e)
        {

            
            //SearchNoticeList
            String searchXml = " <search pageNo=\"1\" recordsPerPage=\"20\">\r\n      <field name=\"NOTICE_NR\"><value>1347901</value></field>\r\n        </search>";

            //SearchNoticeCount
            /*
            int pageNo = 1;
            int recordsPerPage = 10;
            String timespanValue = "YEAR";
            String searchXml = "<search pageNo=\"" + pageNo
            + "\" recordsPerPage =\"" + recordsPerPage + "\" >"
            + "<field name =\"TIMESPAN\"><value>" + timespanValue
            + "</value></field></search>";
            */

            try
            {
                //SearchNoticeList
                long?[] h = client.getSearchNoticeList(searchXml);
                MessageBox.Show(h[0].ToString());

                //SearchNoticeCount
                //var h = client.getSearchNoticeCount(searchXml);
                //MessageBox.Show(h.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
