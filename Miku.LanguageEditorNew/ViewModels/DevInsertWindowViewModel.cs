using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Miku.Database;
using Miku.Database.LanguageEntities;
using Miku.LanguageEditorNew.Models;
using ReactiveUI;

namespace Miku.LanguageEditorNew.ViewModels
{
    public class DevInsertWindowViewModel : ViewModelBase
    {
        LanguageManager lm { get; set; }

        #region PropertyTypes

        public IEnumerable<PropertyType> PropertyTypes => Enum.GetValues(typeof(PropertyType)).Cast<PropertyType>();


        private PropertyType selectedPropertyType;

        public PropertyType SelectedPropertyType
        {
            get => selectedPropertyType;
            set => selectedPropertyType = this.RaiseAndSetIfChanged(ref selectedPropertyType, value);
        }

        #endregion

        #region ChunkName

        private string chunkName;

        public string ChunkName
        {
            get { return chunkName; }
            set { chunkName = this.RaiseAndSetIfChanged(ref chunkName, value); }
        }


        #endregion

        #region InnerIdentifer

        private string innerIdentifier;

        public string InnerIdentifier
        {
            get { return innerIdentifier; }
            set { innerIdentifier = this.RaiseAndSetIfChanged(ref innerIdentifier, value); }
        }


        #endregion

        #region Wrapped Abbreviations

        private ObservableCollection<DevAbbrevationWrapper> wrappedItems;

        public ObservableCollection<DevAbbrevationWrapper> WrappedItems
        {
            get { return wrappedItems; }
            set { wrappedItems = this.RaiseAndSetIfChanged(ref wrappedItems, value); }
        }

        #endregion

        #region New Abbreviation Name

        private string newAbbreviationName;

        public string NewAbbreviationName
        {
            get { return newAbbreviationName; }
            set { newAbbreviationName = this.RaiseAndSetIfChanged(ref newAbbreviationName, value); }
        }

        #endregion

        #region New Abbreviation Description

        private string newAbbreviationDescription;

        public string NewAbbreviationDescription
        {
            get { return newAbbreviationDescription; }
            set { newAbbreviationDescription = this.RaiseAndSetIfChanged(ref newAbbreviationDescription, value); }
        }


        #endregion

        public DevInsertWindowViewModel()
        {
            lm = new LanguageManager();
            Task.Run(FillAbbreviationList);
            IObservable<bool> canAdd = this.WhenAnyValue(x => x.NewAbbreviationName, x => x.NewAbbreviationDescription, (name, description) => !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(description));
            IObservable<bool> canAddToDB = this.WhenAnyValue(x => x.ChunkName, x => x.InnerIdentifier, (name, description) => !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(description));
            AddAbbreviationCommand = ReactiveCommand.CreateFromTask(AddAbbreviation, canAdd);
            AddToDbCommand = ReactiveCommand.CreateFromTask(AddToDbAsync, canAddToDB);
        }

        #region AbbreviationList Fill

        public async Task FillAbbreviationList()
        {
            var abbrs = new List<DevAbbrevationWrapper>();
            var fromDB = await lm.GetAllAbbreviationsAsync();
            foreach (var ab in fromDB)
            {
                if (abbrs.All(x => x.SetAbbreviation.Name != ab.Name))
                {
                    abbrs.Add(new DevAbbrevationWrapper(ab));
                }
            }
            WrappedItems = new ObservableCollection<DevAbbrevationWrapper>(abbrs);
        }

        #endregion

        #region Add Abbreviation Command

        public ICommand AddAbbreviationCommand { get; }

        private Task AddAbbreviation()
        {
            Abbreviation ab = new Abbreviation
            {
                Description = NewAbbreviationDescription,
                Name = NewAbbreviationName,
                Parent = null,
                ParentIdentifier = null,
                ParentLanguageCode = null,
                ParentName = null,
                ParentType = default,
                Position = 0
            };
            WrappedItems.Add(new DevAbbrevationWrapper(ab));
            NewAbbreviationName = "";
            NewAbbreviationDescription = "";
            return Task.CompletedTask;
        }

        #endregion

        #region Add To DB Command

        ICommand AddToDbCommand { get; }

        public async Task AddToDbAsync()
        {
            var c = new Chunk
            {
                InnerIdentifier = InnerIdentifier,
                Language = null,
                LanguageCode = null,
                Name = ChunkName,
                Text = "English base",
                Type = SelectedPropertyType,
                Abbreviations = null
            };
            var ab = WrappedItems.Where(x => x.IsChecked)
                .OrderBy(x => x.Position)
                .Select(x => x.SetAbbreviation)
                .ToList();

            await lm.AddChunkAsync("en-US", c, ab);
            ChunkName = "";
            InnerIdentifier = "";
            await FillAbbreviationList();
        }

        #endregion

    }
}
