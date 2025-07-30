using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static SQLite.SQLite3;
using DisCaddy.Repository.Interfaces;

namespace DisCaddy.Models
{
    public class BagViewModel : INotifyPropertyChanged
    {
        private readonly IDiscRepository _repo;
        private bool isAddingDisc;
        public ObservableCollection<Disc> Discs { get; set; } = new();
        public ICommand AddDiscCommand { get; }
        public ICommand DeleteDiscCommand { get; }
        public ICommand ChangeDiscImageCommand => new Command<Disc>(async (disc) => await ChooseImageSource(disc));

        public Disc NewDisc { get; set; } = new();
        public List<string> DiscTypes { get; } = new() { "Driver", "Control", "Mid", "Putter" };

        public BagViewModel(IDiscRepository repo)
        {
            _repo = repo;
            AddDiscCommand = new Command(async() => await AddDisc());
            DeleteDiscCommand = new Command<Disc>(async(disc) => await DeleteDisc(disc));
            LoadDiscs();
        }

        #region Add disc
        private async Task AddDisc()
        {
            var disc = new Disc
            {
                Name = NewDisc.Name,
                Type = NewDisc.Type,
                Speed = NewDisc.Speed,
                Glide = NewDisc.Glide,
                Turn = NewDisc.Turn,
                Fade = NewDisc.Fade
            };

            await _repo.SaveAsync(disc);
            Discs.Add(disc);
            NewDisc = new Disc();
            OnPropertyChanged(nameof(NewDisc));
            IsAddingDisc = false;
        }
        public bool IsAddingDisc
        {
            get => isAddingDisc;
            set
            {
                if (isAddingDisc != value)
                {
                    isAddingDisc = value;
                    OnPropertyChanged(nameof(IsAddingDisc));
                }
            }
        }

        public ICommand ToggleAddDiscCommand => new Command(() =>
        {
            IsAddingDisc = !IsAddingDisc;
        });
        #endregion

        private async void LoadDiscs()
        {
            var allDiscs = await _repo.GetAllAsync();
            foreach(var disc in allDiscs)
            {
                Discs.Add(disc);
            }
        }

        private async Task DeleteDisc(Disc disc)
        {
            if (disc == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Delete Disc",
                $"Are you sure you want to delete {disc.Name}?",
                "Yes", "Cancel");

            if (!confirm) return;

            await _repo.DeleteAsync(disc);
            Discs.Remove(disc);
        }

        #region Choose photo
        private async Task ChooseImageSource(Disc disc)
        {
            string action = await Application.Current.MainPage.DisplayActionSheet("Set Disc Image", "Cancel", null, "Take Photo", "Choose from Files");

            if (action == "Take Photo")
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult photo = await MediaPicker.CapturePhotoAsync();
                    ProcessPhoto(photo, disc);
                }
            }
            else if (action == "Choose from Files")
            {
                FileResult photo = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Choose Disc Image",
                    FileTypes = FilePickerFileType.Images
                });

                ProcessPhoto(photo, disc);
            }

        }

        private async void ProcessPhoto(FileResult photo, Disc disc)
        {
            if (photo != null)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                string newFile = Path.Combine(FileSystem.AppDataDirectory, fileName);

                using var stream = await photo.OpenReadAsync();
                using var newStream = File.OpenWrite(newFile);
                await stream.CopyToAsync(newStream);

                disc.ImagePath = newFile;

                await _repo.SaveAsync(disc);

                int index = Discs.IndexOf(disc);
                if (index != -1)
                {
                    Discs.RemoveAt(index);
                    Discs.Insert(index, disc);
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
