using System.Linq.Expressions;
using System.Reflection;

namespace InterviewTestApp.Shared.Helpers;

public static class MemberHelper
{
    public static string ExtractPropertyName<T>(Expression<Func<T>> expression)
    {
        if (expression.Body is MemberExpression)
        {
            var expr = expression.Body as MemberExpression;
            var info = expr.Member;
            return info.Name;
        }
        else if (expression.Body is UnaryExpression)
        {
            var expr = expression.Body as UnaryExpression;
            var member = expr.Operand as MemberExpression;
            if (member != null)
            {
                var info = member.Member;
                return info.Name;
            }
        }

        return null;
    }

    public static PropertyInfo GetProperty(string name, object obj)
    {
        if (obj == null || string.IsNullOrEmpty(name))
        {
            return null;
        }

        return obj.GetType().GetProperty(name);
    }

}