using EXE_201.Infrastructure.Interfaces;
using Models.Models;

namespace Repositories.Interfaces
{
    public interface IAccountsRepo : IRepository<SystemAccount>
    {
        public Task<SystemAccount> GetSystemAccountByGmailPass(string gmail, string Pass);
        public Task<IEnumerable<SystemAccount>> SearchAccounts(string name);
        public Task<SystemAccount> GetAccountById(short id);
        public Task DeleteAccount(SystemAccount account);
    }
}
