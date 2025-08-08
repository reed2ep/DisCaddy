using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DisCaddy.Objects;
using DisCaddy.Repository.Interfaces;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;

namespace DisCaddy.Models
{
    public class MapViewModel : INotifyPropertyChanged
    {
        public string CourseName { get; set; }
        public Course CurrentCourse { get; private set; }
        public ObservableCollection<Hole> Holes { get; set; } = new();
        public Location MapCenter => CurrentHole?.TeeLocation.ToLocation();
        public event Action<Location>? CenterMapRequested;

        private readonly ICourseRepository _courseRepo;
        private readonly IHoleRepository _holeRepo;

        private int currentHoleIndex = 0;
        private Hole currentHole;
        public ICommand AddHoleCommand { get; }
        public ICommand SaveCourseCommand { get; }
        public ICommand NextHoleCommand { get; }
        public ICommand PreviousHoleCommand { get; }

        public MapViewModel(ICourseRepository courseRepo, IHoleRepository holeRepo)
        {
            _courseRepo = courseRepo;
            _holeRepo = holeRepo;

            Holes = new ObservableCollection<Hole>();

            AddHoleCommand = new Command<Hole>(async hole => await AddHoleAsync(hole));
            SaveCourseCommand = new Command(async () => await CreateCourseAsync());
            NextHoleCommand = new Command(NextHole);
            PreviousHoleCommand = new Command(PreviousHole);
        }
        private string status;
        public string Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    status = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                }
            }
        }
        public Hole CurrentHole
        {
            get => currentHole;
            set
            {
                if (currentHole != value)
                {
                    currentHole = value;
                    OnPropertyChanged(nameof(CurrentHole));

                    if (currentHole?.TeeLocation != null)
                        CenterMapRequested?.Invoke(currentHole.TeeLocation.ToLocation());
                }
            }
        }
        public async Task CreateCourseAsync()
        {
            if (string.IsNullOrWhiteSpace(CourseName))
                return;

            var course = new Course { Name = CourseName };
            await _courseRepo.SaveCourseAsync(course);
            CurrentCourse = course;
        }

        public async Task AddHoleAsync(Hole hole)
        {
            if (CurrentCourse == null)
                return;

            hole.CourseId = CurrentCourse.Id;
            await _holeRepo.SaveHoleAsync(hole);
            Holes.Add(hole);
        }

        public async Task LoadCourseAsync(Course course)
        {
            CurrentCourse = course;
            CourseName = course.Name;

            var holes = await _holeRepo.GetHolesByCourseIdAsync(course.Id);
            Holes.Clear();

            foreach(var hole in holes)
            {
                Holes.Add(hole);
            }

            currentHoleIndex = 0;
            CurrentHole = Holes.FirstOrDefault();
        }

        private void NextHole()
        {
            if(currentHoleIndex < Holes.Count)
            {
                currentHoleIndex++;
                CurrentHole = Holes[currentHoleIndex];
                OnPropertyChanged(nameof(CurrentHole));
            }
        }

        private void PreviousHole()
        {
            if (currentHoleIndex > 0)
            {
                currentHoleIndex--;
                CurrentHole = Holes[currentHoleIndex];
                OnPropertyChanged(nameof(CurrentHole));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
