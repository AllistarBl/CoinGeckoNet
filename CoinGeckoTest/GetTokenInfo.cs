using System;
using System.Collections.Generic;
using CoinGeckoApiExtensions;
using Newtonsoft.Json.Linq;

namespace CoinGeckoNet
{
    public static class GetTokenInfo
    {
        static string _rootUri = "https://api.coingecko.com/api/v3";

        private static dynamic SimpleListTokensApiCall()
        {
            var apiString = string.Format("{0}/coins/list", _rootUri);
            return apiString.GetSimpleApiResponse();
        }

        private static dynamic ExtensiveListTokensApiCall(string targetCurrency, int listCapacity, bool includeSparkline)
        {
            string sparkline = includeSparkline == true ? "true" : "false"; 
            var apiString = string.Format("{0}/coins/markets?vs_currency={1}&order=market_cap_desc&per_page={2}&page=1&sparkline={3}", 
                _rootUri, targetCurrency.ToLower().Trim(), listCapacity, includeSparkline.ToString().ToLower());
            return apiString.GetSimpleApiResponse();
        }

        private static dynamic SpecificTokenCallFromId(string id)
        {
            var apiString = string.Format("{0}/coins/{1}", _rootUri, id.ToLower().Trim());
            return apiString.GetSimpleApiResponse();
        }

        private static dynamic SpecificTokenCallFromId(string id, DateTime searchDate)
        {
            if (searchDate > DateTime.Now)
                searchDate = DateTime.Now;
            var searchDateString = string.Format("{0}-{1}-{2}", searchDate.Day, searchDate.Month, searchDate.Year);
            var apiString = string.Format("{0}/coins/{1}/history?date={2}", _rootUri, id.ToLower().Trim(), searchDateString);
            return apiString.GetSimpleApiResponse();
        }

        private static dynamic SpecificTokenHistoryCall(string id, string targetCurrency, int historyInDays, string interval)
        {
            var apiString = string.Format("{0}/coins/{1}/market_chart?vs_currency={2}&days={3}&interval={4}", 
                _rootUri, id.ToLower().Trim(), targetCurrency.ToLower().Trim(), historyInDays, interval);
            return apiString.GetSimpleApiResponse();
        }

        private static dynamic SpecificTokenHistoryCallWithSpan(string id, string targetCurrency, double unixStart, double unixEnd)
        {
            var apiString = string.Format("{0}/coins/{1}/market_chart/range?vs_currency={2}&from={3}&to={4}", 
                _rootUri, id.ToLower().Trim(), targetCurrency.ToLower().Trim(), unixStart, unixEnd);
            return apiString.GetSimpleApiResponse();
        }

        private static dynamic SpecificTokenPriceCall(string id, string targetCurrency, bool doReportUpdateTime)
        {
            var apiString = string.Format("{0}/simple/price?ids={1}&vs_currencies={2}&include_last_updated_at={3}",
                _rootUri.ToLower().Trim(), id.ToLower().Trim(), targetCurrency.ToLower().Trim(), doReportUpdateTime.ToString().ToLower());
            return apiString.GetSimpleApiResponse();
        }

        public struct TokenInfo
        {
            public string ID { get; set; }
            public string Symbol { get; set; }
            public string Name { get; set; }
            public string ImageURL { get; set; }
            public double? CurrentPrice { get; set; }
            public double? MarketCap { get; set; }
            public int MarketCapRank { get; set; }
            public double? FDV { get; set; }
            public double? TotalVolume { get; set; }
            public double? High24H { get; set; }
            public double? Low24H { get; set; }
            public double? PriceChange24H { get; set; }
            public double? MarketCapChange24H { get; set; }
            public double? MarketCapPercentChange24H { get; set; }
            public double? CirculatingSupply { get; set; }
            public double? TotalSupply { get; set; }
            public double? MaxSupply { get; set; }
            public double? Ath { get; set; }
            public double? AthChangePercentage { get; set; }
            public DateTime AthDate { get; set; }
            public double? Atl { get; set; }
            public double? AtlChangePercentage { get; set; }
            public DateTime AtlDate { get; set; }
            public DateTime LastUpdated { get; set; }
            public List<double?> PriceHistory7D { get; set; }
        }

