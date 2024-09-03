using System.Collections.Concurrent;
using System.Reflection;

namespace Imgbed.Core.Encoders.Pool;

public class EncoderPool : IDisposable
{
    public EncoderPool()
    {
        _totalSemaphore = new(0, (int)MaxTotalCount);
    }

    public EncoderPool(uint maxPreTypeCount, uint maxTotalCount) : this()
    {
        MaxPerTypeCount = maxPreTypeCount;
        MaxTotalCount = maxTotalCount;
    }

    public EncoderPool(string namespaceName, uint maxPreTypeCount, uint maxTotalCount) : this(maxPreTypeCount, maxTotalCount)
    {
        LoadEncodersFromNamespace(namespaceName, true);
    }

    private readonly ConcurrentDictionary<Type, ConcurrentBag<IEncoder>> _freeEncoders = new();
    private readonly ConcurrentDictionary<Type, SemaphoreSlim> _semaphores = new();

    private readonly SemaphoreSlim _totalSemaphore;

    public uint MaxPerTypeCount { get; } = 4;
    public uint MaxTotalCount { get; private set; } = 16;

    public uint CurrentFreeEncoder<T>() where T : IEncoder
        => (uint)_freeEncoders[typeof(T)].Count;

    public void LoadEncodersFromNamespace(string namespaceName, bool updateCount = true)
    {
        var encoderTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t.Namespace == namespaceName
                && typeof(IEncoder).IsAssignableFrom(t)
                && t is { IsInterface: false, IsAbstract: false })
            .ToArray();

        foreach (var type in encoderTypes)
        {
            _freeEncoders.TryAdd(type, []);
            _semaphores.TryAdd(type, new(0, (int)MaxPerTypeCount));
        }

        if (updateCount)
            MaxTotalCount = (uint)(MaxPerTypeCount * encoderTypes.Length);
    }

    /// <summary>
    /// 从池中获取编码器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(CancellationToken cancellationToken = default) where T : IEncoder, new()
    {
        var encoderType = typeof(T);

        // 进入全局信号量
        await _totalSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        // 进入解码器信号量
        await _semaphores[encoderType]
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        if (_freeEncoders[encoderType].TryTake(out var encoder))
        {
            // 获取解码器实例
            return (T)encoder;
        }

        // 新建实例
        return new();
    }

    /// <summary>
    /// 归还编码器到池中
    /// </summary>
    /// <typeparam name="T">编码器类型</typeparam>
    /// <param name="encoder">编码器实例</param>
    public void Return<T>(T encoder) where T : IEncoder
    {
        var encoderType = typeof(T);

        // 将编码器回到池中并释放信号量
        _freeEncoders[encoderType].Add(encoder);

        if (_semaphores.TryGetValue(encoderType, out var semaphore))
        {
            semaphore.Release();
        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
