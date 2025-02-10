using CurrencyFetch.Core.Interfaces;
using CurrencyFetch.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyFetch.Core.Infrastructure
{
    public static class CurrencyServiceCollectionExtensions
    {
        public static IServiceCollection AddCurrencyFetchServices(this IServiceCollection services)
        {
            services.AddHttpClient<IExchangeMarketHttpClient, ExchangeMarketHttpClient>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            return services;
        }
    }
}