        public struct HistoricTokenInfo
        {
            public string ID { get; set; }
            public string Symbol { get; set; }
            public string Name { get; set; }
            public string[] TargetCurrencies { get; set; }
            public Dictionary<string, double?> Price { get; set; }
            public Dictionary<string, double?> MarketCap { get; set; }
            public Dictionary<string, double?> Volume { get; set; }
        }

        private static TokenInfo GetTokenInfoFromList(dynamic jsonResponseLists, int index)
        {
            var thisTokenInfo = new TokenInfo();
            thisTokenInfo.ID = (string)jsonResponseLists[index].id;
            thisTokenInfo.Symbol = (string)jsonResponseLists[index].symbol;
            thisTokenInfo.Name = (string)jsonResponseLists[index].name;
            thisTokenInfo.ImageURL = (string)jsonResponseLists[index].image;
            thisTokenInfo.CurrentPrice = (double?)jsonResponseLists[index].current_price;
            thisTokenInfo.MarketCap = (double?)jsonResponseLists[index].market_cap;
            thisTokenInfo.MarketCapRank = (int)jsonResponseLists[index].market_cap_rank;
            thisTokenInfo.FDV = jsonResponseLists[index].fully_diluted_valuation != null ? (double?)jsonResponseLists[index].fully_diluted_valuation : 0;
            thisTokenInfo.TotalVolume = (double?)jsonResponseLists[index].total_volume;
            thisTokenInfo.High24H = (double?)jsonResponseLists[index].high_24h;
            thisTokenInfo.Low24H = (double?)jsonResponseLists[index].low_24h;
            thisTokenInfo.PriceChange24H = (double?)jsonResponseLists[index].price_change_24h;
            thisTokenInfo.MarketCapChange24H = (double?)jsonResponseLists[index].market_cap_change_24h;
            thisTokenInfo.MarketCapPercentChange24H = (double?)jsonResponseLists[index].market_cap_change_percentage_24h;
            thisTokenInfo.CirculatingSupply = (double?)jsonResponseLists[index].circulating_supply;
            thisTokenInfo.TotalSupply = (double?)jsonResponseLists[index].total_supply;
            thisTokenInfo.MaxSupply = (double?)jsonResponseLists[index].max_supply;
            thisTokenInfo.Ath = (double?)jsonResponseLists[index].ath;
            thisTokenInfo.AthChangePercentage = (double?)jsonResponseLists[index].ath_change_percentage;
            thisTokenInfo.AthDate = (DateTime)jsonResponseLists[index].ath_date;
            thisTokenInfo.Atl = (double?)jsonResponseLists[index].atl;
            thisTokenInfo.AtlChangePercentage = (double?)jsonResponseLists[index].atl_change_percentage;
            thisTokenInfo.AtlDate = (DateTime)jsonResponseLists[index].atl_date;
            thisTokenInfo.LastUpdated = (DateTime)jsonResponseLists[index].last_updated;
            thisTokenInfo.PriceHistory7D = jsonResponseLists[index].sparkline_in_7d.GetValue("price").ToObject(new List<double?>().GetType());
            return thisTokenInfo;
        }

        /// <summary>
        /// Query the CoinGecko API for details relating to the top 100 Coins by Market Cap.
        /// </summary>
        /// <returns>Detailed TokenInfo List</returns>
        public static List<TokenInfo> GetTokenInfoList()
        {
            var infoLists = new List<TokenInfo>();
            var requestDatas = ExtensiveListTokensApiCall("usd", 100, true);
            for (var i = 0; i < requestDatas.Count; i++)
            {
                var mInfo = GetTokenInfoFromList(requestDatas, i);
                infoLists.Add(mInfo);
            }
            return infoLists;
        }

        /// <summary>
        /// Query the CoinGecko API for details relating to the top 100 Coins by Market Cap.
        /// </summary>
        /// <param name="targetCurrency">
        /// Specify the Target Currency - USD, USDC, BTC, etc.
        /// </param>
        /// <returns>Detailed TokenInfo List</returns>
        public static List<TokenInfo> GetTokenInfoList(string targetCurrency)
        {
            var infoLists = new List<TokenInfo>();
            var requestDatas = ExtensiveListTokensApiCall(targetCurrency.ToLower().Trim(), 100, true);
            for (var i = 0; i < requestDatas.Count; i++)
            {
                var mInfo = GetTokenInfoFromList(requestDatas, i);
                infoLists.Add(mInfo);
            }
            return infoLists;
        }

