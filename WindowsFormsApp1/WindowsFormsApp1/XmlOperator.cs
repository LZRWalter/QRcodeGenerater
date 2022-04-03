using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApp1
{
    class Node
    {
        private string name = "";
        private string select = "";
        private int length = 9;

        public Dictionary<string, string> items = new Dictionary<string, string>();

        public string Name { get => name; set => name = value; }
        public string Select { get => select; set => select = value; }
        public int Length { get => length; set => length = value; }

        public void SetAtribute(string name, string select, int length)
        {
            this.Name = name;
            this.Select = select;
            this.Length = length;
        }
        public int AddItem(string Id, string txt)
        {
            int status = 0;

            if (this.items.ContainsKey(Id))
            {
                status = -1; //Id已经存在
            }
            else
            {
                this.items.Add(Id, txt);
                status = 0;
            }
            return status;
        }
        public int RemoveItem(string Id)
        {
            int status = 0;

            if (this.items.ContainsKey(Id))
            {
                this.items.Remove(Id);
            }
            else
            {
                status = -1; 
            }
            return status;
        }

        public string FindValue(string key)
        {
            string str = "";

            if (this.items.Keys.Contains(key))
            {
                str = this.items[key];
            }

            return str;
        }

    }

    class XmlOperator
    {
        private string xmlFilePath;
        public XmlOperator(string filePath)
        {
            this.xmlFilePath = filePath;
        }

        public int GetOrganiztionId(ref Node OrgNode)
        {
            int count = 0;
            string nodePath_organizationId = "form/qrcodeMsg/organizationId";

            if (!File.Exists(this.xmlFilePath))
            {
                return 0;
            }

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(this.xmlFilePath);

            XmlNode node = xmlFile.SelectSingleNode(nodePath_organizationId);
            string name = ((XmlElement)node).GetAttribute("name").ToString();
            string select = ((XmlElement)node).GetAttribute("select").ToString();
            string length = ((XmlElement)node).GetAttribute("length").ToString();

            OrgNode.SetAtribute(name, select, Convert.ToInt16(length));
            XmlNodeList itemNodeList = node.ChildNodes;

            for (int i = 0; i < itemNodeList.Count; i++)
            {
                XmlElement item = (XmlElement)itemNodeList[i];
                string strId = item.GetAttribute("ID").ToString();
                string strTxt = item.InnerText;

                if (strId.Length != OrgNode.Length)
                {
                    string msg = string.Format("Id:{0} ,\n Val:{1} ,\n err:长度不符合." , strId, strTxt) ;
                    MessageBox.Show(msg, "提示(organizationId)");
                    continue;
                }

                OrgNode.AddItem(strId, strTxt);
                count++;
            }
            return count ;
        }

        public int GetOrderId(ref Node ordNode)
        {
            int count = 0;
            string nodePath_orderId = "form/qrcodeMsg/orderId";

            if (!File.Exists(this.xmlFilePath))
            {
                return 0;
            }

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(this.xmlFilePath);

            XmlNode node = xmlFile.SelectSingleNode(nodePath_orderId);
            string name = ((XmlElement)node).GetAttribute("name").ToString();
            string select = ((XmlElement)node).GetAttribute("select").ToString();
            string length = ((XmlElement)node).GetAttribute("length").ToString();

            ordNode.SetAtribute(name, select, Convert.ToInt16(length));
            XmlNodeList itemNodeList = node.ChildNodes;

            for (int i = 0; i < itemNodeList.Count; i++)
            {
                XmlElement item = (XmlElement)itemNodeList[i];
                string strId = item.GetAttribute("ID").ToString();
                string strTxt = item.InnerText;

                if (strId.Length != ordNode.Length)
                {
                    string msg = string.Format("Id:{0} ,\n Val:{1} ,\n err:长度不符合.", strId, strTxt);
                    MessageBox.Show(msg, "提示(orderId)");
                    continue;
                }

                ordNode.AddItem(strId, strTxt);
                count++;
            }
            return count; 
        }
    }
}
