using System.Collections.Concurrent;
using System.Reflection;
using Imgbed.Core.Encoders;
using Imgbed.Core.Options;

namespace Imgbed.Core;

public sealed class EncoderPool : IDisposable
{
    public EncoderPool()
    {
        _totalSemaphore = new(0, (int)MaxTotalCount);
    }

    public EncoderPool(uint maxPreEncoderCount, uint maxTotalCount)
        : this()
    {
        MaxPerEncoderCount = maxPreEncoderCount;
        MaxTotalCount = maxTotalCount;
    }

    public EncoderPool(ICollection<string> namespaceList, uint maxPreEncoderCount, uint maxTotalCount)
        : this(maxPreEncoderCount, maxTotalCount)
    {
        LoadEncodersFromNamespace(namespaceList, true);
    }

    public EncoderPool(EncoderPoolOption option)
        : this(option.NamespaceList, option.MaxPreEncoderCount, option.MaxTotalCount)
    { }

    private readonly ConcurrentDictionary<Type, ConcurrentBag<IEncoder>> _freeEncoders = new();
    private readonly ConcurrentDictionary<Type, SemaphoreSlim> _semaphores = new();

    private readonly SemaphoreSlim _totalSemaphore;

    public uint MaxPerEncoderCount { get; } = 4;
    public uint MaxTotalCount { get; private set; } = 16;

    public uint CurrentFreeEncoder<T>() where T : IEncoder
        => (uint)_freeEncoders[typeof(T)].Count;

    public void LoadEncodersFromNamespace(ICollection<string> namespaceList, bool updateCount = true)
    {
        var encoderTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.Namespace is not null
                        && namespaceList.Contains(t.Namespace))
            .Where(t => typeof(IEncoder).IsAssignableFrom(t)
                        && t is { IsInterface: false, IsAbstract: false })
            .ToArray();

        foreach (var type in encoderTypes)
        {
            _freeEncoders.TryAdd(type, []);
            _semaphores.TryAdd(type, new(0, (int)MaxPerEncoderCount));
        }

        if (updateCount)
        {
            MaxTotalCount = (uint)(MaxPerEncoderCount * encoderTypes.Length);
        }
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
        
    }
}
