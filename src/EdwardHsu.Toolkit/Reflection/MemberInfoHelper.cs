using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EdwardHsu.Toolkit.Reflection
{
    public static class MemberInfoHelper
    {
        static MemberInfo GetMember(Expression expression)
        {
            if (expression is MethodCallExpression)
            {
                return (expression as MethodCallExpression)?.Method;
            }
            else if (expression is MemberExpression)
            {
                return (expression as MemberExpression)?.Member;
            }
            else if (expression is NewExpression)
            {
                return (expression as NewExpression)?.Constructor;
            }
            else if (expression is UnaryExpression)
            {
                return GetMember((expression as UnaryExpression)?.Operand);
            }
            return null;
        }
        
        public static MemberInfo GetMemberInfo<T>(this T instance, Expression<Action<T>> expression)
        {
            return GetMember((Expression)expression.Body);
        }
        
        public static MemberInfo GetMemberInfo<T>(this T instance, Expression<Func<T, object>> expression)
        {
            return GetMember((Expression)expression.Body);
        }
    }
}
