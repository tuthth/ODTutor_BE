using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<List<Wallet>>> GetAllWallets()
        {
            try
            {
                var wallets = await _context.Wallets.OrderByDescending(c => c.LastBalanceUpdate).ToListAsync();
                if (wallets == null)
                {
                    return new StatusCodeResult(404);
                }
                return wallets;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Wallet>> GetWalletByWalletId(Guid id)
        {
            try
            {
                var wallet = await _context.Wallets.FirstOrDefaultAsync(c => c.WalletId == id);
                if (wallet == null)
                {
                    return new StatusCodeResult(404);
                }
                return wallet;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<Wallet>> GetWalletByUserId(Guid id)
        {
            try
            {
                var wallet = await _context.Wallets.FirstOrDefaultAsync(c => c.UserId == id);
                if (wallet == null)
                {
                    return new StatusCodeResult(404);
                }
                return wallet;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
