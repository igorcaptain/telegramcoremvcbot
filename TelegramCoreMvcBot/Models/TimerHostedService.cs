using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using Parser.Core;
using Parser.Core.bcpep;
using Npgsql;

namespace TelegramCoreMvcBot.Models
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        static bool isSent = false;
        const long interval = 60; // 1 iteration in minute

        private string currentReplacementDate = null;
        private string newReplacementDate = null;
        private static ParserWorker<Replacement[]> parser;

        public TimedHostedService(ILogger<TimedHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");
            _timer = new Timer(CheckReplacementUpdates, null, TimeSpan.Zero, TimeSpan.FromSeconds(interval));

            parser = new ParserWorker<Replacement[]>(
                     new BcpepParser(),
                     new BcpepSettings()
                     );
            
            return Task.CompletedTask;
        }

        private async void CheckReplacementUpdates(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");
            parser.Start();
            //DateTime from web, because time in PaaS and here is different
            //configurate Heroku to heroku config:add TZ="Europe/Kiev"
            DateTime dd = DateTime.Now;

            if (dd.Hour < 11 && currentReplacementDate == null)
            {
                currentReplacementDate = await parser.GetDate();
            }

            //if (dd.Hour == 11 && dd.Minute == 0 && isSent == false)
            if(dd.Hour >= 11 && dd.Hour <= 16 && isSent == false)
            {
                newReplacementDate = await parser.GetDate();
                //DEBUG:
                _logger.LogInformation($"Current: {currentReplacementDate}; New: {newReplacementDate}");

                if (currentReplacementDate != null && currentReplacementDate != newReplacementDate)
                {
                    using (var connection = new NpgsqlConnection(AppSettings.ConnString))
                    {
                        connection.Open();
                        string group = "";
                        Replacement result = null;
                        Replacement[] data = null;
                        ChatId chatId = null;
                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = connection;
                            cmd.CommandText = @"SELECT * FROM users";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while(await reader.ReadAsync())
                                {
                                    group = reader["groupname"].ToString();
                                    chatId = new ChatId(int.Parse(reader["chatid"].ToString()));
                                    result = null;
                                    data = await parser.GetGroups();
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
                                        {
                                            Bot.SendMessageAsync(chatId, result.ToString());
                                            //_logger.LogInformation($"Sended to {int.Parse(reader["chatid"].ToString())}");
                                        }
                                        else
                                        {
                                            Bot.SendMessageAsync(chatId, $"На {newReplacementDate} замін немає.");
                                        }
                                        isSent = true;
                                    }
                                }
                            }
                        }
                    }
                    currentReplacementDate = null;
                }
                //else
                    //_logger.LogInformation("There nothing to send! On the site non actual data");
                //Thread.Sleep(TimeSpan.FromMinutes(1));
            }
            else if (dd.Hour < 11 && dd.Hour > 16)
            {
                isSent = false;
            }
            parser.Abort();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
