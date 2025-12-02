using InterviewApp.Data.Base;
using InterviewApp.Domain;
using InterviewApp.Models.Shared;
using NHibernate.Transform;

namespace InterviewApp.Data
{
    public class CustomerRepository : RepositoryBase<CustomerEntity>, ICustomerRepository
    {
        public CustomerRepository(DataProvider dataProvider) : base(dataProvider)
        {

        }

        public IList<CustomerDto> Search(CustomerFilter filterModel)
        {
            const string sql = @"EXEC [SearchCustomers] :name, :phone, :email, :skip, :take";

            return DataProvider.Session.CreateSQLQuery(sql)
                .SetParameter("name", filterModel.Name)
                .SetParameter("phone", filterModel.Phone)
                .SetParameter("email", filterModel.Email)
                .SetParameter("skip", filterModel.Skip)
                .SetParameter("take", filterModel.Take)
                .SetResultTransformer(Transformers.AliasToBean<CustomerDto>())
                .List<CustomerDto>();
        }
    }
}
