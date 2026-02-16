using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Brupper;

/// <summary> 
/// https://internetexception.com/wp-content/uploads/2014/02/Program.cs 
/// https://internetexception.com/2014/02/22/expression-parsing-and-nested-properties/
/// </summary>
public static partial class PropertyExtensions
{
    public static object? GetPropertyValue(this object obj, string propertyPath)
    {
        object? propertyValue = null;
        if (propertyPath.IndexOf(".") < 0)
        {
            var objType = obj.GetType();
            propertyValue = objType.GetProperty(propertyPath).GetValue(obj, null);
            return propertyValue;
        }
        var properties = propertyPath.Split('.').ToList();
        var midPropertyValue = obj;
        while (properties.Count > 0)
        {
            var propertyName = properties.First();
            properties.Remove(propertyName);
            propertyValue = midPropertyValue.GetPropertyValue(propertyName);
            midPropertyValue = propertyValue;
        }
        return propertyValue;
    }

    // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq.Expressions/src/System/Linq/Expressions/ExpressionStringBuilder.cs
    public static string GetPropertyPath<TObj, TRet>(this TObj obj, Expression<Func<TObj, TRet>> expr)
    {
        return GetPropertyPath(expr);
    }

    public static MemberExpression GetMemberExpression(Expression expression)
    {
        if (expression is MemberExpression)
        {
            return (MemberExpression)expression;
        }
        else if (expression is MethodCallExpression mce)
        {
            var memberExpression = (MemberExpression)mce.Object;
        }
        else if (expression is LambdaExpression)
        {
            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression.Body is MemberExpression)
            {
                return (MemberExpression)lambdaExpression.Body;
            }
            else if (lambdaExpression.Body is UnaryExpression)
            {
                return ((MemberExpression)((UnaryExpression)lambdaExpression.Body).Operand);
            }
        }
        return null;
    }

    public static string GetPropertyPath(Expression expr)
    {
        var path = new StringBuilder();
        var memberExpression = GetMemberExpression(expr);
        do
        {
            if (path.Length > 0)
            {
                path.Insert(0, ".");
            }
            path.Insert(0, memberExpression.Member.Name);
            memberExpression = GetMemberExpression(memberExpression.Expression);
        }
        while (memberExpression != null);
        return path.ToString();
    }
}
