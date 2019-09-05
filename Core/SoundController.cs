using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using JM.LinqFaster;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using SharpDX.XAudio2.Fx;

namespace Exile
{
    public class MyWave
    {
        public AudioBuffer Buffer { get; set; }
        public uint[] DecodedPacketsInfo { get; set; }
        public WaveFormat WaveFormat { get; set; }
    }

    public class SoundController : IDisposable
    {
        private bool initialized;
        XAudio2 xAudio2;
        MasteringVoice masteringVoice;

        private Dictionary<string, MyWave> Sounds;
        string soundsDir;

        public SoundController(string dir) {
            soundsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
            if (!Directory.Exists(soundsDir))
            {
                initialized = false;
                DebugWindow.LogError("Sounds dir not found, continue working without any sound.");
                return;
            }

            xAudio2 = new XAudio2();
            xAudio2.StartEngine();
            masteringVoice = new MasteringVoice(xAudio2);
            /*var reverb = new Reverb(xAudio2);
            var effectDescriptor = new EffectDescriptor(reverb);
            masteringVoice.SetEffectChain(effectDescriptor);
            masteringVoice.EnableEffect(0);*/
            var soundFiles = Directory.GetFiles(soundsDir, "*.wav");
            Sounds = new Dictionary<string, MyWave>(soundFiles.Length);

        /*
            foreach (var file in soundFiles)
            {
                var fileInfo = new FileInfo(file);
                var soundStream = new SoundStream(File.OpenRead(file));
                var waveFormat = soundStream.Format;

                var buffer = new AudioBuffer()
                {
                    Stream = soundStream.ToDataStream(), AudioBytes = (int) soundStream.Length, Flags = BufferFlags.EndOfStream
                };
                soundStream.Close();
                Sounds[fileInfo.Name.Split('.').First()] = new MyWave()
                {
                    Buffer = buffer, WaveFormat = waveFormat, DecodedPacketsInfo = soundStream.DecodedPacketsInfo
                };
            }
*/

            initialized = true;
        }

        List<SourceVoice> _list = new List<SourceVoice>();

        public void PlaySound(string name) {
            if (!initialized)
                return;
            if (Sounds.TryGetValue(name, out var wave))
            {
                if (wave == null)
                {
                   wave = LoadSound(name);
                }

            }
            else
            {
                wave = LoadSound(name);
            }

            if (wave == null)
            {
                DebugWindow.LogError($"Sound file: {name}.wav not found.");
                return;
            }
            var sourceVoice = new SourceVoice(xAudio2, wave.WaveFormat, true);
            sourceVoice.SubmitSourceBuffer(wave.Buffer, wave.DecodedPacketsInfo);
            sourceVoice.Start();
            _list.Add(sourceVoice);
            for (var i = 0; i < _list.Count; i++)
            {
                var sv = _list[i];
                if (sv.State.BuffersQueued <= 0)
                {
                    sv.Stop();
                    sv.DestroyVoice();
                    sv.Dispose();
                    _list.RemoveAt(i);
                }
            }
        }


        public void PreloadSound(string name) { LoadSound(name); }
        
        private MyWave LoadSound(string name) {
            
          
            if (name.IndexOf(".wav", StringComparison.Ordinal) == -1)
            {
                name = Path.Combine(soundsDir, $"{name}.wav");
            }
            var fileInfo = new FileInfo(name);
            if (!fileInfo.Exists) return null;
            var soundStream = new SoundStream(File.OpenRead(name));
            var waveFormat = soundStream.Format;

            var buffer = new AudioBuffer()
            {
                Stream = soundStream.ToDataStream(), AudioBytes = (int) soundStream.Length, Flags = BufferFlags.EndOfStream
            };
            soundStream.Close();
            var wave = new MyWave() {Buffer = buffer, WaveFormat = waveFormat, DecodedPacketsInfo = soundStream.DecodedPacketsInfo};
            Sounds[fileInfo.Name.Split('.').First()] = wave;
            Sounds[fileInfo.Name] = wave;
            return wave;
        }


        public void SetVolume(float volume) { masteringVoice.SetVolume(volume); }

        public void Dispose() {
            foreach (var wave in Sounds)
            {
                wave.Value.Buffer.Stream.Dispose();
            }

            xAudio2.StopEngine();
            masteringVoice?.Dispose();
            xAudio2?.Dispose();
        }
    }
}