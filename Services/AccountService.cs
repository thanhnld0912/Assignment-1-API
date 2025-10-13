using Models.Models;
using Repositories.Interfaces;

namespace Services
{
    public class AccountService
    {
        private readonly IAccountsRepo _accountRepo;
        public AccountService(IAccountsRepo accountRepository)
        {
            _accountRepo = accountRepository;
        }
        public Task<IEnumerable<SystemAccount>> GetSystemAccounts() => _accountRepo.GetAll();
        public Task CreateAccount(SystemAccount account) => _accountRepo.Add(account);
        public Task DeleteAccount(SystemAccount account) => _accountRepo.DeleteAccount(account);
        public Task<SystemAccount> GetAccountById(short id) => _accountRepo.GetAccountById(id);

        public Task<SystemAccount> GetSystemAccountByGmailPass(string gmail, string Pass) => _accountRepo.GetSystemAccountByGmailPass(gmail, Pass);
        public Task<IEnumerable<SystemAccount>> SearchAccounts(string name) => _accountRepo.SearchAccounts(name);

        public Task UpdateAccount(SystemAccount account) => _accountRepo.Update(account);

    }
}
