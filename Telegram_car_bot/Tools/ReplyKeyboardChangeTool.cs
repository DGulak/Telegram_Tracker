using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram_car_bot.Tools
{
    internal class ReplyKeyboardChangeTool
    {
        Mutex mutex = new Mutex();
        public async Task ChangeMarkup(IEnumerable<IEnumerable<KeyboardButton>> buttons, ChatId chatId, ITelegramBotClient bot, CancellationToken cancellationToken)
        {
            try
            {
                mutex.WaitOne();
                ReplyKeyboardMarkup keyboardMarkup = new ReplyKeyboardMarkup(buttons);
                await bot.SendTextMessageAsync(chatId, "Вот, что можно сделать", null, null, null, null, null, null, null, keyboardMarkup, cancellationToken);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
