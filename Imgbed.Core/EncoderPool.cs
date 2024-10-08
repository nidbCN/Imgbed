﻿using System.Collections.Concurrent;
using System.Reflection;
using Imgbed.Core.Encoders;
using Imgbed.Core.Options;

namespace Imgbed.Core;

public sealed class EncoderPool : IDisposable
{
    #region 构造函数
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
    #endregion

    #region 字段
    private readonly ConcurrentDictionary<Type, ConcurrentBag<IEncoder>> _freeEncodersPool = new();


    /* Use ConcurrentDictionary instead of HashSet because:
     * > Closing since the benefits over using the ConcurrentDictionary<T, byte> workaround
     * > or a third-party package don't seem to outweigh the cost of authoring a new collection type.
     * > We might revisit in the future if more evidence comes up.
     * https://github.com/dotnet/runtime/issues/39919#issuecomment-954774092
     */
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<IEncoder, byte>> _inUseEncodersPool = new();
    private readonly ConcurrentDictionary<Type, SemaphoreSlim> _semaphores = new();

    private readonly SemaphoreSlim _totalSemaphore;
    #endregion

    #region 属性
    public uint MaxPerEncoderCount { get; } = 4;
    public uint MaxTotalCount { get; private set; } = 16;

    public uint CurrentFreeEncoder<T>() where T : IEncoder
        => (uint)_freeEncodersPool[typeof(T)].Count;
    #endregion

    #region 私有方法
    private void LoadEncodersFromNamespace(ICollection<string> namespaceList, bool updateCount = true)
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
            _freeEncodersPool.TryAdd(type, []);
            _inUseEncodersPool.TryAdd(type, []);

            _semaphores.TryAdd(type, new(0, (int)MaxPerEncoderCount));
        }

        if (updateCount)
        {
            MaxTotalCount = (uint)(MaxPerEncoderCount * encoderTypes.Length);
        }
    }
    #endregion

    #region 公开方法

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

        // 获取解码器实例
        var returnEncoder = _freeEncodersPool[encoderType].TryTake(out var encoder)
            ? (T)encoder
            : new();

        _inUseEncodersPool[encoderType].TryAdd(returnEncoder, 0x00);

        return returnEncoder;
    }

    /// <summary>
    /// 归还编码器到池中
    /// </summary>
    /// <typeparam name="T">编码器类型</typeparam>
    /// <param name="encoder">编码器实例</param>
    public void Return<T>(T encoder) where T : IEncoder
    {
        var encoderType = typeof(T);



        if (!_inUseEncodersPool[encoderType].TryRemove(encoder, out _))
        {
            throw new NotSupportedException("What are you fucking doing return a non-pool encoder.");
        }

        // 将编码器回到池中并释放信号量
        _freeEncodersPool[encoderType].Add(encoder);

        if (_semaphores.TryGetValue(encoderType, out var semaphore))
        {
            semaphore.Release();
        }
    }

    public void Dispose()
    {
        // _semaphores.Keys, _freeEncodersPool.Keys and _inUseEncodersPool.Keys is equal here
        foreach (var encoderType in _semaphores.Keys)
        {
            var semaphore = _semaphores[encoderType];

            // 锁住全部编码器的信号数
            semaphore.Wait((int)MaxPerEncoderCount);

            // 有信号时说明编码器已经全部退还
            foreach (var encoder in _freeEncodersPool[encoderType])
            {
                encoder.Dispose();
            }

            semaphore.Release((int)MaxPerEncoderCount);
        }
    }
    #endregion
}
