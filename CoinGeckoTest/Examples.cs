using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using CoinGeckoApiExtensions;

namespace CoinGeckoNet
{
    class Examples
    {
        static string _rootUri = "https://api.coingecko.com/api/v3";
        static void Main(string[] args)
        {
            //var spotMarkets = GetInfoByExchange.GetExchangeList();
            //var derivsMarkets = GetInfoByExchange.GetDerivativesExchangeList(GetInfoByExchange.DerivativesExchangesSortOrder.DescendingByOI);
            //var derivsMarketNames = derivsMarkets.Select(x => x.ID).ToList();
            //var derivsMarketInfos = GetInfoByExchange.GetDetailedDerivativesExchangeDataList(derivsMarketNames);
            //var allKnownCoins = GetTokenInfo.GetAllKnownTokenNames();
            //var closingBtcPriceFromDate = GetTokenInfo.GetClosingPriceFromDate("bitcoin", new DateTime(2019,3,12), new string[] { "usd", "eth" });
            //var getTopTokenInfos = GetTokenInfo.GetTokenInfoList();
            //var now = DateTime.Now;
            //var testSomeMore = getTopTokenInfos.FirstOrDefault().GetDatePricePairs(now.AddDays(-10), now);
            var thisObj = GetTokenInfo.GetPriceData("bitcoin", "usd", true);
            var x = 0;
        }
    }

    public static class GetTrendingTokens
    {
        static string _rootUri = "https://api.coingecko.com/api/v3";

        private static dynamic DoApiCall()
        {
            var trendingUrl = string.Format("{0}/search/trending", _rootUri);
            return trendingUrl.GetSimpleApiResponse();
        }
    }
}

namespace CoinGeckoApiExtensions
{
    /// <summary>
    /// Helper Class that accepts and API URL string constructor and returns an unparsed JSON object.
    /// </summary>
    public static class GetApiResponse
    {
        /// <summary>
        /// Queries the URL to attempt to return a JSON object.
        /// </summary>
        public static dynamic GetSimpleApiResponse(this string apiUrl)
        {
            var req = WebRequest.Create(apiUrl);
            dynamic deserializeResponse;
            using (var read = req.GetResponse())
            {
                using (var reader = new StreamReader(read.GetResponseStream()))
                {
                    var responseString = reader.ReadToEnd();
                    deserializeResponse = JsonConvert.DeserializeObject(responseString);
                }
            }
            return deserializeResponse;
        }

        /// <summary>
        /// Helper Extension to convert UNIX/js time to human readable DateTime object.
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            if (unixTimeStamp.ToString().Length <= 11)
                dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            else
                dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public static double DateTimeToUnixTimeStamp(this DateTime dateTime)
        {
            DateTime zeroDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Math.Floor((dateTime - zeroDateTime).TotalMilliseconds / 1000);
        }
    }
}
