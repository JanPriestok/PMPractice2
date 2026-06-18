using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MasterFloorAPP.Models;
using MasterFloorAPP.Services;

namespace MasterFloorAPP.ViewModels
{

    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private bool _isLoading;
        private ObservableCollection<PartnerListItem> _partners = new();



        public ObservableCollection<PartnerListItem> Partners
        {
            get => _partners;
            set { _partners = value; OnPropertyChanged(); }
        }



        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ICommand LoadPartnersCommand { get; }
        public ICommand AddPartnerCommand { get; }
        public ICommand EditCommand { get; }

        public MainPageViewModel()
        {
            _apiService = new ApiService();

            LoadPartnersCommand = new Command(async () => await LoadPartnersAsync());
            AddPartnerCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(AddEditPartnerPage)));
            EditCommand = new Command<PartnerListItem>(async (partner) =>
            {
                if (partner != null)
                    await Shell.Current.GoToAsync($"{nameof(AddEditPartnerPage)}?partnerId={partner.Id}");
            });
            LoadPartnersCommand.Execute(null);
        }

        private async Task LoadPartnersAsync()
        {
            if (IsLoading) return;
            IsLoading = true;

            try
            {
                var partners = await _apiService.GetPartnersAsync();
                if (partners != null && partners.Any())
                {
                    Partners.Clear();
                    foreach (var p in partners)
                        Partners.Add(p);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка",
                        "Не удалось загрузить список партнёров. Проверьте подключение к API.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Исключение", ex.Message, "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}