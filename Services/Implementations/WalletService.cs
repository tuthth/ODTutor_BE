using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models.Views;
using Services.Interfaces;
using System.Web.Mvc;


namespace Services.Implementations
{
    public class WalletService : BaseService, IWalletService
    {
        public WalletService(ODTutorContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public async Task<IActionResult> GetWallet(Guid userId)
        {
            var wallet = _context.Wallets.Where(w => w.UserId.Equals(userId)).ProjectTo<WalletView>(_mapper.ConfigurationProvider).FirstOrDefault();
            if (wallet == null)
            {
                return new StatusCodeResult(404);
            }
            return new Microsoft.AspNetCore.Mvc.JsonResult(wallet);

        }
    }
}
