using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tethys.Server.DbModel
{
    public abstract class SpecificationBase<TDomainModel> : ISpecification<TDomainModel>
    {
        private IEnumerable<Expression<Func<TDomainModel, object>>> _includedProperties;

        #region CTOR

        protected SpecificationBase(Expression<Func<TDomainModel, bool>> criteria)
        {
            Criteria = criteria;
        }

        #endregion

        public Expression<Func<TDomainModel, bool>> Criteria { get; }

        public IEnumerable<Expression<Func<TDomainModel, object>>> IncludedProperties
        {
            get => _includedProperties ??
                   (_includedProperties = new List<Expression<Func<TDomainModel, object>>>());
            protected set => _includedProperties = value;
        }

        protected virtual void AddIncludeProperty(Expression<Func<TDomainModel, object>> includeExpression)
        {
            (IncludedProperties as ICollection<Expression<Func<TDomainModel, object>>>).Add(includeExpression);
        }
    }
}