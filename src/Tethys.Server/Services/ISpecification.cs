using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tethys.Server.Services
{
    public interface ISpecification<TDomainModel>
    {
        Expression<Func<TDomainModel, bool>> Criteria { get; }
        IEnumerable<Expression<Func<TDomainModel, object>>> IncludedProperties { get; }
    }
}