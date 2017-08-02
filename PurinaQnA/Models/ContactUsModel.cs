using System;

namespace PurinaQnA.Models
{
    public class ContactUsModel
    {
        public string Species { get; set; }
        public string Question { get; set; }
        public string Name { get; set; }
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