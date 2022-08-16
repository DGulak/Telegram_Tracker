using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram_car_bot.Features;
using Telegram_car_bot.Tools;

namespace Telegram_car_bot.DML
{
    internal class Bot : IBot
    {
        private ITelegramBotClient _client;
        private CancellationTokenSource cts;

        private Authorization _authorization;
        private readonly IChatProcessor _chatProcessor;
        private readonly IConfiguration _config;

        public Bot(IChatProcessor chatProcessor, Authorization authorization, IConfiguration config)
        {
            _authorization = authorization;
            _chatProcessor = chatProcessor;
            _config = config;
           

        }
        public void Start()
        {
            var token = _config.GetValue<string>("tg_token");

            _client = new TelegramBotClient(token);

            cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            _client.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            Console.WriteLine("Запущен бот " + _client.GetMeAsync().Result.FirstName);
        }

        public void Stop()
        {
                
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var auth = await _authorization.isChatAuthorized(botClient, update, cancellationToken);
            if(!auth)
            {
                return;
            }

            await Task.Run(() => _chatProcessor.HandleUpdate(botClient, update, cancellationToken));
        }
        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


    }
}
