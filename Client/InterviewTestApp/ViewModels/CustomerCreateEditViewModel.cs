using InterviewApp.Models.Shared;
using InterviewApp.Models.Shared.Enums;
using InterviewTestApp.Remote.Services;
using InterviewTestApp.ViewModels.Base;
using InterviewTestApp.ViewModels.Base.Core;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace InterviewTestApp.ViewModels
{
    public class CustomerCreateEditViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        #region fields

        private long _id;
        private string _name;
        private string _email;
        private string _phone;
        private ActiveState _activeState = ActiveState.Active;

        private readonly CustomerService _service;
        private readonly SimpleAsyncDelegateCommand _saveCommand;
        private readonly DelegateCommand _cancelCommand;
        public Action<bool?> CloseAction { get; set; }

        #endregion

        #region ctor

        public CustomerCreateEditViewModel(CustomerService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));

            _saveCommand = new SimpleAsyncDelegateCommand(SaveAsync);
            _cancelCommand = new DelegateCommand(Cancel);
        }

        #endregion

        #region commands

        public SimpleAsyncDelegateCommand SaveCommand => _saveCommand;
        public DelegateCommand CancelCommand => _cancelCommand;

        #endregion

        #region properties

        public long Id
        {
            get => _id;
            set => SetProperty(() => Id, ref _id, value);
        }

        public bool IsNew => Id == 0;

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(() => Name, ref _name, value))
                {
                    ValidateName();
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(() => Email, ref _email, value))
                {
                    ValidateEmail();
                }
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (SetProperty(() => Phone, ref _phone, value))
                {
                    ValidatePhone();
                }
            }
        }

        public IEnumerable<ActiveState> ActiveStateValues => new[]
        {
            ActiveState.Active,
            ActiveState.InActive
        };

        public ActiveState ActiveState
        {
            get => _activeState;
            set => SetProperty(() => ActiveState, ref _activeState, value);
        }
        public CustomerDto SavedCustomer { get; private set; }

        #endregion

        #region Methods

        protected override Task InitializeInternal()
        {
            ActiveState = ActiveState.Active;
            return Task.CompletedTask;
        }

        private async Task<bool> SaveAsync()
        {
            ValidateAll();

            if (HasErrors)
                return false;

            if (!await ValidateEmailUniqueAsync())
                return false;

            var dto = new CustomerDto
            {
                Id = Id,
                Name = Name,
                Email = Email,
                Phone = Phone,
                ActiveState = ActiveState
            };

            try
            {
                await _service.SaveAsync(dto);
                if (Id != null && Id != 0)
                {
                    SavedCustomer = await _service.GetByIdAsync(Id);
                }

                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                CloseAction?.Invoke(false);
                return false;
            }

            return true;
        }

        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }

        #endregion

        #region validation
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public bool HasErrors => _errors.Any(kv => kv.Value != null && kv.Value.Count > 0);

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;

            return _errors.TryGetValue(propertyName, out var list) ? list : null;
        }

        private void ValidateAll()
        {
            ValidateName();
            ValidateEmail();
            ValidatePhone();
        }

        private void ValidateName()
        {
            ClearErrors(nameof(Name));

            if (string.IsNullOrWhiteSpace(Name))
            {
                AddError(nameof(Name), "Name is required.");
                return;
            }

            if (!Regex.IsMatch(Name, @"^[\p{L}\s'-]+$"))
            {
                AddError(nameof(Name), "Name can contain letters only.");
            }
        }

        private void ValidateEmail()
        {
            ClearErrors(nameof(Email));

            if (string.IsNullOrWhiteSpace(Email))
            {
                AddError(nameof(Email), "Email is required.");
                return;
            }

            if (Email.Contains(" "))
            {
                AddError(nameof(Email), "Email cannot contain spaces.");
                return;
            }

            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                AddError(nameof(Email), "Email format is invalid.");
                return;
            }
        }


        private void ValidatePhone()
        {
            ClearErrors(nameof(Phone));

            if (string.IsNullOrWhiteSpace(Phone))
                return;

            if (!Regex.IsMatch(Phone, @"^\+\d+$"))
            {
                AddError(nameof(Phone), "Phone must start with '+' and contain digits only.");
            }
        }

        private async Task<bool> ValidateEmailUniqueAsync()
        {
            if (HasErrorsFor(nameof(Email)))
                return false;

            var isUnique = await _service.IsEmailUniqueAsync(Email, IsNew ? (long?)null : Id);

            if (!isUnique)
            {
                AddError(nameof(Email), "This email is already used by another customer.");
                return false;
            }

            return true;
        }

        private bool HasErrorsFor(string propertyName)
        {
            return _errors.ContainsKey(propertyName) && _errors[propertyName].Count > 0;
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.TryGetValue(propertyName, out var list))
            {
                list = new List<string>();
                _errors[propertyName] = list;
            }

            if (!list.Contains(error))
            {
                list.Add(error);
                RaiseErrorsChanged(propertyName);
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                RaiseErrorsChanged(propertyName);
            }
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        #endregion
    }
}