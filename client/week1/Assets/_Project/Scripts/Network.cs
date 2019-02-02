using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class Network : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 上传Json格式的数据到url地址
    /// </summary>
    /// <param name="url"></param>
    /// <param name="json"></param>
    /// <returns>Post返回</returns>
    public string PostJson(string url, string json)
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

}
