using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_car_bot.Tools
{
    internal class Authorization
    {
        private Mutex _mutex = new Mutex();

        IConfiguration _config;
        public Authorization(IConfiguration config)
        {
            _config = config;
        }
        public async Task<bool> isChatAuthorized(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            _mutex.WaitOne();

            var users = _config.GetSection("AllowedUsers").Get<string[]>();

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                try
                {
                    if (users.Contains(update?.Message?.Chat?.Username ?? "nonAuthorized"))
                    {
                        return true;
                    }
                    else
                    {
                        SendDeny(bot, update, cancellationToken);
                        return false;
                    }
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            }
            else
            {
                return false;
            }
        }

        private async void SendDeny(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            await bot.SendTextMessageAsync(update?.Message?.Chat, "You can not use this bot");
        }
    }
}
