namespace MeterReadingsService.Repositories
{
    public interface IAccountsRepository
    {
        public bool Exists(int accountId);
    }
}
