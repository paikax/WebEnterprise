using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System;


namespace WebEnterprise.Models
{
    public class Contribution
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CoordinatorComment { get; set; }
        public string ImageUrl { get; set; }
        public DateTime SubmissionDate { get; set; }
        public bool SelectedForPublication { get; set; }
        public bool TermsAndConditionsAccepted { get; set; }
        public string StudentId { get; set; }
        public User Student { get; set; }
        public int FacultyId { get; set; }
        public Faculty Faculty { get; set; }
        public string FilePath { get; set; } // Reference to the uploaded file
        public byte[] FileContent { get; set; }
    }

    // ViewModel to handle file uploads
    public class ContributionViewModel
    {
        public IFormFile File { get; set; } // For file upload (e.g., .doc, .pdf)

        public IFormFile Image { get; set; } // For image upload

        // Other properties for the Contribution entity
    }
}

