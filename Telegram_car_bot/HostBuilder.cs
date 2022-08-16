using avtofon_api_client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Telegram_car_bot.DML;
using Telegram_car_bot.Features;
using Telegram_car_bot.Features.FeatureList;
using Telegram_car_bot.Tools;

namespace Telegram_car_bot
{
    public class HostBuilder
    {
        public static IHost? host;
        public static IHost Build()
        {

            return host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    //services.AddSingleton<IServiceProvider>();
                    services.AddSingleton(context.Configuration);
                    services.AddLogging();

                    // add tools 
                    services.AddSingleton<Authorization>();
                    services.AddTransient(typeof(ReplyKeyboardChangeTool));

                    //add DML
                    services.AddSingleton<IBot, Bot>();
                    services.AddSingleton<IChatProcessor, ChatProcessor>();
                    services.AddTransient<IChat, Chat>();

                    //add tracker client
                    services.AddTransient<Tracker_Client>();

                    // add features
                    services.TryAddEnumerable(new[]
                    {
                        ServiceDescriptor.Transient<IFeature, LocationLiveStreamFeature>(),
                        ServiceDescriptor.Transient<IFeature, StartFeature>(),
                        ServiceDescriptor.Transient<IFeature, SendGeoFeature>()
                    }
                    );
                })
                .Build();
        }
    }
}
