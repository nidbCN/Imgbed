using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace Imgbed.Core.Extensions;

public static class FFMpegExtension
{
    public static unsafe string? ErrorToString(int error)
    {
        /* 错误信息的 buffer
         * ffmpeg 最长的错误信息长度为60+1
         * 参加：https://ffmpeg.org/doxygen/6.1/error_8c_source.html
         */
        const int bufferSize = 64;

        var buffer = stackalloc byte[bufferSize];
        ffmpeg.av_strerror(error, buffer, bufferSize);
        var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
        return message;
    }

    public static int ThrowExceptionIfError(this int error, Action<Exception>? catchInvoke = null)
    {
        if (error >= 0) return error;

        var e = new ApplicationException(ErrorToString(error));
        catchInvoke?.Invoke(e);
        throw e;
    }
}
