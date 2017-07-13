using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PurinaQnA.Utils
{
    public static class CommonResponseKeywords
    {
        public static List<string> Keywords
        {
            get
            {
                return new List<string> {
                    "Hi", "Hi There", "Hello there", "Hey", "Hello",
            "Hey there", "Greetings", "Good morning", "Good afternoon", "Good evening", "Good day",
            "bye","bye bye","got to go","see you later","laters","adios",
            "no","not","nothing", "ok", "thanks"
                };
            }
        }
    }

    public static class OptionKeywords
    {
        public static List<string> Keywords
        {
            get
            {
                return new List<string> {
                    "faq","retailer","retailer finder"
                };
            }
        }
    }
}