using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Watermark.Bot.Abstract;
using Watermark.Bot.Data;
using Watermark.Bot.Models;

namespace Watermark.Bot.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly UserDataDbContext userDataDbContext;

        public UserDataService(UserDataDbContext userDataDbContext)
        {
            this.userDataDbContext = userDataDbContext;
        }

        private async Task<bool> UserExists(long chatId)
        {
            return await userDataDbContext.UserDatas.AnyAsync(u => u.ChatId == chatId);
        }

        public async Task AddNewUser(long chatId, int state)
        {
            if (!await UserExists(chatId))
            {
                var user = new UserData { ChatId = chatId, State = state, LastContact = DateTime.Now };
                var result = await userDataDbContext.UserDatas.AddAsync(user);
                await userDataDbContext.SaveChangesAsync();
            }
            return;
        }
        public async Task SetUserState(long chatId, int state)
        {
            if (!await UserExists(chatId))
                throw new Exception("Схоже, що вам треба перезапустити бота, натисніть /start");
            var user = await userDataDbContext.UserDatas.SingleAsync(u => u.ChatId == chatId);
            
            user.State = state;
            user.LastContact = DateTime.Now;
            await userDataDbContext.SaveChangesAsync();
        }
        public async Task<int> GetUserState(long chatId)
        {
            if (!await UserExists(chatId))
                throw new Exception("Схоже, що вам треба перезапустити бота, натисніть /start");
            var user = await userDataDbContext.UserDatas.SingleAsync(u => u.ChatId == chatId);
            return user.State;
        }
        public async Task SetImageName(long chatId, string imageName, int state)
        {
            if (!await UserExists(chatId))
                throw new Exception("Схоже, що вам треба перезапустити бота, натисніть /start");
            var user = await userDataDbContext.UserDatas.SingleAsync(u => u.ChatId == chatId);
            switch (state)
            {
                case 1:
                    user.SourceImageStoredFileName = imageName;
                    break;
                case 2:
                    user.WatermarkImageStoredFileName = imageName;
                    break;
                default:
                    throw new Exception("Щось пішло не так");
            }
            user.LastContact = DateTime.Now;
            await userDataDbContext.SaveChangesAsync();
        }

        public async Task<string?> GetSouceImageName(long chatId)
        {
            var user = await userDataDbContext.UserDatas.SingleAsync(u => u.ChatId == chatId);
            return user.SourceImageStoredFileName;
        }

        public async Task<string?> GetWatermarkImageName(long chatId)
        {
            var user = await userDataDbContext.UserDatas.SingleAsync(u => u.ChatId == chatId);
            return user.WatermarkImageStoredFileName;
        }

        public async Task DeleteExpiredUserData()
        {
            var expiredUsers = userDataDbContext.UserDatas.Where(u => u.LastContact.AddDays(7) <= DateTime.Now);
            if (!expiredUsers.Any())
                return;

            foreach (var expiredUser in expiredUsers)
                userDataDbContext.UserDatas.Remove(expiredUser);

            await userDataDbContext.SaveChangesAsync();
        }
    }
}
