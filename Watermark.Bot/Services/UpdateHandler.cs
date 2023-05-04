using Microsoft.Extensions.Logging;
using Telegram.Bot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Watermark.Bot.Abstract;
using Telegram.Bot.Types.InputFiles;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using Watermark.Models.Dtos;
using System.Threading;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace Watermark.Bot.Services
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<UpdateHandler> _logger;
        private readonly IUserDataService _userDataService;
        private readonly HttpClient httpClient;
        private readonly IHostEnvironment env;

        public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, IUserDataService userDataService, HttpClient httpClient, IHostEnvironment env)
        {
            _botClient = botClient;
            _logger = logger;
            _userDataService = userDataService;
            this.httpClient = httpClient;
            this.env = env;
        }
        
        private async Task<int?> GetUserState(Message message, CancellationToken cancellationToken)
        {
            try
            {
                return await _userDataService.GetUserState(message.Chat.Id);
            }
            catch (Exception ex)
            {
                await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: ex.Message, cancellationToken: cancellationToken);
                return null;
            }
        }
        private async Task<UploadImagesDto> UploadFileToServer(HttpContent content, Message message, CancellationToken cancellationToken)
        {
            try
            {
                var response = await httpClient.PostAsync("api/Image", content);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        Console.WriteLine("Something went wrong");
                        return new UploadImagesDto { Uploaded = false, ErrorMessage = "Щось пішло не так."};
                    }
                    return await response.Content.ReadFromJsonAsync<UploadImagesDto>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Http status code - {response.StatusCode} Message - {error}");
                    return new UploadImagesDto { Uploaded = false, ErrorMessage = error };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task<WatermarkedImageDto> RequestImage(Message message, CancellationToken cancellationToken)
        {
            var souceImageStoredFileName = await _userDataService.GetSouceImageName(message.Chat.Id);
            var watermarkImageStoredFileName = await _userDataService.GetWatermarkImageName(message.Chat.Id);
            if (souceImageStoredFileName == null || watermarkImageStoredFileName == null)
            {
                await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Спочатку вам треба відправити усі зображення");
                return null;
            }

            try
            {
                var requestImageDto = new RequestImageDto { SourceImageStoredFileName = souceImageStoredFileName, 
                    WatermarkImageStoredFileName = watermarkImageStoredFileName, 
                    WatermarkStyle = 1 };
                var content = JsonSerializer.Serialize(requestImageDto);
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

                var requestResult = await httpClient.PostAsync("api/Image/requestImage", bodyContent);
                var requestContent = await requestResult.Content.ReadFromJsonAsync<WatermarkedImageDto>();

                if (requestResult.IsSuccessStatusCode)
                    return requestContent;
                else if (requestResult.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, 
                        text: "Завантажені вами зображення не були знайдені на сервері.\nСпробуйте відправити їх знову.", 
                        cancellationToken: cancellationToken);
                    return null;
                }
                else
                {
                    var errMessage = requestContent;
                    throw new Exception($"Http staus code - {requestResult.StatusCode}, Message - {errMessage}");
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task DeleteFile(string fullPath)
        {
            FileInfo file = new FileInfo(fullPath);
            file.Delete();
            return;
        }

        private async Task SendWatermarkedImage(Message message, CancellationToken cancellationToken, WatermarkedImageDto watermarkedImageDto)
        {
            await _botClient.SendChatActionAsync(
                    message.Chat.Id,
                    ChatAction.UploadPhoto,
                    cancellationToken: cancellationToken);

            await Task.Delay(500);

            byte[] fileContent = Convert.FromBase64String(watermarkedImageDto.ImageBase64Data);
            MemoryStream fileStream = new MemoryStream(fileContent);

            await _botClient.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: new InputOnlineFile(fileStream),
                caption: "Nice Picture",
                cancellationToken: cancellationToken);

            _logger.LogInformation($"Send an image to chat {message.Chat.Id}, user {message.Chat.Username}, at {DateTime.Now}");

            return;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
        {
            var handler = update switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                // UpdateType.Poll:
                { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update, cancellationToken)
            };

            await handler;
        }

        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Received message type: {message.Type} from chat {message.Chat.Id}, user {message.Chat.Username}");

            if (message.Photo is { } messagePhoto)
            {
                await ReceiveImage(message, cancellationToken);
                return;
            }

            if (message.Document is { } messageDocument)
            {
                await ReceiveDocument(message, cancellationToken);
                return;
            }

            if (message.Text is not { } messageText)
                return;

            var action = messageText.Split(' ')[0] switch
            {
                "/start" => StartWork(message, cancellationToken),
                "/help"  => SendHelp(message, cancellationToken),
                _        => Ignore(message, cancellationToken)
            };;
            Message sentMessage = await action;

            async Task<Message> Ignore(Message message, CancellationToken cancellationToken)
            {
                return null;
            }

            async Task<Message> StartWork(Message message, CancellationToken cancellationToken)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Цей бот може наносити водяний знак на графічне зображення.\n/start - перезапустити бота\n/help - як користуватися ботом",
                    cancellationToken: cancellationToken);

                try
                {
                    await _userDataService.AddNewUser(message.Chat.Id, 0);
                }
                catch (Exception ex)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: ex.Message,
                        cancellationToken: cancellationToken);
                    _logger.LogCritical(ex.Message);
                    throw;
                }

                return await SendInlineMessage(_botClient, message, cancellationToken);
            }

            async Task<Message> SendHelp(Message message, CancellationToken cancellationToken)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Перед нанесенням вашого водяного знаку на зоображення, мені спочатку необхідно отримати і зображення водяного знаку, і зоображення, на яке він буде нанесений, від вас." +
                    "\nПеред їх відправкою використовуйте кнопки з наступного повідомлення для уточненння, що і куди треба нанести." +
                    "\nПісля відправки обох зображень, натискайте кнопку \"Нанести водяний знак\", щоб отримати результат.",
                    cancellationToken: cancellationToken);

                try
                {
                    await _userDataService.AddNewUser(message.Chat.Id, 0);
                }
                catch (Exception ex)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: ex.Message,
                        cancellationToken: cancellationToken);
                    _logger.LogCritical(ex.Message);
                    throw;
                }

                return await SendInlineMessage(_botClient, message, cancellationToken);
            }

            async Task<Message> SendInlineMessage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            {
                await _botClient.SendChatActionAsync(
                    chatId: message.Chat.Id,
                    chatAction: ChatAction.Typing,
                    cancellationToken: cancellationToken);
                // Simulate longer running task
                await Task.Delay(500, cancellationToken);

                InlineKeyboardMarkup inlineKeyboard = new(
                    new[]
                    {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Відправити вихідне зображення", "1"),
                        InlineKeyboardButton.WithCallbackData("Відправити водяний знак", "2"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Нанести водяний знак", "3"),
                    },
                    });

                await _userDataService.SetUserState(message.Chat.Id, 0);

                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Що бажаєте зробити?",
                    replyMarkup: inlineKeyboard,
                    cancellationToken: cancellationToken);
            }

            async Task ReceiveImage(Message message, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Received an Image File From {message.Chat.Id}, user {message.Chat.Username}");
                var fileInfo = await _botClient.GetFileAsync(messagePhoto[messagePhoto.Length - 1].FileId, cancellationToken);
                await ProcessImage(message, cancellationToken, fileInfo);
                return;
            }

            async Task ReceiveDocument(Message message, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"Received a Document File From {message.Chat.Id}, user {message.Chat.Username}");
                var fileInfo = await _botClient.GetFileAsync(message.Document.FileId, cancellationToken);
                await ProcessImage(message, cancellationToken, fileInfo);
                return;
            }

            async Task ProcessImage(Message message, CancellationToken cancellationToken, Telegram.Bot.Types.File fileInfo)
            {
                var userState = await GetUserState(message, cancellationToken);
                if (userState == 0 || userState == null || userState == 3)
                {
                    await SendInlineMessage(_botClient, message, cancellationToken);
                    return;
                }

                string[] permittedExtensoins = { ".jpeg", ".png", ".jpg" };

                var ext = Path.GetExtension(fileInfo.FilePath).ToLowerInvariant();

                if (string.IsNullOrEmpty(ext) || !permittedExtensoins.Contains(ext))
                {
                    await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Непідтримуваний формат файлу", cancellationToken: cancellationToken);
                    return;
                }

                if (fileInfo.FileSize > 1000000)
                {
                    await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Файл занадто великий", cancellationToken: cancellationToken);
                    return;
                }

                var path = Path.Combine(env.ContentRootPath, "TempUploads");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var fullPath = Path.Combine(path, fileInfo.FileUniqueId + Path.GetExtension(fileInfo.FilePath));

                using (var saveImageStream = new FileStream(fullPath, FileMode.Create))
                {
                    await _botClient.DownloadFileAsync(fileInfo.FilePath, saveImageStream);
                }

                using (var fileStream = System.IO.File.OpenRead(fullPath))
                {
                    var fileContent = new StreamContent(fileStream);

                    fileContent.Headers.Add("Content-Type", "application/octet-stream");
                    fileContent.Headers.Add("Content-Length", fileStream.Length.ToString());

                    var content = new MultipartFormDataContent();
                    content.Add(content: fileContent, name: "file", fileName: Path.GetFileName(fullPath));
                    var uploadedImagesDto = await UploadFileToServer(content, message, cancellationToken);

                    if (!uploadedImagesDto.Uploaded || uploadedImagesDto.ErrorMessage != null)
                    {
                        await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: uploadedImagesDto.ErrorMessage, cancellationToken: cancellationToken);
                        DeleteFile(fullPath);
                        return;
                    }
                    try
                    {
                        await _userDataService.SetImageName(message.Chat.Id, uploadedImagesDto.StoredFileName, (int)userState);
                        await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Зображення отримано", cancellationToken: cancellationToken);
                        await SendInlineMessage(_botClient, message, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: ex.Message, cancellationToken: cancellationToken);
                    }
                }

                DeleteFile(fullPath);
                return;
            }
        }

        // Process Inline Keyboard callback data
        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);

            await _botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                cancellationToken: cancellationToken);
            try
            {
                switch (callbackQuery.Data)
                {
                    case "1":
                        await _botClient.SendTextMessageAsync(
                            chatId: callbackQuery.Message!.Chat.Id,
                            text: "Відправте зображення, на яке буде нанесено водяний знак",
                            cancellationToken: cancellationToken);
                        await _userDataService.SetUserState(callbackQuery.Message!.Chat.Id, 1);
                        break;
                    case "2":
                        await _botClient.SendTextMessageAsync(
                            chatId: callbackQuery.Message!.Chat.Id,
                            text: "Відправте зображення водяного знаку, який буде нанесено на основне зображення",
                            cancellationToken: cancellationToken);
                        await _userDataService.SetUserState(callbackQuery.Message!.Chat.Id, 2);
                        break;
                    case "3":
                        var watermarkedImageDto = await RequestImage(callbackQuery.Message!, cancellationToken);
                        if (watermarkedImageDto != null)
                            await SendWatermarkedImage(callbackQuery.Message!, cancellationToken, watermarkedImageDto);
                        break;
                }
            }
            catch (Exception ex)
            {
                await _botClient.SendTextMessageAsync(chatId: callbackQuery.Message!.Chat.Id, text: ex.Message, cancellationToken: cancellationToken);
            }
            
        }

        private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
            return Task.CompletedTask;
        }

        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

            // Cooldown in case of network connection error
            if (exception is RequestException)
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }
}
