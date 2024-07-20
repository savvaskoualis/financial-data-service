namespace FinancialDataService.Shared;

public class AsyncQueue<T>
{
    private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
    private readonly Queue<T> _items = new Queue<T>();

    public void Enqueue(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        lock (_items)
        {
            _items.Enqueue(item);
        }

        _signal.Release();
    }

    public async Task<T> DequeueAsync(CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);

        lock (_items)
        {
            return _items.Dequeue();
        }
    }

    public int Count
    {
        get
        {
            lock (_items)
            {
                return _items.Count;
            }
        }
    }
}