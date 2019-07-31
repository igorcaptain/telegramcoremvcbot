using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Parser.Core;
using Parser.Core.bcpep;

namespace TelegramCoreMvcBot.Commands
{
    public class ReplacementsCommand : Command
    {
        private static ParserWorker<Replacement[]> parser;

        public override string Name => @"/replacements";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;
            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            parser = new ParserWorker<Replacement[]>(
                     new BcpepParser(),
                     new BcpepSettings()
                     );
            parser.Start();
            
            string group = (message.Text.Length > Name.Length + 1) ? message.Text.Substring(Name.Length + 1).Trim() : "";
            string botMsg = "Error: Некоректні дані!";
            //Replacement[] data = null;
            Replacement result = null;
            Replacement[] data = await parser.GetGroups();
            if (group.Length < 10 && group.Length > 2)
            {
                if (data != null)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i].Group.Contains(group))
                        {
                            result = data[i];
                            break;
                        }
                    }
                    if (result != null)
                        //await botClient.SendTextMessageAsync(chatId, result.ToString(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        botMsg = result.ToString();
                    else
                        //await botClient.SendTextMessageAsync(chatId, $"Error: Не вдалося знайти групу \"{group}\"", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        botMsg = $"Error: Не вдалося знайти групу \"{group}\"!";
                }
                else
                {
                    //await botClient.SendTextMessageAsync(chatId, $"Error: Помилка підключення до bcpep.org.ua", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    botMsg = $"Error: Помилка підключення до bcpep.org.ua!";
                }
            }
            await botClient.SendTextMessageAsync(chatId, botMsg, parseMode: Telegram.Bot.Types.Enums.ParseMode.Default);
            parser.Abort();
        }
    }
}
