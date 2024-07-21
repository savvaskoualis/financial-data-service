namespace FinancialDataService.Application.Interfaces
{
    public interface IBackplane
    {
        Task PublishAsync(string message);
        Task SubscribeAsync(Func<string, Task> onMessageReceived);
    }
}