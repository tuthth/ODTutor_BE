using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class FirebaseRealtimeDatabaseService : IFirebaseRealtimeDatabaseService
    {   
        private readonly IFirebaseClient _firebaseClient;
        private readonly IConfiguration configuration;

        public FirebaseRealtimeDatabaseService(IConfiguration configuration)
        {
            _firebaseClient = new FirebaseClient(new FirebaseConfig
            {
                AuthSecret = configuration["Firebase:AuthSecret"],
                BasePath = configuration["Firebase:BasePath"]
            });
            this.configuration = configuration;
        }
        public async Task<T> GetAsync<T>(string key)
        {
            FirebaseResponse response = await _firebaseClient.GetAsync(key);
            if(response.Body != "null")
            {
                return response.ResultAs<T>();
            }
            else
            {
                return default(T);
            }
        }

        public async Task<bool> RemoveData(string key)
        {
            var _exist = await _firebaseClient.GetAsync(key);
            Console.WriteLine(_exist.ToString());
            if (_exist.Body != "null")
            {
                await _firebaseClient.DeleteAsync(key);
                return true;
            }
            else
            {
                return false;
            }
        }
         
        public async Task SetAsync<T>(string key, T value)
        {
            await _firebaseClient.SetAsync<T>(key, value);
        }
    }
}
