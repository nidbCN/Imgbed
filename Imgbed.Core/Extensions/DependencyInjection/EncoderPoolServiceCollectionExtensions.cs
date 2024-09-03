using Imgbed.Core.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Imgbed.Core.Extensions.DependencyInjection;

public static class EncoderPoolServiceCollectionExtensions
{
    public static IServiceCollection AddEncoderPool(this IServiceCollection services, Action<EncoderPoolOption>? configureAction = null)
    {
        services.AddSingleton(_ =>
        {
            var option = new EncoderPoolOption();
            configureAction?.Invoke(option);

            var encoderPool = new EncoderPool(option);

            return encoderPool;
        });

        return services;
    }
}
