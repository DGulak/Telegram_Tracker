using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_car_bot.DML
{
    internal interface IChat
    {
        Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);

        public ChatId ChatId { get; }

        event Action<IChat> onChatTimeout;
    }
}
