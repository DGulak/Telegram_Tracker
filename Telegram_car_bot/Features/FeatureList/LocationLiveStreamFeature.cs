using avtofon_api_client;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram_car_bot.Tools;

namespace Telegram_car_bot.Features.FeatureList
{
    internal class LocationLiveStreamFeature : IFeature
    {
        private readonly Tracker_Client _avtofonClient;
        private readonly ReplyKeyboardChangeTool _replyKeyboardChangeTool;
        private System.Timers.Timer _timer = new System.Timers.Timer();

        private ITelegramBotClient _bot;
        private ChatId _chatID;
        private Message _locationMessage;
        private CancellationToken _cancellationToken;

        private bool featureActive = false;
        public LocationLiveStreamFeature(ReplyKeyboardChangeTool keyboardChangeTool, Tracker_Client tracker_Client)
        {
            _avtofonClient = tracker_Client;
            _replyKeyboardChangeTool = keyboardChangeTool;
        }
        public async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            _bot = bot;

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message?.Text?.ToLower() == "транслировать геопозицию")
                {
                    var values = await _avtofonClient.GetLastUpdates();
                    await StartLocationLive(values, message.Chat, bot, cancellationToken);
                    return;
                }
                else if( message?.Text?.ToLower() == "остановить трансляцию")
                {
                    await StopLocationLive();
                }
            }
        }

        private async Task StartLocationLive(Values values, ChatId chatId, ITelegramBotClient bot, CancellationToken cancellationToken)
        {
            _timer.Interval = 60000;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            _chatID = chatId;

            _locationMessage = await bot.SendLocationAsync(chatId, GetDouble(values.lat), GetDouble(values.lng), 600, null, null, null, null, null, null, null, cancellationToken);
            _cancellationToken = cancellationToken;

            await SendNewKeyboard(chatId, bot, cancellationToken);

            featureActive = true;
        }

        private async void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // send location update
            var values = await _avtofonClient.GetLastUpdates();

            await _bot.EditMessageLiveLocationAsync(_chatID, _locationMessage.MessageId, GetDouble(values.lat), GetDouble(values.lng));

            _timer.Stop();
            _timer.Start();
        }

        private double GetDouble(string value)
        {
            float lt = float.Parse(value?.Replace('.', ','));
            return  Convert.ToDouble(lt);
        }
        private async Task SendNewKeyboard(ChatId chatId, ITelegramBotClient bot, CancellationToken cancellationToken)
        {
            List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>()
            {
                new List<KeyboardButton>()
                {
                    new KeyboardButton("Остановить трансляцию"),
                }
            };

            await _replyKeyboardChangeTool.ChangeMarkup(buttons, chatId, bot, cancellationToken);
        }

        private async Task SendOldKeyboard(ChatId chatId, ITelegramBotClient bot, CancellationToken cancellationToken)
        {
            List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>()
            {
                new List<KeyboardButton>()
                {
                    new KeyboardButton("Получить геопозицию"),
                    new KeyboardButton("Транслировать геопозицию")
                }
            };

            await _replyKeyboardChangeTool.ChangeMarkup(buttons, chatId, bot, cancellationToken);
        }

        private async Task StopLocationLive()
        {
            await _bot.StopMessageLiveLocationAsync(_chatID, _locationMessage.MessageId);

            await SendOldKeyboard(_chatID, _bot, _cancellationToken);

            featureActive = false;
        }

        public bool isFeatureActive()
        {
            return featureActive;
        }
    }

    internal class MyTimer : System.Timers.Timer
    {

        public MyTimer(ITelegramBotClient bot, CancellationToken cancellationToken)
        {

        }
    }
}
