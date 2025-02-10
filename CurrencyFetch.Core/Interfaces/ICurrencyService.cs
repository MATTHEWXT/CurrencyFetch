namespace CurrencyFetch.Core.Interfaces
{
    public interface ICurrencyService
    {
        Task InsertCurrencyDataAsync(string symbol, DateTime startDate, DateTime endDate);
    }
}