using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BluePointLilac.Methods
{
    public static class RichTextBoxExtension
    {
        /// <summary>RichTextBox中ini语法高亮</summary>
        /// <param name="iniStr">要显示的ini文本</param>
        public static void LoadIni(this RichTextBox box, string iniStr)
        {
            string[] lines = iniStr.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            for(int i = 0; i < lines.Length; i++)
            {
                string str = lines[i].Trim();
                if(str.StartsWith(";") || str.StartsWith("#"))
                {
                    box.AppendText(str, Color.SkyBlue);//注释
                }
                else if(str.StartsWith("["))
                {
                    if(str.Contains("]"))
                    {
                        int index = str.IndexOf(']');
                        box.AppendText(str.Substring(0, index + 1), Color.DarkCyan, null, true);//section
                        box.AppendText(str.Substring(index + 1), Color.SkyBlue);//section标签之后的内容视作注释
                    }
                    else box.AppendText(str, Color.SkyBlue);//section标签未关闭视作注释
                }
                else if(str.Contains("="))
                {
                    int index = str.IndexOf('=');
                    box.AppendText(str.Substring(0, index), Color.DodgerBlue);//key
                    box.AppendText(str.Substring(index), Color.DimGray);//value
                }
                else box.AppendText(str, Color.SkyBlue);//非section行和非key行视作注释
                if(i != lines.Length - 1) box.AppendText("\r\n");
            }
        }

        /// 代码原文：https://archive.codeplex.com/?p=xmlrichtextbox
        /// 本人（蓝点lilac）仅作简单修改，将原继承类改写为扩展方法
        /// <summary>RichTextBox中xml语法高亮</summary>
        /// <param name="xmlStr">要显示的xml文本</param>
        /// <remarks>可直接用WebBrowser的Url加载本地xml文件，但无法自定义颜色</remarks>
        public static void LoadXml(this RichTextBox box, string xmlStr)
        {
            XmlStateMachine machine = new XmlStateMachine();
            if(xmlStr.StartsWith("<?"))
            {
                string declaration = machine.GetXmlDeclaration(xmlStr);
                try
                {
                    xmlStr = XDocument.Parse(xmlStr, LoadOptions.PreserveWhitespace).ToString().Trim();
                    if(string.IsNullOrEmpty(xmlStr) && declaration == string.Empty) return;
                }
                catch { throw; }
                xmlStr = declaration + "\r\n" + xmlStr;
            }

            int location = 0;
            int failCount = 0;
            int tokenTryCount = 0;
            while(location < xmlStr.Length)
            {
                string token = machine.GetNextToken(xmlStr.Substring(location), out XmlTokenType ttype);
                Color color = machine.GetTokenColor(ttype);
                bool isBold = ttype == XmlTokenType.DocTypeName || ttype == XmlTokenType.NodeName;
                box.AppendText(token, color, null, isBold);
                location += token.Length;
                tokenTryCount++;

                // Check for ongoing failure
                if(token.Length == 0) failCount++;
                if(failCount > 10 || tokenTryCount > xmlStr.Length)
                {
                    string theRestOfIt = xmlStr.Substring(location, xmlStr.Length - location);
                    //box.AppendText(Environment.NewLine + Environment.NewLine + theRestOfIt); // DEBUG
                    box.AppendText(theRestOfIt);
                    break;
                }
            }
        }

        public static void AppendText(this RichTextBox box, string text, Color color = default, Font font = null, bool isBold = false)
        {
            FontStyle fontStyle = isBold ? FontStyle.Bold : FontStyle.Regular;
            box.SelectionFont = new Font(font ?? box.Font, fontStyle);
            box.SelectionColor = color != default ? color : box.ForeColor;
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        sealed class XmlStateMachine
        {
            public XmlTokenType CurrentState = XmlTokenType.Unknown;
            private string subString = string.Empty;
            private string token = string.Empty;

            public string GetNextToken(string s, out XmlTokenType ttype)
            {
                ttype = XmlTokenType.Unknown;
                // skip past any whitespace (token added to it at the end of method)
                string whitespace = GetWhitespace(s);
                subString = s.TrimStart();
                token = string.Empty;
                if(CurrentState == XmlTokenType.CDataStart)
                {
                    // check for empty CDATA
                    if(subString.StartsWith("]]>"))
                    {
                        CurrentState = XmlTokenType.CDataEnd;
                        token = "]]>";
                    }
                    else
                    {
                        CurrentState = XmlTokenType.CDataValue;
                        int n = subString.IndexOf("]]>");
                        token = subString.Substring(0, n);
                    }
                }
                else if(CurrentState == XmlTokenType.DocTypeStart)
                {
                    CurrentState = XmlTokenType.DocTypeName;
                    token = "DOCTYPE";
                }
                else if(CurrentState == XmlTokenType.DocTypeName)
                {
                    CurrentState = XmlTokenType.DocTypeDeclaration;
                    int n = subString.IndexOf("[");
                    token = subString.Substring(0, n);
                }
                else if(CurrentState == XmlTokenType.DocTypeDeclaration)
                {
                    CurrentState = XmlTokenType.DocTypeDefStart;
                    token = "[";
                }
                else if(CurrentState == XmlTokenType.DocTypeDefStart)
                {
                    if(subString.StartsWith("]>"))
                    {
                        CurrentState = XmlTokenType.DocTypeDefEnd;
                        token = "]>";
                    }
                    else
                    {
                        CurrentState = XmlTokenType.DocTypeDefValue;
                        int n = subString.IndexOf("]>");
                        token = subString.Substring(0, n);
                    }
                }
                else if(CurrentState == XmlTokenType.DocTypeDefValue)
                {
                    CurrentState = XmlTokenType.DocTypeDefEnd;
                    token = "]>";
                }
                else if(CurrentState == XmlTokenType.DoubleQuotationMarkStart)
                {
                    // check for empty attribute value
                    if(subString[0] == '\"')
                    {
                        CurrentState = XmlTokenType.DoubleQuotationMarkEnd;
                        token = "\"";
                    }
                    else
                    {
                        CurrentState = XmlTokenType.AttributeValue;
                        int n = subString.IndexOf("\"");
                        token = subString.Substring(0, n);
                    }
                }
                else if(CurrentState == XmlTokenType.SingleQuotationMarkStart)
                {
                    // check for empty attribute value
                    if(subString[0] == '\'')
                    {
                        CurrentState = XmlTokenType.SingleQuotationMarkEnd;
                        token = "\'";
                    }
                    else
                    {
                        CurrentState = XmlTokenType.AttributeValue;
                        int n = subString.IndexOf("'");
                        token = subString.Substring(0, n);
                    }
                }
                else if(CurrentState == XmlTokenType.CommentStart)
                {
                    // check for empty comment
                    if(subString.StartsWith("-->"))
                    {
                        CurrentState = XmlTokenType.CommentEnd;
                        token = "-->";
                    }
                    else
                    {
                        CurrentState = XmlTokenType.CommentValue;
                        token = ReadCommentValue(subString);
                    }
                }
                else if(CurrentState == XmlTokenType.NodeStart)
                {
                    CurrentState = XmlTokenType.NodeName;
                    token = ReadNodeName(subString);
                }
                else if(CurrentState == XmlTokenType.XmlDeclarationStart)
                {
                    CurrentState = XmlTokenType.NodeName;
                    token = ReadNodeName(subString);
                }
                else if(CurrentState == XmlTokenType.NodeName)
                {
                    if(subString[0] != '/' &&
                        subString[0] != '>')
                    {
                        CurrentState = XmlTokenType.AttributeName;
                        token = ReadAttributeName(subString);
                    }
                    else
                    {
                        HandleReservedXmlToken();
                    }
                }
                else if(CurrentState == XmlTokenType.NodeEndValueStart)
                {
                    if(subString[0] == '<')
                    {
                        HandleReservedXmlToken();
                    }
                    else
                    {
                        CurrentState = XmlTokenType.NodeValue;
                        token = ReadNodeValue(subString);
                    }
                }
                else if(CurrentState == XmlTokenType.DoubleQuotationMarkEnd)
                {
                    HandleAttributeEnd();
                }
                else if(CurrentState == XmlTokenType.SingleQuotationMarkEnd)
                {
                    HandleAttributeEnd();
                }
                else
                {
                    HandleReservedXmlToken();
                }
                if(token != string.Empty)
                {
                    ttype = CurrentState;
                    return whitespace + token;
                }
                return string.Empty;
            }

            public Color GetTokenColor(XmlTokenType ttype)
            {
                switch(ttype)
                {
                    case XmlTokenType.NodeValue:
                    case XmlTokenType.EqualSignStart:
                    case XmlTokenType.EqualSignEnd:
                    case XmlTokenType.DoubleQuotationMarkStart:
                    case XmlTokenType.DoubleQuotationMarkEnd:
                    case XmlTokenType.SingleQuotationMarkStart:
                    case XmlTokenType.SingleQuotationMarkEnd:
                        return Color.DimGray;
                    case XmlTokenType.XmlDeclarationStart:
                    case XmlTokenType.XmlDeclarationEnd:
                    case XmlTokenType.NodeStart:
                    case XmlTokenType.NodeEnd:
                    case XmlTokenType.NodeEndValueStart:
                    case XmlTokenType.CDataStart:
                    case XmlTokenType.CDataEnd:
                    case XmlTokenType.CommentStart:
                    case XmlTokenType.CommentEnd:
                    case XmlTokenType.AttributeValue:
                    case XmlTokenType.DocTypeStart:
                    case XmlTokenType.DocTypeEnd:
                    case XmlTokenType.DocTypeDefStart:
                    case XmlTokenType.DocTypeDefEnd:
                        return Color.DimGray;
                    case XmlTokenType.CDataValue:
                    case XmlTokenType.DocTypeDefValue:
                        return Color.SkyBlue;
                    case XmlTokenType.CommentValue:
                        return Color.SkyBlue;
                    case XmlTokenType.DocTypeName:
                    case XmlTokenType.NodeName:
                        return Color.DarkCyan;
                    case XmlTokenType.AttributeName:
                    case XmlTokenType.DocTypeDeclaration:
                        return Color.DodgerBlue;
                    default:
                        return Color.DimGray;
                }
            }

            public string GetXmlDeclaration(string s)
            {
                int start = s.IndexOf("<?");
                int end = s.IndexOf("?>");
                if(start > -1 && end > start)
                {
                    return s.Substring(start, end - start + 2);
                }
                return string.Empty;
            }

            private void HandleAttributeEnd()
            {
                if(subString.StartsWith(">"))
                {
                    HandleReservedXmlToken();
                }
                else if(subString.StartsWith("/>"))
                {
                    HandleReservedXmlToken();
                }
                else if(subString.StartsWith("?>"))
                {
                    HandleReservedXmlToken();
                }
                else
                {
                    CurrentState = XmlTokenType.AttributeName;
                    token = ReadAttributeName(subString);
                }
            }

            private void HandleReservedXmlToken()
            {
                // check if state changer
                // <, >, =, </, />, <![CDATA[, <!--, -->
                if(subString.StartsWith("<![CDATA["))
                {
                    CurrentState = XmlTokenType.CDataStart;
                    token = "<![CDATA[";
                }
                else if(subString.StartsWith("<!DOCTYPE"))
                {
                    CurrentState = XmlTokenType.DocTypeStart;
                    token = "<!";
                }
                else if(subString.StartsWith("</"))
                {
                    CurrentState = XmlTokenType.NodeStart;
                    token = "</";
                }
                else if(subString.StartsWith("<!--"))
                {
                    CurrentState = XmlTokenType.CommentStart;
                    token = "<!--";
                }
                else if(subString.StartsWith("<?"))
                {
                    CurrentState = XmlTokenType.XmlDeclarationStart;
                    token = "<?";
                }
                else if(subString.StartsWith("<"))
                {
                    CurrentState = XmlTokenType.NodeStart;
                    token = "<";
                }
                else if(subString.StartsWith("="))
                {
                    CurrentState = XmlTokenType.EqualSignStart;
                    token = "=";
                }
                else if(subString.StartsWith("?>"))
                {
                    CurrentState = XmlTokenType.XmlDeclarationEnd;
                    token = "?>";
                }
                else if(subString.StartsWith(">"))
                {
                    CurrentState = XmlTokenType.NodeEndValueStart;
                    token = ">";
                }
                else if(subString.StartsWith("-->"))
                {
                    CurrentState = XmlTokenType.CommentEnd;
                    token = "-->";
                }
                else if(subString.StartsWith("]>"))
                {
                    CurrentState = XmlTokenType.DocTypeEnd;
                    token = "]>";
                }
                else if(subString.StartsWith("]]>"))
                {
                    CurrentState = XmlTokenType.CDataEnd;
                    token = "]]>";
                }
                else if(subString.StartsWith("/>"))
                {
                    CurrentState = XmlTokenType.NodeEnd;
                    token = "/>";
                }
                else if(subString.StartsWith("\""))
                {
                    if(CurrentState == XmlTokenType.AttributeValue)
                    {
                        CurrentState = XmlTokenType.DoubleQuotationMarkEnd;
                    }
                    else
                    {
                        CurrentState = XmlTokenType.DoubleQuotationMarkStart;
                    }
                    token = "\"";
                }
                else if(subString.StartsWith("'"))
                {
                    if(CurrentState == XmlTokenType.AttributeValue)
                    {
                        CurrentState = XmlTokenType.SingleQuotationMarkEnd;
                    }
                    else
                    {
                        CurrentState = XmlTokenType.SingleQuotationMarkStart;
                    }
                    token = "'";
                }
            }

            private string ReadNodeName(string s)
            {
                string nodeName = "";
                for(int i = 0; i < s.Length; i++)
                {
                    if(s[i] == '/' || s[i] == ' ' || s[i] == '>') return nodeName;
                    else nodeName += s[i].ToString();
                }
                return nodeName;
            }

            private string ReadAttributeName(string s)
            {
                string attName = "";
                int n = s.IndexOf('=');
                if(n != -1) attName = s.Substring(0, n);
                return attName;
            }

            private string ReadNodeValue(string s)
            {
                string nodeValue = "";
                int n = s.IndexOf('<');
                if(n != -1) nodeValue = s.Substring(0, n);
                return nodeValue;
            }

            private string ReadCommentValue(string s)
            {
                string commentValue = "";
                int n = s.IndexOf("-->");
                if(n != -1) commentValue = s.Substring(0, n);
                return commentValue;
            }

            private string GetWhitespace(string s)
            {
                string whitespace = "";
                for(int i = 0; i < s.Length; i++)
                {
                    char c = s[i];
                    if(char.IsWhiteSpace(c)) whitespace += c;
                    else break;
                }
                return whitespace;
            }
        }

        enum XmlTokenType
        {
            Whitespace, XmlDeclarationStart, XmlDeclarationEnd, NodeStart, NodeEnd, NodeEndValueStart, NodeName,
            NodeValue, AttributeName, AttributeValue, EqualSignStart, EqualSignEnd, CommentStart, CommentValue,
            CommentEnd, CDataStart, CDataValue, CDataEnd, DoubleQuotationMarkStart, DoubleQuotationMarkEnd,
            SingleQuotationMarkStart, SingleQuotationMarkEnd, DocTypeStart, DocTypeName, DocTypeDeclaration,
            DocTypeDefStart, DocTypeDefValue, DocTypeDefEnd, DocTypeEnd, DocumentEnd, Unknown
        }
    }
}