using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Invest_Cloud_Technical.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public string Index()
        {
            Random rnd = new Random();
            int size = 2; /*rnd.Next(2, 1000)*/;

            //Populate Matrix Data against Array Size by using Get/init
            using (var client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri("https://recruitment-test.investcloud.com/api/");
                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HTTP GET
                {
                    var responseTask = client1.GetAsync($"numbers/init/{size}").Result;
                    string res = "";
                    using (HttpContent content = responseTask.Content)
                    {
                        // ... Read the string.
                        System.Threading.Tasks.Task<string> result = content.ReadAsStringAsync();
                        res = result.Result;
                    }
                }
            }
            //Creating Array as per Size
            int[,] A = new int[size, size];
            int[,] B = new int[size, size];
            int[,] resultMatrix = new int[size, size];

            A = GetMatrixData(size, "A", "row");
            B = GetMatrixData(size, "B", "col");
            //declared Stringbuilder 
            System.Text.StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                for (int k = 0; k < size; k++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        resultMatrix[i, j] += A[i, k] * B[k, j];
                    }
                }
            }
            //Append array item in string
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    sb.Append(resultMatrix[i, j]);
                }
            }
            //Converting String into MD5 Hash
            string MD5hash = CreateMD5(sb.ToString());
            String resultPost;
            //Submit value for validate MD5 Hash string post Method
            using (var client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri("https://recruitment-test.investcloud.com/api/");

                var postTask = client1.PostAsync("numbers/validate", new StringContent(
                                new JavaScriptSerializer().Serialize(MD5hash), System.Text.Encoding.UTF8, "application/json"));
                postTask.Wait();
                resultPost = postTask.Result.ToString();
            }

            return resultPost;
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                System.Text.StringBuilder sb1 = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb1.Append(hashBytes[i].ToString("X2"));
                }
                return sb1.ToString();
            }
        }
        public int[,] GetMatrixData(int size, String Dataset, String type)
        {
            int[,] matrix = new int[size, size];

            ///Data dumping in array
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://recruitment-test.investcloud.com/api/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HTTP GET
                for (int i = 0; i < size; i++)
                {
                    var responseTask = client.GetAsync($"numbers/{Dataset}/{type}/{i}").Result;
                    string res = "";
                    using (HttpContent content = responseTask.Content)
                    {
                        //Read the string.
                        System.Threading.Tasks.Task<string> result = content.ReadAsStringAsync();
                        res = result.Result;
                        JObject o = JObject.Parse(res);
                        JArray array = (JArray)o["Value"];
                        for (int j = 0; j < size; j++)
                        {
                            matrix[i, j] = (int)(array[j]);
                        }
                    }

                }
            }
            return matrix;
        }
    }
}