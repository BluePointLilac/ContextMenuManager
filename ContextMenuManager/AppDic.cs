using BluePointLilac.Methods;
using System.IO;
using System.Xml;

namespace ContextMenuManager
{
    static class AppDic
    {
        public static XmlDocument ReadXml(string webDicPath, string userDicPath, string appDic)
        {
            XmlDocument doc1 = new XmlDocument();
            try
            {
                if(File.Exists(webDicPath))
                {
                    doc1.LoadXml(File.ReadAllText(webDicPath, EncodingType.GetType(webDicPath)));
                }
                else
                {
                    doc1.LoadXml(appDic);
                }
                if(File.Exists(userDicPath))
                {
                    XmlDocument doc2 = new XmlDocument();
                    doc2.LoadXml(File.ReadAllText(userDicPath, EncodingType.GetType(userDicPath)));
                    foreach(XmlNode xn in doc2.DocumentElement.ChildNodes)
                    {
                        XmlNode node = doc1.ImportNode(xn, true);
                        doc1.DocumentElement.AppendChild(node);
                    }
                }
            }
            catch { }
            return doc1;
        }
    }
}