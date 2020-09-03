using Avalonia.Media;
using DynamicData;
using DynamicData.Binding;
using Miku.Database;
using Miku.Database.LanguageEntities;
using Miku.LanguageEditorNew.Views;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Miku.LanguageEditorNew.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        LanguageManager languageManager { get; set; }

        #region Languages

        private ObservableCollection<Language> languages;

        public ObservableCollection<Language> Languages
        {
            get { return languages; }
            set { languages = this.RaiseAndSetIfChanged(ref languages, value); }
        }

        private Language selectedLanguage;

        public Language SelectedLanguage
        {
            get { return selectedLanguage; }
            set
            {
                selectedLanguage = this.RaiseAndSetIfChanged(ref selectedLanguage, value);
                if (selectedChunkName == null) 
                    Task.Run(FillChunkNamesAsync);
                if (selectedChunkName != null &&
                    selectedInnerIdentifier != null)
                {
                    Task.Run(FillTextFieldsAsync);
                }
            }
        }

        #endregion

        #region PropertyTypes

        public IEnumerable<PropertyType> PropertyTypes
        {
            get 
            {
                return Enum.GetValues(typeof(PropertyType)).Cast<PropertyType>(); 
            }
        }


        private PropertyType selectedPropertyType;

        public PropertyType SelectedPropertyType
        {
            get { return selectedPropertyType; }
            set 
            {
                selectedPropertyType = this.RaiseAndSetIfChanged(ref selectedPropertyType, value);
                Task.Run(FillChunkNamesAsync);
                this.InnerIdentifiers = null;
                this.SelectedInnerIdentifier = null;
                this.DefaultOriginalText = null;
                this.TranslatedText = null;
            }
        }

        #endregion

        #region ChunkNames

        private ObservableCollection<string> chunkNames;

        public ObservableCollection<string> ChunkNames
        {
            get { return chunkNames; }
            set { chunkNames = this.RaiseAndSetIfChanged(ref chunkNames, value); }
        }

        private string selectedChunkName;

        public string SelectedChunkName
        {
            get { return selectedChunkName; }
            set
            {
                selectedChunkName = this.RaiseAndSetIfChanged(ref selectedChunkName, value);
                Task.Run(FillInnerIdentifiersAsync);
            }
        }

        #endregion

        #region InnerIdentifiers

        private ObservableCollection<Chunk> innerIdentifiers;

        public ObservableCollection<Chunk> InnerIdentifiers
        {
            get { return innerIdentifiers; }
            set { innerIdentifiers = this.RaiseAndSetIfChanged(ref innerIdentifiers, value); }
        }

        private Chunk selectedInnerIdentifier;

        public Chunk SelectedInnerIdentifier
        {
            get { return selectedInnerIdentifier; }
            set
            {
                selectedInnerIdentifier = this.RaiseAndSetIfChanged(ref selectedInnerIdentifier, value);
                Task.Run(FillAbbreviations);
                Task.Run(FillTextFieldsAsync);
            }
        }

        #endregion

        #region Abbreviations

        private List<Abbreviation> abbreviations;

        public List<Abbreviation> Abbreviations
        {
            get { return abbreviations; }
            set { abbreviations = this.RaiseAndSetIfChanged(ref abbreviations, value); }
        }

        private Abbreviation selectedAbbreviation;

        public Abbreviation SelectedAbbreviation
        {
            get { return selectedAbbreviation; }
            set
            {
                TranslatedText += value.Name;
                selectedAbbreviation = this.RaiseAndSetIfChanged(ref selectedAbbreviation, null);
            }
        }


        #endregion

        #region Textboxes

        private string defaultOriginalText;

        public string DefaultOriginalText
        {
            get { return defaultOriginalText; }
            set { defaultOriginalText = this.RaiseAndSetIfChanged(ref defaultOriginalText, value); }
        }

        private string translatedText;

        public string TranslatedText
        {
            get { return translatedText; }
            set { translatedText = this.RaiseAndSetIfChanged(ref translatedText, value); }
        }

        #endregion

        #region Fonts

        public IList<FontFamily> Fonts
        {
            get
            {
                return FontFamily.SystemFontFamilies.ToList().OrderByDescending(x => x.Name).ToList();
            }
        }

        private FontFamily selectedFont = new FontFamily("Segoe UI Emoji");

        public FontFamily SelectedFont
        {
            get { return selectedFont; }
            set
            {
                selectedFont = this.RaiseAndSetIfChanged(ref selectedFont, value); 

            }
        }

        #endregion

        public MainWindowViewModel()
        {
            languageManager = new LanguageManager();
            Task.Run(FillLanguagesAsync);
            IObservable<bool> canSave = this.WhenAnyValue(x => x.SelectedInnerIdentifier, x => x.SelectedChunkName,(innerIdent, chunkName) => innerIdent != null && chunkName != null);
            SaveCommand = ReactiveCommand.Create(SaveTextsAsync, canSave);
            RefreshCommand = ReactiveCommand.Create(RefreshAsync);

            #if DEVEDITION
            OpenDevWindowCommand = ReactiveCommand.Create(OpenDevWindow);
            #endif
            
        }

        #region Fill Methods

        public async Task FillLanguagesAsync()
        {
            var langs = await languageManager.GetLanguagesAsync();
            Languages = new ObservableCollection<Language>(langs);
            SelectedLanguage = Languages.First();
        }

        public async Task FillChunkNamesAsync()
        {
            if (SelectedLanguage == null) return;
            var chunkNames = await languageManager.GetChunksAsync("en-US", SelectedPropertyType);
            ChunkNames = new ObservableCollection<string>(chunkNames.Select(x => x.Name).Distinct());
        }

        public async Task FillInnerIdentifiersAsync()
        {
            if (SelectedLanguage == null) return;
            InnerIdentifiers = new ObservableCollection<Chunk>(await languageManager.GetChunksAsync("en-US", SelectedPropertyType, SelectedChunkName));
        }

        public Task FillAbbreviations()
        {
            Abbreviations = SelectedInnerIdentifier?.Abbreviations;
            return Task.CompletedTask;
        }

        public async Task FillTextFieldsAsync()
        {
            if (this.SelectedChunkName == null || SelectedInnerIdentifier == null || SelectedLanguage == null) return;
            DefaultOriginalText = (await languageManager.GetOrAddSpecificChunk("en-US", SelectedPropertyType,
                SelectedChunkName, SelectedInnerIdentifier.InnerIdentifier)).Text;
            TranslatedText = (await languageManager.GetOrAddSpecificChunk(SelectedLanguage.Code, SelectedPropertyType,
                SelectedChunkName, SelectedInnerIdentifier.InnerIdentifier)).Text;
        }

        #endregion

        #region Save Command

        public ICommand SaveCommand { get; }

        public async Task SaveTextsAsync()
        {
            var n = new Chunk
            {
                InnerIdentifier = SelectedInnerIdentifier.InnerIdentifier,
                Language = SelectedLanguage,
                LanguageCode = SelectedLanguage.Code,
                Name = SelectedChunkName,
                Text = TranslatedText,
                Type = SelectedPropertyType,
                Abbreviations = null
            };
            await languageManager.UpdateChunkAsync(n);
        }

        #endregion

        #region Refresh Command

        ICommand RefreshCommand { get; set; }

        public async Task RefreshAsync()
        {
            await languageManager.DisposeAsync();
            languageManager = new LanguageManager();
            ChunkNames = null;
            InnerIdentifiers = null;
            Abbreviations = null;
            TranslatedText = null;
            DefaultOriginalText = null;
            await FillLanguagesAsync();
        }

        #endregion

        #region Open Dev Window Command

        ICommand OpenDevWindowCommand { get; set; } 

        public Task OpenDevWindow()
        {
            var dev = new DevInsertWindow();
            dev.Show();
            return Task.CompletedTask;
        }

        #endregion
    }
}
