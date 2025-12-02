using FluentNHibernate.Mapping;

namespace InterviewApp.Domain.Customers
{
    public class CustomerMap : ClassMap<CustomerEntity>
    {
        public CustomerMap()
        {
            Table("[Customers]");

            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.CreateDate).ReadOnly();

            Map(x => x.Name);
            Map(x => x.Phone);
            Map(x => x.Email);
            Map(x => x.ActiveState).CustomType<ActiveState>();
        }
    }
}