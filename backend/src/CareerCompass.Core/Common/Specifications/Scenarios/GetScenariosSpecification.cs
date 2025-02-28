using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Scenarios;

public class GetScenariosSpecification
    : EquatableModel<GetScenariosSpecification>, ISpecification<Scenario, ScenarioId>

{
    private UserId? _userId;
    private IList<TagId> _tagIds = [];
    private IList<FieldId> _fieldIds = [];
    private int? _page = null;
    private int? _pageSize = null;

    public GetScenariosSpecification()
    {
    }

    public GetScenariosSpecification(UserId userId)
    {
        _userId = userId;
    }

    public IQueryable<Scenario> Apply(IQueryable<Scenario> query)
    {
        if (_userId is not null)
        {
            query = query
                .Where(s => s.UserId == _userId);
        }

        if (_tagIds.Count > 0)
        {
            query = query
                .Where(scenario =>
                    scenario.TagIds.Any(x => _tagIds
                        .Select(tagId => tagId.Value)
                        .Contains(x.Value)
                    )
                );
        }


        if (_fieldIds.Count > 0)
        {
            query = query
                .Where(s => s.ScenarioFields
                    .Any(sf => _fieldIds.Contains(sf.FieldId)));
        }
        
        if (_page.HasValue && _pageSize.HasValue)
        {
            query = query
                .Skip((_page.Value - 1) * _pageSize.Value )
                .Take(_pageSize.Value);
        }
        return query;
    }


    public void BelongsTo(UserId userId)
    {
        _userId = userId;
    }


    public void WithTags(IList<TagId> tagIds)
    {
        _tagIds = tagIds;
    }

    public void WithFields(IList<FieldId> fieldIds)
    {
        _fieldIds = fieldIds;
    }


    public void WithPagination(int page, int pageSize)
    {
        _page = page;
        _pageSize = pageSize;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        if (_userId is not null)
        {
            yield return _userId;
        }

        foreach (var tagId in _tagIds)
        {
            yield return tagId;
        }

        foreach (var id in _fieldIds)
        {
            yield return id;
        }
    }
}