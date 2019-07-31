using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramCoreMvcBot.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;
            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            string botMsg = "Вас вітає *BOT*, для роботи з ним скористайтесь командами:\n" +
                            "*/replacements* _ГРУПА_ – отримати заміни для вказаної групи;\n" +
                            "*/autoreplacements* _ГРУПА_ – підписатись на автоматичні оновлення замін для вказаної групи;\n" +
                            "*/stop* – відписатись від оновлень.\n" +
                            "_ПРИМІТКА_. Назву групи вказувати з великої літери з кодом через через дефіс (*П-121*, *Ю-141*) або тільки код (*121*, *141*).";
            await botClient.SendTextMessageAsync(chatId, botMsg, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: new ReplyKeyboardRemove() ); ;
        }
    }
}
