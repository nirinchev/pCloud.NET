using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace pCloud.Data
{
    public enum ListSortDirection
    {
        Ascending,
        Descending
    }

    public abstract class SortDescriptorBase
    {
        public ListSortDirection Direction { get; private set; }

        protected SortDescriptorBase(ListSortDirection direction)
        {
            this.Direction = direction;
        }

        public abstract Expression CreateSortKeyExpression(ParameterExpression itemExpression);

        internal LambdaExpression CreateSortKeyLambda(ParameterExpression itemExpression)
        {
            return Expression.Lambda(this.CreateSortKeyExpression(itemExpression), itemExpression);
        }
    }

    public class SortDescriptor : SortDescriptorBase
    {
        public string MemberName { get; private set; }

        public SortDescriptor(string memberName, ListSortDirection direction) : base(direction)
        {
            this.MemberName = memberName;
        }

        public override Expression CreateSortKeyExpression(ParameterExpression itemExpression)
        {
            return Expression.PropertyOrField(itemExpression, this.MemberName);
        }
    }

    public static class LambdaSortDescriptor
    {
        public static SortDescriptorBase Create<TElement, TKey>(Expression<Func<TElement, TKey>> lambda, ListSortDirection direction)
        {
            return new SortDescriptor<TElement, TKey>(lambda, direction);
        }

        private class SortDescriptor<TElement, TKey> : SortDescriptorBase
        {
            private readonly Expression<Func<TElement, TKey>> lambda;

            internal SortDescriptor(Expression<Func<TElement, TKey>> lambda, ListSortDirection direction)
                : base(direction)
            {
                this.lambda = lambda;
            }

            public override Expression CreateSortKeyExpression(ParameterExpression itemExpression)
            {
                return ParameterRewriter.Rewrite(this.lambda.Body, this.lambda.Parameters[0], itemExpression);
            }

            private class ParameterRewriter : ExpressionVisitor
            {
                private readonly ParameterExpression source;
                private readonly ParameterExpression target;

                private ParameterRewriter(ParameterExpression source, ParameterExpression target)
                {
                    this.source = source;
                    this.target = target;
                }

                internal static Expression Rewrite(Expression expression, ParameterExpression source, ParameterExpression target)
                {
                    return new ParameterRewriter(source, target).Visit(expression);
                }

                protected override Expression VisitParameter(ParameterExpression node)
                {
                    if (node == source)
                    {
                        return target;
                    }

                    return base.VisitParameter(node);
                }
            }
        }
    }
}
