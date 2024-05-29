using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AppExtension
    {
        private readonly IConfiguration _cf;

        public AppExtension(IConfiguration cf)
        {
            _cf = cf;
        }

        public string CreateHashPassword(string password)
        {
            using (var sha512 = SHA512.Create())
            {
                // Băm mật khẩu thành một mảng byte
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedBytes = sha512.ComputeHash(passwordBytes);

                // Chuyển đổi mảng byte thành chuỗi hex
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashedBytes)
                {
                    builder.Append(b.ToString("X2"));
                }

                return builder.ToString();
            }
        }
        public string GeneratePassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789*&^%$#@";
            Random random = new Random();
            StringBuilder password = new StringBuilder();
            // Thêm một ký tự chữ thường, một ký tự chữ in hoa và một chữ số vào mật khẩu
            password.Append(validChars[random.Next(validChars.Length)]);
            password.Append(validChars[random.Next(26, 52)]); // Chữ in hoa
            password.Append(validChars[random.Next(52, 62)]); // Số

            int requiredLength = random.Next(8, 13);
            while (password.Length < requiredLength)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }
            return password.ToString();
        }
        public bool VerifyPasswordHash(string password, string passwordHash)
        {
            // Băm mật khẩu nhập vào
            string hashedPassword = CreateHashPassword(password);

            // So sánh mật khẩu băm tính toán được với mật khẩu đã lưu trữ
            return hashedPassword.Equals(passwordHash, StringComparison.OrdinalIgnoreCase);
        }
        public async Task<List<string>> UploadImagesToImgBB(List<IFormFile> files)
        {
            try
            {
                string apikey = _cf.GetValue<string>("ImgbbSettings:ApiKey");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", apikey);
                var imageUrlList = new List<string>();
                // List all of images
                foreach (var image in files)
                {
                    using (var stream = image.OpenReadStream())
                    {
                        var content = new MultipartFormDataContent();
                        content.Add(new StreamContent(stream), "image", image.FileName);
                        var response = await client.PostAsync("https://api.imgbb.com/1/upload?key=" + apikey, content);
                        response.EnsureSuccessStatusCode();
                        var responseString = await response.Content.ReadAsStringAsync();
                        var jsonData = JsonConvert.DeserializeObject<dynamic>(responseString);
                        var imageUrl = (string)jsonData.data.url;
                        imageUrlList.Add(imageUrl);
                    }
                }
                return imageUrlList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
