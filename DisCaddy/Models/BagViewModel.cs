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

        public BagViewModel()
        {
            AddDiscCommand = new Command(AddDisc);
            Discs.Add(new Disc { Name = "Destroyer", Type = "Driver", Speed = 12, Glide = 5, Turn = -1, Fade = 3 });
        }

        void AddDisc()
        {
            Discs.Add(new Disc { Name = "New Disc", Type = "Putter", Speed = 2, Glide = 3, Turn = 0, Fade = 1 });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
