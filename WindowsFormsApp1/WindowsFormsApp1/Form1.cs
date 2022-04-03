using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    
    public partial class Form1 : Form
    {
        private string picturePath;
        private string xmlPath;
        private Node organizationIdNode = null;
        private Node orderIdNode = null;
        private XmlOperator xmlOperator = null;
        private object unsigned;

        public Form1()
        {
            InitializeComponent();
            this.xmlPath = "./cfg/config.xml";
            this.xmlOperator = new XmlOperator(this.xmlPath);
            this.organizationIdNode = new Node();
            this.orderIdNode = new Node();

            this.xmlOperator.GetOrganiztionId(ref this.organizationIdNode);
            this.xmlOperator.GetOrderId(ref this.orderIdNode);
        }

        private void InitOrgOrd()
        {
            for (int i = 0; i < this.organizationIdNode.items.Count; i++)
            {
                string strTxt = "";
                string key = this.organizationIdNode.items.Keys.ToList()[i];
                string val = this.organizationIdNode.items[key].Trim();
                strTxt = key + ":" + val;
                this.cbOrganizationId.Items.Add(strTxt);
            }
            int cbOrganizationIdIndex = cbOrganizationId.FindStringExact(organizationIdNode.Select);
            if (cbOrganizationIdIndex >= 0)
            {
                this.cbOrganizationId.SelectedIndex = cbOrganizationIdIndex;
            }
            else
            {
                if(cbOrganizationId.Items.Count > 0)
                    this.cbOrganizationId.SelectedIndex = 0;
            }

            for (int i = 0; i < this.orderIdNode.items.Count; i++)
            {
                string strTxt = "";
                string key = this.orderIdNode.items.Keys.ToList()[i];
                string val = this.orderIdNode.items[key].Trim();
                strTxt = key + ":" + val;
                this.cbOrderId.Items.Add(strTxt);
            }
            int cbOrderIdIndex = this.cbOrderId.FindStringExact(orderIdNode.Select);
            if (cbOrderIdIndex > 0)
            {
                this.cbOrderId.SelectedIndex = cbOrderIdIndex;
            }
            else
            {
                if (cbOrderId.Items.Count > 0)
                    this.cbOrderId.SelectedIndex = 0;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 1; i < 10; i++)
            {
                cb_icon_border.Items.Add(i.ToString());
            }
            cb_icon_border.SelectedIndex = 1;

            for (int i = 1; i < 41; i++)
            {
                cb_version.Items.Add(i.ToString());
            }
            cb_version.SelectedIndex = 39;

            List<int> icon_size = new List<int>();
            for (int i = 0; i < 90; i++)
            {
                cb_icon_size.Items.Add(i.ToString());
            }
            cb_icon_size.SelectedIndex = 1;

            List<int> pixel = new List<int>();
            for (int i = 1; i < 30; i=i+1)
            {
                cb_pixel.Items.Add(i.ToString());
            }
            cb_pixel.SelectedIndex = 0;

            this.InitOrgOrd();

            this.EnableUsePicture(false);
        }

        private void EnableUsePicture(bool flag)
        {
            btnUsePicture.Enabled = flag;
            cb_icon_border.Enabled = flag;
            cb_icon_size.Enabled = flag;
            rb_we_y.Enabled = flag;
            pictureBox1.Enabled = flag;
        }

        private int Crc(string msg)
        {
            /*
             纯数字采用累加校验和;字母采用ASCII码值进行计算,前23位的累加和对10取余数
             */
            int crc = 0;

            int count = msg.Length;
            for (int i = 0; i < count; i++)
            {
                char tmpchar = msg[i];
                if (Char.IsNumber(tmpchar))
                {
                    int tmpNum = UInt16.Parse(tmpchar.ToString());
                    crc += tmpNum;
                }
                else if( Char.IsLetter(tmpchar) )
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes(tmpchar.ToString());
                    int tmpNum = byteArray[0];
                    crc += tmpNum;
                }
            }
            int re = crc % 10;
            return re;
        }

        private string CreateMsg()
        {
            this.tb_msg.Text = "";
            string msg = "";
            string orgId = this.cbOrganizationId.Text.ToString().Trim().Split(':')[0];
            string strDate = this.dateBox.Text.ToString();
            string orderId = this.cbOrderId.Text.ToString().Trim().Split(':')[0] + cbOrderId_No.Text.Trim();
            msg = orgId + strDate + orderId;
            int crc = this.Crc(msg);
            msg += crc.ToString();

            this.tb_msg.Text += string.Format("组织机构代码:{0}\r\n" , orgId);
            this.tb_msg.Text += string.Format("生产日期:{0}\r\n", strDate);
            this.tb_msg.Text += string.Format("序列码:{0}\r\n", orderId);
            this.tb_msg.Text += string.Format("校验和:{0}\r\n", crc);
            this.tb_msg.Text += string.Format("合成信息:{0}\r\n", msg); 

            return msg;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            int version = Convert.ToInt16(cb_version.Text);
            int pixel = Convert.ToInt16(cb_pixel.Text);

            string str_msg = this.CreateMsg();

            int int_icon_size = Convert.ToInt16(cb_icon_size.Text);
            int int_icon_border = Convert.ToInt16(cb_icon_border.Text);

            bool b_we = rb_we_y.Checked ? true : false;

            try
            {
                Bitmap bmp = Encoder.code(str_msg, version, pixel, picturePath, int_icon_size, int_icon_border, b_we);
                pb_qrcode.Image = bmp;
            }
            catch( Exception ei )
            {
                MessageBox.Show(ei.Message, "提示:");
            }
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (pb_qrcode.Image != null)
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "(*.png)|*.png|(*.bmp)|*.bmp";

                    if (sfd.ShowDialog() == DialogResult.OK)
                        pb_qrcode.Image.Save(sfd.FileName);
                }
            }  
        }

        private void btnUsePicture_Click(object sender, EventArgs e)
        {
            if (cbUsePicture.Checked == true)
            {

            }
            OpenFileDialog of = new OpenFileDialog();

            of.Filter = "(*.png)|*.png|(*.bmp)|*.bmp|(*.jpg)|*.jpg";
            if (of.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(of.FileName))
                {
                    this.picturePath = of.FileName;
                    Bitmap icon = new Bitmap(this.picturePath);
                    this.pictureBox1.Image = icon;
                }
            }

        }

        private void cbUsePicture_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUsePicture.Checked == true)
            {
                this.EnableUsePicture(true);
            }
            else
            {
                this.EnableUsePicture(false);
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose();
                    this.pictureBox1.Image = null;
                }
                
            }
        }
    }
}
