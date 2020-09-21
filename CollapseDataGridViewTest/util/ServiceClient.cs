using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CollapseDataGridViewTest.util
{
    public class ServiceClient
    {

        private static HttpClient httpClient;

        private static HttpClient useDefaultHttpClient;

        static JavaScriptSerializer Serializer = new JavaScriptSerializer
        {
            MaxJsonLength = Int32.MaxValue
        };

        static void Init()
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(""),
                    Timeout = new TimeSpan(0, 30, 0),
                    MaxResponseContentBufferSize = Int32.MaxValue
                };
                httpClient.DefaultRequestHeaders.Add("KeepAlive", "true");
                httpClient.DefaultRequestHeaders.Add("ApiKey", "");
            }

            if (useDefaultHttpClient == null)
            {
                var httpclientHandler = new HttpClientHandler()
                {
                    UseDefaultCredentials = true
                };

                useDefaultHttpClient = new HttpClient(httpclientHandler, true);
                useDefaultHttpClient.DefaultRequestHeaders.Add("KeepAlive", "true");
                useDefaultHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public static bool UploadFile(string filePath, string url)
        {
            Init();
            FileInfo FileInfo = new FileInfo(filePath);
            byte[] FileStream = File.ReadAllBytes(filePath);
            var Upload = new
            {
                Name = Path.GetFileName(filePath),
                Stream = FileStream,
                MD5 = GetMD5Hash(FileStream),
                LastWriteDateTime = FileInfo.LastWriteTime,
                AppGuid = "",
                Creator = Environment.UserName
            };
            var jsonResult = HttpPostJson(url, Upload);
           
            dynamic result = JsonConvert.DeserializeObject(jsonResult);

            return result.Result;
        }

        public static bool UploadFile(string filePath, string url, CancellationToken token)
        {
            Init();
            FileInfo FileInfo = new FileInfo(filePath);
            byte[] FileStream = File.ReadAllBytes(filePath);
            var Upload = new
            {
                Name = Path.GetFileName(filePath),
                Stream = FileStream,
                MD5 = GetMD5Hash(FileStream),
                LastWriteDateTime = FileInfo.LastWriteTime,
                AppGuid = "",
                Creator = Environment.UserName
            };
            var jsonResult = HttpPostJson(url, Upload, token);
            
            dynamic result = JsonConvert.DeserializeObject(jsonResult);

            return result.Result;
        }

        public static string HttpPostJson(string url, object obj)
        {
            string Json = Serializer.Serialize(obj);
            try
            {
                HttpResponseMessage response = null;
                using (MemoryStream MemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Json)))
                {
                    response = httpClient.PostAsync(url, new StreamContent(MemoryStream)).Result;
                }
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                //Logger.Error($"url: {url} get exception: {ex.Message}\r\n details:{ex.ToString()}");
                //MessageBox.Show($"url: {url} get exception: {ex.Message}\r\n details:{ex.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

        }

        public static string HttpPostJson(string url, object obj, CancellationToken token)
        {
            string Json = Serializer.Serialize(obj);
            try
            {
                HttpResponseMessage response = null;
                using (MemoryStream MemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Json)))
                {
                    response = httpClient.PostAsync(url, new StreamContent(MemoryStream), token).Result;
                }
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.GetType().Name != "TaskCanceledException")
                {
                    //MessageBox.Show($"url: {url} get exception: {ex.Message}\r\n details:{ex.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                //Logger.Error($"url: {url} get exception: {ex.Message}\r\n details:{ex.ToString()}");
                return JsonConvert.SerializeObject(new
                {
                    Result = false
                });
            }

        }

        static string GetMD5Hash(byte[] Stream)
        {
            byte[] md5Hash = null;
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                md5Hash = md5.ComputeHash(Stream);
            };

            return md5Hash != null && md5Hash.Length > 0 ? string.Join(string.Empty, md5Hash.Select(i => i.ToString("X2"))) : null;
        }

        public static string GetFromCPM(string url)
        {
            Init();
            try
            {
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                //Logger.Error($"url: {url} get exception: {ex.Message}\r\n details:{ex.ToString()}");
                //MessageBox.Show($"url: {url} get exception: {ex.Message}\r\n details:{ex.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        public static string Get(string url)
        {
            Init();
            try
            {
                HttpResponseMessage response = useDefaultHttpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                //Logger.Error($"url: {url} get exception: {ex.Message}\r\n details:{ex.ToString()}");
                //MessageBox.Show($"url: {url} get exception: {ex.Message}\r\n details:{ex.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        public static string Del(string url)
        {
            Init();
            try
            {
                HttpResponseMessage response = useDefaultHttpClient.DeleteAsync(url).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                //Logger.Error($"url: {url} get exception: {ex.Message}\r\n details:{ex.ToString()}");
                //MessageBox.Show($"url: {url} get exception: {ex.Message}\r\n details:{ex.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        public static byte[] HttpGetStream(string url)
        {
            byte[] Result = null;
            try
            {
                Task<HttpResponseMessage> T_HttpResponseMessage = null;
                Task.Run(() => T_HttpResponseMessage = httpClient.GetAsync(url)).Wait();

                if (T_HttpResponseMessage != null && T_HttpResponseMessage.Result != null)
                {
                    Task<byte[]> T_Stream = null;
                    Task.Run(() => T_Stream = T_HttpResponseMessage.Result.Content.ReadAsByteArrayAsync()).Wait();

                    if (T_Stream != null) Result = T_Stream.Result;
                }
            }
            catch (Exception ex)
            {
                //Logger.Error($"url: {url} HttpGetStream exception: {ex.Message}\r\n details:{ex.ToString()}");
                //MessageBox.Show($"url: {url} HttpGetStream exception: {ex.Message}\r\n details:{ex.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return Result;
        }
    }
}
