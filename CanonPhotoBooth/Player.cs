using CanonPhotoBooth.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CanonPhotoBooth
{
    public class Player : Visitor
    {
        const int MaxDataPoints = 10;

        private ControlBoard board = null;
        private List<SampleEntry> Entries = new List<SampleEntry>();

        [JsonProperty(PropertyName = "fullName")]
        public string FullName { get { return string.Format("{0} {1}", this.Name, this.Surname); } }

        [JsonProperty(PropertyName = "powerGenerated")]
        public double PowerGenerated { get; set; }

        [JsonProperty(PropertyName = "speed")]
        public double Speed { get; set; }

        [JsonProperty(PropertyName = "distanceCovered")]
        public double DistanceCovered { get; set; }

        [JsonProperty(PropertyName = "caloriesBurnt")]
        public double CaloriesBurnt { get; set; }

        [JsonIgnore]
        public ControlBoard Board
        {
            get
            {
                return board;
            }
            set
            {
                board = value;

                board.SampleAcquired -= Board_SampleAcquired;
                board.SampleAcquired += Board_SampleAcquired;
            }
        }

        private void Board_SampleAcquired(ulong revs, ulong timePassed)
        {
            if (revs == 0)
                return;

            this.Entries.Add(new SampleEntry() { Revolutions = revs, TimeElapsed = timePassed });


            var totalRevs = Entries.Sum(q => q.Revolutions);
            var totalElapsed = Entries.Sum(q => q.TimeElapsed);

            var revPerMs = totalRevs / totalElapsed;

            var totalRevPerHour = revPerMs * 1000 * 60 * 60;

            var totalDistanceAchieved = totalRevs * Config.DistancePerRevolution / 100 / 1000; //km

            var estimatedKmph = Math.Round(totalDistanceAchieved / (totalElapsed / 1000 / 60 / 60), 2) / 40;
            var caloriesBurnt = Math.Round(totalElapsed * Config.CaloriesPerMillisecond, 2); //cal

            this.Speed = estimatedKmph;
            this.CaloriesBurnt = caloriesBurnt;

            EventSink.InvokePlayerUpdated(this);
        }

        public Player()
        {

        }

        public static Player FromVisitor(Visitor visitor)
        {
            return new Player()
            {
                Address = visitor.Address,
                DateOfBirth = visitor.DateOfBirth,
                Email = visitor.Email,
                Gender = visitor.Gender,
                Mobile = visitor.Mobile,
                Name = visitor.Name,
                Surname = visitor.Surname
            };
        }
    }

    public class SampleEntry
    {
        public double Revolutions { get; set; }
        public double TimeElapsed { get; set; }
    }
}