        /// <summary>
        /// Query the CoinGecko API for details relating to top tokens. Sorted by Market Cap.
        /// </summary>
        /// <param name="targetCurrency">
        /// Specify the Target Currency - USD, USDC, BTC, etc.
        /// </param>
        ///  /// <param name="listCapacity">
        /// Specify how many tokens you want included in the list. Maximum of 250.
        /// </param>
        /// <returns>Detailed TokenInfo List</returns>
        public static List<TokenInfo> GetTokenInfoList(string targetCurrency, int listCapacity)
        {
            var infoLists = new List<TokenInfo>();
            var requestDatas = ExtensiveListTokensApiCall(targetCurrency.ToLower().Trim(), listCapacity, true);
            for (var i = 0; i < requestDatas.Count; i++)
            {
                var mInfo = GetTokenInfoFromList(requestDatas, i);
                infoLists.Add(mInfo);
            }
            return infoLists;
        }

        /// <summary>
        /// Query the CoinGecko API for details relating to top tokens. Sorted by Market Cap.
        /// </summary>
        /// <param name="targetCurrency">
        /// Specify the Target Currency - USD, USDC, BTC, etc.
        /// </param>
        ///  /// <param name="listCapacity">
        /// Specify how many tokens you want included in the list. Maximum of 250.
        /// </param>
        /// <param name="includeSparkline">
        /// Setting to True returns a list of seven day price history.
        /// </param>
        /// <returns>Detailed TokenInfo List</returns>
        public static List<TokenInfo> GetTokenInfoList(string targetCurrency, int listCapacity, bool includeSparkline)
        {
            var infoLists = new List<TokenInfo>();
            var requestDatas = ExtensiveListTokensApiCall(targetCurrency.ToLower().Trim(), listCapacity, includeSparkline);
            for (var i = 0; i < requestDatas.Count; i++)
            {
                var mInfo = GetTokenInfoFromList(requestDatas, i);
                infoLists.Add(mInfo);
            }
            return infoLists;
        }
        /// <summary>
        /// Queries CoinGecko API for a simple list of all known tokens.
        /// </summary>
        /// <returns>Token Name, ID and Symbol</returns>
        public static List<TokenInfo> GetAllKnownTokenNames()
        {
            var infoLists = new List<TokenInfo>();
            var requestDatas = SimpleListTokensApiCall();
            for (var i = 0; i < requestDatas.Count; i++)
            {
                var t = new TokenInfo();
                t.Name = requestDatas[i].name;
                t.ID = requestDatas[i].id;
                t.Symbol = requestDatas[i].symbol;
                infoLists.Add(t);
            }
            return infoLists;
        }

        /// <summary>
        /// Queries CoinGecko API for specific token Price, Volume and Market Cap data from a specific daily close.
        /// </summary>
        /// <param name="id">Token ID</param>
        /// <param name="date">Date to Search</param>
        /// <param name="targetCurrencies">Currency to measure against Token</param>
        /// <returns></returns>
        public static HistoricTokenInfo GetClosingPriceFromDate(string id, DateTime date, string[] targetCurrencies)
        {
            var historicInfoList = new HistoricTokenInfo();
            var requestDatas = SpecificTokenCallFromId(id, date);
            if (requestDatas.market_data != null)
            {
                historicInfoList.ID = requestDatas.id;
                historicInfoList.Name = requestDatas.name;
                historicInfoList.Symbol = requestDatas.symbol;
                var priceDict = new Dictionary<string, double?>();
                var marketCapDict = new Dictionary<string, double?>();
                var volumeDict = new Dictionary<string, double?>();
                foreach (var uTargetCurrency in targetCurrencies)
                {
                    var targetCurrency = uTargetCurrency.ToLower().Trim();
                    if (!priceDict.ContainsKey(targetCurrency))
                    { 
                        var price = (double)requestDatas.market_data.current_price.GetValue(targetCurrency);
                        priceDict.Add(targetCurrency, price);
                    }
                    if (!marketCapDict.ContainsKey(targetCurrency))
                    {
                        var marketCap = (double)requestDatas.market_data.market_cap.GetValue(targetCurrency);
                        marketCapDict.Add(targetCurrency, marketCap);
                    }
                    if (!volumeDict.ContainsKey(targetCurrency))
                    {
                        var totalVolume = (double)requestDatas.market_data.total_volume.GetValue(targetCurrency);
                        volumeDict.Add(targetCurrency, totalVolume);
                    }
                }
                historicInfoList.Price = priceDict;
                historicInfoList.MarketCap = marketCapDict;
                historicInfoList.Volume = volumeDict;
                return historicInfoList;
            }
            else
            {
                Console.WriteLine("There was an error retrieving data for this date");
                return historicInfoList;
            }
        }

