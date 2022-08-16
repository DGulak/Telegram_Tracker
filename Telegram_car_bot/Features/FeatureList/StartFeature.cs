using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram_car_bot.Tools;

namespace Telegram_car_bot.Features.FeatureList
{
    internal class StartFeature : IFeature
    {
        ReplyKeyboardChangeTool _keyboardChangeTool;
        public StartFeature(ReplyKeyboardChangeTool keyboardChangeTool)
        {
            _keyboardChangeTool = keyboardChangeTool;
        }
        public async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message?.Text?.ToLower() == "/start")
                {
                    await SendKeyboard(message.Chat, bot, cancellationToken);
                    return;
                }
            }
        }

        public bool isFeatureActive()
        {
            return false;
        }

        private async Task SendKeyboard(ChatId chatId, ITelegramBotClient bot, CancellationToken cancellationToken)
        {
            List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>()
            {
                new List<KeyboardButton>()
                {
                    new KeyboardButton("Получить геопозицию"),
                    new KeyboardButton("Транслировать геопозицию")
                }
            };
            await _keyboardChangeTool.ChangeMarkup(buttons, chatId, bot, cancellationToken);   
        }
    }
}
