using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using ExchangeRateCharter.Models;
using ExchangeRateCharter.ViewModels;

namespace ExchangeRateCharter.ViewModels
{
    internal class ApplicationViewModel : INotifyPropertyChanged
    {
        public ApplicationViewModel()
        {
            _selectedState = new CurrentState();
            ValuesForChart = new ObservableCollection<RateShort>();
            Currencies = new ObservableCollection<string>() { "USD", "RUB", "EUR" };

            //GetRates = new DelegateCommand<string>(str => {Currencies.Add(str);});
            ButtonCommand = new RelayCommand(o => AcceptButton_Click(_selectedState));

            httpClient = new HttpClient();
        }
        public DateTime StartDate
        {
            get { return _selectedState.StartDate; }
            set { _selectedState.StartDate = value; }
        }
        public DateTime EndDate
        {
            get { return _selectedState.EndDate; }
            set { _selectedState.EndDate = value; }
        }



        async Task<ObservableCollection<RateShort>> GetResult(CurrentState currentState, CancellationToken cancellationToken)
        {
            var httpResult = await httpClient.GetAsync($"https://localhost:5002/Currencies?startDate=" +
                $"{currentState.StartDate}" +
                $"&endDate={currentState.EndDate}&abreviature=" +
                $"{currentState.SelectedCurrency}", cancellationToken);

            if (!httpResult.IsSuccessStatusCode)
                MessageBox.Show("Server Lost");

            var rateShortCollection = new ObservableCollection<RateShort>();

            return rateShortCollection;
        }

        //public DelegateCommand<string> GetRates { get; }
        public ObservableCollection<string> Currencies { get; set; }


        public ObservableCollection<RateShort> ValuesForChart { get; set; }

        public ICommand ButtonCommand { get; set; }

        public void GetRandom()
        {
            ValuesForChart.Clear();
            Random random = new Random();
            int numberOfResults = random.Next(10, 1500);
            DateTime date = DateTime.Today.AddYears(-5);
            for (int i = 0; i < numberOfResults; i++, date = date.AddDays(1))
            {
                var key = date;
                var value = random.Next(50);
                ValuesForChart.Add(new RateShort() { Date = key, Cur_OfficialRate = value });
            }
            OnPropertyChanged("Values");
        }
        public string? SelectedCurrency
        {
            get { return _selectedState.SelectedCurrency; }
            set { OnPropertyChanged("SelectedCurrency"); _selectedState.SelectedCurrency = value; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        private readonly HttpClient httpClient;
        private CancellationToken cancellationToken;
        private CurrentState _selectedState { get; set; }
        private void AcceptButton_Click(CurrentState currentState)
        {
            if (string.IsNullOrEmpty(currentState.SelectedCurrency))
                MessageBox.Show("Выберите валюту");

            //("Начальная дата" + currentState.StartDate.ToString() + $"/n" + "Конечная дата" + currentState.EndDate.ToString() + currentState.SelectedCurrency);
            GetRandom();
            var result = GetResult(currentState, cancellationToken);
        }

    }
}
