using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ResMed.Models
{
    public class Doctors
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Płeć")]
        public string Gender { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "Aktywny")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Numer licencji")]
        public string LicenseNr { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        public string Image { get; set; }

        [Display(Name = "Id Specjalizacji")]
        public int? SpecializationId { get; set; }

        [ForeignKey("SpecializationId")]
        public virtual Specializations Specializations { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Średni czas wizyty [min]")]
        [Range(0, 360, ErrorMessage = "Wprowadź właściwą liczbę")]
        public int AverageVisitTime { get; set; }

        [Display(Name = "Start pracy")]
        public DateTime StartWorkHours { get; set; }

        [Display(Name = "Koniec pracy")]
        public DateTime EndWorkHours { get; set; }

        [Display(Name = "Poniedziałek")]
        public bool WorkingMonday { get; set; }

        [Display(Name = "Wtorek")]
        public bool WorkingTuesday { get; set; }

        [Display(Name = "Środa")]
        public bool WorkingWednesday { get; set; }

        [Display(Name = "Czwartek")]
        public bool WorkingThursday { get; set; }

        [Display(Name = "Piątek")]
        public bool WorkingFriday { get; set; }

        [Display(Name = "Sobota")]
        public bool WorkingSaturday { get; set; }

        [Display(Name = "Niedziela")]
        public bool WorkingSunday { get; set; }

        [NotMapped]
        public int[] WorkDaysArr { get; set; }

        [NotMapped]
        public List<string> WorkHours { get; set; }

        [NotMapped]
        public List<string> TakenHoursInDay { get; set; }

        [NotMapped]
        public DateTime SelectedDate { get; set; }

        [Display(Name = "Średnia ocen")]
        public double AverageRating { get; set; }

        [Display(Name = "Ilość ocen")]
        public int RatingCount { get; set; }

    }
}
