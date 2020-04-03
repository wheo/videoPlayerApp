using System;
using System.Runtime.InteropServices;
using System.Windows;
using FFmpeg.AutoGen;

namespace videoPlayerApp.FFmpeg.Decoder
{
    public sealed unsafe class VideoFrameConveter : IDisposable
    {
        //sealed : 상속을 못하게 함(상속해도 쓸 수 없음)
        //unsafe : 포인터를 사용할 때

        private readonly IntPtr _convertedFrameBufferPtr;
        private readonly Size _destinationSize;
        private readonly byte_ptrArray4 _dstData;
        private readonly int_array4 _dstLineSize;
        private readonly SwsContext* _pConvertContext;

        public VideoFrameConveter(Size sourceSize, AVPixelFormat sourcePixelFormat, Size destinationSize, AVPixelFormat destinationPixelFormat)
        {
            _destinationSize = destinationSize;
            _pConvertContext = ffmpeg.sws_getContext((int)sourceSize.Width, (int)sourceSize.Height, sourcePixelFormat,
                (int)destinationSize.Width, (int)destinationSize.Height, destinationPixelFormat,
                ffmpeg.SWS_FAST_BILINEAR, null, null, null);

            var convertedFrameBufferSize = ffmpeg.av_image_get_buffer_size(destinationPixelFormat, (int)destinationSize.Width, (int)destinationSize.Height, 1);
            _convertedFrameBufferPtr = Marshal.AllocHGlobal(convertedFrameBufferSize);
            _dstData = new byte_ptrArray4();
            _dstLineSize = new int_array4();

            ffmpeg.av_image_fill_arrays(ref _dstData, ref _dstLineSize, (byte*)_convertedFrameBufferPtr, destinationPixelFormat,
                (int)destinationSize.Width, (int)destinationSize.Height, 1);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_convertedFrameBufferPtr);
            ffmpeg.sws_freeContext(_pConvertContext);
        }

        public AVFrame Convert(AVFrame sourceFrame)
        {
            ffmpeg.sws_scale(_pConvertContext, sourceFrame.data, sourceFrame.linesize, 0, sourceFrame.height, _dstData, _dstLineSize);

            var data = new byte_ptrArray8();
            data.UpdateFrom(_dstData);
            var linesize = new int_array8();
            linesize.UpdateFrom(_dstLineSize);

            return new AVFrame
            {
                data = data,
                linesize = linesize,
                width = (int)_destinationSize.Width,
                height = (int)_destinationSize.Height
            };
        }        
    }
}
