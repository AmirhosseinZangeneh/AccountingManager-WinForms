using AccountingManager.DataLayer.Repositories;
using AccountingManager.DataLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingManager.DataLayer.Context
{
    public sealed class UnitOfWork : IDisposable
    {
        private readonly AccountingManagerDbEntities _db = new AccountingManagerDbEntities();
        private ICustomerRepository _customerRepository;
        private GenericRepository<Accounting> _accountingRepository;
        private GenericRepository<Login> _loginRepository;
        private bool _disposed;

        public GenericRepository<Accounting> AccountingRepository
        {
            get
            {
                if (_accountingRepository == null)
                {
                    _accountingRepository = new GenericRepository<Accounting>(_db);
                }

                return _accountingRepository;
            }
        }

        public ICustomerRepository CustomerRepository
        {
            get
            {
                if (_customerRepository == null)
                {
                    _customerRepository = new CustomerRepository(_db);
                }

                return _customerRepository;
            }
        }

        public GenericRepository<Login> LoginRepository
        {
            get
            {
                if (_loginRepository == null)
                {
                    _loginRepository = new GenericRepository<Login>(_db);
                }
                return _loginRepository;
            }
        }

        public void Save()
        {
            ThrowIfDisposed();
            _db.SaveChanges();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _db.Dispose();
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(UnitOfWork));
            }
        }
    }
}
