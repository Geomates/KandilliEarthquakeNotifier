using Common.Models;
using Common.Services;
using KandilliEarthquakeBot.Enums;
using KandilliEarthquakeBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KandilliEarthquakeBot.Services
{
    public interface IBotService
    {
        Task<bool> AskLocationAsync(int chatId);
        Task<bool> AskMagnitudeAsync(int chatId);
        Task<bool> SetMagnitudeAsync(string callbackQueryId, int chatId, double magnitude);
        Task<bool> SetLocationAsync(int chatId, Location location);
        Task<bool> StartSubscriptionAsync(int chatId);
        Task<bool> RemoveSubscriptionAsync(int chatId);
    }

    public class BotService : IBotService
    {
        private readonly ISubscribtionStore _subscribtionStore;
        private readonly ITelegramService _telegramService;
        private readonly ISubscriptionUpdateRequestBuilder _updateRequestBuilder;

        public BotService(ISubscribtionStore subscribtionStore, ITelegramService telegramService, ISubscriptionUpdateRequestBuilder updateRequestBuilder)
        {
            _subscribtionStore = subscribtionStore;
            _updateRequestBuilder = updateRequestBuilder;
            _telegramService = telegramService;
        }

        public async Task<bool> StartSubscriptionAsync(int chatId)
        {
            var updateRequest = _updateRequestBuilder
                                    .CreateRequest(chatId)
                                    .Build();

            var updateResult = await _subscribtionStore.UpdateAsync(updateRequest);

            if (!updateResult)
            {
                return false;
            }

            var message = new TelegramMessage
            {
                ChatId = chatId.ToString(),
                Text = BotDialog.START_SUBSCRIPTION
            };

            return await _telegramService.SendMessage(message);
        }

        public async Task<bool> RemoveSubscriptionAsync(int chatId)
        {
            var updateRequest = _updateRequestBuilder
                                    .CreateRequest(chatId)
                                    .Build();

            var updateResult = await _subscribtionStore.RemoveAsync(updateRequest);

            if (!updateResult)
            {
                return false;
            }

            var message = new TelegramMessage
            {
                ChatId = chatId.ToString(),
                Text = BotDialog.REMOVE_SUBSCRIPTION
            };

            return await _telegramService.SendMessage(message);
        }

        public Task<bool> AskMagnitudeAsync(int chatId)
        {
            var replyMarkup = new TelegramInlineKeyboardMarkup
            {
                InlineKeyboard = new List<IEnumerable<TelegramInlineKeyboardButton>>{
                    new List<TelegramInlineKeyboardButton>
                    {
                        new TelegramInlineKeyboardButton { Text = "Hepsi", CallBackData = "0" },
                        new TelegramInlineKeyboardButton { Text = "3.0+", CallBackData = "3" },
                        new TelegramInlineKeyboardButton { Text = "4.0+", CallBackData = "4" },
                        new TelegramInlineKeyboardButton { Text = "5.0+", CallBackData = "5" },
                        new TelegramInlineKeyboardButton { Text = "6.0+", CallBackData = "6" }
                    }
                }
            };


            var message = new TelegramMessage
            {
                ChatId = chatId.ToString(),
                Text = BotDialog.ASK_MAGNITUDE,
                ReplyMarkup = JsonConvert.SerializeObject(replyMarkup)
            };

            return _telegramService.SendMessage(message);
        }

        public async Task<bool> SetMagnitudeAsync(string callbackQueryId, int chatId, double magnitude)
        {
            var updateRequest = _updateRequestBuilder
                                    .CreateRequest(chatId)
                                    .SetMagnitude(magnitude)
                                    .Build();

            var updateResult = await _subscribtionStore.UpdateAsync(updateRequest);

            if (!updateResult)
            {
                return false;
            }

            var message = new AnswerCallbackQuery
            {
                CallbackQueryId = callbackQueryId,
                Text = string.Format(BotDialog.REPLY_MAGNITUDE, magnitude)
            };

            return await _telegramService.AnswerCallbackQuery(message);
        }

        public async Task<bool> SetLocationAsync(int chatId, Location location)
        {
            var updateRequest = _updateRequestBuilder
                                    .CreateRequest(chatId)
                                    .SetLocation(location)
                                    .Build();

            var updateResult = await _subscribtionStore.UpdateAsync(updateRequest);

            if (!updateResult)
            {
                return false;
            }

            var message = new TelegramMessage
            {
                ChatId = chatId.ToString(),
                Text = BotDialog.REPLY_LOCATION
            };

            return await _telegramService.SendMessage(message);
        }

        public Task<bool> AskLocationAsync(int chatId)
        {
            var message = new TelegramMessage
            {
                ChatId = chatId.ToString(),
                Text = BotDialog.ASK_LOCATION
            };

            return _telegramService.SendMessage(message);
        }
    }
}
