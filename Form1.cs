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

        private void button1_Click(object sender, EventArgs e)
        {
            List<Int64> idList = new List<Int64>();
            
            Boolean finished = false;
            int counter = 0;
            while (!finished) {

                string searchXML = "<search pageNo=\"" + counter + "\" recordsPerPage=\"1000\">" +
                  "<field name=\"STAT_TM_1\"><value>01.01.2000</value></field><field name=\"STAT_TM_2\"><value>01.01.2023</value></field></search>";


                try
                {
                    long?[] pageContent = client.getSearchNoticeList(searchXML);
                    MessageBox.Show("Type: " + pageContent.ToString() + "Length: " + pageContent.Length);
                }
                catch (Exception ex)
                {
                    finished = true;
                    MessageBox.Show(ex.ToString());
                }

                counter++;
            }

            
            


        }
    }
}
