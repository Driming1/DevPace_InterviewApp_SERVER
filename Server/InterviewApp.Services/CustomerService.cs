using InterviewApp.Data;
using InterviewApp.Domain.Customers;
using InterviewApp.Models.Shared;
using InterviewApp.Models.Shared.Enums;
using DomainActiveState = InterviewApp.Domain.Customers.ActiveState;
using DtoActiveState = InterviewApp.Models.Shared.Enums.ActiveState;

namespace InterviewApp.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public IList<CustomerDto> Search(CustomerFilter filterModel)
        {
            if (filterModel == null)
                throw new ArgumentNullException(nameof(filterModel));

            if (filterModel.Take <= 0)
                filterModel.Take = 100;

            if (filterModel.Skip < 0)
                filterModel.Skip = 0;

            if (string.IsNullOrWhiteSpace(filterModel.OrderBy))
            {
                filterModel.OrderBy = nameof(CustomerDto.Id);
                filterModel.OrderDirection = OrderDirection.Ascending;
            }

            filterModel.ActiveState = filterModel.ActiveState;

            return _customerRepository.Search(filterModel);
        }

        public CustomerDto GetById(long id)
        {
            var entity = _customerRepository.GetById(id);
            if (entity == null)
                return null;

            return MapToDto(entity);
        }

        public CustomerDto Save(CustomerDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            CustomerEntity entity;

            if (dto.Id.HasValue)
            {
                entity = _customerRepository.GetById(dto.Id.Value) ?? new CustomerEntity();
            }
            else
            {
                entity = new CustomerEntity();
            }

            MapToEntity(dto, entity);

            _customerRepository.Save(entity);

            dto.Id = entity.Id;
            dto.CreateDate = entity.CreateDate;

            return dto;
        }

        private static CustomerDto MapToDto(CustomerEntity entity)
        {
            return new CustomerDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Phone = entity.Phone,
                Email = entity.Email,
                CreateDate = entity.CreateDate,
                ActiveState = (DtoActiveState)entity.ActiveState
            };
        }

        private static void MapToEntity(CustomerDto dto, CustomerEntity entity)
        {
            entity.Name = dto.Name;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.ActiveState = (DomainActiveState)dto.ActiveState;
        }
    }
}
