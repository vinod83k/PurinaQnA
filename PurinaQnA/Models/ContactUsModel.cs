using System;
using System.ComponentModel.DataAnnotations;

namespace PurinaQnA.Models
{
    public class ContactUsModel
    {
        [Required]
        public string Species { get; set; }
        [Required]
        public string Question { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string EmailAddress { get; set; }

        public static ContactUsModel Parse(dynamic o)
        {
            try
            {
                return new ContactUsModel
                {
                    Species = o.Species.ToString(),
                    Question = o.Question.ToString(),
                    Name = o.Name.ToString(),
                    EmailAddress = o.EmailAddress.ToString()
                };
            }
            catch
            {
                throw new InvalidCastException("ContactUsModel could not be read");
            }
        }
    }
}