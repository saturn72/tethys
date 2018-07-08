using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tethys.WebApi.DbModel
{
    public interface ISpecification<TDomainModel>
    {
        Expression<Func<TDomainModel, bool>> Criteria { get; }
        IEnumerable<Expression<Func<TDomainModel, object>>> IncludedProperties { get; }
    }
}