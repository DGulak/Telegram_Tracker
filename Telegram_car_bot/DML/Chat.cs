using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram_car_bot.Features;

namespace Telegram_car_bot.DML
{
    internal class Chat : IChat
    {
        public event Action<IChat>? onChatTimeout;
        private System.Timers.Timer _timeoutTimer = new System.Timers.Timer();
        private double timeout = 5 * 60 * 1000; //5 minute timer;

        private ITelegramBotClient? _botClient;
        private ChatId _chatId;
        public ChatId ChatId { get { return _chatId; } }

        private readonly IEnumerable<IFeature> _features;
        public Chat(IEnumerable<IFeature> features)
        {
            _features = features;
            _timeoutTimer.Interval = timeout;
            _timeoutTimer.Elapsed += _timeoutTimer_Elapsed;
            _timeoutTimer.Start();
        }

        private void _timeoutTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _timeoutTimer.Elapsed -= _timeoutTimer_Elapsed;
            _timeoutTimer.Stop();
            _timeoutTimer.Dispose();

            if (_botClient != null && _chatId != null)
                _botClient.SendTextMessageAsync(_chatId, "До свидания!");

            onChatTimeout?.Invoke(this);
        }

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if(_botClient is null & _chatId is null)
            {
                _botClient = botClient;
                _chatId = update?.Message?.Chat;
            }

            _timeoutTimer.Stop();
            _timeoutTimer.Start();

            Parallel.ForEach(_features, (feature) =>
            {
                try
                {
                    feature.HandleUpdate(botClient, update, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
        }
    }
}
