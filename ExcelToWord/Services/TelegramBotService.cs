using Serilog;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class TelegramBotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly CancellationTokenSource _cts;

    public TelegramBotService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
        _cts = new CancellationTokenSource();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await StartPollingAsync(stoppingToken);
    }

    private async Task StartPollingAsync(CancellationToken cancellationToken)
    {
        int offset = 0;
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var updates = await _botClient.GetUpdates(
                    offset: offset,
                    limit: 10,
                    timeout: 30,
                    allowedUpdates: new[] { UpdateType.Message, UpdateType.CallbackQuery },
                    cancellationToken: cancellationToken);

                foreach (var update in updates)
                {
                    try
                    {
                        await HandleUpdateAsync(update, cancellationToken);
                        offset = update.Id + 1;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error while handling update");
                    }
                }

                await Task.Delay(1000, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while polling updates");
        }
    }

    private async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;

            if (message is not null && message.Text == "/start")
            {
                await _botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"<b>Salom, {message.Chat.FirstName}!</b>\n\n" +
                          "Siz <i>Excel</i> faylini <b>Word</b> ga o'tkazishingiz mumkin. 📝\n\n",
                    parseMode: ParseMode.Html,
                    cancellationToken: _cts.Token
                );
            }
            else if(message is not null && message.Document is not null)
                await HandleExcelFileAsync(update.Message, cancellationToken);
        }
        
    }

    private async Task HandleExcelFileAsync(Message? message, CancellationToken cancellationToken)
    {
        
    }
}
