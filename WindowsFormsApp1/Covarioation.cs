using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    class Covarioation : ICloneable
    {
        public Covarioation()
        {

        }

        public Covarioation(MySqlDataReader reader)
        {
            this.isDateFromDb = true;

            Initialization(
                reader["ticker_A"].ToString(), 
                reader["ticker_B"].ToString(), 
                reader["value"].ToString(), 
                reader["period"].ToString(), 
                reader["date"].ToString(), 
                reader["time"].ToString(),
                reader["period_analize"].ToString(),                                    
                reader["id"].ToString());
        }

        public Covarioation(string tickerA, string tickerB, float value, string period, DateTime date,
                    DateTime time, int periodAnalize, int id = 0)
        {
            this.tickerA = tickerA;
            this.tickerB = tickerB;
            this.value = value;
            this.period = period;
            this.date = date;
            this.time = time;
            this.periodAnalize = periodAnalize;
            this.id = id;
        }

        public Covarioation(string tickerA, string tickerB, string value, string period, string date,
                    string time, string periodAnalize)
        {
            Initialization(tickerA, tickerB,value, period, date,
                     time,periodAnalize);
        }

        private void Initialization(string tickerA, string tickerB, string value, string period, string date,
                    string time, string periodAnalize, string id = "0")
        {
            this.tickerA = tickerA;
            this.tickerB = tickerB;
            this.period = period;

            if (isDateFromDb) { this.date = DateTime.Parse(date); }
            else
            {
                try { this.date = DateTime.ParseExact(date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture); }
                catch (Exception) { this.date = DateTime.MinValue; }
            }
            try { this.time = DateTime.ParseExact(time, "HHmmss", System.Globalization.CultureInfo.InvariantCulture); }
            catch (Exception) { this.time = DateTime.ParseExact("000000", "HHmmss", System.Globalization.CultureInfo.InvariantCulture); }

            try { this.value = Convert.ToSingle(value.Replace('.', ',')); }
            catch (Exception) { this.value = 0; }
            
            try { this.id = Convert.ToInt32(id); }
            catch (Exception) { this.id = 0; }

            try { this.periodAnalize = Convert.ToInt32(periodAnalize); }
            catch (Exception) { this.periodAnalize = 0; }
        }

        public object Clone()
        {
            return new Covarioation
            {
                id = this.id,
                tickerA = this.tickerA,
                tickerB = this.tickerB,
                value = this.value,
                period = this.period,
                date = this.date,
                time = this.time,
                periodAnalize=this.periodAnalize
            };
        }

        public int id { get; set; }
        public string tickerA { get; set; }
        public string tickerB { get; set; }
        public float value { get; set; }
        public string period { get; set; }
        public DateTime date { get; set; }
        public DateTime time { get; set; }
        public int periodAnalize { get; set; }

        private bool isDateFromDb { get; set; }
    }




}
