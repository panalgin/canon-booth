using CanonPhotoBooth.Contracts;

namespace CanonPhotoBooth
{
    public class Player : Visitor
    {
        public string FullName { get { return string.Format("{0} {1}", this.Name, this.Surname); } }

        public double PowerGenerated { get; set; }
        public double Speed { get; set; }

        public Player()
        {

        }
    }
}