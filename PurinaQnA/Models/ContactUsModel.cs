using System;

namespace PurinaQnA.Models
{
    public class ContactUsModel
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }

        public static ContactUsModel Parse(dynamic o)
        {
            try
            {
                return new ContactUsModel
                {
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