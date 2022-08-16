
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_car_bot.DML
{
    internal interface IChatProcessor
    {
        Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
    }
}
