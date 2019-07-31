using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramCoreMvcBot.Commands;

namespace TelegramCoreMvcBot.Models
{
    public class Bot
    {
        private static TelegramBotClient botClient;
        private static List<Command> commandsList;
        public static IReadOnlyList<Command> Comands => commandsList.AsReadOnly();
        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if(botClient != null)
            {
                return botClient;
            }
            commandsList = new List<Command>();
            commandsList.Add(new StartCommand());
            commandsList.Add(new ReplacementsCommand());
            commandsList.Add(new AutoReplacementsCommand());
            commandsList.Add(new StopCommand());

            botClient = new TelegramBotClient(AppSettings.Key);
            string hook = string.Format(AppSettings.Url, "api/message/update");
            await botClient.SetWebhookAsync(hook);
            return botClient;
        }
        
        public static async void SendMessageAsync(ChatId chatId, string msg)
        {
            await botClient.SendTextMessageAsync(chatId, msg, parseMode: Telegram.Bot.Types.Enums.ParseMode.Default);
        }
    }
}
