using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models.Models.Views;
using Services.Interfaces;
using System.Web.Mvc;


namespace Services.Implementations
{
    public class WalletService : BaseService, IWalletService
    {
        public WalletService(Context context, IMapper mapper) : base(context, mapper)
        {
        }
        //public async Task<IActionResult> GetWallet(int userId)
        //{
        //    var wallet = _context.Wallets.GetMany(w => w.StudentId.Equals(userId)).ProjectTo<WalletView>(_mapper.ConfigurationProvider).FirstOrDefault();
        //    if (wallet == null)
        //    {
        //        return new StatusCodeResult(404);
        //    }
        //    return new JsonResult(wallet);

        //}
        public async Task<IActionResult> GetWallet(int userId) => new StatusCodeResult(200);
    }
}
