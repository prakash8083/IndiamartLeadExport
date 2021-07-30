using IndiaMart_CSV.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndiaMart_CSV
{
    public partial class LeadsExtractor : Form
    {
        public LeadsExtractor()
        {
            InitializeComponent();
        }

        private async void btnExport_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtAPIKey.Text))
            {
                MessageBox.Show("API Key is required");
                return;
            }
            if (string.IsNullOrEmpty(txtMobile.Text))
            {
                MessageBox.Show("Registered Mobile is required");
                return;
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://mapi.indiamart.com/");

            HttpResponseMessage msg = await client.GetAsync(string.Format("wservce/enquiry/listing/GLUSR_MOBILE/"+txtMobile.Text+"/GLUSR_MOBILE_KEY/"+txtAPIKey.Text+"/Start_Time/{0}/End_Time/{1}/", dtStart.Value.ToString("dd-MMM-yyyy"), dtEndDate.Value.ToString("dd-MMM-yyyy")));
            string resp = await msg.Content.ReadAsStringAsync();
            List<LeadModel> leads = JsonConvert.DeserializeObject<List<LeadModel>>(resp);

            if(leads.Count==0)
            {
                MessageBox.Show("No Data to Export");
                return;
            }
            string data = "";
            data += "QUERY_ID,QTYPE,SENDERNAME,SENDEREMAIL,SUBJECT,DATE_RE,DATE_R,DATE_TIME_RE,MOB,COUNTRY_FLAG,QUERY_MODID,ENQ_MESSAGE,ENQ_ADDRESS,ENQ_CITY,ENQ_STATE,PRODUCT_NAME,EMAIL_ALT,MOBILE_ALT,PHONE,PHONE_ALT,TOTAL_COUNT"+Environment.NewLine;
            foreach (var item in leads)
            {
                data += "\""+item.QUERY_ID + "\",\""  + item.QTYPE + "\",\""  + item.SENDERNAME + "\",\"" + item.SENDEREMAIL + "\",\""  + item.SUBJECT + "\",\""  + item.DATE_RE + "\",\""  + item.DATE_R + "\",\""  + item.DATE_TIME_RE + "\",\""  +
                    ReplaceStringLiteral(item.MOB) + "\",\""  + item.COUNTRY_FLAG
                    + "\",\""  + item.QUERY_MODID + "\",\""  + item.ENQ_MESSAGE + "\",\""  + item.ENQ_ADDRESS + "\",\""  + item.ENQ_CITY + "\",\""  +
                    item.ENQ_STATE + "\",\""  + item.PRODUCT_NAME + "\",\""  + item.EMAIL_ALT + "\",\"" + ReplaceStringLiteral(item.MOBILE_ALT) + "\",\"" + ReplaceStringLiteral(item.PHONE) + "\",\""  + ReplaceStringLiteral(item.PHONE_ALT) + "\",\""  + item.TOTAL_COUNT + "\""+Environment.NewLine;

            }

            SaveFileDialog diag = new SaveFileDialog();
            diag.DefaultExt = "csv";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fParameter = new FileStream(diag.FileName, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter m_WriterParameter = new StreamWriter(fParameter);
                    m_WriterParameter.BaseStream.Seek(0, SeekOrigin.End);
                    m_WriterParameter.Write(data);
                    m_WriterParameter.Flush();
                    m_WriterParameter.Close();
                }
                MessageBox.Show("File Saved, Exported "+leads.Count +" data");
            }


             string ReplaceStringLiteral(string req)
            {
                if(string.IsNullOrEmpty(req))
                {
                    return string.Empty;
                }
                return req.Replace('-', ' ');
            }
        }

        private void lnkDoc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://help.indiamart.com/knowledge-base/lms-crm-integration/");
        }
    }
}
