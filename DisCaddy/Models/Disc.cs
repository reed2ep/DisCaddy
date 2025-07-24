using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace DisCaddy.Models
{
    public class Disc : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }            // e.g., "Destroyer"
        public string Type { get; set; }            // e.g., "Driver", "Putter"
        public int Speed { get; set; }              // 1–14
        public int Glide { get; set; }              // 1–7
        public int Turn { get; set; }               // -5 to +1
        public int Fade { get; set; }               // 0–5
        public string Stability => GetStability();  // derived from Turn/Fade
        public string Summary => $"S:{Speed} G:{Glide} T:{Turn} F:{Fade}";
        public string ImagePath
        {
            get => imagePath;
            set
            {
                if (imagePath != value)
                {
                    imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string GetStability()
        {
            if (Turn <= -3) return "Understable";
            if (Turn >= 0 && Fade >= 3) return "Overstable";
            return "Neutral";
        }

        public override string ToString()
        {
            return $"{Name} ({Type}) - S:{Speed} G:{Glide} T:{Turn} F:{Fade}";
        }
        private string imagePath;
    }
}
