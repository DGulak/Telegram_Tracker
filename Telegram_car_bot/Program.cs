using Telegram_car_bot.DML;

namespace Telegram_car_bot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = HostBuilder.Build();

            IBot? bot = (IBot?)host.Services.GetService(typeof(IBot));

            bot?.Start();

            Console.WriteLine("Bot started");
            Console.ReadLine();

            bot?.Stop();

            Console.WriteLine("Bot stopped");
            Console.ReadLine();
        }
    }
}