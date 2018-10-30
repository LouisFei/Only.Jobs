using System;
using System.Collections.Generic;

namespace Only.Jobs.Core.Common
{
    public class Utils
    {
        /// <summary>
        /// 字符串转List<Guid> 
        /// </summary>
        /// <param name="strArray">字符串,多个ID以 , 隔开</param>
        /// <returns></returns>
        public static List<Guid> StringToGuidList(string strArray)
        {
            List<Guid> list = new List<Guid>();
            if (!string.IsNullOrWhiteSpace(strArray))
            {
                string[] arrays = strArray.Split(',');
                foreach (string str in arrays)
                {
                    Guid result = Guid.NewGuid();
                    if (Guid.TryParse(str, out result))
                    {
                        list.Add(result);
                    }
                        
                }
            }
            return list;
        }
    }
}
