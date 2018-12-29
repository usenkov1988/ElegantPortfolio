using Microsoft.SolverFoundation.Services;
using Microsoft.SolverFoundation.Solvers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        List<string> _covariationQuery;

        string _DateFormat { get { return "yyyy-MM-dd"; } }
        string _TimeFormat { get { return "HH:mm:ss"; } }

        List<string> _Tickers { get; set; }
        // Спосок всех пар инструментов
        List<List<string>> _PairsTickers { get; set; }
        List<string> _Periods { get; set; }

        string _GetPeriod { get { return _cbPeriodLong.SelectedItem.ToString(); } }
        int _GetPeriodAnalize { get { return Convert.ToInt32(_tbPeriodAnalize.Text); } }


        public Form1()
        {
            InitializeComponent();
            Initialization();
        }

        private void Initialization()
        {
            _covariationQuery = new List<string>();
            _Tickers = new List<string>();
            _PairsTickers = new List<List<string>>();
            _Periods = new List<string>();

            // Настраиваем соединение с БД
            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();

                    // Вытягиваем из БД все доступные периоды в компонент для настройки
                    _cbPeriodLong.Items.Clear();
                    query.CommandText = "select distinct `period` from `rates`";
                    var reader = query.ExecuteReader();
                    while (reader.Read())
                    {
                        _Periods.Add(reader["period"].ToString());
                        _cbPeriodLong.Items.Add(reader["period"].ToString());
                    }
                    reader.Close();
                    _cbPeriodLong.SelectedIndex = 0;

                    // Вытягиваем из БД список инструментов
                    query.CommandText = "select distinct `ticker` from `rates`";
                    reader = query.ExecuteReader();
                    while (reader.Read())
                    {
                        _Tickers.Add(reader["ticker"].ToString());
                    }
                    reader.Close();

                    connection.Close();
                }
            }

            // Создаем заголовки матрицы, все возможные пары из имеющихся инструментов
            int i = 0;
            foreach (var tickerA in _Tickers)
            {
                for (i = _Tickers.IndexOf(tickerA); i < _Tickers.Count; i++)
                {
                    string[] pair = { tickerA, _Tickers[i] };
                    _PairsTickers.Add(new List<string>(pair));
                }

            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            using (var connection = new MySqlConnection("Server=localhost;Database=portfolio_db;Uid=root;Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();
                    query.CommandText = "SELECT COUNT(*) from rates where id=1";
                    var mysqlint = int.Parse(query.ExecuteScalar().ToString());

                    MessageBox.Show(mysqlint.ToString());

                    connection.Close();
                }


                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "select * from rates where id=1";

                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                        {
                            var d = reader.ToString();
                            textBox1.Text = reader.GetString(0) + ": " + reader.GetString(1);
                            //Console.WriteLine(reader.GetString(0) + ": " + reader.GetString(1));
                        }

                    connection.Close();
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "CSV файл|*.csv", Multiselect = true, ValidateNames = true })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {

                        foreach (string file_name in ofd.FileNames)
                        {
                            using (StreamReader sr = new StreamReader(file_name))
                            {
                                textBox1.Text = textBox1.Text + '\n' + (await sr.ReadToEndAsync());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            char[] dimension = { ';' }; // разделитель параметров в строке
            List<Rate> rates = new List<Rate>(); // коллекция котировок

            // Проходися по всем строкам текстового блока
            var lines = textBox1.Lines;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Split(dimension);
                // Если строка содержит 9 параметров (<TICKER>;<PER>;<DATE>;<TIME>;<OPEN>;<HIGH>;<LOW>;<CLOSE>;<VOL>)
                // то парсим ее.
                if (line.Length == 9)
                {
                    rates.Add(new Rate(line[0], line[1], line[2], line[3],
                        line[4], line[5], line[6], line[7], line[8]));
                }
            }

            // Настраиваем соединение с БД
            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();

                    // Проверяем каждую запись на наличие в БД

                    int all_count = 0; // счетчик числа итераций
                    int write_count = 0; // счетчик числа записанных данных
                    foreach (Rate rate in rates)
                    {
                        if (rate.date == DateTime.MinValue) { continue; }

                        all_count++;

                        query.CommandText =
                            "select count(*) " +
                            "from rates " +
                            "where ticker='" + rate.ticker + "'" +
                            "   AND date='" + rate.date.ToString(_DateFormat) + "'" +
                            "   AND time='" + rate.time.ToString(_TimeFormat) + "'";

                        var mysqlint = int.Parse(query.ExecuteScalar().ToString());

                        if (mysqlint == 0)
                        {
                            query.CommandText =
                                "insert into rates (ticker, period, date, time, open, high, low, close, volume) " +
                                "values ('" + rate.ticker +
                                "', '" + rate.period +
                                "', '" + rate.date.ToString(_DateFormat) +
                                "', '" + rate.time.ToString(_TimeFormat) +
                                "', '" + rate.open.ToString().Replace(',', '.') +
                                "', '" + rate.high.ToString().Replace(',', '.') +
                                "', '" + rate.low.ToString().Replace(',', '.') +
                                "', '" + rate.close.ToString().Replace(',', '.') +
                                "', '" + rate.volume.ToString() + "')";

                            query.ExecuteNonQuery();
                            write_count++;
                        }
                    }
                    connection.Close();

                    MessageBox.Show("Всего итераций: " + all_count + ", из них записано: " + write_count);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            Stopwatch swMSQ = new Stopwatch();
            Stopwatch swMDF = new Stopwatch();



            swMSQ.Start();
            var tickers = new List<string>();





            // Перерасчет доходности для каждой записи. 
            //  Для этого переберем каждую запись, найдем по дате предыдущую ей и по формуле:
            //  ДОХОДНОСТЬ_ЗА_ТЕКУЩИЙ_ПЕРИОД = (ЦЕНА_ТЕКУЩЕГО_ПЕРИОДА - ЦЕНА_ПРЕДЫДУЩЕГО_ПЕРИОДА) / ЦЕНА_ПРЕДЫДУЩЕГО_ПЕРИОДА
            //  все посчитаем и запишем в БД

            // Настраиваем соединение с БД
            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();

                    // Получаем список инструментов из БД
                    query.CommandText = "select distinct `ticker` from `rates`";
                    var reader = query.ExecuteReader();
                    while (reader.Read())
                    {
                        tickers.Add(reader["ticker"].ToString());
                    }
                    reader.Close();

                    // Учитывая каждый инструмент обрабатываем
                    foreach (var ticker in tickers)
                    {
                        // Получаем список периодов для инструмента из БД
                        /*var periods = new List<string>(); // периоды для данного инструмента
                        query.CommandText = "select distinct `period` from `rates` where `ticker`='" + ticker + "'";
                        reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            periods.Add(reader["period"].ToString());
                        }
                        reader.Close();*/
                        var periods = new List<string>(); // периоды для данного инструмента
                        periods.Add(_GetPeriod);

                        // Учитывая каждый период инструмента обрабатываем
                        foreach (var period in periods)
                        {
                            Rate rateCurrent = new Rate(); // текущая котировка
                            Rate ratePrevious = new Rate(); // предыдущая
                            int test = 0;

                            query.CommandText = "select * from `rates` where `date`=(select max(`date`) from `rates`) AND " +
                                "`ticker`= '" + ticker + "'" +
                                "AND `period`= '" + period + "'";
                            //"select distinct `period` from `rates` where `ticker`='" + ticker + "'";
                            reader = query.ExecuteReader();
                            while (reader.Read())
                            {
                                rateCurrent = new Rate(reader);
                            }
                            reader.Close();

                            // Начиная с самой старшей даты расчитываем доходность
                            while (true)
                            {
                                query.CommandText = "select * from `rates` " +
                                    "where `date` < '" + rateCurrent.date.ToString(_DateFormat) + "' " +
                                    "AND `ticker`= '" + ticker + "' " +
                                    "AND `period`= '" + period + "' " +
                                    " order by `date` desc limit 1";
                                reader = query.ExecuteReader();

                                // Если после запроса ответ пустой, то выходим из расчетки
                                if (!reader.HasRows)
                                {
                                    reader.Close();
                                    break;
                                }
                                test++;

                                while (reader.Read())
                                {
                                    ratePrevious = new Rate(reader);
                                    float profitable = (rateCurrent.close - ratePrevious.close) / ratePrevious.close;

                                    // Записываем расчетное число в БД
                                    query.CommandText = "update `rates` set `_profitable`='" + profitable.ToString().Replace(',', '.') + "'" +
                                        "where `id`='" + rateCurrent.id + "'";
                                    reader.Close();
                                    query.ExecuteNonQuery();

                                    rateCurrent = (Rate)ratePrevious.Clone();
                                    break;
                                }
                                reader.Close();
                            }
                        }
                    }

                    connection.Close();
                }
            }
            swMSQ.Stop();

            tickers.Clear();

            swMDF.Start();
            using (LinqToDBDataContext linqToDB = new LinqToDBDataContext())
            {
                tickers.Clear();
                // Получаем список инструментов из БД
                var query = (from rate in linqToDB.rates
                             select rate.ticker).Distinct();
                //var tickers = query.ToList();
                tickers.AddRange(query.ToList());

                foreach (var ticker in tickers)
                {
                    var currentDate = (from rate in linqToDB.rates
                                       where rate.date == (from d in linqToDB.rates select d.date).Max()
                                       && rate.ticker == ticker
                                       && rate.period == _GetPeriod
                                       select rate).First().date;

                    // Начиная с самой старшей даты расчитываем доходность
                    while (true)
                    {
                        try
                        {
                            var rateCurrent = (from rate in linqToDB.rates
                                               where rate.date == currentDate
                                               && rate.ticker == ticker
                                               && rate.period == _GetPeriod
                                               select rate).First();

                            var ratePrevious = (from rate in linqToDB.rates
                                                where rate.date < rateCurrent.date
                                                && rate.ticker == ticker
                                                && rate.period == _GetPeriod
                                                orderby rate.date descending
                                                select rate).First();

                            rateCurrent._profitable = (rateCurrent.close - ratePrevious.close) / ratePrevious.close;
                            linqToDB.SubmitChanges();
                            currentDate = ratePrevious.date;

                        }
                        // Если после запроса ответ пустой, то выходим из расчетки
                        catch (Exception) { break; }
                    }
                }
            }



            swMDF.Stop();


            label4.Text = "1: "+swMSQ.Elapsed.ToString()+"; 2: "+ swMDF.Elapsed.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Рассчитываем среднюю доходность и риск за установленный период анализа
            // Настраиваем соединение с БД
            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();

                    // Проходимся по всему списку инструментов
                    foreach (var ticker in _Tickers)
                    {
                        // Узнаем старшую дату от которой пойдем назад в прошлое
                        var lastRate = new Rate();

                        query.CommandText = "select * from `rates` where `ticker`='" + ticker + "'" +
                            " AND `period`='" + _GetPeriod + "' order by `date` desc limit 1";
                        var reader = query.ExecuteReader();
                        while (reader.Read()) { lastRate = new Rate(reader); }
                        reader.Close();

                        // Запускаем путешествие по котировкам назад во времени
                        while (true)
                        {
                            var ratesToCalc = new List<Rate>();
                            query.CommandText = "select * from `rates` where `date`<='" + lastRate.date.ToString(_DateFormat) + "'" +
                                " and `ticker`='" + ticker + "'" +
                                " and `period`='" + _GetPeriod + "'" +
                                " order by `date` desc limit " + _GetPeriodAnalize.ToString();
                            reader = query.ExecuteReader();

                            while (reader.Read()) { ratesToCalc.Add(new Rate(reader)); }
                            reader.Close();

                            // Если подошли к концу списка, то выходим из расчетки, переходим на следующий инструмент
                            if (ratesToCalc.Count() < _GetPeriodAnalize) { break; }

                            var data = new List<float>();
                            for (int i = 0; i < _GetPeriodAnalize; i++) { data.Add(ratesToCalc[i]._profitable); }

                            var profitable_by_period = data.Average();
                            var risk_by_period = StdDev(data);

                            // Записываем рассчитанные значения
                            query.CommandText = "update `rates` set `_profitable_by_period`='" + profitable_by_period.ToString().Replace(',', '.') + "', " +
                                "`_risk_by_period`= '" + risk_by_period.ToString().Replace(',', '.') + "' " +
                                "where `id`='" + lastRate.id + "'";
                            reader.Close();
                            query.ExecuteNonQuery();

                            // После пересчета, подвигаем дату 
                            lastRate = (Rate)ratesToCalc[1].Clone();
                        }
                    }

                    connection.Close();
                }
            }

            label7.Text = "работа завершена!";
            Console.Beep();
        }

        private float StdDev(IEnumerable<float> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return Convert.ToSingle(ret);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //var covList = new List<string>();

            int k = 2673;
            for (int i1 = k; i1 < _PairsTickers.Count; i1++)
            {
                List<string> pair = _PairsTickers[i1];
                //label10.Text = "Общитано пар: "+k.ToString()+"/"+ _PairsTickers.Count().ToString();
                //this.Refresh();
                k++; // для теста
                DateTime lastDateA = GetLastRateFromDB(pair[0], _GetPeriod).date;
                DateTime lastDateB = GetLastRateFromDB(pair[1], _GetPeriod).date;

                //DateTime lastCov = GetLastCovFromDB(pair[0], pair[1], _GetPeriod).date;
                // Получаем последнюю котировку для каждой пары

                //Синхронизация котировок
                bool isSynchronized = false;
                //bool isSynchronized = true;
                while (true)
                {
                    if (lastDateA < lastDateB)
                    {
                        var r = GetRatesFromDB(lastDateA, 1, pair[1], _GetPeriod)[0];
                        if (r != null) { lastDateB = r.date; }
                        else { break; }
                    }
                    else if (lastDateA > lastDateB)
                    {
                        var r = GetRatesFromDB(lastDateB, 1, pair[0], _GetPeriod)[0];
                        if (r != null) { lastDateB = r.date; }
                        else { break; }
                    }
                    else
                    {
                        isSynchronized = true;
                        break;
                    }
                }

                // Проходимся по всей истории и рассчитываем ковариации
                while (isSynchronized)
                {
                    //todo: вопрос синхронизации котировок между собой для пары не решен!!!

                    // Получаем массив для обоих пар, считаем ковариацию
                    List<Rate> ratesA = new List<Rate>();
                    List<Rate> ratesB = new List<Rate>();

                    var a = GetRatesFromDB(lastDateA, _GetPeriodAnalize, pair[0], _GetPeriod);
                    var b = GetRatesFromDB(lastDateB, _GetPeriodAnalize, pair[1], _GetPeriod);

                    if (a != null && b != null)
                    {
                        ratesA.AddRange(a);
                        ratesB.AddRange(b);

                        float result = 0;
                        float profitable_by_period_A = ratesA[0]._profitable_by_period;
                        float profitable_by_period_B = ratesB[0]._profitable_by_period;

                        for (int i = 0; i < _GetPeriodAnalize; i++)
                        {
                            result += (ratesA[i]._profitable - profitable_by_period_A) * (ratesB[i]._profitable - profitable_by_period_B);
                        }
                        result = result / (_GetPeriodAnalize - 1);

                        SetCovariationInDB(result, pair[0], pair[1], _GetPeriod, ratesA[0].date, _GetPeriodAnalize, _DateFormat);
                        //covList.Add(result + ";" + pair[0] + ";" + pair[1] + ";" + _GetPeriod + ";" + ratesA[0].date + ";" + _GetPeriodAnalize);

                        // сдвигаем даты на следующую строку
                        lastDateA = ratesA[1].date;
                        lastDateB = ratesB[1].date;
                    }
                    else { break; }
                }
            }

            /* using (TextWriter tw = new StreamWriter("SavedList.txt"))
             {
                 foreach (String s in covList)
                     tw.WriteLine(s);
             }*/
            /*
            // После завершения расчетов, запишем библиотеку
            string covTotal = "";
            foreach (var cov in _covariationQuery)
            {
                covTotal = covTotal + cov + ";";
            }

            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();

                    // Ищем запись о ковариации для нашего случая
                    query.CommandText = covTotal;
                    query.ExecuteNonQuery();
                }
                connection.Close();
            }
            */

            label5.Text = "работа завершена!";
            Console.Beep();
        }


        private void SetCovariationInDB(float value, string tickerA, string tickerB, string period,
                                        DateTime date, int periodAnalize, string dateFormat)
        {
            int updateCount = 0;
            int insertCount = 0;

            // Настраиваем соединение с БД
            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();

                    query.CommandText = "insert into covariations (ticker_A, ticker_B, value, period, date, period_analize) " +
                                                 "values ('" + tickerA + "', '" + tickerB + "', '" + value.ToString().Replace(',', '.') + "', '" +
                                                 period + "', '" + date.ToString(dateFormat) + "', '" +
                                                 periodAnalize.ToString() + "')";
                    query.ExecuteNonQuery();

                    // Ищем запись о ковариации для нашего случая
                    /*query.CommandText =
                    "INSERT INTO `covariations` (ticker_A, ticker_B, value, period, date, period_analize) " +
                    "SELECT '" + tickerA + "', '" +
                    tickerB + "', '" +
                    value.ToString().Replace(',', '.') + "', '" +
                    period + "', '" +
                    date.ToString(dateFormat) + "', '" +
                    periodAnalize.ToString() + "' " +
                    "FROM DUAL " +
                    "WHERE NOT EXISTS(" +
                    "   SELECT * FROM `covariations`  " +
                    "   WHERE ticker_A = 'test') " +
                    "LIMIT 1";*/

                    /*
                                 "select * from `covariations` " +
                                 "where `period`='" + period + "' " +
                                 "and `ticker_A`='" + tickerA + "' " +
                                 "and `ticker_B`='" + tickerB + "' " +
                                 "and `period_analize`='" + periodAnalize.ToString() + "' " +
                                 "and `date`='" + date.ToString(dateFormat) + "'";*/
                    query.ExecuteNonQuery();
                    /*
                                        var id = -1;
                                        float v = 0;
                                        while (reader.Read())
                                        {
                                            id = (int)reader["id"];
                                            v = (float)reader["value"];
                                        }
                                        reader.Close();

                                        if (v != value)
                                        {
                                            if (id > 0)
                                            {
                                                query.CommandText = "update `covariations`" +
                                                    "set `value`='" + value.ToString() + "'" +
                                                    " where `id`='" + id.ToString() + "'";
                                                query.ExecuteNonQuery();


                                                updateCount++; // для теста
                                            }
                                            else
                                            {
                                                query.CommandText = "insert into covariations (ticker_A, ticker_B, value, period, date, period_analize) " +
                                                                     "values ('" + tickerA + "', '" + tickerB + "', '" + value.ToString().Replace(',', '.') + "', '" +
                                                                     period + "', '" + date.ToString(dateFormat) + "', '" +
                                                                     periodAnalize.ToString() + "')";
                                                query.ExecuteNonQuery();

                                                insertCount++; // для теста
                                            }
                                        }*/



                }
                connection.Close();
            }
        }

        /** 
         * Функция получения данных из БД 
         */
        private List<Rate> GetRatesFromDB(DateTime dateStart, int rowCount,
                                            string ticker, string period,
                                            string dateFormat = "yyyy-MM-dd")
        {
            var result = new List<Rate>();

            // Настраиваем соединение с БД
            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();
                    query.CommandText = "select * from `rates` where `date`<='" + dateStart.ToString(dateFormat) + "'" +
                        " and `ticker`='" + ticker + "'" +
                        " and `period`='" + period + "'" +
                        " order by `date` desc limit " + rowCount.ToString();
                    var reader = query.ExecuteReader();

                    while (reader.Read()) { result.Add(new Rate(reader)); }
                    reader.Close();
                }

                connection.Close();
            }

            // В случае если сборка меньше заданного количества строк (rowCount)
            //      возвращаем null
            if (result.Count < rowCount) { return null; }

            return result;
        }


        /**
         * Получаем последнюю запись котировки из БД
         */
        private Rate GetLastRateFromDB(string ticker, string period)
        {
            var result = new Rate();

            // Настраиваем соединение с БД
            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();
                    query.CommandText = "select * from `rates` where `ticker`='" + ticker + "'" +
                            " AND `period`='" + period + "' order by `date` desc limit 1";
                    var reader = query.ExecuteReader();

                    while (reader.Read()) { result = new Rate(reader); }
                    reader.Close();
                }

                connection.Close();
            }

            return result;
        }

        private Covarioation GetLastCovFromDB(string tickerA, string tickerB, string period)
        {
            var result = new Covarioation();

            // Настраиваем соединение с БД
            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();
                    query.CommandText = "select * from `covariations` where `ticker_A`='" + tickerA + "'" +
                            " AND `ticker_B`='" + tickerB + "'" +
                            " AND `period`='" + period + "' order by `date` asc limit 1";
                    var reader = query.ExecuteReader();

                    while (reader.Read()) { result = new Covarioation(reader); }
                    reader.Close();
                }

                connection.Close();
            }

            return result;
        }

        public void SetRateToDB(Rate rate, string dateFormat, string timeFormat)
        {
            // Настраиваем соединение с БД
            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();

                    // Ищем запись о ковариации для нашего случая
                    query.CommandText =
                                 "select * from `rates` " +
                                 "where `id`='" + rate.id + "' " +
                                 "or (`ticker`='" + rate.ticker + "' " +
                                 "and `period`='" + rate.period + "' " +
                                 "and `date`='" + rate.date.ToString(dateFormat) + "')";
                    var reader = query.ExecuteReader();

                    var id = -1;
                    while (reader.Read()) { id = (int)reader["id"]; }
                    reader.Close();

                    if (id > 0)
                    {
                        query.CommandText = "update `rates`" +
                            "set `_x`='" + rate._x.ToString().Replace(',', '.') + "'" +
                            " where `id`='" + id.ToString() + "'";
                        query.ExecuteNonQuery();
                    }
                    else
                    {
                        query.CommandText = "insert into rates (ticker, period, date, time, open, high, low, close, volume, " +
                            "_profitable, _profitable_by_period, _risk_by_period, _x) " +
                                            "values ('" + rate.ticker + "', '" + rate.period + "', '" + rate.date.ToString(dateFormat) +
                                "', '" + rate.time.ToString(timeFormat) + "', '" + rate.open.ToString().Replace(',', '.') + "', '" + rate.high.ToString().Replace(',', '.') +
                                "', '" + rate.low.ToString().Replace(',', '.') + "', '" + rate.close.ToString().Replace(',', '.') + "', '" + rate.volume.ToString() +
                                 "', '" + rate._profitable.ToString().Replace(',', '.') + "', '" + rate._profitable_by_period.ToString().Replace(',', '.') +
                                  "', '" + rate._risk_by_period.ToString().Replace(',', '.') + "', '" + rate._x.ToString().Replace(',', '.') +
                                "')";

                        query.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }


        private void button7_Click(object sender, EventArgs e)
        {
            using (LinqToDBDataContext linqToDB = new LinqToDBDataContext())
            {
                // Получаем список инструментов из БД
                var query = (from rate in linqToDB.rates
                             select rate.ticker).Distinct();
                var tickers = query.ToList();

                foreach (var ticker in tickers)
                {
                    var currentDate = (from rate in linqToDB.rates
                                       where rate.date == (from d in linqToDB.rates select d.date).Max()
                                       && rate.ticker == ticker
                                       && rate.period == _GetPeriod
                                       select rate).First().date;

                    // Начиная с самой старшей даты расчитываем доходность
                    while (true)
                    {
                        try
                        {
                            var rateCurrent = (from rate in linqToDB.rates
                                               where rate.date == currentDate
                                               && rate.ticker == ticker
                                               && rate.period == _GetPeriod
                                               select rate).First();

                            var ratePrevious = (from rate in linqToDB.rates
                                                where rate.date < rateCurrent.date
                                                && rate.ticker == ticker
                                                && rate.period == _GetPeriod
                                                orderby rate.date descending
                                                select rate).First();

                            rateCurrent._profitable = (rateCurrent.close - ratePrevious.close) / ratePrevious.close;
                            linqToDB.SubmitChanges();
                            currentDate = ratePrevious.date;

                        }
                        // Если после запроса ответ пустой, то выходим из расчетки
                        catch (Exception) { break; }
                    }
                }
            }


            /*
            /// Create new stopwatch.
            Stopwatch swRead = new Stopwatch();
            Stopwatch swWrite = new Stopwatch();

            swRead.Start();
            var ratesMSQL = new List<Rate>();
            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();
                    query.CommandText = "select * from `rates`";
                    var reader = query.ExecuteReader();
                    while (reader.Read())
                    {
                        ratesMSQL.Add(new Rate(reader));
                    }
                    reader.Close();

                    connection.Close();
                }
            }
            swRead.Stop();

            swWrite.Start();
            int c = 0;
            using (LinqToDBDataContext linqToDB = new LinqToDBDataContext())
            {
                foreach (var r in ratesMSQL)
                {
                    rates rate = new rates()
                    {
                        id = r.id,
                        ticker = r.ticker,
                        period = r.period,
                        date = r.date,
                        time = r.time.TimeOfDay,
                        open = r.open,
                        high = r.high,
                        low = r.low,
                        close = r.close,
                        volume = r.volume,
                        _profitable = r._profitable,
                        _profitable_by_period = r._profitable_by_period,
                        _risk_by_period = r._risk_by_period,
                        _x = r._x
                    };

                    linqToDB.rates.InsertOnSubmit(rate);
                }
                    linqToDB.SubmitChanges();

                var query = from r in linqToDB.rates select c;
                c = query.ToList().Count;
            }
            swWrite.Stop();

            textBox2.Text= "Чтение из MySQL"+ swRead.Elapsed.ToString()+"\n";// для теста
            textBox2.Text+= "Запись в MDF"+ swWrite.Elapsed.ToString() + "\n";// для теста
            textBox2.Text+= "Записей в БД"+ c.ToString();// для теста
            */
        }

        private void button8_Click(object sender, EventArgs e)
        {/*
            using (LinqToDBDataContext linqToDB = new LinqToDBDataContext())
            {
                    rates rate = new rates()
                    {
                        //id = 1,
                        ticker = "test2"
                    };

                    linqToDB.rates.InsertOnSubmit(rate);
                    linqToDB.SubmitChanges();
                
            }*/
            using (LinqToDBDataContext linqToDB = new LinqToDBDataContext())
            {
                // Query for a specific customer.
                var cust =
                    (from c in linqToDB.rates
                     select c).First();

                // Change the name of the contact.
                cust.ticker = "New Contact";

                // Create and add a new Order to the Orders collection.
                rates ord = new rates { ticker = "two two" };
                rates ord1 = new rates { ticker = "two1 two" };
                rates ord2 = new rates { ticker = "two2 two" };
                //cust.Orders.Add(ord);

                // Delete an existing Order.
                //Order ord0 = cust.Orders[0];

                // Removing it from the table also removes it from the Customer’s list.
                //db.Orders.DeleteOnSubmit(ord0);

                // Ask the DataContext to save all the changes.
                linqToDB.rates.InsertOnSubmit(ord);
                linqToDB.rates.InsertOnSubmit(ord1);
                linqToDB.rates.InsertOnSubmit(ord2);
                linqToDB.SubmitChanges();

            }


        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Определяем самую старшую дату в котировках и в ковариационной матрице
            /*
             select * from `rates` order by `date` asc limit 1;

select * from `covariations` order by `date` asc limit 1
             */

            DateTime lastRateDate = DateTime.MinValue;
            DateTime lastCovariationDate = DateTime.MinValue;

            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();
                    query.CommandText = "select * from `rates` order by `date` desc limit 1";
                    var reader = query.ExecuteReader();
                    while (reader.Read())
                    {
                        lastRateDate = Convert.ToDateTime(reader["date"].ToString());
                    }
                    reader.Close();

                    query.CommandText = "select * from `covariations` order by `date` desc limit 1";
                    reader = query.ExecuteReader();
                    while (reader.Read())
                    {
                        lastCovariationDate = Convert.ToDateTime(reader["date"].ToString());
                    }
                    reader.Close();

                    connection.Close();
                }
            }

            if (lastCovariationDate > lastRateDate)
            {
                lastCovariationDate = lastRateDate;
            }
            else if (lastCovariationDate < lastRateDate)
            {
                lastRateDate = lastCovariationDate;
            }

            // Идем от первой даты, до последней рассчитывая доли участия акций в портфеле
            while (true)
            {
                // Определяем оптимизационный механизм
                var solver = SolverContext.GetContext();
                solver.ClearModel(); // очищаем модель, иначе при повторном вызове выдаст ошибку
                var model = solver.CreateModel();

                // Второе условие сумма всех доходностей стремится к максимуму
                List<Rate> rates = new List<Rate>();
                using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
                {
                    using (var query = connection.CreateCommand())
                    {
                        connection.Open();
                        query.CommandText = "select * from `rates` where `date`='" + lastRateDate.ToString(_DateFormat) + "'";
                        var reader = query.ExecuteReader();
                        while (reader.Read()) { rates.Add(new Rate(reader)); }
                        reader.Close();
                        connection.Close();
                    }
                }

                // если перестали получать котировки, то выходим
                if (rates.Count() == 0) { break; }
                if (rates[0]._profitable_by_period == 0) { break; }

                // Формула, сумма всех долей равна 1
                Term t1 = 0;
                Term g2 = 0;
                foreach (var rate in rates)
                {
                    var x_i = new Decision(Domain.RealRange(0, 1), "X_" + rate.ticker);
                    var d_i = rate._profitable_by_period;
                    model.AddDecision(x_i);

                    t1 = t1 + x_i;
                    g2 = g2 + (x_i * d_i);
                }

                // Достаем все коварииации 
                // Настраиваем соединение с БД
                List<Covarioation> cov = new List<Covarioation>();
                using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
                {
                    using (var query = connection.CreateCommand())
                    {
                        connection.Open();
                        query.CommandText = "select * from `covariations` where `date`='" + lastRateDate.ToString(_DateFormat) + "'";
                        var reader = query.ExecuteReader();
                        while (reader.Read())
                        {
                            cov.Add(new Covarioation(reader));
                        }
                        reader.Close();

                        // Смещаем дату на день раньше
                        // todo: не учтен period!!!
                        query.CommandText = "select * from `rates` where `date`<'" + lastRateDate.ToString(_DateFormat) + "' " +
                            "order by `date` desc limit 1";
                        reader = query.ExecuteReader();
                        while (reader.Read()) { lastRateDate = Convert.ToDateTime(reader["date"].ToString()); }
                        reader.Close();

                        connection.Close();
                    }
                }

                if (cov.Count() == 0) { continue; }

                Term g1 = 0;
                foreach (var pair in _PairsTickers)
                {
                    if (model.Decisions.Count(x => x.Name == "X_" + pair[0]) > 0 &&
                        model.Decisions.Count(x => x.Name == "X_" + pair[1]) > 0
                        && cov.Count(x => x.tickerA == pair[0] && x.tickerB == pair[1]) > 0
                        )
                    {
                        var x_i = model.Decisions.First(x => x.Name == "X_" + pair[0]);
                        var x_j = model.Decisions.First(x => x.Name == "X_" + pair[1]);
                        var cov_ij = cov.First(x => x.tickerA == pair[0] && x.tickerB == pair[1]);

                        g1 = g1 + (x_i * x_j * cov_ij.value);
                    }
                }

                model.AddGoal("Target1", GoalKind.Minimize, g1);
                model.AddConstraint("Parts", t1 == 1);
                //model.AddConstraint("Sootnoshenie", g2 >= Model.Sqrt(g1));
                model.AddConstraint("Dohodnost", g2 >= 0.02);

                // Ограничиваем работу оптимизационного механизма
                Directive directive = new Directive();
                directive.TimeLimit = Convert.ToInt32(_tBTimeLimitBySeconds.Text) * 1000; // ограничение по времени

                var solution = solver.Solve(directive);

                var s = new List<string>();
                double d = 0;

                s.Add("Расчетная дата: " + lastRateDate.ToString());
                s.Add("");
                try { s.Add("Отчет: " + solution.GetReport()); }
                catch (Exception) { s.Add("По отчету проехалась ошибка."); }
                s.Add("");

                foreach (var decision in model.Decisions) { s.Add(decision.Name + ": " + decision.GetDouble()); }

                foreach (var rate in rates)
                {
                    var x_i = model.Decisions.First(x => x.Name == "X_" + rate.ticker);
                    rate._x = Convert.ToSingle(x_i.ToDouble());
                    var d_i = rate._profitable_by_period;
                    d += x_i.ToDouble() * d_i;
                }

                s.Add("");
                s.Add("Доходность: " + d);
                foreach (var g in model.Goals) { s.Add(g.Name + ": " + g.ToDouble()); }
                textBox2.Lines = s.ToArray();

                // После оптимизации записываем части в БД
                foreach (var rate in rates) { SetRateToDB(rate, _DateFormat, _TimeFormat); }

                Console.Beep();

                //lastRateDate = rates[1].date;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // Настраиваем соединение с БД

            List<Rate> rates = new List<Rate>();
            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();
                    query.CommandText = "select * from `rates` where `_x`>'0' order by `date` desc";
                    var reader = query.ExecuteReader();
                    while (reader.Read())
                    {
                        rates.Add(new Rate(reader));
                    }
                    reader.Close();

                    connection.Close();
                }
            }

            Dictionary<DateTime, float> d = new Dictionary<DateTime, float>();

            foreach (var rate in rates)
            {
                if (d.ContainsKey(rate.date))
                {
                    d[rate.date] += rate._profitable_by_period * rate._x;
                }
                else
                {
                    d.Add(rate.date, rate._profitable_by_period * rate._x);
                }
            }


            Dictionary<DateTime, float> d1 = new Dictionary<DateTime, float>();
            Dictionary<DateTime, float> d2 = new Dictionary<DateTime, float>();
            Dictionary<DateTime, float> d3 = new Dictionary<DateTime, float>();
            for (int i = 0; i < rates.Count(); i++)
            {
                if (i > 1)
                {
                    if (d1.ContainsKey(rates[i - 1].date)) { d1[rates[i - 1].date] += rates[i - 1]._profitable_by_period * rates[i]._x; }
                    else { d1.Add(rates[i - 1].date, rates[i - 1]._profitable_by_period * rates[i]._x); }
                }

                if (i > 2)
                {
                    if (d2.ContainsKey(rates[i - 2].date)) { d2[rates[i - 2].date] += rates[i - 2]._profitable_by_period * rates[i]._x; }
                    else { d2.Add(rates[i - 2].date, rates[i - 2]._profitable_by_period * rates[i]._x); }
                }

                if (i > 3)
                {
                    if (d3.ContainsKey(rates[i - 3].date)) { d3[rates[i - 3].date] += rates[i - 3]._profitable_by_period * rates[i]._x; }
                    else { d3.Add(rates[i - 3].date, rates[i - 3]._profitable_by_period * rates[i]._x); }
                }
            }

            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            chart1.Series[3].Points.Clear();
            //chart1.Series[0].Points.Add()
            chart1.Series[0].Points.DataBindXY(d.Keys, d.Values);
            chart1.Series[1].Points.DataBindXY(d1.Keys, d1.Values);
            chart1.Series[2].Points.DataBindXY(d2.Keys, d2.Values);
            chart1.Series[3].Points.DataBindXY(d3.Keys, d3.Values);

            chart1.Series[0].XValueType = ChartValueType.DateTime;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd";
            //chart1.ChartAreas[0].AxisX.Interval = 1;
            //chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Months;
            //chart1.ChartAreas[0].AxisX.IntervalOffset = 1;


            // Set a variable to the My Documents path.
            string mydocpath =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(mydocpath, "WriteLines.txt")))
            {
                foreach (var dd in d)
                {
                    float v = dd.Value;

                    float v1 = 0;
                    if (d1.Count(x => x.Key == dd.Key) > 0) { v1 = d1.First(x => x.Key == dd.Key).Value; }

                    float v2 = 0;
                    if (d2.Count(x => x.Key == dd.Key) > 0) { v2 = d2.First(x => x.Key == dd.Key).Value; }

                    float v3 = 0;
                    if (d3.Count(x => x.Key == dd.Key) > 0) { v3 = d3.First(x => x.Key == dd.Key).Value; }

                    outputFile.WriteLine(dd.Key.ToString() + ";" + dd.Value.ToString() + ";" + v1.ToString() + ";" + v2.ToString() + ";" + v3.ToString());
                }

            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            var rates = new Dictionary<string, string>();
            var headers = new List<string>();
            var date = new List<string>();
            //List<string> rates = new List<string>();
            string uslovie = " where `date`>='2017-06-01' and `period`='M'";
            using (var connection = new MySqlConnection("Server=localhost; Database=portfolio_db; Uid=root; Pwd=;"))
            {
                using (var query = connection.CreateCommand())
                {
                    connection.Open();

                    // заголовки столбцов
                    query.CommandText = "select distinct `ticker` from `rates`" + uslovie;
                    var reader = query.ExecuteReader();
                    while (reader.Read())
                    {
                        headers.Add(reader[0].ToString());
                    }
                    reader.Close();

                    // столбец дат
                    query.CommandText = "select distinct `date` from `rates`" + uslovie;
                    reader = query.ExecuteReader();
                    while (reader.Read())
                    {
                        date.Add(reader[0].ToString());
                    }
                    reader.Close();

                    // все данные
                    query.CommandText = "select `ticker`,`date`,`close` from `rates`" + uslovie;
                    reader = query.ExecuteReader();
                    while (reader.Read())
                    {
                        rates.Add(reader["ticker"].ToString() + reader["date"].ToString(), reader["close"].ToString());
                    }
                    reader.Close();

                    connection.Close();
                }
            }

            List<string> result = new List<string>();

            // устанавливаем шапочку 
            result.Add("date;" + string.Join(";", headers.ToArray()));

            foreach (var d in date)
            {
                string s = d + ";";
                foreach (var head in headers)
                {
                    if (rates.Count(x => x.Key == head + d) == 0)
                    {
                        s = s + "0;";
                    }
                    else
                    {
                        s = s + rates.First(x => x.Key == head + d).Value + ";";
                    }
                }
                //result.Add(d + string.Join(";", headers.ToArray()));
                result.Add(s);
            }

            textBox1.Text = String.Join(Environment.NewLine, result); ;
        }
    }
}
