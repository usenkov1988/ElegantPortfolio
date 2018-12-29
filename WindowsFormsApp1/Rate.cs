using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    public class Rate : ICloneable
    {
        public Rate()
        {

        }

        public Rate(MySqlDataReader reader)
        {
            this.isDateFromDb = true;

            Initialization(reader["ticker"].ToString(), reader["period"].ToString(), reader["date"].ToString(), reader["time"].ToString(),
                                    reader["open"].ToString(), reader["high"].ToString(), reader["low"].ToString(),
                                    reader["close"].ToString(), reader["volume"].ToString(), reader["id"].ToString(),
                                    reader["_profitable"].ToString(), reader["_profitable_by_period"].ToString(),
                                    reader["_risk_by_period"].ToString(), reader["_x"].ToString());
        }

        public Rate(string ticker, string period, DateTime date,
                    DateTime time, float open, float high, float low,
                    float close, int volume, int id = 0, 
                    float _profitable = 0, float _profitable_by_period=0, 
                    float _risk_by_period=0, float _x=0)
        {
            this.ticker = ticker;
            this.period = period;
            this.date = date;
            this.time = time;
            this.open = open;
            this.high = high;
            this.low = low;
            this.close = close;
            this.volume = volume;
            this.id = id;
            this._profitable = _profitable;
            this._profitable_by_period = _profitable_by_period;
            this._risk_by_period = _risk_by_period;
            this._x = _x;
        }

        public Rate(string ticker, string period, string date,
                    string time, string open, string high, string low,
                    string close, string volume)
        {
            Initialization(ticker, period, date,
                     time, open, high, low,
                     close, volume);
        }

        private void Initialization(string ticker, string period, string date,
                    string time, string open, string high, string low,
                    string close, string volume,
                    string id = "0", string _profitable = "0", 
                    string _profitable_by_period = "0", string _risk_by_period = "0",
                    string _x="0")
        {
            this.ticker = ticker;
            this.period = period;

            if (isDateFromDb) { this.date = DateTime.Parse(date); }
            else
            {
                try { this.date = DateTime.ParseExact(date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture); }
                catch (Exception) { this.date = DateTime.MinValue; }
            }
            try { this.time = DateTime.ParseExact(time, "HHmmss", System.Globalization.CultureInfo.InvariantCulture); }
            catch (Exception) { this.time = DateTime.ParseExact("000000", "HHmmss", System.Globalization.CultureInfo.InvariantCulture); }

            try { this.open = Convert.ToSingle(open.Replace('.', ',')); }
            catch (Exception) { this.open = 0; }

            try { this.high = Convert.ToSingle(high.Replace('.', ',')); }
            catch (Exception) { this.high = 0; }

            try { this.low = Convert.ToSingle(low.Replace('.', ',')); }
            catch (Exception) { this.low = 0; }

            try { this.close = Convert.ToSingle(close.Replace('.', ',')); }
            catch (Exception) { this.close = 0; }

            try { this.volume = Convert.ToInt32(volume); }
            catch (Exception) { this.volume = 0; }

            try { this.id = Convert.ToInt32(id); }
            catch (Exception) { this.id = 0; }

            try { this._profitable = Convert.ToSingle(_profitable.Replace('.', ',')); }
            catch (Exception) { this._profitable = 0; }

            try { this._profitable_by_period = Convert.ToSingle(_profitable_by_period.Replace('.', ',')); }
            catch (Exception) { this._profitable_by_period = 0; }

            try { this._risk_by_period = Convert.ToSingle(_risk_by_period.Replace('.', ',')); }
            catch (Exception) { this._risk_by_period = 0; }

            try { this._x = Convert.ToSingle(_x.Replace('.', ',')); }
            catch (Exception) { this._x = 0; }
        }

        public object Clone()
        {
            return new Rate
            {
                ticker = this.ticker,
                period = this.period,
                date = this.date,
                time = this.time,
                open = this.open,
                high = this.high,
                low = this.low,
                close = this.close,
                volume = this.volume,
                id = this.id,
                _profitable = this._profitable,
                _profitable_by_period = this._profitable_by_period,
                _risk_by_period = this._risk_by_period,
                _x = this._x
            };
        }

        public int id { get; set; }
        public string ticker { get; set; }
        public string period { get; set; }
        public DateTime date { get; set; }
        public DateTime time { get; set; }
        public float open { get; set; }
        public float high { get; set; }
        public float low { get; set; }
        public float close { get; set; }
        public int volume { get; set; }

        public float _profitable { get; set; }
        public float _profitable_by_period { get; set; }
        public float _risk_by_period { get; set; }
        public float _x { get; set; }

        private bool isDateFromDb { get; set; }

    }




}
