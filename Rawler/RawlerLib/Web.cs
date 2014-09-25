using System;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace RawlerLib
{
    /// <summary>
    /// Web �̊T�v�̐����ł��B
    /// </summary>
    public class Web
    {
        public Web()
        {

        }


        /// <summary>
        /// URL���u���E�U�ɕ\��������BURL�`�F�b�N�����Ȃ��Ƃ܂�����Ȃ��E�E�E
        /// </summary>
        /// <param name="url"></param>
        public static void OpenUrl(string url)
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(url);
        }

        /// <summary>
        /// HTML�̃^�O��S���폜�B�܂������Q�Ɓi&lt;�Ȃǁj���u�����܂�
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlTagAllDelete(string html)
        {
            bool tagStart = false;

            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

            foreach (char c in html.ToCharArray())
            {
                if (tagStart == true)
                {
                    if (c.Equals('>'))
                    {
                        tagStart = false;
                        //tag�̏I�����ɂ̓X�y�[�X��������
                        strBuilder.Append(' ');
                    }
                }
                else
                {
                    if (c.Equals('<'))
                    {
                        tagStart = true;
                    }
                    else
                    {
                        strBuilder.Append(c);
                    }
                }
            }
            strBuilder.Replace("&nbsp;", " ");
            strBuilder.Replace("&lt;", "<");
            strBuilder.Replace("&gt;", ">");
            strBuilder.Replace("&amp;", "&");
            strBuilder.Replace("&quot;", "\"");
            return strBuilder.ToString();


        }

        /// <summary>
        /// ���ʁ���������HTML�G���R�[�h����
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string SimpleHtmlEncode(string txt)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(txt);
            strBuilder.Replace("<", "&lt;");
            strBuilder.Replace(">", "&gt;");
            return strBuilder.ToString();
        }

        /// <summary>
        /// ���ʁ���������HTML�f�R�[�h����
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string SimpleHtmlDecode(string txt)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(txt);
            strBuilder.Replace("&lt;", "<");
            strBuilder.Replace("&gt;", ">");
            return strBuilder.ToString();
        }

        public static List<string> GetTagContentList(string txt, string start, string end)
        {
            List<string> list = new List<string>();
            if (txt == null)
            {
                return list;
            }

            int idx = 0;
            int s = 0;
            int e = 0;
            while (idx > -1)
            {
                s = txt.IndexOf(start, idx);
                if (s < 0)
                {
                    break;
                }
                e = txt.IndexOf(end, s + 1);

                if (s < 0 || e < 0)
                {
                    break;
                }
                list.Add(txt.Substring(s + start.Length, e - s - start.Length));
                idx = e + 1;
            }
            return list;
        }

        /// <summary>
        /// �^�O�ɋ��܂ꂽ���̂����X�g�����܂��Btag��True�ɂ����Ƃ��A�^�O���g���܂߂܂��B
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static List<string> GetTagContentList(string txt, string start, string end, bool tag)
        {
            List<string> list = new List<string>();
            int idx = 0;
            int s = 0;
            int e = 0;
            while (idx > -1)
            {
                s = txt.IndexOf(start, idx);
                if (s < 0)
                {
                    break;
                }
                e = txt.IndexOf(end, s);

                if (s < 0 || e < 0)
                {
                    break;
                }
                if (tag)
                {
                    list.Add(txt.Substring(s, e - s));
                }
                else
                {
                    list.Add(txt.Substring(s + start.Length, e - s - start.Length));
                }
                idx = e;
            }
            return list;
        }


        public static List<string> GetTagContentList(string txt, string start)
        {
            List<string> list = new List<string>();
            int idx = 0;
            int s = 0;
            int e = 0;
            while (idx > -1)
            {
                s = txt.IndexOf(start, idx);
                if (s < 0)
                {
                    break;
                }
                e = txt.IndexOf(start, s + 1);

                if (s < 0 || e < 0)
                {

                    break;
                }
                list.Add(txt.Substring(s, e - s));
                idx = e;
            }
            if (s > -1)
            {
                list.Add(txt.Substring(s, txt.Length - s));
            }
            return list;
        }


        public static IEnumerable<Link> GetBackImageLink(string Html, string url)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<[^>]*?\s*(background|background-image)\s*:\s*url\s*\((\s*[']|\s*)([^'\)>]+)[^>]*?>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            List<Link> linkList = new List<Link>();
            foreach (System.Text.RegularExpressions.Match match in regex.Matches(Html))
            {
                Link link = new Link();
                if (url.Length > 0)
                {
                    link.Url = ChangeAbsoluteUriForUrl(match.Groups[3].Value, url);
                }
                else
                {
                    link.Url = match.Groups[3].Value;
                }
                link.Tag = match.Value;
                linkList.Add(link);
            }
            return linkList;
        }


        /// <summary>
        /// HTML����IMG�^�O�𒊏o���ALink�I�u�W�F�N�g�ŕԂ��B
        /// </summary>
        /// <param name="Html"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static ICollection<Link> GetImageLink(string Html, string url)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<IMG[^>]*?SRC\s*=(\s*|\s*[""])([^"">]+)([""][^>]*?|[^>]*?)>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            List<Link> linkList = new List<Link>();
            foreach (System.Text.RegularExpressions.Match match in regex.Matches(Html))
            {
                Link link = new Link();
                //link.Label = 
                string label = GetTagContent(match.Value, "alt=\"", "\"");

                if (label == null || label.Length == 0)
                {
                    label = GetTagContent(match.Value, "alt='", "'");
                    if (label == null || label.Length == 0)
                    {
                        label = GetTagContent(match.Value, "alt=", " ");
                    }
                }
                link.Label = label;
                if (url.Length > 0)
                {
                    link.Url = ChangeAbsoluteUriForUrl(match.Groups[2].Value, url);
                }
                else
                {
                    link.Url = match.Groups[2].Value;
                }
                link.Tag = match.Value;
                linkList.Add(link);
            }
            return linkList;
        }


        public static ICollection<Link> GetImageLink(string Html)
        {
            return GetImageLink(Html, "");
        }

        /// <summary>
        /// �^�O�ɋ��܂ꂽ������𒊏o����B�^�O���̂��̂͏����܂��B
        /// ���܂ꂽ�����񂪑��݂��Ȃ��Ƃ���Null��Ԃ��܂��B
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="startTag"></param>
        /// <param name="endTag"></param>
        /// <returns></returns>
        public static string GetTagContent(string txt, string startTag, string endTag)
        {
            if (txt == null || txt.Length == 0)
            {
                return null;
            }

            string content = null;
            int s = txt.IndexOf(startTag);
            if (s < 0)
            {
                return null;
            }
            int e = txt.IndexOf(endTag, s + startTag.Length);

            if (s < 0 || e < 0)
            {
                return null;
            }
            else
            {
                content = txt.Substring(s + startTag.Length, e - s - startTag.Length);
            }
            return content;
        }

        /// <summary>
        /// �^�O�ɋ��܂ꂽ������𒊏o����Btag��Ture�̂Ƃ��A�^�O�������܂݂܂��B
        /// ���܂ꂽ�����񂪑��݂��Ȃ��Ƃ���Null��Ԃ��܂��B
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="startTag"></param>
        /// <param name="endTag"></param>
        /// <returns></returns>
        public static string GetTagContent(string txt, string startTag, string endTag, bool tag)
        {
            if (txt == null || txt.Length == 0)
            {
                return null;
            }
            string content = null;
            int s = txt.IndexOf(startTag);
            int e = txt.IndexOf(endTag, s + startTag.Length);

            if (s < 0 || e < 0)
            {
                return null;
            }
            else
            {
                if (tag)
                {
                    content = txt.Substring(s, e - s + endTag.Length);
                }
                else
                {
                    content = txt.Substring(s + startTag.Length, e - s - startTag.Length);
                }
            }
            return content;
        }


        
        /// <summary>
        /// HTML�t�@�C�����烊���N�𔲂��o���܂��B�Ԃ��̂�URL��Label�̃Z�b�g�̃R���N�V�����ł��B
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static ICollection<Link> GetLinkForHTML(string HTML)
        {
            //            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<A[^>]*?HREF\s*=\s*""([^""]+)""[^>]*?>([\s\S]*?)<\/A>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<A[^>]*?HREF\s*=(\s*|\s*[""])([^"">]+)([""][^>]*?|[^>]*?)>([\s\S]*?)<\/A>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            List<Link> linkList = new List<Link>();
            foreach (System.Text.RegularExpressions.Match match in regex.Matches(HTML))
            {
                Link link = new Link();
                link.Label = match.Groups[4].Value;
                link.Url = match.Groups[2].Value;
                link.Tag = match.Value.Replace(match.Groups[2].Value, string.Empty);
                linkList.Add(link);
            }
            return linkList;
        }

        public static ICollection<Link> GetLinkForHTML(string HTML, string url)
        {


            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<A[^>]*?HREF\s*=(\s*|\s*[""])([^"">]+)([""][^>]*?|[^>]*?)>([\s\S]*?)<\/A>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regexHttp = new System.Text.RegularExpressions.Regex("^http", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            List<Link> linkList = new List<Link>();

            foreach (System.Text.RegularExpressions.Match match in regex.Matches(HTML))
            {
                Link link = new Link();
                link.Label = match.Groups[4].Value;
                link.Url = HtmlTagAllDelete(match.Groups[2].Value).Replace("'", "");
                link.Tag = match.Value;
                link.TagWithoutUrl = match.Value.Replace(match.Groups[2].Value, string.Empty);
                //HTTP�������Ă���Ƃ�
                if (regexHttp.IsMatch(link.Url) == false && url != null)
                {
                    string[] tmpUrl = url.Split('?');
                    string[] tmpUrl2 = link.Url.Split('?');
                    try
                    {
                        Uri baseUri = new Uri(tmpUrl[0]);
                        Uri uri = new Uri(baseUri, tmpUrl2[0]);
                        if (tmpUrl2.Length > 1)
                        {
                            link.Url = uri.AbsoluteUri + "?" + tmpUrl2[1];
                        }
                        else
                        {
                            link.Url = uri.AbsoluteUri;
                        }
                    }
                    catch
                    {

                        link.Url = string.Empty;
                    }
                }
                linkList.Add(link);
            }

            return linkList;

        }

        public static ICollection<Link> GetLinkForHTML(string HTML, string url,string targetTag)
        {


            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<"+targetTag+@"[^>]*?HREF\s*=(\s*|\s*[""])([^"">]+)([""][^>]*?|[^>]*?)(>([\s\S]*?)<\/"+targetTag+">|/>)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regexHttp = new System.Text.RegularExpressions.Regex("^http", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            List<Link> linkList = new List<Link>();

            foreach (System.Text.RegularExpressions.Match match in regex.Matches(HTML))
            {
                Link link = new Link();
                if (match.Groups.Count > 4)
                {
                    link.Label = match.Groups[4].Value;
                }
                link.Url = HtmlTagAllDelete(match.Groups[2].Value).Replace("'", "");
                link.Tag = match.Value;
                link.TagWithoutUrl = match.Value.Replace(match.Groups[2].Value, string.Empty);

                //HTTP�������Ă���Ƃ�
                if (regexHttp.IsMatch(link.Url) == false && url != null)
                {
                    string[] tmpUrl = url.Split('?');
                    string[] tmpUrl2 = link.Url.Split('?');
                    try
                    {
                        Uri baseUri = new Uri(tmpUrl[0]);
                        Uri uri = new Uri(baseUri, tmpUrl2[0]);
                        if (tmpUrl2.Length > 1)
                        {
                            link.Url = uri.AbsoluteUri + "?" + tmpUrl2[1];
                        }
                        else
                        {
                            link.Url = uri.AbsoluteUri;
                        }
                    }
                    catch
                    {

                        link.Url = string.Empty;
                    }
                }
                linkList.Add(link);
            }

            return linkList;

        }


        /// <summary>
        /// ���߂Ɍ������������N��Ԃ��B������Ȃ������Ƃ��͋�̃����N��Ԃ��B
        /// </summary>
        /// <param name="html"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Link GetLinkFirstForHTML(string html, string url)
        {
            List<Link> list = new List<Link>(GetLinkForHTML(html, url));
            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return new Link();
            }
        }

        /// <summary>
        /// ���߂Ɍ������������N��Ԃ��B������Ȃ������Ƃ��͋�̃����N��Ԃ��B
        /// </summary>
        /// <param name="html"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Link GetLinkFirstForHTML(string html)
        {
            List<Link> list = new List<Link>(GetLinkForHTML(html));
            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return new Link();
            }
        }



        /// <summary>
        /// HTML���̑��΃p�X���΃p�X��
        /// </summary>
        /// <param name="HTML"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ChangeAbsoluteUri(string HTML, string url)
        {
            Uri baseUri = new Uri(url);
            System.Text.RegularExpressions.Regex regexHref = new System.Text.RegularExpressions.Regex(@"HREF\s*=(\s*|\s*[""])([^""\s]+)([""]|\s)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regexSrc = new System.Text.RegularExpressions.Regex(@"SRC\s*=(\s*|\s*[""])([^""\s]+)([""]|\s)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regexHttp = new System.Text.RegularExpressions.Regex("^http", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Dictionary<string, string> repleceDic = new Dictionary<string, string>();
            foreach (System.Text.RegularExpressions.Match match in regexHref.Matches(HTML))
            {
                string href = match.Groups[2].Value;
                if (regexHttp.IsMatch(href) == false)
                {
                    Uri uri = new Uri(baseUri, href);
                    if (repleceDic.ContainsKey(href) == false)
                    {
                        repleceDic.Add(href, uri.AbsoluteUri);
                    }
                }
            }
            foreach (System.Text.RegularExpressions.Match match in regexSrc.Matches(HTML))
            {
                string href = match.Groups[2].Value;
                if (regexHttp.IsMatch(href) == false)
                {
                    Uri uri = new Uri(baseUri, href);
                    if (repleceDic.ContainsKey(href) == false)
                    {
                        repleceDic.Add(href, uri.AbsoluteUri);
                    }
                }
            }
            StringBuilder strBuilder = new StringBuilder(HTML);
            foreach (string key in repleceDic.Keys)
            {
                strBuilder.Replace("\"" + key + "\"", "\"" + repleceDic[key] + "\"");

            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// URL���΂ɕϊ����܂��B
        /// </summary>
        /// <param name="url"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public static string ChangeAbsoluteUriForUrl(string url, string baseUrl)
        {
            Uri baseUri = new Uri(baseUrl);
            string url1 = url;
            System.Text.RegularExpressions.Regex regexHttp = new System.Text.RegularExpressions.Regex("^http", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (regexHttp.IsMatch(url) == false)
            {
                Uri uri = new Uri(baseUri, url);
                url1 = uri.AbsoluteUri;
            }
            return url1;
        }


        public static ICollection<Link> CreateLinkCollection(string[] lines)
        {
            List<Link> list = new List<Link>();
            foreach (string line in lines)
            {
                if (line.Length > 0)
                {
                    string[] data = line.Split('\t');
                    Link link = new Link();
                    if (data.Length > 1)
                    {
                        link.Url = data[0];
                        link.Label = data[1];

                    }
                    else
                    {
                        link.Url = data[0];
                        link.Label = System.IO.Path.GetFileNameWithoutExtension(data[0]);
                    }
                    list.Add(link);
                }
            }
            return list;
        }

        public static string GetTitleForHTML(string html)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<title>([^<]*)</title>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match match = regex.Match(html);
            if (match.Groups.Count > 0)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }

 


        public static string GetHTML(string url)
        {
            string text = string.Empty;
            try
            {
                //HttpWebRequest�̍쐬
                System.Net.HttpWebRequest webreq =
                    (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                //               webreq.Timeout = webreq.Timeout * timeout;
                //HttpWebResponse�̎擾
                System.Net.HttpWebResponse webres =
                    (System.Net.HttpWebResponse)webreq.GetResponse();


                //�����f�[�^����M���邽�߂�Stream���擾
                System.IO.Stream st = webres.GetResponseStream();

                List<byte> byteArray = new List<byte>();
                int b;
                while ((b = st.ReadByte()) > -1)
                {
                    byteArray.Add((byte)b);
                }

                RawlerLib.Text.TxtEnc txtEnc = new RawlerLib.Text.TxtEnc();
                byte[] byteArray2 = byteArray.ToArray();
                txtEnc.SetFromByteArray(ref byteArray2);

                txtEnc.Codec = "shift_jis";
                string html = txtEnc.Text;
                text = html;
            }
            catch
            {
                text = string.Empty;
            }
            return text;
        }

        public static string GetHTML(string url, Encoding enc)
        {
            string text = string.Empty;
            try
            {
                //HttpWebRequest�̍쐬
                System.Net.HttpWebRequest webreq =
                    (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                //               webreq.Timeout = webreq.Timeout * timeout;
                //HttpWebResponse�̎擾
                System.Net.HttpWebResponse webres =
                    (System.Net.HttpWebResponse)webreq.GetResponse();


                //�����f�[�^����M���邽�߂�Stream���擾
                System.IO.Stream st = webres.GetResponseStream();

                List<byte> byteArray = new List<byte>();
                int b;
                while ((b = st.ReadByte()) > -1)
                {
                    byteArray.Add((byte)b);
                }

                byte[] byteArray2 = byteArray.ToArray();
                text = enc.GetString(byteArray2);

            }
            catch
            {
                text = string.Empty;
            }
            return text;
        }

        public static string GetHTML(string url, Encoding enc, WebEnvironment webE)
        {
            string text = string.Empty;
            try
            {
                //HttpWebRequest�̍쐬
                System.Net.HttpWebRequest webreq =
                    (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                //               webreq.Timeout = webreq.Timeout * timeout;
                //HttpWebResponse�̎擾
                webreq.UserAgent = webE.UserAgent;
                webreq.Referer = webE.Referer;



                System.Net.HttpWebResponse webres =
                    (System.Net.HttpWebResponse)webreq.GetResponse();


                //�����f�[�^����M���邽�߂�Stream���擾
                System.IO.Stream st = webres.GetResponseStream();

                List<byte> byteArray = new List<byte>();
                int b;
                while ((b = st.ReadByte()) > -1)
                {
                    byteArray.Add((byte)b);
                }

                byte[] byteArray2 = byteArray.ToArray();
                text = enc.GetString(byteArray2);

            }
            catch
            {
                text = string.Empty;
            }
            return text;
        }

        /// <summary>
        /// web�p�̊��ϐ����i�[������̂ł��B
        /// </summary>
        public struct WebEnvironment
        {
            public string UserAgent;
            public string Referer;

            public WebEnvironment(string agent, string referer)
            {
                UserAgent = agent;
                Referer = referer;
            }
        }


        public static string GetMailAddress(string txt)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[a-z][0-9a-z_.\-]*@[0-9a-z\-]+(\.[0-9a-z\-]+)+");
            return regex.Match(txt).Value;
        }

        public static string DeleteTargetBlank(string html)
        {
            StringBuilder strBuilder = new StringBuilder(html);
            foreach (string link in GetLinkHtml(html))
            {
                if (link.Contains("target=\"_blank"))
                {
                    string link2 = link.Replace("target=\"_blank", "");
                    strBuilder.Replace(link, link2);
                }
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// HTML�t�@�C������A���J�[�^�O�����������N�𔲂��o���܂��B
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static ICollection<string> GetLinkHtml(string HTML)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<A[^>]*?HREF\s*=(\s*|\s*[""])([^"">]+)([""][^>]*?|[^>]*?)>([\s\S]*?)<\/A>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            List<string> list = new List<string>();
            foreach (System.Text.RegularExpressions.Match match in regex.Matches(HTML))
            {
                list.Add(match.Value);
            }
            return list;
        }

        /// <summary>
        /// �������O�C���p��HTML�����܂��B
        /// </summary>
        /// <param name="loginUrl"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string CreateLoginHtml(string loginUrl, List<KeyValuePair<string, string>> list)
        {
            StringBuilder strbuilder = new StringBuilder();
            strbuilder.AppendLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\">");
            strbuilder.AppendLine("<html lang=\"ja\">");
            strbuilder.AppendLine("<head>");
            strbuilder.AppendLine("<meta http-equiv=\"content-type\" content=\"text/html; charset=shift_jis\">");
            strbuilder.AppendLine("<meta http-equiv=\"content-script-type\" content=\"text/javascript\">");
            strbuilder.AppendLine("<title>�������O�C��</title>");
            strbuilder.AppendLine("</head>");
            strbuilder.AppendLine("<body onLoad=\"document.form.submit()\">");
            strbuilder.AppendLine("<form action=\"" + loginUrl + "\" method=\"post\" name=\"form\">");
            strbuilder.AppendLine("<div>");

            foreach (KeyValuePair<string, string> pair in list)
            {
                strbuilder.AppendLine("<input type=\"hidden\" name=\"" + pair.Key + "\" value=\"" + pair.Value + "\">");
            }

            strbuilder.AppendLine("</div></form></body></html>");
            return strbuilder.ToString();
        }




        /// <summary>
        /// �X�N���v�g���폜����B
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string DeleteScript(string html)
        {
            StringBuilder strBuilder = new StringBuilder(html);
            List<string> list = RawlerLib.Web.GetTagContentList(html, "<script", "</script>", true);
            foreach (string s in list)
            {
                strBuilder.Replace(s, "");
            }
            return strBuilder.ToString();
        }

        ///// <summary>
        ///// URL�Ɋ܂ރp�����[�^��DIC�Ŏ擾���܂��BURL�f�R�[�h������܂��B
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //public static Dictionary<string, string> GetUrlParameter(string url)
        //{
        //    Dictionary<string, string> dic = new Dictionary<string, string>();
        //    url = url.Replace("&amp;", "&");
        //    string[] data = url.Split('?');
        //    if (data.Length > 1)
        //    {
        //        string[] para = data[1].Split('&');
        //        foreach (var item in para)
        //        {
        //            string[] keyval = item.Split('=');
        //            if (keyval.Length > 1)
        //            {
        //                dic.Add(keyval[0], HttpUtility.UrlDecode(keyval[1]));
        //            }
        //        }
        //    }
        //    return dic;
        //}


        public struct Link
        {
            private string url;

            public string Url
            {
                get { return url; }
                set { url = value; }
            }
            private string label;

            public string Label
            {
                get { return label; }
                set { label = value; }
            }
            private string tag;

            public string Tag
            {
                get { return tag; }
                set { tag = value; }
            }

            public string TagWithoutUrl
            {
                get;
                set;
            }

        }
    }

}
