namespace CareerCompass.Application.Tags;

public interface ITagRepository
{
    
    public Task<bool> Exists(TagId id, CancellationToken cancellationToken);
    public Task<Tag> Get(TagId id, CancellationToken cancellationToken);
    
}