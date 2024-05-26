namespace Vocab.Core
{
    public interface IEntity
    {
        public long Id { get; }
        public DateTime LastModified { get; }
    }
}
