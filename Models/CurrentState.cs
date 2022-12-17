using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateCharter.Models
{
    internal class CurrentState
    {
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (value > DateTime.Today)
                    _endDate = DateTime.Today;
                else if (value < _startDate)
                    _endDate = _startDate;
                else
                    _endDate = value;
            }
        }
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (value <= DateTime.Today.AddYears(-5))
                    _startDate = DateTime.Today.AddYears(-5);
                else if (value >= _endDate)
                    _startDate = _endDate;
                else
                    _startDate = value;
            }
        }
        public string? SelectedCurrency { get; set; }

        private DateTime _startDate = DateTime.Today.AddDays(-1);

        private DateTime _endDate = DateTime.Today;
    }
}
