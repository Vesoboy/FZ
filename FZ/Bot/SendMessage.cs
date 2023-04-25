using System.Threading.Tasks;
using Telegram.Bot;

namespace FZ.Bot
{
    public class SendMessage
    {
        public async Task SendMessageAsync(string message)
        {
            var inf = new InfBot();

            var botClient = new TelegramBotClient(inf.botToken);
            await botClient.SendTextMessageAsync(inf.chatId, message);
        }
    }
}
