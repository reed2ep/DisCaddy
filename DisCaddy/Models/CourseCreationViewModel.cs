using DisCaddy.Objects;
using DisCaddy.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DisCaddy.Models
{
    public class CourseCreationViewModel : INotifyPropertyChanged
    {
        public string CourseName { get; set; }
        public ObservableCollection<Hole> Holes { get; set; } = new();
        public ICommand AddHoleCommand { get; }
        public ICommand SaveCourseCommand { get; }
        private readonly ICourseRepository _repo;

        public CourseCreationViewModel(ICourseRepository repo)
        {
            _repo = repo;
            AddHoleCommand = new Command<Hole>(hole => Holes.Add(hole));
            SaveCourseCommand = new Command(async () => await SaveCourseAsync());
        }

        private async Task SaveCourseAsync()
        {
            var course = new Course
            {
                Name = CourseName,
            };

            await _repo.SaveCourseAsync(course);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
