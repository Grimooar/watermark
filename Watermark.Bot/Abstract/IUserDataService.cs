using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watermark.Bot.Abstract
{
    public interface IUserDataService
    {
        Task AddNewUser(long chatId, int state);
        Task SetUserState(long chatId, int state);
        Task<int> GetUserState(long chatId);
        Task SetImageName(long chatId, string imageName, int state);
        Task<string?> GetSouceImageName(long chatId);
        Task<string?> GetWatermarkImageName(long chatId);
        Task DeleteExpiredUserData();
    }
}
