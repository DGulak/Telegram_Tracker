using avtofon_api_client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram_car_bot.Features.FeatureList
{
    internal class SendGeoFeature : IFeature
    {
        private Tracker_Client _avtofonClient;

        public SendGeoFeature(Tracker_Client tracker_Client)
        {
            _avtofonClient = tracker_Client;
        }
        public async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message?.Text?.ToLower() == "получить геопозицию")
                {
                    var values = await _avtofonClient.GetLastUpdates();
                    await SendGeo(values, message.Chat, bot, cancellationToken);
                    return;
                }
            }
        }

        private async Task SendGeo(Values? values, ChatId chatId, ITelegramBotClient bot, CancellationToken cancellationToken)
        {
            double lat;
            double lng;
            try
            {
                float lt = float.Parse(values?.lat?.Replace('.',','));
                float lg = float.Parse(values?.lng?.Replace('.', ','));

                lat = Convert.ToDouble(lt);
                lng = Convert.ToDouble(lg);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await SendError(chatId, bot, cancellationToken);
                return;
            }

            await bot.SendLocationAsync(chatId, lat, lng, null, null, null, null, null, null, null, null, cancellationToken);
        }

        private async Task SendError(ChatId chatId, ITelegramBotClient bot, CancellationToken cancellationToken)
        {
            await bot.SendTextMessageAsync(chatId, "Не удалось получить координаты", null,null,null,null,null,null,null,null, cancellationToken);
        }

        public bool isFeatureActive()
        {
            return false;
        }
    }
}
