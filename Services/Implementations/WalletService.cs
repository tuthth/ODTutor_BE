using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Models.Requests;
using Models.Models.Views;
using Models.PageHelper;
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
        public async Task<ActionResult<WalletTransaction>> GetLastTransaction(Guid id)
        {
            try
            {
                var walletTransaction = await _context.WalletTransactions.Where(c => c.SenderWalletId == id || c.ReceiverWalletId == id).OrderByDescending(c => c.CreatedAt).FirstOrDefaultAsync();
                if (walletTransaction == null)
                {
                    return new StatusCodeResult(404);
                }
                return walletTransaction;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<ActionResult<PageResults<Wallet>>> GetAllWalletsPaging(PagingRequest request)
        {
            try
            {
                var walletsList = await _context.Wallets.OrderByDescending(c => c.LastBalanceUpdate).ToListAsync();
                if (walletsList == null || !walletsList.Any())
                {
                    return new StatusCodeResult(404);
                }

                var paginatedWallets = PagingHelper<Wallet>.Paging(walletsList, request.Page, request.PageSize);
                if (paginatedWallets == null)
                {
                    return new StatusCodeResult(400);
                }

                return paginatedWallets;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
