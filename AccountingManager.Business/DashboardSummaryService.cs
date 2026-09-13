using AccountingManager.DataLayer.Context;
using AccountingManager.ViewModels.Accounting;
using System;
using System.Linq;

namespace AccountingManager.Business
{
    public static class DashboardSummaryService
    {
        public static ReportViewModel GetCurrentMonthSummary()
        {
            DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime endDate = startDate.AddMonths(1);

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                var transactions = unitOfWork.AccountingRepository
                    .Get(item => item.DateTitle >= startDate && item.DateTitle < endDate)
                    .ToList();

                int receive = transactions
                    .Where(item => item.TypeId == (int)TransactionType.Receipt)
                    .Sum(item => item.Amount);
                int pay = transactions
                    .Where(item => item.TypeId == (int)TransactionType.Payment)
                    .Sum(item => item.Amount);

                return new ReportViewModel
                {
                    Receive = receive,
                    Pay = pay,
                    AccountBalance = receive - pay
                };
            }
        }
    }
}
