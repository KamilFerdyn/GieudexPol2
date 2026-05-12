using GieudexPol.Application.Interfaces;
using GieudexPol.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GieudexPol.Infrastructure.Repositories
{
    public class ExchangeRateRepository : GenericRepository<ExchangeRate>, IExchangeRateRepository
    {
        public ExchangeRateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ExchangeRate> GetByCurrencyPairAsync(string baseCurrencySymbol, string targetCurrencySymbol)
        {
            return await _dbSet.FirstOrDefaultAsync(er => er.Currency.Symbol == baseCurrencySymbol);
        }
    }
}
