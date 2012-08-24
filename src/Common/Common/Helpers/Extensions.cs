using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace Common.Helpers
{
    public static class Extensions
    {
        public static string ToNameValuePairs(this object obj, bool includeEmptyProperties = true)
        {
            string result = "";

            foreach (PropertyInfo p in obj.GetType().GetProperties())
            {
                var objVal = p.GetValue(obj, null);
                var value = objVal != null ? objVal.ToString() : null;

                if (string.IsNullOrEmpty(value))
                {
                    if (includeEmptyProperties)
                    {
                        if (!string.IsNullOrEmpty(result))
                        {
                            result += "&";
                        }

                        result += string.Format("{0}={1}", p.Name, value);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        result += "&";
                    }

                    result += string.Format("{0}={1}", p.Name, value);
                }
            }

            return result;
        }

        public static string ToJson(this object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }

        public static string TrimEnd(this string source, string value)
        {
            if (!source.EndsWith(value))
                return source;

            return source.Remove(source.LastIndexOf(value));
        }
    }
}
