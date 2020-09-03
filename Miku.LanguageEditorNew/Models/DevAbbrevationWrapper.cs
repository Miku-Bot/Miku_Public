using Miku.Database.LanguageEntities;
using Miku.LanguageEditorNew.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miku.LanguageEditorNew.Models
{
    public class DevAbbrevationWrapper : ViewModelBase
    {
        private int position;

        public int Position
        {
            get { return position; }
            set
            {
                position = this.RaiseAndSetIfChanged(ref position, value);
                var prevdes = SetAbbreviation.Description;
                var prevna = SetAbbreviation.Name;
                var prevtyp = SetAbbreviation.ParentType;
                SetAbbreviation = new Abbreviation
                {
                    Description = prevdes,
                    Name = prevna,
                    ParentLanguageCode = null,
                    Parent = null,
                    ParentIdentifier = null,
                    ParentName = null,
                    ParentType = prevtyp,
                    Position = value
                };
            }
        }


        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = this.RaiseAndSetIfChanged(ref isChecked, value); }
        }

        private Abbreviation abbreviation;

        public Abbreviation SetAbbreviation
        {
            get { return abbreviation; }
            set { abbreviation = this.RaiseAndSetIfChanged(ref abbreviation, value); }
        }

        public DevAbbrevationWrapper(Abbreviation abbr)
        {
            SetAbbreviation = new Abbreviation
            {
                Description = abbr.Description,
                Name = abbr.Name,
                ParentLanguageCode = null,
                Parent = null,
                ParentIdentifier = null,
                ParentName = null,
                ParentType = abbr.ParentType,
                Position = 0
            };
            IsChecked = false;
            Position = 0;
        }


    }
}
