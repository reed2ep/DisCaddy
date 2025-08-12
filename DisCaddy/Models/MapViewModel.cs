using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections.ObjectModel;
using DisCaddy.Objects;
using DisCaddy.Repository.Interfaces;
using Microsoft.Maui.Devices.Sensors;

namespace DisCaddy.Models
{
    public class MapViewModel : INotifyPropertyChanged
    {
        private readonly ICourseRepository _courseRepo;
        private readonly IHoleRepository _holeRepo;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<Location>? CenterMapRequested;
        public event Action<List<GeoPoint>>? FlightPathRequested;
        void OnPropertyChanged([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
        void OnStateChanged()
        {
            OnPropertyChanged(nameof(SelectedCourse));
            OnPropertyChanged(nameof(IsCreatingCourse));
            OnPropertyChanged(nameof(ShowCreateButton));
            OnPropertyChanged(nameof(ShowNameDialog));
            OnPropertyChanged(nameof(ShowSelectedRow));
        }

        public MapViewModel(ICourseRepository courseRepo, IHoleRepository holeRepo)
        {
            _courseRepo = courseRepo;
            _holeRepo = holeRepo;

            Holes = new ObservableCollection<Hole>();
            AddHoleCommand = new Command<Hole>(async h => await AddHoleAsync(h));
            StartCreateCourseCommand = new Command(() => IsCreatingCourse = true);
            CancelCreateCourseCommand = new Command(() =>
            {
                IsCreatingCourse = false;
                CourseName = string.Empty;
                OnPropertyChanged(nameof(CourseName));
            });
            SaveCourseCommand = new Command(async () => await CreateCourseAsync());
            NextHoleCommand = new Command(NextHole);
            PreviousHoleCommand = new Command(PreviousHole);
        }

        #region UI state
        private Course? _selectedCourse;
        public Course? SelectedCourse
        {
            get => _selectedCourse;
            set { if (_selectedCourse != value) { _selectedCourse = value; OnStateChanged(); } }
        }

        private bool _isCreatingCourse;
        public bool IsCreatingCourse
        {
            get => _isCreatingCourse;
            set { if (_isCreatingCourse != value) { _isCreatingCourse = value; OnStateChanged(); } }
        }

        public bool ShowCreateButton => SelectedCourse == null && !IsCreatingCourse;
        public bool ShowNameDialog => IsCreatingCourse;
        public bool ShowSelectedRow => SelectedCourse != null && !IsCreatingCourse;
        #endregion

        #region course/hole data
        public string CourseName { get; set; } = string.Empty;
        public Course? CurrentCourse { get; private set; }
        public ObservableCollection<Hole> Holes { get; }

        private int _currentHoleIndex = 0;
        private Hole? _currentHole;
        public Hole? CurrentHole
        {
            get => _currentHole;
            set
            {
                if (_currentHole == value) return;
                _currentHole = value;
                OnPropertyChanged();

                if (value?.TeeLocation != null)
                    CenterMapRequested?.Invoke(value.TeeLocation.ToLocation());

                var path = value?.FlightPath;
                if (path != null && path.Count > 0)
                    FlightPathRequested?.Invoke(path);
            }
        }
        #endregion

        #region commands
        public ICommand AddHoleCommand { get; }
        public ICommand SaveCourseCommand { get; }
        public ICommand StartCreateCourseCommand { get; }
        public ICommand CancelCreateCourseCommand { get; }
        public ICommand NextHoleCommand { get; }
        public ICommand PreviousHoleCommand { get; }
        #endregion
        #region actions
        public async Task CreateCourseAsync()
        {
            if (string.IsNullOrWhiteSpace(CourseName)) return;

            var course = new Course { Name = CourseName.Trim() };
            await _courseRepo.SaveCourseAsync(course);

            CurrentCourse = course;
            SelectedCourse = course;
            IsCreatingCourse = false;

            CourseName = string.Empty;
            OnPropertyChanged(nameof(CourseName));
        }

        public async Task AddHoleAsync(Hole hole)
        {
            if (CurrentCourse == null) return;
            hole.CourseId = CurrentCourse.Id;
            await _holeRepo.SaveHoleAsync(hole);
            Holes.Add(hole);
        }

        public async Task LoadCourseAsync(Course course)
        {
            CurrentCourse = course;
            SelectedCourse = course;

            var holes = await _holeRepo.GetHolesByCourseIdAsync(course.Id);
            Holes.Clear();
            foreach (var h in holes) Holes.Add(h);

            _currentHoleIndex = 0;
            CurrentHole = Holes.FirstOrDefault();
        }

        private void NextHole()
        {
            if (Holes.Count == 0) return;
            if (_currentHoleIndex < Holes.Count - 1)
            {
                _currentHoleIndex++;
                CurrentHole = Holes[_currentHoleIndex];
            }
        }

        private void PreviousHole()
        {
            if (Holes.Count == 0) return;
            if (_currentHoleIndex > 0)
            {
                _currentHoleIndex--;
                CurrentHole = Holes[_currentHoleIndex];
            }
        }
        #endregion
    }
}
