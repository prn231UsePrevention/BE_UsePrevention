using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.helper
{
    public static class ScheduleHelper
    {
        public static List<(DateTime Start, DateTime End)> ParseWorkSchedule(string workSchedule, DateTime date)
        {
            var slots = new List<(DateTime Start, DateTime End)>();
            var schedule = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(workSchedule);
            var dayOfWeek = date.DayOfWeek.ToString();

            if (schedule.ContainsKey(dayOfWeek))
            {
                foreach (var slot in schedule[dayOfWeek])
                {
                    var times = slot.Split('-');
                    var startTime = TimeSpan.Parse(times[0]);
                    var endTime = TimeSpan.Parse(times[1]);
                    slots.Add((date.Date + startTime, date.Date + endTime));
                }
            }

            return slots;
        }
    }
}
