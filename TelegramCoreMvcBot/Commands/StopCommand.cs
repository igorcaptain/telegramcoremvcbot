using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Npgsql;
using TelegramCoreMvcBot.Models;

namespace TelegramCoreMvcBot.Commands
{
    public class StopCommand : Command
    {
        public override string Name => @"/stop";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;
            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            string botMsg = "Error: Невідома помилка!";
            using (var connection = new NpgsqlConnection(AppSettings.ConnString))
            {
                connection.Open();
                string command = "";
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = @"SELECT * FROM users WHERE chatid=" + chatId;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            command = @"DELETE FROM users WHERE id=" + reader["id"];
                            botMsg = "Ви відписались від автоматичних оновлень замін!";
                        }
                        else
                        {
                            command = @"";
                            botMsg = "Ви не були підписані на автоматичні оновлення замін!";
                            return;
                        }
                    }
                    cmd.CommandText = command;
                    if (await cmd.ExecuteNonQueryAsync() == 0)
                        botMsg = "Error: Не вдалося виконати запит!";
                }
            }
            await botClient.SendTextMessageAsync(chatId, botMsg, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}
