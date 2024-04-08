namespace Vocab.Core.Abstractions
{
    public interface ILastModifiedEntity : IEntity
    {
        public DateTime LastModified { get; }
    }
}
