namespace AttackDefenseRunner.Util.Parsing.Json
{
    public class DockerTagJson
    {
        public string Tag { get; set; }

        protected bool Equals(DockerTagJson other)
        {
            return Tag == other.Tag;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DockerTagJson) obj);
        }

        public override int GetHashCode()
        {
            return (Tag != null ? Tag.GetHashCode() : 0);
        }
    }
}