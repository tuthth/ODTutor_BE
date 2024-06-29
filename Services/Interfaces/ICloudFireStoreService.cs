using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICloudFireStoreService
    {
        Task<T> GetAsync<T>(string collectionName, string documentName);
        Task SetAsync<T>(string collectionName, string documentName, T value);
        Task<bool> RemoveData(string collectionName, string documentName);
    }
}
