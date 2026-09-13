using AccountingManager.DataLayer.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using AccountingManager.ViewModels.Customers;
namespace AccountingManager.DataLayer.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AccountingManagerDbEntities _db;

        public CustomerRepository(AccountingManagerDbEntities context)
        {
            _db = context;
        }
        public List<Customers> GetAllCustomers()
        {
            return _db.Customers.AsNoTracking().OrderBy(customer => customer.FullName).ToList();
        }
        public IEnumerable<Customers> GetCustomersByFilter(string parameter)
        {
            string filter = (parameter ?? string.Empty).Trim();
            return _db.Customers
                .AsNoTracking()
                .Where(customer =>
                    customer.FullName.Contains(filter)
                    || (customer.Email != null && customer.Email.Contains(filter))
                    || customer.Mobile.Contains(filter))
                .OrderBy(customer => customer.FullName)
                .ToList();
        }

        public List<ListCustomerViewModel> GetNameCustomers(string filter = "")
        {
            filter = (filter ?? string.Empty).Trim();
            if (filter.Length == 0)
            {
                return _db.Customers.OrderBy(c => c.FullName).Select(c => new ListCustomerViewModel()
                {
                    CustomerId = c.CustomerId,
                    FullName = c.FullName,
                }).ToList();
            }

            return _db.Customers.Where(c => c.FullName.Contains(filter)).OrderBy(c => c.FullName).Select(c => new ListCustomerViewModel()
            {
                CustomerId = c.CustomerId,
                FullName = c.FullName,
            }).ToList();
        }

        public Customers GetCustomerById(int customerId)
        {
            return _db.Customers.AsNoTracking().SingleOrDefault(customer => customer.CustomerId == customerId);
        }

        public bool InsertCustomer(Customers customer)
        {
            _db.Customers.Add(customer);
            return true;
        }
        public bool UpdateCustomer(Customers customer)
        {
            var local = _db.Set<Customers>().Local.FirstOrDefault(f => f.CustomerId == customer.CustomerId);
            if (local != null)
            {
                _db.Entry(local).State = EntityState.Detached;
            }
            _db.Entry(customer).State = EntityState.Modified;
            return true;
        }

        public bool DeleteCustomer(Customers customer)
        {
            if (customer == null)
            {
                return false;
            }

            _db.Entry(customer).State = EntityState.Deleted;
            return true;
        }

        public bool DeleteCustomer(int customerId)
        {
            var customer = _db.Customers.Find(customerId);
            return DeleteCustomer(customer);
        }

        public string GetCustomerNameById(int customerId)
        {
            return _db.Customers
                .Where(customer => customer.CustomerId == customerId)
                .Select(customer => customer.FullName)
                .SingleOrDefault();
        }

        public bool HasTransactions(int customerId)
        {
            return _db.Accounting.Any(transaction => transaction.CustomerId == customerId);
        }
    }
}
