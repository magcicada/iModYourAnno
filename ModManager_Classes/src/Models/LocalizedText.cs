﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ModManager_Classes.src.Enums;
using ModManager_Classes.src.Handlers;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ModManager_Classes.src.Models
{
    public class LocalizedText: INotifyPropertyChanged
    {
        [JsonIgnore]
        public String Text {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }
        [JsonIgnore]
        private String _text;

        public Localized? Texts { get; set; }
        
        public LocalizedText()
        {
            
        }

        [OnDeserialized]
        private void OnSerialized(StreamingContext context)
        {
            if (Texts is Localized)
            {
                Text = Texts.getText();
            }
            else
            {
                Text = String.Empty;
            }
            LanguageManager.Instance.LanguageChanged += OnLanguageChanged;
        }

        public void OnLanguageChanged(ApplicationLanguage lang)
        {
            if (Texts is Localized)
            {
                Text = Texts.getText(lang);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler is PropertyChangedEventHandler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
