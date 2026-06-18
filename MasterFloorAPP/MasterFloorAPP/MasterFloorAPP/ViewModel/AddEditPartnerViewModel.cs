using MasterFloorAPP.Models;
using MasterFloorAPP.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MasterFloorAPP.ViewModels
{
    public class AddEditPartnerViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private int? _partnerId;
        private Partner _partner = new Partner();
        private ObservableCollection<PartnerType> _partnerTypes = new();
        private PartnerType _selectedPartnerType;
        private bool _isLoading;
        private string _title = string.Empty;

        public Partner Partner
        {
            get => _partner;
            set { _partner = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PartnerType> PartnerTypes
        {
            get => _partnerTypes;
            set { _partnerTypes = value; OnPropertyChanged(); }
        }

        public PartnerType SelectedPartnerType
        {
            get => _selectedPartnerType;
            set
            {
                if (_selectedPartnerType != value)
                {
                    _selectedPartnerType = value;
                    OnPropertyChanged();
                    if (value != null)
                        Partner.TypeId = value.Id;
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditPartnerViewModel(int? partnerId = null)
        {
            _apiService = new ApiService();
            _partnerId = partnerId;
            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(async () => await GoBackAsync());
            LoadData();
        }

        private async void LoadData()
        {
            IsLoading = true;
            try
            {
                var types = await _apiService.GetPartnerTypesAsync();
                if (types != null)
                {
                    PartnerTypes.Clear();
                    foreach (var t in types)
                    {
                        PartnerTypes.Add(t);
                        Console.WriteLine($"Добавлен тип: Id={t.Id}, Name={t.Name}");
                    }
                    Console.WriteLine($"Всего типов: {PartnerTypes.Count}");
                }
                else
                {
                    Console.WriteLine("GetPartnerTypesAsync вернул null");
                }

                if (_partnerId.HasValue)
                {
                    Title = "Редактирование партнёра";
                    var loaded = await _apiService.GetPartnerDetailAsync(_partnerId.Value);
                    if (loaded != null)
                    {
                        Partner = loaded;
                        // После загрузки партнёра и наличия списка типов устанавливаем выбранный тип
                        SelectedPartnerType = PartnerTypes.FirstOrDefault(t => t.Id == Partner.TypeId);
                    }
                    else
                        await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось загрузить партнёра", "OK");
                }
                else
                {
                    Title = "Добавление партнёра";
                    Partner.LegalAddress = new Address();
                    if (PartnerTypes.Any())
                        SelectedPartnerType = PartnerTypes.First();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }


        private async Task SaveAsync()
        {
            if (Partner.Rating.HasValue && Partner.Rating.Value < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Рейтинг должен быть >= 0", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Partner.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите наименование", "OK");
                return;
            }

            if (Partner.LegalAddress == null) Partner.LegalAddress = new Address();

            IsLoading = true;
            try
            {
                bool success;
                if (_partnerId.HasValue)
                    success = await _apiService.UpdatePartnerAsync(_partnerId.Value, Partner);
                else
                    success = await _apiService.CreatePartnerAsync(Partner);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Успешно", "Сохранено", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось сохранить", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GoBackAsync() => await Shell.Current.GoToAsync("//MainPage");

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}