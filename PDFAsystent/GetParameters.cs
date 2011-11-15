using System;
using System.Text;

namespace PDFAsystent
{
    public static class GetParameters
    {
        public static string GetParam(string param_name)
        {
            string temp_value = "";

            foreach (string param in Environment.GetCommandLineArgs())
            {
                int index = param.IndexOf("=");
                if (index > 0)
                    temp_value = param.Substring(0, index);
                if (temp_value == param_name)
                {
                    return param.Substring(index + 1);                    
                }
            }
            return "";
        }
        
    }
}
