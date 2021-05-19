using System.Linq;
using MeterReadingsService.Data;

namespace MeterReadingsService.Repositories
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly MeterReadingServiceContext _context;

        public AccountsRepository(MeterReadingServiceContext context)
        {
            _context = context;
        }

        public bool Exists(int accountId)
        {
            return _context.Accounts.Any(x => x.AccountId == accountId);
        }
    }
}
