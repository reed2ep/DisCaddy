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
        public ObservableCollection<Disc> Discs { get; set; } = new();
        public ICommand AddDiscCommand { get; }
        public Disc NewDisc { get; set; } = new();
        public List<string> DiscTypes { get; } = new() { "Driver", "Control", "Mid", "Putter" };

        public BagViewModel()
        {
            AddDiscCommand = new Command(AddDisc);
        }

        void AddDisc()
        {
            Discs.Add(new Disc { Name = NewDisc.Name, Type = NewDisc.Type, Speed = NewDisc.Speed, Glide = NewDisc.Glide, Turn = NewDisc.Turn, Fade = NewDisc.Fade });
            NewDisc = new Disc();
            OnPropertyChanged(nameof(NewDisc));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
