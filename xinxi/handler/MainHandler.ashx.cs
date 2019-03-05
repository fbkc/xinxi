using AutoSend;
using HRMSys.DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace xinxi
{
    /// <summary>
    /// MainHandler 的摘要说明
    /// </summary>
    public class MainHandler : IHttpHandler
    {
        BLL bll = new BLL();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain;charset=utf-8;";
            StringBuilder _strContent = new StringBuilder();
            if (_strContent.Length == 0)
            {
                string _strAction = context.Request.Params["action"];
                if (string.IsNullOrEmpty(_strAction))
                {
                    _strContent.Append(_strContent.Append(json.WriteJson(0, "禁止访问", new { })));
                }
                else
                {
                    switch (_strAction.Trim().ToLower())
                    {
                        case "getmainhtmllist": _strContent.Append(GetMainHtmlInfo(context)); break;
                        case "getcidhtmllist": _strContent.Append(GetCIdHtmlInfo(context)); break;
                        case "pushbaidu": _strContent.Append(PushBaidu(context)); break;
                        default: break;
                    }
                }
            }
            context.Response.Write(_strContent.ToString());
        }
        /// <summary>
        /// 主动push百度
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string PushBaidu(HttpContext context)
        {
            string count = context.Request["count"];
            List<string> urlList = bll.GetUrl(int.Parse(count));
            return PostUrl(urlList);
        }
        public static string PostUrl(List<string> urls)
        {
            try
            {
                string formUrl = "http://data.zz.baidu.com/urls?site=www.16fafa.cn&token=YBFkvfnC8iO0c3Fh";

                string formData = "";

                foreach (string url in urls)
                {
                    formData += url + "\n";
                }

                byte[] postData = System.Text.Encoding.UTF8.GetBytes(formData);

                // 设置提交的相关参数 
                System.Net.HttpWebRequest request = System.Net.WebRequest.Create(formUrl) as System.Net.HttpWebRequest;
                System.Text.Encoding myEncoding = System.Text.Encoding.UTF8;
                request.Method = "POST";
                request.KeepAlive = false;
                request.AllowAutoRedirect = true;
                request.ContentType = "text/plain";
                request.UserAgent = "curl/7.12.1";
                request.ContentLength = postData.Length;

                // 提交请求数据 
                System.IO.Stream outputStream = request.GetRequestStream();
                outputStream.Write(postData, 0, postData.Length);
                outputStream.Close();

                System.Net.HttpWebResponse response;
                System.IO.Stream responseStream;
                System.IO.StreamReader reader;
                string srcString;
                response = request.GetResponse() as System.Net.HttpWebResponse;
                responseStream = response.GetResponseStream();
                reader = new System.IO.StreamReader(responseStream, System.Text.Encoding.GetEncoding("UTF-8"));
                srcString = reader.ReadToEnd();
                string result = srcString;   //返回值赋值
                reader.Close();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 目录首页显示信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetMainHtmlInfo(HttpContext context)
        {
            string columnId = context.Request["columnId"];//行业id
            List<htmlInfo> hList = null;
            try
            {
                //根据行业id、目录名和添加时间降序查询
                hList = bll.GetMainHtmlList();
                if (hList == null || hList.Count < 1)
                    return json.WriteJson(0, "未获取到标题信息", new { });
            }
            catch (Exception ex)
            {
                return json.WriteJson(0, ex.Message, new { });
            }
            return json.WriteJson(1, "成功", new { htmlList = hList });
        }
        
        /// <summary>
        /// 每个栏目拿到的信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetCIdHtmlInfo(HttpContext context)
        {
            string realmNameId = "1";//目录名
            string columnId = context.Request["columnId"];//行业id
            string pageIndex = context.Request["page"];
            string pageSize = context.Request["pageSize"];
            if (string.IsNullOrEmpty(pageIndex))
                pageIndex = "1";
            if (string.IsNullOrEmpty(pageSize))
                pageSize = "10";
            List<htmlPara> hList = null;
            try
            {
                //根据行业id、目录名和添加时间降序查询
                //hList = bll.GetHtmlList(string.Format("where columnId='{0}' order by addTime desc", columnId), "20");
                hList = null;
                if (hList == null || hList.Count < 1)
                    return json.WriteJson(0, "未获取到标题信息", new { });
            }
            catch (Exception ex)
            { return json.WriteJson(0, ex.Message, new { }); }
            //查询分页数据
            var pageData = hList.Where(u => u.Id > 0)
                            .OrderByDescending(u => u.Id)
                            .Skip((int.Parse(pageIndex) - 1) * int.Parse(pageSize))
                            .Take(int.Parse(pageSize)).ToList();
            return json.WriteJson(1, "成功", new { total = hList.Count(), htmlList = pageData });
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}