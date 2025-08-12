using DisCaddy.Objects;
using DisCaddy.Repository.Interfaces;
using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisCaddy.Models
{
    public class CourseSelectionViewModel
    {
        private readonly ICourseRepository _repo;
        public ObservableCollection<Course> Courses { get; set; } = new();
        public CourseSelectionViewModel(ICourseRepository repo)
        {
            _repo = repo;
            LoadCourses();
        }

        private async void LoadCourses()
        {
            var allCourses = await _repo.GetAllCoursesAsync();
            foreach (var course in allCourses)
            {
                Courses.Add(course);
            }
        }
    }
}
