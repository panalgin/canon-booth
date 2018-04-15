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
            if (this.Entries.Count >= MaxDataPoints)
                this.Entries = this.Entries.Skip(1).ToList();

            this.Entries.Add(new SampleEntry() { Revolutions = Math.Pow(revs, 2), TimeElapsed = Math.Pow(timePassed, 2) });


            var totalRevsSq = this.Entries.Sum(q => q.Revolutions);
            var totalTimeElapsedSq = this.Entries.Sum(q => q.TimeElapsed);

            var revPerMs = Math.Sqrt(totalRevsSq / totalTimeElapsedSq);
            var totalRevPerHour = revPerMs * 1000 * 60 * 60;
            var distancePerRev = Config.DistancePerRevolution; //cm
            var estimatedKmph = (totalRevPerHour * distancePerRev) / 100 / 1000; // divided for meter / then kilometer

            this.Speed = estimatedKmph;
            this.DistanceCovered += (revs * distancePerRev) / 100; //in meters
            this.CaloriesBurnt += timePassed * Config.CaloriesPerMillisecond; //cal
            this.PowerGenerated = (this.CaloriesBurnt * 0.001163) * 1000; //to watt/hour

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