        /// <summary>
        /// Get historical market data include price, market cap, and 24h volume. The granularity can be fine tuned but is automatically reduced when the data interval exceeds the threshold.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="targetCurrency">The target currency of market data (usd, eur, jpy, etc.). Default is 'usd'.</param>
        /// <param name="daySpan">Data up to number of days ago (eg. 1, 14, 30, max).</param>
        /// <param name="dataInterval">Data interval. Possible value: daily, hourly.</param>
        /// <returns></returns>
        public static Dictionary<string, List<Tuple<DateTime, double>>> GetDatePricePairs(this TokenInfo token, string targetCurrency, int daySpan, string dataInterval)
        {
            var d = new Dictionary<string, List<Tuple<DateTime, double>>>();
            var tokenId = token.ID;
            var requestString = SpecificTokenHistoryCall(tokenId, targetCurrency.ToLower().Trim(), daySpan, dataInterval);
            var prices = requestString.prices;
            var marketCaps = requestString.market_caps;
            var volumes = requestString.volumes;
            var priceLists = new List<Tuple<DateTime, double>>();
            var marketCapLists = new List<Tuple<DateTime, double>>();
            var volumeLists = new List<Tuple<DateTime, double>>();
            for (var i = 0; i < prices.Count; i++)
            {
                var thisPricePair = prices[i];
                var thisMarketCap = marketCaps[i];
                var thisVolume = volumes[i];
                var unixPriceDate = ((double)thisPricePair[i][0]).UnixTimeStampToDateTime();
                var price = (double)thisPricePair[i][1];
                var unixMarketCapDate = ((double)thisMarketCap[i][0]).UnixTimeStampToDateTime();
                var marketCap = (double)thisMarketCap[i][1];
                var unixVolumeDate = ((double)thisVolume[i][0]).UnixTimeStampToDateTime();
                var volume = (double)thisVolume[i][1];
                priceLists.Add(new Tuple<DateTime, double>(unixPriceDate, price));
                marketCapLists.Add(new Tuple<DateTime, double>(unixMarketCapDate, marketCap));
                volumeLists.Add(new Tuple<DateTime, double>(unixVolumeDate, volume));
            }
            d.Add("Price", priceLists);
            d.Add("Market Cap", marketCapLists);
            d.Add("Volume", volumeLists);
            return d;
        }

        /// <summary>
        /// Get historical market data include price, market cap, and 24h volume. The granularity can be fine tuned but is automatically reduced when the data interval exceeds the threshold.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="startDate">The date from which to start collecting data.</param>
        /// <param name="endDate">The last date from which to collect data.</param>
        /// <returns></returns>
        public static Dictionary<string, List<Tuple<DateTime, double>>> GetDatePricePairs(this TokenInfo token, DateTime startDate, DateTime endDate)
        {
            var d = new Dictionary<string, List<Tuple<DateTime, double>>>();
            var tokenId = token.ID;
            var startUnixTime = startDate.DateTimeToUnixTimeStamp();
            var endUnixTime = endDate.DateTimeToUnixTimeStamp();
            var requestString = SpecificTokenHistoryCallWithSpan(tokenId, "usd", startUnixTime, endUnixTime);
            var prices = requestString.prices;
            var marketCaps = requestString.market_caps;
            var volumes = requestString.total_volumes;
            var priceLists = new List<Tuple<DateTime, double>>();
            var marketCapLists = new List<Tuple<DateTime, double>>();
            var volumeLists = new List<Tuple<DateTime, double>>();
            for (var i = 0; i < prices.Count; i++)
            {
                var thisPricePair = prices[i];
                var thisMarketCap = marketCaps[i];
                var thisVolume = volumes[i];
                var unixPriceDate = ((double)thisPricePair[0]).UnixTimeStampToDateTime();
                var price = (double)thisPricePair[1];
                var unixMarketCapDate = ((double)thisMarketCap[0]).UnixTimeStampToDateTime();
                var marketCap = (double)thisMarketCap[1];
                var unixVolumeDate = ((double)thisVolume[0]).UnixTimeStampToDateTime();
                var volume = (double)thisVolume[1];
                priceLists.Add(new Tuple<DateTime, double>(unixPriceDate, price));
                marketCapLists.Add(new Tuple<DateTime, double>(unixMarketCapDate, marketCap));
                volumeLists.Add(new Tuple<DateTime, double>(unixVolumeDate, volume));
            }
            d.Add("Price", priceLists);
            d.Add("Market Cap", marketCapLists);
            d.Add("Volume", volumeLists);
            return d;
        }

