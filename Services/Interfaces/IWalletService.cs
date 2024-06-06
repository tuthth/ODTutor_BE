using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IWalletService
    {
        Task<ActionResult<List<Wallet>>> GetAllWallets();
        Task<ActionResult<Wallet>> GetWalletByWalletId(Guid id);
        Task<ActionResult<Wallet>> GetWalletByUserId(Guid id);
    }
}
