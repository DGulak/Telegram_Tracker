using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_car_bot.Features
{
    // Implementation must have zero args constructor only
    internal interface IFeature 
    {
        Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken cancellationToken);
        bool isFeatureActive();
    }
}
