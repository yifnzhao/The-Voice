using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzureSpeechTest
{
    public class NetworkLayer
    {
        public string PostJson(string url, string json)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 80000;
                Encoding encoding = Encoding.UTF8;
                Stream streamrequest = request.GetRequestStream();
                StreamWriter streamWriter = new StreamWriter(streamrequest, encoding);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
                streamrequest.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamresponse = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamresponse, encoding);
                string result = streamReader.ReadToEnd();
                streamresponse.Close();
                streamReader.Close();

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "";
            }
            
        }
    }
}
