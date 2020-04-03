using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Drawing;
using System.IO;


using FFmpeg.AutoGen;
using videoPlayerApp.FFmpeg.Decoder;
using videoPlayerApp.FFmpeg;

namespace videoPlayerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread thread;
        ThreadStart ts;
        Dispatcher dispatcher = Application.Current.Dispatcher;

        private bool activeThread;
        String _filepath;

        public MainWindow()
        {
            InitializeComponent();

            ts = new ThreadStart(DecodeAllFramesToImages);
            thread = new Thread(ts);

            FFmpegBinarisHelper.RegisterFFmpegBinries();

            activeThread = true;            
        }

        private unsafe void DecodeAllFramesToImages()
        {
            String filepath = "";
            if ( String.IsNullOrEmpty(_filepath))
            {
                filepath = _filepath;
            } else
            {
                return;
            }
            using (var vsd = new VideoStreamDecoder(filepath))
            {
                var info = vsd.GetContextInfo();
                info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

                var sourceSize = vsd.FrameSize;
                var sourcePixelFormat = vsd.PixelFormat;
                var destnationSize = sourceSize;
                var destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
                using ( var vfc = new VideoFrameConveter(sourceSize, sourcePixelFormat, destnationSize, destinationPixelFormat))
                {
                    var frameNumber = 0;
                    while(vsd.TryDecodeNextFrame(out var frame) && activeThread)
                    {
                        var convertedFrame = vfc.Convert(frame);
                        
                    }
                }

            }            
        }

        private void Btn_Play_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ( thread.IsAlive)
            {
                activeThread = false;
                thread.Join();
            }
        }
    }
}
