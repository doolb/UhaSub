using CSCore;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Streams.Effects;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WinformsVisualization;
using WinformsVisualization.Visualization;

namespace UhaSub
{
    /// <summary>
    /// Interaction logic for Audio.xaml
    /// </summary>
    public partial class Audio : UserControl
    {

        public Audio()
        {
            this.InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += OnTick;

            OpenDefault();
        }

        public void Close()
        {
            Stop();
        }


        private WasapiCapture _soundIn;
        private ISoundOut _soundOut;
        private IWaveSource _source;
        private PitchShifter _pitchShifter;
        private LineSpectrum _lineSpectrum;

        private readonly System.Drawing.Bitmap _bitmap = 
            new System.Drawing.Bitmap(200, 60);

        private DispatcherTimer timer;

       
        private void OpenDefault()
        {
            Stop();

            //open the default device 
            _soundIn = new WasapiLoopbackCapture();
            //Our loopback capture opens the default render device by default so the following is not needed
            //_soundIn.Device = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Console);
            _soundIn.Initialize();

            var soundInSource = new SoundInSource(_soundIn);
            ISampleSource source = soundInSource.ToSampleSource().AppendSource(x => new PitchShifter(x), out _pitchShifter);

            SetupSampleSource(source);

            // We need to read from our source otherwise SingleBlockRead is never called and our spectrum provider is not populated
            byte[] buffer = new byte[_source.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += (s, aEvent) =>
            {
                int read;
                while ((read = _source.Read(buffer, 0, buffer.Length)) > 0) ;
            };


            //play the audio
            _soundIn.Start();

            timer.Start();

        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aSampleSource"></param>
        private void SetupSampleSource(ISampleSource aSampleSource)
        {
            const FftSize fftSize = FftSize.Fft4096;
            //create a spectrum provider which provides fft data based on some input
            var spectrumProvider = new BasicSpectrumProvider(aSampleSource.WaveFormat.Channels,
                aSampleSource.WaveFormat.SampleRate, fftSize);

            //linespectrum and voiceprint3dspectrum used for rendering some fft data
            //in oder to get some fft data, set the previously created spectrumprovider 
            _lineSpectrum = new LineSpectrum(fftSize)
            {
                SpectrumProvider = spectrumProvider,
                UseAverage = true,
                BarCount = 25,
                BarSpacing = 2,
                IsXLogScale = true,
                ScalingStrategy = ScalingStrategy.Sqrt
            };
            

            //the SingleBlockNotificationStream is used to intercept the played samples
            var notificationSource = new SingleBlockNotificationStream(aSampleSource);
            //pass the intercepted samples as input data to the spectrumprovider (which will calculate a fft based on them)
            notificationSource.SingleBlockRead += (s, a) => spectrumProvider.Add(a.Left, a.Right);

            _source = notificationSource.ToWaveSource(16);

        }


        private void Stop()
        {
            timer.Stop();

            if (_soundOut != null)
            {
                _soundOut.Stop();
                _soundOut.Dispose();
                _soundOut = null;
            }
            if (_soundIn != null)
            {
              _soundIn.Stop();
              _soundIn.Dispose();
              _soundIn = null;
            }
            if (_source != null)
            {
                _source.Dispose();
                _source = null;
            }


        }

        private void OnTick(object sender, EventArgs e)
        {
            //render the spectrum
            GenerateLineSpectrum();
        }

        private void GenerateLineSpectrum()
        {
            System.Drawing.Image image = wave_box.Image;
            var newImage = _lineSpectrum.CreateSpectrumLine(wave_box.Size,
                System.Drawing.Color.Green, System.Drawing.Color.Red, System.Drawing.Color.Black, true);
            if (newImage != null)
            {
                wave_box.Image = newImage;
                if (image != null)
                    image.Dispose();
            }
        }

 


    }
}
