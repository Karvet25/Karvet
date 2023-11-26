using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("6736482441:AAGlABy5Z63-8QYpAig778J1nQ-hZ3Isck4");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    if (messageText == "Проверка")
    {
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Проверка бота: работа корректна",
            cancellationToken: cancellationToken);
    }

    if (messageText == "Привет")
    {
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Здравствуй, пользователь ",
            cancellationToken: cancellationToken);
    }

    if (messageText == "Картинка")
    {
        await botClient.SendPhotoAsync(
            chatId: chatId,
            photo: InputFile.FromUri("https://cojo.ru/wp-content/uploads/2022/12/shevrole-impala-1967-7.webp"),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }
    if (messageText == "Видео")
    { 
        await botClient.SendVideoAsync(
        chatId: chatId,
        video: InputFile.FromUri("https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-countdown.mp4"),
        thumbnail: InputFile.FromUri("https://raw.githubusercontent.com/TelegramBots/book/master/src/2/docs/thumb-clock.jpg"),
        supportsStreaming: true,
        cancellationToken: cancellationToken);
    }

     if (messageText == "Стикер")
     {
        Message message1 = await botClient.SendStickerAsync(
    chatId: chatId,
    sticker: InputFile.FromUri("https://stickerpacks.ru/wp-content/uploads/2023/02/nabor-stikerov-s-mashinami-10-dlja-telegram-19.webp"),
    cancellationToken: cancellationToken);

        
    }
    if (messageText == "Опрос")
    {
       await botClient.SendPollAsync(
       chatId: chatId,
       question: "Karvet топ?",
       options: new[]
       {
        "Yes",
        "No"
       },
       cancellationToken: cancellationToken);
    }


}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}