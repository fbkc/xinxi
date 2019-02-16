using AutoSend;
using HRMSys.DAL;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace xinxi
{
    /// <summary>
    /// ModelHandler 的摘要说明
    /// </summary>
    public class ModelHandler : IHttpHandler
    {
        private BLL bll = new BLL();
        private string host = "http://xinxi.100dh.cn/";
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            StringBuilder _strContent = new StringBuilder();
            if (_strContent.Length == 0)
            {
                string _strAction = context.Request.Params["action"];
                if (string.IsNullOrEmpty(_strAction))
                {
                    _strContent.Append(_strContent.Append("404.html"));
                }
                else
                {
                    switch (_strAction.Trim())
                    {
                        case "moduleHtml": _strContent.Append(ModuleHtml(context)); break;
                        default: break;
                    }
                }
            }
            context.Response.Write(_strContent.ToString());
        }
        public string ModuleHtml(HttpContext context)
        {
            //需要做一个时间，每隔多长时间才允许访问一次
            string keyValue = NetHelper.GetMD5("liu" + "100dh888");
            string url = "";
            string key = context.Request["key"];
            if (key != keyValue)
                return json.WriteJson(0, "key值错误", new { });
            string username = context.Request["username"];
            if (string.IsNullOrEmpty(username))
                return json.WriteJson(0, "用户名不能为空", new { });
            try
            {
                htmlPara hInfo = new htmlPara();
                hInfo.userId = bll.GetUserId(username);//用户名
                hInfo.title = context.Request["title"];
                string cid = context.Request["catid"];
                if (string.IsNullOrEmpty(cid))
                    return json.WriteJson(0, "行业或栏目不能为空", new { });
                hInfo.columnId = cid;//行业id，行业新闻id=20
                string content = context.Request["content"];
                if (string.IsNullOrEmpty(content) || content.Length < 500)
                    return json.WriteJson(0, "文章不能少于500字，请丰富文章内容", new { });
                hInfo.articlecontent = content;
                //hInfo.articlecontent = HttpUtility.UrlDecode(jo["content"].ToString(), Encoding.UTF8);//内容,UrlDecode解码
                //命名规则：ip/目录/用户名/show_行业id+(五位数id)
                long htmlId = (bll.GetMaxId() + 1);
                hInfo.Id = htmlId;
                string showName = "show_" + cid + "_" + htmlId + ".html";
                url = host + username + "/" + showName;
                hInfo.titleURL = url;
                //hInfo.titleURL = string.Format("handler/TestHandler.ashx?action=DetailPage&cId={0}&Id={1}", cid, htmlId);
                //url = host + hInfo.titleURL;
                hInfo.pinpai = context.Request["pinpai"];
                hInfo.xinghao = context.Request["xinghao"];
                hInfo.price = context.Request["price"];
                hInfo.smallCount = context.Request["qiding"];
                hInfo.sumCount = context.Request["gonghuo"];
                hInfo.unit = context.Request["unit"];
                hInfo.city = context.Request["city"];
                hInfo.titleImg = context.Request["thumb"];
                hInfo.addTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                hInfo.username = username;
                //公司 / 会员信息
                cmUserInfo uInfo = bll.GetUser(string.Format("where username='{0}'", username));
                hInfo.companyName = uInfo.companyName;
                hInfo.com_web = uInfo.com_web;
                //hInfo.realmNameId = "1";//发到哪个站
                bll.AddHtml(hInfo);//存入数据库

                string keyword = "";//关键词
                string description = "";//描述
                if (hInfo.title.Length > 6)
                    keyword = hInfo.title + "," + hInfo.title.Substring(0, 2) + "," + hInfo.title.Substring(2, 2) + "," + hInfo.title.Substring(4, 2);
                else
                    keyword = hInfo.title;
                description = BLL.ReplaceHtmlTag(hInfo.articlecontent, 80);//产品简介
                List<htmlPara> pList = bll.GetHtmlBAPage(cid, htmlId.ToString());//上一篇，下一篇
                var data = new
                {
                    htmlTitle = hInfo.title + "_" + hInfo.companyName,
                    hInfo,
                    keyword,
                    description,
                    host,
                    columnName = bll.GetColumns(" where Id=" + cid)[0].columnName,
                    columnsList = bll.GetColumns(""),//导航
                    BPage = new { Href = pList[0].titleURL, Title = pList[0].title },//上一篇
                    ProductFloat = bll.GetProFloat(hInfo.userId),
                    NewsFloat = bll.GetNewsFloat(hInfo.userId)
                };
                string html = SqlHelperCatalog.WriteTemplate(data, "DetailModel.html");
                WriteFile(html, showName, username);//写模板
            }
            catch (Exception ex)
            {
                return json.WriteJson(0, ex.ToString(), new { });
            }
            return json.WriteJson(1, "发布成功", new { url, username });
        }

        /// <summary>
        /// 写模板
        /// </summary>
        /// <param name="hInfo"></param>
        /// <param name="uInfo"></param>
        /// <param name="username"></param>
        /// <param name="hName"></param>
        /// <returns></returns>
        public static bool WriteFile(string moduleHtml, string htmlfilename, string username)
        {
            //文件输出目录
            string path = HttpContext.Current.Server.MapPath("~/" + username + "/");
            //无此路径，则创建路径
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // 写文件
            using (StreamWriter sw = new StreamWriter(path + htmlfilename, true))
            {
                sw.Write(moduleHtml);
                sw.Flush();
                sw.Close();
            }
            return true;
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