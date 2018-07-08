using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tethys.Server.DbModel
{
    public class SimpleSpecification<TDomainModel> : SpecificationBase<TDomainModel>
    {
        public SimpleSpecification(Expression<Func<TDomainModel, bool>> criteria) : base(criteria)
        { }

        public SimpleSpecification(Expression<Func<TDomainModel, bool>> criteria,
            IEnumerable<Expression<Func<TDomainModel, object>>> includedProperties) : base(criteria)
        {
            IncludedProperties = includedProperties;
        }
    }
}