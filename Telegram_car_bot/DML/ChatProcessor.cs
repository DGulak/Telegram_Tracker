using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_car_bot.DML
{
    internal class ChatProcessor : IChatProcessor
    {
        private List<IChat> _chats = new List<IChat>();

        private readonly IServiceProvider _serviceProvider;
        private readonly Mutex mutex = new Mutex();
        public ChatProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            mutex.WaitOne();

            try
            {
                var chat = update?.Message?.Chat;

                if (chat is not null)
                {
                    var activeChat = _chats.FirstOrDefault(c => c.ChatId == chat);
                    if (activeChat is null)
                    {
                        activeChat = _serviceProvider.GetService(typeof(IChat)) as IChat;
                        activeChat.onChatTimeout += ActiveChat_onChatTimeout;

                        _chats.Add(activeChat);
                    }
                    
                    Task.Run(() => activeChat.HandleUpdate(botClient, update, cancellationToken));
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private void ActiveChat_onChatTimeout(IChat chat)
        {
            chat.onChatTimeout -= ActiveChat_onChatTimeout;
            _chats.Remove(chat);
        }
    }
}