        /// <summary>
        /// Get historical market data include price, market cap, and 24h volume. The granularity can be fine tuned but is automatically reduced when the data interval exceeds the threshold.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="startDate">The date from which to start collecting data.</param>
        /// <param name="endDate">The last date from which to collect data.</param>
        /// <returns></returns>
        public static Dictionary<string, List<Tuple<DateTime, double>>> GetDatePricePairs(this TokenInfo token, DateTime startDate, DateTime endDate, string targetId)
        {
            var d = new Dictionary<string, List<Tuple<DateTime, double>>>();
            var tokenId = token.ID;
            var startUnixTime = startDate.DateTimeToUnixTimeStamp();
            var endUnixTime = endDate.DateTimeToUnixTimeStamp();
            var requestString = SpecificTokenHistoryCallWithSpan(tokenId, targetId, startUnixTime, endUnixTime);
            var prices = requestString.prices;
            var marketCaps = requestString.market_caps;
            var volumes = requestString.volumes;
            var priceLists = new List<Tuple<DateTime, double>>();
            var marketCapLists = new List<Tuple<DateTime, double>>();
            var volumeLists = new List<Tuple<DateTime, double>>();
            for (var i = 0; i < prices.Count; i++)
            {
                var thisPricePair = prices[i];
                var thisMarketCap = marketCaps[i];
                var thisVolume = volumes[i];
                var unixPriceDate = ((double)thisPricePair[i][0]).UnixTimeStampToDateTime();
                var price = (double)thisPricePair[i][1];
                var unixMarketCapDate = ((double)thisMarketCap[i][0]).UnixTimeStampToDateTime();
                var marketCap = (double)thisMarketCap[i][1];
                var unixVolumeDate = ((double)thisVolume[i][0]).UnixTimeStampToDateTime();
                var volume = (double)thisVolume[i][1];
                priceLists.Add(new Tuple<DateTime, double>(unixPriceDate, price));
                marketCapLists.Add(new Tuple<DateTime, double>(unixMarketCapDate, marketCap));
                volumeLists.Add(new Tuple<DateTime, double>(unixVolumeDate, volume));
            }
            d.Add("Price", priceLists);
            d.Add("Market Cap", marketCapLists);
            d.Add("Volume", volumeLists);
            return d;
        }

        /// <summary>
        /// Queries the CoinGecko API for the most updated price data for the specified token pair. 
        /// </summary>
        /// <param name="tokenId">Token ID</param>
        /// <param name="targetId">The currency against which the token is being measured.</param>
        /// <param name="doReportUpdateTime">If set to true this will return a DateTime object that indicates the time at which the price feed was last updated.</param>
        /// <returns>A TokenInfo object that contains simple price data. Note that the current price will be in terms of the specified target pair.</returns>
        public static TokenInfo GetPriceData(string tokenId, string targetId, bool doReportUpdateTime)
        {
            var priceData = SpecificTokenPriceCall(tokenId, targetId, doReportUpdateTime);
            var t = new TokenInfo();
            t.ID = tokenId.ToLower().Trim();
            var thisPrice = priceData.GetValue(tokenId.ToLower().Trim())
                .GetValue(targetId).ToObject(0d.GetType());
            t.CurrentPrice = (double?)thisPrice;
            DateTime? updateTime = doReportUpdateTime ? 
                t.LastUpdated = ((double)priceData.GetValue(tokenId.ToLower().Trim()).GetValue("last_updated_at")).UnixTimeStampToDateTime() :
                null;
            return t;
        }
    }
}
