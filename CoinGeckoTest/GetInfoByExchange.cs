using System;
using System.Linq;
using System.Collections.Generic;
using CoinGeckoApiExtensions;

namespace CoinGeckoNet
{
    /// <summary>
    /// Helper Class to query exchanges CoinGecko Exchange API Data. Special consideration needs to be made for Spot, Derivatives and DEX's
    /// </summary>
    public static class GetInfoByExchange
    {
        private const string _rootUri = "https://api.coingecko.com/api/v3";

        /// <summary>
        /// ExchangeInfo Struct provides basic information to reference for more advanced queries. ID and Name are both strings that assist in downstream queries. 
        /// </summary>
        public struct ExchangeInfo
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// DerivativesExchangeInfo Struct provides basic information to reference for more advanced derivatives exchange queries. ID and Name are both strings that assist in downstream queries. 
        /// </summary>
        public struct DerivativesExchangeInfo
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public double? OpenInterestBTC { get; set; }
            public double? TradeVolumeBTC24H { get; set; }
            public int? NumberPerpPairs { get; set; }
            public int? NumberFuturesPairs { get; set; }
            public string URL { get; set; }
            public int? YearEstablished { get; set; }
        }

        /// <summary>
        /// DetailedDerivativesExchangeData provides a more detailed evaluation of relevant derivatives exchange metrics.
        /// </summary>
        public struct DetailedDerivativesExchangeData
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public double? OpenInterestBTC { get; set; }
            public double? TradeVolumeBTC24H { get; set; }
            public int? NumberPerpPairs { get; set; }
            public int? NumberFuturesPairs { get; set; }
            public int? YearEstablished { get; set; }
            public List<DerivativesTickerInfoCollection> DerivativesTickerInfo { get; set; }
        }

        /// <summary>
        /// The DerivativesTickerInfoCollection structure provides details about specific derivatives markets within an exchange.
        /// </summary>
        public struct DerivativesTickerInfoCollection
        {
            public string Symbol { get; set; }
            public string Base { get; set; }
            public string Target { get; set; }
            public string TradeURL { get; set; }
            public string ContractType { get; set; }
            public double? PercentageChange24H { get; set; }
            public double? LastPrice { get; set; }
            public double? Volume24H { get; set; }
            public double? BTCPairPrice { get; set; }
            public double? ETHPairPrice { get; set; }
            public double? USDPairPrice { get; set; }
            public double? ConvertedVolumeBTC { get; set; }
            public double? ConvertedVolumeETH { get; set; }
            public double? ConvertedVolumeUSD { get; set; }
            public double? BidAskSpread { get; set; }
            public DateTime? LastTradeTime { get; set; }
            public DateTime? Expiration { get; set; }
            public double? IndexPrice { get; set; }
            public double? IndexBasisPercentage { get; set; }
            public double? FundingRate { get; set; }
        }

        /// <summary>
        /// DetailedExchangeData provides a more detailed evaluation of relevant spot exchange metrics.
        /// </summary>
        public struct DetailedExchangeData
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public bool Centralized { get; set; }
            public int? TrustScore { get; set; }
            public int? TrustScoreRank { get; set; }
            public double? TradeVolumeBTC24H { get; set; }
            public List<TickerInfoCollection> TickerInfo { get; set; }
        }

        /// <summary>
        /// The TickerInfoCollection structure provides details about specific spot markets within an exchange.
        /// </summary>
        public struct TickerInfoCollection
        {
            public string Base { get; set; }
            public string Target { get; set; }
            public double? LastPrice { get; set; }
            public double? VolumeUSD { get; set; }
            public double? BTCPairPrice { get; set; }
            public double? ETHPairPrice { get; set; }
            public double? USDPairPrice { get; set; }
            public double? ConvertedVolumeBTC { get; set; }
            public double? ConvertedVolumeETH { get; set; }
            public double? ConvertedVolumeUSD { get; set; }
            public double? BidAskSpreadPercentage { get; set; }
            public DateTime? LastTradeTime { get; set; }
            public DateTime? Timestamp { get; set; }
            public string CoinID { get; set; }
        }

        /// <summary>
        /// Provides the option to specify the order in which derivatives exchange results are returned.
        /// </summary>
        public enum DerivativesExchangesSortOrder
        {
            None,
            DescendingByOI,
            DescendingByVolume,
            AscendingByOI,
            AscendingByVolume
        }

        ///<summary>
        ///Queries the CoinGecko API to return a simple list of exchange names and ID's.  More queries can be performed upon this list.
        ///</summary>
        public static List<ExchangeInfo> GetExchangeList()
        {
            var exchangeInfoLists = new List<ExchangeInfo>();
            try
            {
                var apiResponse = string.Format("{0}/{1}", _rootUri, "exchanges/list").GetSimpleApiResponse();

                for (var i = 0; i < apiResponse.Count; i++)
                {
                    var t = new ExchangeInfo();
                    t.ID = apiResponse[i].id;
                    t.Name = apiResponse[i].name;
                    exchangeInfoLists.Add(t);
                }
                return exchangeInfoLists;
            }
            catch (Exception responseFail)
            {
                // Console.WriteLine("{0} - API response failure -> Check Internet Connection or Coin Gecko API status.", DateTime.Now);
                return exchangeInfoLists;
            }
        }

        /// <summary>
        /// Sends query the CoinGecko API to get high level information about derivatives exchanges.
        /// </summary>
        public static List<DerivativesExchangeInfo> GetDerivativesExchangeList()
        {
            var exchangeInfoLists = new List<DerivativesExchangeInfo>();
            try
            {
                var apiResponse = string.Format("{0}/derivatives/exchanges", _rootUri).GetSimpleApiResponse();

                for (var i = 0; i < apiResponse.Count; i++)
                {
                    var t = new DerivativesExchangeInfo();
                    t.ID = apiResponse[i].id;
                    t.Name = apiResponse[i].name;
                    t.NumberFuturesPairs = apiResponse[i].number_of_futures_pairs;
                    t.NumberPerpPairs = apiResponse[i].number_of_perpetual_pairs;
                    t.OpenInterestBTC = apiResponse[i].open_interest_btc;
                    t.TradeVolumeBTC24H = apiResponse[i].trade_volume_24h_btc;
                    t.URL = apiResponse[i].url;
                    t.YearEstablished = apiResponse[i].year_established;
                    exchangeInfoLists.Add(t);
                }
                return exchangeInfoLists;
            }
            catch (Exception responseFail)
            {
                // Console.WriteLine("{0} - API response failure -> Check Internet Connection or Coin Gecko API status.", DateTime.Now);
                return exchangeInfoLists;
            }
        }

        public static string GetSortOrderApiString(this DerivativesExchangesSortOrder order)
        {
            if (order == DerivativesExchangesSortOrder.None)
                return "";
            else if (order == DerivativesExchangesSortOrder.AscendingByOI)
                return "?order=open_interest_btc_asc";
            else if (order == DerivativesExchangesSortOrder.AscendingByVolume)
                return "?order=trade_volume_24h_btc_asc";
            else if (order == DerivativesExchangesSortOrder.DescendingByOI)
                return "?order=open_interest_btc_desc";
            else if (order == DerivativesExchangesSortOrder.DescendingByVolume)
                return "?order=trade_volume_24h_btc_desc";
            else
                return "";
        }

        /// <summary>
        /// Sends query the CoinGecko API to get high level information about derivatives exchanges.
        /// </summary>
        /// <param name="order">Optional - Argument used to order DerivativesExchangeInfo results by preferred method.</param>
        /// <returns></returns>
        public static List<DerivativesExchangeInfo> GetDerivativesExchangeList(DerivativesExchangesSortOrder order)
        {           
            var exchangeInfoLists = new List<DerivativesExchangeInfo>();
            try
            {
                var apiResponse = string.Format("{0}/derivatives/exchanges{1}", _rootUri, order.GetSortOrderApiString()).GetSimpleApiResponse();

                for (var i = 0; i < apiResponse.Count; i++)
                {
                    var t = new DerivativesExchangeInfo();
                    t.ID = apiResponse[i].id;
                    t.Name = apiResponse[i].name;
                    t.NumberFuturesPairs = apiResponse[i].number_of_futures_pairs;
                    t.NumberPerpPairs = apiResponse[i].number_of_perpetual_pairs;
                    t.OpenInterestBTC = apiResponse[i].open_interest_btc;
                    t.TradeVolumeBTC24H = apiResponse[i].trade_volume_24h_btc;
                    t.URL = apiResponse[i].url;
                    t.YearEstablished = apiResponse[i].year_established;
                    exchangeInfoLists.Add(t);
                }
                return exchangeInfoLists;
            }
            catch (Exception responseFail)
            {
                // Console.WriteLine("{0} - API response failure -> Check Internet Connection or Coin Gecko API status.", DateTime.Now);
                return exchangeInfoLists;
            }
        }

        ///<summary>
        ///Queries the CoinGecko API to return a DetailedDerivativesExchangeData object that includes the exchange name, URL, volume and DerivativesTickerInfoCollection.
        ///</summary>
        ///<param name="exchangeId">
        ///The exchange ID string that corresponds to the exchange you are searching.
        /// </param>
        public static DetailedDerivativesExchangeData GetDetailedDerivativesExchangeData(string exchangeId)
        {
            var detailedExchangeData = new DetailedDerivativesExchangeData();
            var apiResponse = string.Format("{0}/derivatives/exchanges/{1}?include_tickers=%5B'all'%2C%20'unexpired'%5D", _rootUri, exchangeId).GetSimpleApiResponse();
            detailedExchangeData.Name = apiResponse.name;
            detailedExchangeData.Url = apiResponse.url;
            detailedExchangeData.NumberFuturesPairs = apiResponse.number_of_futures_pairs;
            detailedExchangeData.NumberPerpPairs = apiResponse.number_of_perpetual_pairs;
            detailedExchangeData.YearEstablished = apiResponse.year_established;
            detailedExchangeData.TradeVolumeBTC24H = apiResponse.trade_volume_24h_btc;
            var listTickerInfos = new List<DerivativesTickerInfoCollection>();
            var tickerData = apiResponse.tickers;
            for (var i = 0; i < tickerData.Count; i++)
            {
                DerivativesTickerInfoCollection t = new DerivativesTickerInfoCollection();
                t.Symbol = tickerData[i].symbol;
                t.Base = tickerData[i]["base"];
                t.Target = tickerData[i].target;
                t.TradeURL = tickerData[i].trade_url;
                t.ContractType = tickerData[i].contract_type;
                t.LastPrice = tickerData[i].last;
                t.BTCPairPrice = tickerData[i].converted_last.btc;
                t.ETHPairPrice = tickerData[i].converted_last.eth;
                t.USDPairPrice = tickerData[i].converted_last.usd;
                t.ConvertedVolumeBTC = tickerData[i].converted_volume.btc;
                t.ConvertedVolumeETH = tickerData[i].converted_volume.eth;
                t.ConvertedVolumeUSD = tickerData[i].converted_volume.usd;
                t.BidAskSpread = tickerData[i].bid_ask_spread;
                var lastTradeTimeUnixTime = (double?)tickerData[i].last_traded_at;
                t.LastTradeTime = lastTradeTimeUnixTime == null ? null : ((double)lastTradeTimeUnixTime).UnixTimeStampToDateTime();
                var expiryUnixTime = (double?)tickerData[i]["expired_at"];
                t.Expiration = expiryUnixTime == null ? null : ((double)expiryUnixTime).UnixTimeStampToDateTime();
                t.IndexPrice = tickerData[i].index;
                t.IndexBasisPercentage = tickerData[i].index_basis_percentage;
                t.FundingRate = tickerData[i].funding_rate;
                listTickerInfos.Add(t);
            }
            double sd = 10;
            sd.UnixTimeStampToDateTime();
            detailedExchangeData.DerivativesTickerInfo = listTickerInfos;
            return detailedExchangeData;
        }
        /// <summary>
        /// Queries the CoinGecko API to return a List DetailedDerivativesExchangeData objects that includes the exchange name, URL, volume and DerivativesTickerInfoCollection.
        /// </summary>
        ///<param name="exchangeIds">
        ///The string ID's of the exchanges you want to query.
        /// </param>
        public static List<DetailedDerivativesExchangeData> GetDetailedDerivativesExchangeDataList(IEnumerable<string> exchangeIds)
        {
            var detailedDerivsExchangeDataList = new List<DetailedDerivativesExchangeData>();
            for (var j = 0; j < exchangeIds.Count(); j++)
            {
                var exchangeId = exchangeIds.ElementAt(j);
                var detailedExchangeData = new DetailedDerivativesExchangeData();
                var apiResponse = string.Format("{0}/derivatives/exchanges/{1}?include_tickers=%5B'all'%2C%20'unexpired'%5D", _rootUri, exchangeId).GetSimpleApiResponse();
                detailedExchangeData.Name = apiResponse.name;
                detailedExchangeData.Url = apiResponse.url;
                detailedExchangeData.NumberFuturesPairs = apiResponse.number_of_futures_pairs;
                detailedExchangeData.NumberPerpPairs = apiResponse.number_of_perpetual_pairs;
                detailedExchangeData.YearEstablished = apiResponse.year_established;
                detailedExchangeData.TradeVolumeBTC24H = apiResponse.trade_volume_24h_btc;
                var listTickerInfos = new List<DerivativesTickerInfoCollection>();
                var tickerData = apiResponse.tickers;
                for (var i = 0; i < tickerData.Count; i++)
                {
                    DerivativesTickerInfoCollection t = new DerivativesTickerInfoCollection();
                    t.Symbol = tickerData[i].symbol;
                    t.Base = tickerData[i]["base"];
                    t.Target = tickerData[i].target;
                    t.TradeURL = tickerData[i].trade_url;
                    t.ContractType = tickerData[i].contract_type;
                    t.LastPrice = tickerData[i].last;
                    t.BTCPairPrice = tickerData[i].converted_last.btc;
                    t.ETHPairPrice = tickerData[i].converted_last.eth;
                    t.USDPairPrice = tickerData[i].converted_last.usd;
                    t.ConvertedVolumeBTC = tickerData[i].converted_volume.btc;
                    t.ConvertedVolumeETH = tickerData[i].converted_volume.eth;
                    t.ConvertedVolumeUSD = tickerData[i].converted_volume.usd;
                    t.BidAskSpread = tickerData[i].bid_ask_spread;
                    var lastTradeTimeUnixTime = (double?)tickerData[i].last_traded_at;
                    t.LastTradeTime = lastTradeTimeUnixTime == null ? null : ((double)lastTradeTimeUnixTime).UnixTimeStampToDateTime();
                    var expiryUnixTime = (double?)tickerData[i]["expired_at"];
                    t.Expiration = expiryUnixTime == null ? null : ((double)expiryUnixTime).UnixTimeStampToDateTime();
                    t.IndexPrice = tickerData[i].index;
                    t.IndexBasisPercentage = tickerData[i].index_basis_percentage;
                    t.FundingRate = tickerData[i].funding_rate;
                    listTickerInfos.Add(t);
                }
                double sd = 10;
                sd.UnixTimeStampToDateTime();
                detailedExchangeData.DerivativesTickerInfo = listTickerInfos;
                detailedDerivsExchangeDataList.Add(detailedExchangeData);
            }
            return detailedDerivsExchangeDataList;
        }

        ///<summary>
        ///Queries the CoinGecko API to return a DetailedExchangeData object that includes the exchange name, URL, volume and TickerInfoCollection.
        ///</summary>
        ///<param name="exchangeId">
        ///The exchange ID string that corresponds to the exchange you are searching.
        /// </param>
        public static DetailedExchangeData GetDetailedExchangeData(string exchangeId)
        {
            var detailedExchangeData = new DetailedExchangeData();
            var apiResponse = string.Format("{0}/exchanges/{1}", _rootUri, exchangeId).GetSimpleApiResponse();
            detailedExchangeData.Name = apiResponse.name;
            detailedExchangeData.Url = apiResponse.url;
            detailedExchangeData.Centralized = apiResponse.centralized;
            detailedExchangeData.TrustScore = apiResponse.trust_score;
            detailedExchangeData.TrustScoreRank = apiResponse.trust_score_rank;
            detailedExchangeData.TradeVolumeBTC24H = apiResponse.trade_volume_24h_btc;
            var listTickerInfos = new List<TickerInfoCollection>();
            var tickerData = apiResponse.tickers;
            for (var i = 0; i < tickerData.Count; i++)
            {
                TickerInfoCollection t = new TickerInfoCollection();
                t.Base = tickerData[i]["base"];
                t.Target = tickerData[i].target;
                t.LastPrice = tickerData[i].last;
                t.VolumeUSD = tickerData[i].volume;
                t.BTCPairPrice = tickerData[i].converted_last.btc;
                t.ETHPairPrice = tickerData[i].converted_last.eth;
                t.USDPairPrice = tickerData[i].converted_last.usd;
                t.ConvertedVolumeBTC = tickerData[i].converted_volume.btc;
                t.ConvertedVolumeETH = tickerData[i].converted_volume.eth;
                t.ConvertedVolumeUSD = tickerData[i].converted_volume.usd;
                t.BidAskSpreadPercentage = tickerData[i].bid_ask_spread_percentage;
                t.LastTradeTime = tickerData[i].last_traded_at;
                t.Timestamp = tickerData[i].timestamp;
                t.CoinID = tickerData[i].coin_id;
                listTickerInfos.Add(t);
            }
            detailedExchangeData.TickerInfo = listTickerInfos;
            return detailedExchangeData;
        }

        ///<summary>
        ///Queries the CoinGecko API to return a List DetailedExchangeData objects that includes the exchange name, URL, volume and TickerInfoCollection.
        ///Filter exchanges selections with the exchangeIds argument.
        ///</summary>
        ///<param name="exchangeIds">
        ///The string ID's of the exchanges you want to query.
        ///</param>
        public static List<DetailedExchangeData> GetDetailedExchangeDataList(IEnumerable<string> exchangeIds)
        {
            var allRequestedExchangeDatas = new List<DetailedExchangeData>();
            for (var j = 0; j < exchangeIds.Count(); j++)
            {
                var exchangeId = exchangeIds.ElementAt(j);
                var detailedExchangeData = new DetailedExchangeData();
                var apiResponse = string.Format("{0}/exchanges/{1}", _rootUri, exchangeId).GetSimpleApiResponse();
                detailedExchangeData.Name = apiResponse.name;
                detailedExchangeData.Url = apiResponse.url;
                detailedExchangeData.Centralized = apiResponse.centralized;
                detailedExchangeData.TrustScore = apiResponse.trust_score;
                detailedExchangeData.TrustScoreRank = apiResponse.trust_score_rank;
                detailedExchangeData.TradeVolumeBTC24H = apiResponse.trade_volume_24h_btc;
                var listTickerInfos = new List<TickerInfoCollection>();
                var tickerData = apiResponse.tickers;
                for (var i = 0; i < tickerData.Count; i++)
                {
                    TickerInfoCollection t = new TickerInfoCollection();
                    t.Base = tickerData[i]["base"];
                    t.Target = tickerData[i].target;
                    t.LastPrice = tickerData[i].last;
                    t.VolumeUSD = tickerData[i].volume;
                    t.BTCPairPrice = tickerData[i].converted_last.btc;
                    t.ETHPairPrice = tickerData[i].converted_last.eth;
                    t.USDPairPrice = tickerData[i].converted_last.usd;
                    t.ConvertedVolumeBTC = tickerData[i].converted_volume.btc;
                    t.ConvertedVolumeETH = tickerData[i].converted_volume.eth;
                    t.ConvertedVolumeUSD = tickerData[i].converted_volume.usd;
                    t.BidAskSpreadPercentage = tickerData[i].bid_ask_spread_percentage;
                    t.LastTradeTime = tickerData[i].last_traded_at;
                    t.Timestamp = tickerData[i].timestamp;
                    t.CoinID = tickerData[i].coin_id;
                    listTickerInfos.Add(t);
                }
                detailedExchangeData.TickerInfo = listTickerInfos;
                allRequestedExchangeDatas.Add(detailedExchangeData);
            }
            return allRequestedExchangeDatas;
        }

        /// <summary>
        /// Extension to build spot price and volume dictionary based off the DetailedExchangeData List.
        /// </summary>
        /// <param name="detailedExchangeDatas"></param>
        /// <param name="cBase">Specify Base Currency - BTC, ETH, etc.</param>
        /// <param name="target">Specify Target Currency - USD, USDT, BTC, etc.</param>
        /// <returns></returns>
        public static Dictionary<string, Tuple<double?, double?>> GetSpotPairPriceAndVolume(this List<DetailedExchangeData> detailedExchangeDatas, string cBase, string target)
        {
            var getPriceAndVolume = detailedExchangeDatas
                .Select(x => new Tuple<double?, double?>(x.TradeVolumeBTC24H, x.TickerInfo
                    .Where(y => y.Base == cBase.ToUpper() && y.Target.StartsWith(target.ToUpper()))
                    .ToList().FirstOrDefault().LastPrice))
                .ToList();
            return detailedExchangeDatas.ToDictionary(x => x.Name, x => getPriceAndVolume.ElementAt(detailedExchangeDatas.IndexOf(x)));
        }
    }
}
