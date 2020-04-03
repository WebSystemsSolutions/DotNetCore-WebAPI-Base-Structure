using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SampleProj.Repository
{
    public static class IqueribleExpressionСhain
    {
        public static IQueryable<T> AddExpressionСhain<T>(this IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return query.Where(expression);
        }
    }
}
