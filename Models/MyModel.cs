using System;
using WeddingPlanner.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Models
{

    // Product is the Guest
    public class Guest
    {
        [Key]
        public int GuestId { get; set; }

        [Required]
        [MinLength(2)]
        [Display(Name = "First Name: ")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [Display(Name = "Last Name: ")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email: ")]

        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters or longer")]
        public string Password { get; set; }

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        List<RSVP> WeddingsIAttend { get; set; }
    }
    public class GuestLogin
    {
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    // RSVP is the association
    public class RSVP
    {
        public int RsvpId { get; set; }
        public int WeddingId { get; set; }
        public int GuestId { get; set; }
        public Guest Person { get; set; }
        public Wedding Wedding { get; set; }
    }

    // Wedding is the category
    public class Wedding
    {
        public int WeddingId {get;set;}

        [Required]
        [MinLength(2)]
        public string WedderOne { get; set; }

        [Required]
        [MinLength(2)]
        public string WedderTwo { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(25)]
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guest Planner {get;set;} 
        public int GuestId {get;set;}
        public List<RSVP> AttendingGuests { get; set; }

    }
}