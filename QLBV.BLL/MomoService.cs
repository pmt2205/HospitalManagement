using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace QLBV.BLL
{
    public class MomoService
    {
        private readonly IConfiguration _config;

        public MomoService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> CreatePaymentUrlAsync(string orderId, decimal amount, string orderInfo)
        {
            try
            {
                var endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";

                string partnerCode = _config["Momo:PartnerCode"];
                string accessKey = _config["Momo:AccessKey"];
                string secretKey = _config["Momo:SecretKey"];
                string redirectUrl = _config["Momo:RedirectUrl"];
                string ipnUrl = _config["Momo:IpnUrl"];
                string requestType = _config["Momo:RequestType"];

                string rawHash = $"accessKey={accessKey}&amount={amount}&extraData=&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestType={requestType}";

                string signature;
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
                {
                    var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
                    signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }

                var request = new
                {
                    partnerCode,
                    accessKey,
                    requestType,
                    amount = amount.ToString(),
                    orderId,
                    orderInfo,
                    redirectUrl,
                    ipnUrl,
                    extraData = "",
                    signature
                };

                using (var client = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(endpoint, content);
                    var json = await response.Content.ReadAsStringAsync();
                    var jobj = JObject.Parse(json);

                    if (jobj["resultCode"]?.ToString() != "0")
                    {
                        return null;
                    }

                    return jobj["payUrl"]?.ToString();
                }
            }
            catch
            {
                return null;
            }
        }
    }

}
