﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace TTSAutomate
{
    public abstract class TTSProvider: INotifyPropertyChanged
    {
        //DispatcherTimer PlayTimer = new DispatcherTimer(DispatcherPriority.Render);
        string message = "";
        public Boolean initialLoad = true;
        public TTSProvider()
        {
            System.IO.Directory.CreateDirectory(String.Format("{0}\\mp3", Path.GetTempPath()));
        }

        public enum Class
        {
            Local,
            Web,
        }

        private Boolean hasVoices = false;

        public Boolean HasVoices
        {
            get { return hasVoices; }
            set
            {
                hasVoices = value;
                OnPropertyChanged("HasVoices");
            }
        }

        private Boolean hasNumericSpeed = false;

        public Boolean HasNumericSpeed
        {
            get { return hasNumericSpeed; }
            set
            {
                hasNumericSpeed = value;
                OnPropertyChanged("HasNumericSpeed");
            }
        }

        private Boolean hasDiscreteSpeed= false;

        public Boolean HasDiscreteSpeed
        {
            get { return hasDiscreteSpeed; }
            set
            {
                hasDiscreteSpeed = value;
                OnPropertyChanged("HasDiscreteSpeed");
            }
        }

        private Boolean hasNumericVolume = false;

        public Boolean HasNumericVolume
        {
            get { return hasNumericVolume; }
            set
            {
                hasNumericVolume = value;
                OnPropertyChanged("HasNumericVolume");
            }
        }

        private Boolean hasDiscreteVolume = false;

        public Boolean HasDiscreteVolume
        {
            get { return hasDiscreteVolume; }
            set
            {
                hasDiscreteVolume = value;
                OnPropertyChanged("HasDiscreteVolume");
            }
        }


        public string Name { get; set; }

        public Class ProviderClass { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Name, ProviderClass);
        }

        //public abstract byte[] GetVoice(string phrase);

        private List<Voice> availableVoices = new List<Voice>();

        public List<Voice> AvailableVoices
        {
            get { return availableVoices; }
            set
            {
                availableVoices = value;
                OnPropertyChanged("AvailableVoices");
            }
        }

        private Voice selectedVoice;

        public Voice SelectedVoice
        {
            get { return selectedVoice; }
            set
            {
                selectedVoice = value;
                PlayMessage(String.Format("{0} selected", SelectedVoice.Name));
                OnPropertyChanged("SelectedVoice");
            }
        }

        private void PlayMessage(string messageToPlay)
        {
            if (!initialLoad)
            {
                new Task(() => { 
                string filename = String.Format("{0}", Guid.NewGuid());
                    DownloadItem(new PhraseItem { Phrase = String.Format("{0}", messageToPlay), FileName = filename, Folder = "." }, Path.GetTempPath(), false);
                    MainWindow.PlayAudioFullPath(String.Format("{1}\\mp3\\{0}.mp3", filename, Path.GetTempPath()), true);
                }).Start();
            }
            //message = messageToPlay;
            //PlayTimer.Start();

        }

        private List<String> availableSpeeds = new List<string>();

        public List<String> AvailableSpeeds
        {
            get { return availableSpeeds; }
            set
            {
                availableSpeeds = value;
                OnPropertyChanged("AvailableSpeeds");
            }
        }

        private String selectedDiscreteSpeed = "";

        public String SelectedDiscreteSpeed
        {
            get { return selectedDiscreteSpeed; }
            set
            {
                selectedDiscreteSpeed = value;
                PlayMessage(String.Format("{0}", SelectedDiscreteSpeed));
                OnPropertyChanged("SelectedDiscreteSpeed");
            }
        }

        private int selectedNumericSpeed = 100;

        public int SelectedNumericSpeed
        {
            get { return selectedNumericSpeed; }
            set
            {
                selectedNumericSpeed = value;
                PlayMessage(String.Format("{0}", SelectedNumericSpeed));
                OnPropertyChanged("SelectedNumericSpeed");
            }
        }

        private List<String> availableVolumes = new List<string>();

        public List<String> AvailableVolumes
        {
            get { return availableVolumes; }
            set
            {
                availableVolumes = value;
                OnPropertyChanged("AvailableVolumes");
            }
        }

        private String selectedDiscreteVolume = "";

        public String SelectedDiscreteVolume
        {
            get { return selectedDiscreteVolume; }
            set
            {
                selectedDiscreteVolume = value;
                PlayMessage(String.Format("{0}", SelectedDiscreteVolume));
                OnPropertyChanged("SelectedDiscreteVolume");
            }
        }

        private int selectedNumericVolume= 100;

        public int SelectedNumericVolume
        {
            get { return selectedNumericVolume; }
            set
            {
                selectedNumericVolume = value;
                PlayMessage(String.Format("{0}", SelectedNumericVolume));
                OnPropertyChanged("SelectedNumericVolume");
            }
        }

        public abstract Boolean DownloadItem(PhraseItem item, string folder, Boolean? convertToWav = true);

        public abstract Boolean DownloadAndPlay(PhraseItem item);

    }

    public class Voice
    {
        public String Name { get; set; }
        public String Language { get; set; }
        public String Gender { get; set; }

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", Name, Language, Gender);
        }
    }


}
