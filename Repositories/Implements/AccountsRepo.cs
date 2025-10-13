using EXE_201.Infrastructure.Implements;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Repositories.Interfaces;

namespace Repositories.Implements
{
    public class AccountsRepo : Repository<SystemAccount>, IAccountsRepo
    {

        public AccountsRepo(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<SystemAccount> GetAccountById(short id)
        {
            return await _context.SystemAccounts.AsNoTracking()
                .Include(x => x.NewsArticles)
                .SingleOrDefaultAsync(x => x.AccountId == id);
        }

        public async Task<SystemAccount> GetSystemAccountByGmailPass(string gmail, string Pass)
        {

            return await _context.SystemAccounts.FirstOrDefaultAsync(c => c.AccountPassword == Pass && c.AccountEmail == gmail);
        }

        public async Task<IEnumerable<SystemAccount>> SearchAccounts(string name)
        {
            return await _context.SystemAccounts.Where(c => c.AccountName == name).ToListAsync();
        }
        public async Task DeleteAccount(SystemAccount account)
        {
            var p1 = await _context.SystemAccounts.FindAsync(account.AccountId);
            if (p1 == null) return;

            _context.SystemAccounts.Remove(p1);
            await _context.SaveChangesAsync();
        }
    }
}
