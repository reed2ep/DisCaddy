using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace DisCaddy.Models
{
    public class BagViewModel : INotifyPropertyChanged
    {
        private readonly IDiscRepository _repo;
        public ObservableCollection<Disc> Discs { get; set; } = new();
        public ICommand AddDiscCommand { get; }
        public Disc NewDisc { get; set; } = new();
        public List<string> DiscTypes { get; } = new() { "Driver", "Control", "Mid", "Putter" };

        public BagViewModel(IDiscRepository repo)
        {
            _repo = repo;
            AddDiscCommand = new Command(async() => await AddDisc());
            LoadDiscs();
        }

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
        }

        private async void LoadDiscs()
        {
            var allDiscs = await _repo.GetAllAsync();
            foreach(var disc in allDiscs)
            {
                Discs.Add(disc);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
