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
    public class MapViewModel
    {
        public string CourseName { get; set; }
        public Course CurrentCourse { get; private set; }
        public ObservableCollection<Hole> Holes { get; set; } = new();
        public ICommand AddHoleCommand { get; }
        public ICommand SaveCourseCommand { get; }
        private readonly ICourseRepository _courseRepo;
        private readonly IHoleRepository _holeRepo;

        public MapViewModel(ICourseRepository courseRepo, IHoleRepository holeRepo)
        {
            _courseRepo = courseRepo;
            _holeRepo = holeRepo;

            Holes = new ObservableCollection<Hole>();

            AddHoleCommand = new Command<Hole>(async hole => await AddHoleAsync(hole));
            SaveCourseCommand = new Command(async () => await CreateCourseAsync());
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
