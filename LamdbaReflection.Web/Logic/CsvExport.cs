using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LamdbaReflection.Web.Logic
{
    public class CsvExport<T> where T : class
    {
        public List<T> Objects;

        public CsvExport(List<T> objects)
        {
            Objects = objects;
        }

        public string Export()
        {
            return Export(true);
        }

        public string Export(bool includeHeaderLine)
        {

            StringBuilder sb = new StringBuilder();
            //Get properties using reflection.
            IList<PropertyInfo> propertyInfos = typeof(T).GetProperties();

            if (includeHeaderLine)
            {
                //add header line.
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    var atts = propertyInfo.GetCustomAttributes(
                        typeof(DisplayNameAttribute), true);
                    if (atts.Length == 0)
                    {
                        //Don't include anything that doens't have a custom attribute
                        //this will get rid of IDs
                        //sb.Append(propertyInfo.Name).Append(",");
                    }
                    else
                    {
                        bool isRandom = false;
                        if (propertyInfo.Name.Length > 4)
                        {
                            if (propertyInfo.Name.Contains("dom"))
                            {
                                isRandom = true;
                            }
                        }

                        if (false == isRandom)
                        {
                            string displayText = "";
                            System.ComponentModel.DisplayNameAttribute displayName = (System.ComponentModel.DisplayNameAttribute)atts[0];

                            displayText = displayName.DisplayName;
                            displayText = displayText.Replace(":", "");
                            sb.Append(displayText);
                            sb.Append(",");
                        }
                    }

                }

                sb.Remove(sb.Length - 1, 1).AppendLine();
            }

            //add value for each property.
            foreach (T obj in Objects)
            {
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {

                    var atts = propertyInfo.GetCustomAttributes(
                       typeof(DisplayNameAttribute), true);
                    if (atts.Length == 0)
                    {
                        //Don't include anything that doens't have a custom attribute
                        //this will get rid of IDs
                    }
                    else
                    {
                        //exclude random columns
                        bool isRandom = false;
                        if (propertyInfo.Name.Length > 4)
                        {
                            if (propertyInfo.Name.Contains("dom"))
                            {
                                isRandom = true;
                            }
                        }

                        if (false == isRandom)
                        {
                            sb.Append(MakeValueCsvFriendly(propertyInfo.GetValue(obj, null))).Append(",");
                        }

                    }

                }
                sb.Remove(sb.Length - 1, 1).AppendLine();
            }

            return sb.ToString();
        }

        //export to a file.
        public void ExportToFile(string path)
        {
            File.WriteAllText(path, Export());
        }

        //export as binary data.
        public byte[] ExportToBytes()
        {
            return Encoding.UTF8.GetBytes(Export());
        }

        //get the csv value for field.
        private string MakeValueCsvFriendly(object value)
        {
            if (value == null) return "";
            if (value is Nullable && ((INullable)value).IsNull) return "";

            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            string output = value.ToString();

            if (output.Contains(",") || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';

            return output;

        }
    }
}