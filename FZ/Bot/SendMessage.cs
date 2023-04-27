using FZ.DB;
using FZ.WriteLogs;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace FZ.Bot
{
    public class SendMessage
    {
        public async Task SendMessageAsync(string message, string whoLog)
        {
            var inf = new InfBot();

            var botClient = new TelegramBotClient(inf.botToken);
            await botClient.SendTextMessageAsync(whoLog, message);
        }
    }
}
