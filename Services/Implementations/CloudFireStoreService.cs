using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;

namespace Services.Implementations
{
    public class CloudFireStoreService : ICloudFireStoreService
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly IConfiguration _configuration;

        public CloudFireStoreService( IConfiguration configuration)
        {
            _configuration = configuration;
            string relativePath = _configuration["Firebase:JsonPath"];
            string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", absolutePath);
            string projectId = _configuration["Firebase:ProjectId"];
            _firestoreDb = FirestoreDb.Create(projectId);

        }

        public async Task<T> GetAsync<T>(string collectionName, string documentName)
        {
            DocumentReference docRef = _firestoreDb.Collection(collectionName).Document(documentName);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<T>();
            }
            else
            {
                return default(T);
            }
        }

        public async Task<bool> RemoveData(string collectionName, string documentName)
        {
            DocumentReference docRef = _firestoreDb.Collection(collectionName).Document(documentName);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                await docRef.DeleteAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task SetAsync<T>(string collectionName, string documentName, T value)
        {
            DocumentReference docRef = _firestoreDb.Collection(collectionName).Document(documentName);
            await docRef.SetAsync(value);
        }

        // Get From Collection
        public async Task<List<T>> GetCollectionAsync<T>(string collectionName)
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection(collectionName).GetSnapshotAsync();
            List<T> list = new List<T>();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                list.Add(document.ConvertTo<T>());
            }
            return list;
        }
        //
        public async Task<T> GetDocumentAsync<T>(string collectionName, string documentId)
        {
            DocumentSnapshot snapshot = await _firestoreDb.Collection(collectionName).Document(documentId).GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<T>();
            }
            else
            {
                return default(T);
            }
        }
    }
}
