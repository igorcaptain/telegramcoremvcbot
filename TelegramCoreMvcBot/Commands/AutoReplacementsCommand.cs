using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Parser.Core;
using Parser.Core.bcpep;

using TelegramCoreMvcBot.Models;
using Npgsql;

namespace TelegramCoreMvcBot.Commands
{
    public class AutoReplacementsCommand : Command
    {
        public override string Name => @"/autoreplacements";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;
            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            string group = (message.Text.Length > Name.Length + 1) ? message.Text.Substring(Name.Length + 1).Trim() : "";
            string botMsg = "Error: Некоректні дані!";
            if (group.Length < 10 && group.Length > 2)
            {
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
                                command = @"UPDATE users SET groupname='" + group + "' WHERE id=" + reader["id"];
                                botMsg = "Вашу групу оновлено! Тепер ви будете отримувати заміни для " + group;
                            }
                            else
                            {
                                command = @"INSERT INTO users (chatid, groupname) VALUES(" + chatId + ", '" + group + "')";
                                botMsg = "Готово! Тепер ви будете отримувати заміни для " + group;
                            }
                        }
                        cmd.CommandText = command;
                        if (await cmd.ExecuteNonQueryAsync() == 0)
                            botMsg = "Error: Не вдалося виконати запит!";
                    }
                }
            }
            await botClient.SendTextMessageAsync(chatId, botMsg, parseMode: Telegram.Bot.Types.Enums.ParseMode.Default);
        }
    }
}
