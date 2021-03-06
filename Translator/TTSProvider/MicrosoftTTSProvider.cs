﻿using NAudio.Lame;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TTSAutomate
{
    class MicrosoftTTSProvider : TTSProvider
    {

        SpeechSynthesizer speechSynth = new SpeechSynthesizer();

        public MicrosoftTTSProvider()
        {
            Name = "Microsoft Windows Text To Speech";
            ProviderClass = Class.Local;
            HasVoices = true;
            HasDiscreteSpeed = true;
            HasDiscreteVolume = true;

            new Task(() =>
            {
                foreach (var v in speechSynth.GetInstalledVoices().Select(v => v.VoiceInfo))
                {
                    AvailableVoices.Add(new Voice { Name = v.Name, Gender = v.Gender.ToString(), Language = v.Culture.Name});
                }
                if (this.Name == Properties.Settings.Default.LastTTSProvider)
                {
                    if (Properties.Settings.Default.RememberLanguageSettings)
                    {
                        SelectedVoice = AvailableVoices.Find(n => n.Name == Properties.Settings.Default.LastTTSVoice);
                    }
                    else
                    {
                        SelectedVoice = AvailableVoices[0];
                    }
                }
            }).Start();

            for (int i = -10; i <= 10; i++)
            {
                AvailableSpeeds.Add(i.ToString());
            }
            SelectedDiscreteSpeed = "0";

            for (int i = 0; i <= 100; i++)
            {
                AvailableVolumes.Add(i.ToString());
            }
            SelectedDiscreteVolume = "100";
        }

        public override void DownloadItem(PhraseItem item, string folder)
        {
            String SSMLText = String.Format(@"<?xml version=""1.0""?>
<speak xmlns=""http://www.w3.org/2001/10/synthesis"" version=""1.0"" xml:lang=""{1}"">
<p>
<s>{0}</s>
</p>
</speak>
", item.Phrase.Replace("&", "&amp;"), SelectedVoice.Language);

            try
            {
                new Task(() =>
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        using (var synth = new SpeechSynthesizer())
                        {
                            synth.SelectVoice(SelectedVoice.Name);
                            synth.Volume = Int32.Parse(SelectedDiscreteVolume);
                            synth.Rate = Int32.Parse(SelectedDiscreteSpeed);
                            synth.SetOutputToWaveStream(ms);//.SetOutputToWaveFile(String.Format("{0}\\wav22050\\{1}\\{2}.wav", folder, item.Folder, item.FileName));
                            synth.SpeakSsml(SSMLText);
                        }
                        ms.Seek(0, SeekOrigin.Begin);
                        using (WaveFileReader wav = new WaveFileReader(ms))// String.Format("{0}\\wav22050\\{1}\\{2}.wav", folder, item.Folder, item.FileName)))
                        {
                            using (FileStream fs = new FileStream(String.Format("{0}\\mp3\\{1}\\{2}.mp3", folder, item.Folder, item.FileName), FileMode.Create))
                            {
                                using (var writer = new LameMP3FileWriter(fs, wav.WaveFormat, 128))
                                {
                                    wav.CopyTo(writer);
                                }
                            }
                        }
                        ms.Seek(0, SeekOrigin.Begin);
                        using (WaveFileReader wav = new WaveFileReader(ms))// String.Format("{0}\\wav22050\\{1}\\{2}.wav", folder, item.Folder, item.FileName)))
                        {
                            ConvertToWav(item, folder, false, new String[] { Name, SelectedVoice.Name, SelectedDiscreteSpeed, SelectedDiscreteVolume });
                        }
                    }
                }).Start();
            }
            catch (Exception Ex)
            {
                Logger.Log(Ex.ToString());
            }
        }

        public override void DownloadAndPlayItem(PhraseItem item, string folder)
        {
            String SSMLText = String.Format(@"<?xml version=""1.0""?>
<speak xmlns=""http://www.w3.org/2001/10/synthesis"" version=""1.0"" xml:lang=""{1}"">
<p>
<s>{0}</s>
</p>
</speak>
", item.Phrase.Replace("&", "&amp;"), SelectedVoice.Language);

            try
            {
                new Task(() =>
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        using (var synth = new SpeechSynthesizer())
                        {
                            synth.SelectVoice(SelectedVoice.Name);
                            synth.Volume = Int32.Parse(SelectedDiscreteVolume);
                            synth.Rate = Int32.Parse(SelectedDiscreteSpeed);
                            synth.SetOutputToWaveStream(ms);//.SetOutputToWaveFile(String.Format("{0}\\wav22050\\{1}\\{2}.wav", folder, item.Folder, item.FileName));
                            synth.SpeakSsml(SSMLText);
                        }
                        ms.Seek(0, SeekOrigin.Begin);
                        using (WaveFileReader wav = new WaveFileReader(ms))// String.Format("{0}\\wav22050\\{1}\\{2}.wav", folder, item.Folder, item.FileName)))
                        {
                            using (FileStream fs = new FileStream(String.Format("{0}\\mp3\\{1}\\{2}.mp3", folder, item.Folder, item.FileName), FileMode.Create))
                            {
                                using (var writer = new LameMP3FileWriter(fs, wav.WaveFormat, 128))
                                {
                                    wav.CopyTo(writer);
                                }
                            }
                        }
                        ms.Seek(0, SeekOrigin.Begin);
                        using (WaveFileReader wav = new WaveFileReader(ms))// String.Format("{0}\\wav22050\\{1}\\{2}.wav", folder, item.Folder, item.FileName)))
                        {
                            ConvertToWav(item, folder, true, new String[] { Name, SelectedVoice.Name, SelectedDiscreteSpeed, SelectedDiscreteVolume });
                        }
                    }
                }).Start();
            }
            catch (Exception Ex)
            {
                Logger.Log(Ex.ToString());
            }
        }

        public override void Play(PhraseItem item)
        {
            using (var synth = new SpeechSynthesizer())
            {
                synth.SelectVoice(SelectedVoice.Name);
                synth.Volume = Int32.Parse(SelectedDiscreteVolume);
                synth.Rate = Int32.Parse(SelectedDiscreteSpeed);
                synth.SetOutputToDefaultAudioDevice();
                synth.Speak(item.Phrase);
            }
        }

    }

}
