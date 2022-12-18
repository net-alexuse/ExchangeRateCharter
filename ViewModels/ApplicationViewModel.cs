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
using System.Text.Json;

namespace ExchangeRateCharter.ViewModels
{
    internal class ApplicationViewModel : INotifyPropertyChanged
    {
        public ApplicationViewModel()
        {
            _selectedState = new CurrentState();
            ValuesForChart = new ObservableCollection<RateShort>();
            Currencies = new ObservableCollection<string>() { "USD", "RUB", "EUR" };
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
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        async Task GetResult(CurrentState currentState)
        {
            try
            {
                var httpResult = await httpClient.GetAsync($"https://localhost:44325/Currency?startDate=" +
                                                        $"{currentState.StartDate.ToString("yyyy-MM-dd")}" +
                                                        $"&endDate={currentState.EndDate.ToString("yyyy-MM-dd")}&abbreviation=" +
                                                        $"{currentState.SelectedCurrency}", cancellationToken);

                string content = await httpResult.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (httpResult.IsSuccessStatusCode)
                {
                    ValuesForChart = JsonSerializer.Deserialize<ObservableCollection<RateShort>>(content);
                    OnPropertyChanged("ValuesForChart");
                }
                else if (httpResult.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ResponseContent responseContent = JsonSerializer.Deserialize<ResponseContent>(content);
                    MessageBox.Show($"Ответ от сервера: {responseContent.Message}");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public ObservableCollection<string> Currencies { get; set; }

        public ObservableCollection<RateShort> ValuesForChart { get; set; }

        public ICommand ButtonCommand { get; set; }

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
        private async void AcceptButton_Click(CurrentState currentState)
        {
            if (string.IsNullOrEmpty(currentState.SelectedCurrency))
            {
                MessageBox.Show("Выберите валюту");
                return;
            }
            IsEnabled = false;
            await GetResult(currentState);
            IsEnabled = true;
        }

    }
}
