using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebEnterprise.Models;

namespace WebEnterprise.ViewModels
{
    public class AssignViewModel
    {
        public int FacultyId { get; set; }

        [Display(Name = "Select Coordinator")]
        public string SelectedCoordinatorId { get; set; }

        public List<User> Coordinators { get; set; }
        public List<Faculty> Faculties { get; set; }

        public AssignViewModel()
        {
            // Initialize Coordinators and Faculties lists
            Coordinators = new List<User>();
            Faculties = new List<Faculty>();
        }
    }
}