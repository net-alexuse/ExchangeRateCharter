using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateCharter.Models
{
    public class RateShort
    {
        public int Cur_ID { get; set; }
        [Key]
        public DateTime Date { get; set; }
        //{
        //    get { return _date.ToString("dd.MM.yyyy"); }
        //    set { _date = Convert.ToDateTime(value); }
        //}

        public decimal? Cur_OfficialRate { get; set; }
        //public System.DateTime _date { get; set; }
    }
}
