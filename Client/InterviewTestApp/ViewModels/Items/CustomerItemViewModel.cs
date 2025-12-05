using System;
using InterviewApp.Models.Shared;
using InterviewApp.Models.Shared.Enums;
using InterviewTestApp.ViewModels.Base;

namespace InterviewTestApp.ViewModels.Items
{
    public class CustomerItemViewModel : BaseViewModel
    {
        public CustomerDto Dto { get; }

        public CustomerItemViewModel(CustomerDto dto)
        {
            Dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }

        public long? Id
        {
            get => Dto.Id;
            set
            {
                if (Equals(Dto.Id, value))
                    return;

                Dto.Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get => Dto.Name;
            set
            {
                if (string.Equals(Dto.Name, value))
                    return;

                Dto.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Email
        {
            get => Dto.Email;
            set
            {
                if (string.Equals(Dto.Email, value))
                    return;

                Dto.Email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Phone
        {
            get => Dto.Phone;
            set
            {
                if (string.Equals(Dto.Phone, value))
                    return;

                Dto.Phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        public ActiveState ActiveState
        {
            get => Dto.ActiveState;
            set
            {
                if (Dto.ActiveState == value)
                    return;

                Dto.ActiveState = value;
                OnPropertyChanged(nameof(ActiveState));
            }
        }

        public void UpdateFrom(CustomerDto dto)
        {
            if (dto == null)
                return;

            Id = dto.Id;
            Name = dto.Name;
            Email = dto.Email;
            Phone = dto.Phone;
            ActiveState = dto.ActiveState;
        }
    }
}
