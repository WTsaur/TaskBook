using System;
using System.Collections.Generic;
using System.Globalization;

namespace TaskBook.Models
{
    public class Appointment : Item
    {
        public DateTime Start { get; set; } = DateTime.Today;
        public DateTime Stop { get; set; } = DateTime.Today;
        public List<string> Attendees { get; set; } = new List<string>();

        public override string ToString()
        {
            CultureInfo cultureInfo = CultureInfo
                .CreateSpecificCulture("en-US");
            string attendees = string.Join(", ", Attendees);
            return $"{Name}" +
                $"\n\tPriority: {Priority}" +
                $"\n\tDescription: {Description}" +
                $"\n\tStart: {Start.ToString("G", cultureInfo)}" +
                $"\n\tEnd: {Stop.ToString("G", cultureInfo)}" +
                $"\n\tAttendees: {attendees}";
        }
    }
}
