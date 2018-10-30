using System;
using System.Collections.Concurrent;
using System.Web.Mvc;

using Newtonsoft.Json;
using Only.Jobs.Core.Business.Info;
using Only.Jobs.Core.Common;

namespace Only.Jobs.Web.Controllers
{
    public class BaseController : Controller
    {
        #region JsonResult

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <returns></returns>
        public new JsonResult Json(object data)
        {
            return new NewtonJsonResult(data, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="behavior">请求行为设置</param>
        /// <returns></returns>
        public new JsonResult Json(object data, JsonRequestBehavior behavior)
        {
            return new NewtonJsonResult(data, behavior, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="behavior">请求行为设置</param>
        /// <param name="DateFormatString">日期格式化</param>
        /// <returns></returns>
        public JsonResult Json(object data, JsonRequestBehavior behavior, string DateFormatString)
        {
            return new NewtonJsonResult(data, behavior, new JsonSerializerSettings() { DateFormatString = DateFormatString });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="DateFormatString">日期格式化字符串</param>
        /// <returns></returns>
        public new JsonResult Json(object data, string DateFormatString)
        {
            return new NewtonJsonResult(data, new JsonSerializerSettings() { DateFormatString = DateFormatString });
        }

        #endregion


        /// <summary>
        /// 获取页面参数
        /// </summary>
        /// <returns></returns>
        public PageParameter GetPageParameter()
        {
            PageParameter parameter = new PageParameter();
            parameter.Rows = WebHelper.GetRequestInt("rows", 1);
            parameter.CurrentPageIndex = WebHelper.GetRequestInt("page", 1);
            string Param = WebHelper.GetRequestString("Param");
            if (!string.IsNullOrEmpty(Param))
            {
                try
                {
                    if (Param.Contains(",}"))
                    {
                        Param = Param.Replace(",}", "}");
                    }
                    System.Web.Script.Serialization.JavaScriptSerializer sr = new System.Web.Script.Serialization.JavaScriptSerializer();
                    parameter.SetDictionary(sr.Deserialize(Param, typeof(ConcurrentDictionary<string, string>)) as ConcurrentDictionary<string, string>);
                }
                catch (Exception)
                { }
            }

            foreach (string key in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
            {
                if (!string.IsNullOrWhiteSpace(key) && !key.Equals("rows") && !key.Equals("page") && !key.Equals("Param"))
                {
                    parameter.AddParameter(key, Request.QueryString[key]);
                }
            }
            return parameter;
        }
    }
}