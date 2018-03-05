using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace UCHome.PublicHelper {
	public static class ExpressionExtHelper {
		/// <summary>
		/// 默认True条件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Expression<Func<T, bool>> True<T>() { return f => true; }

		/// <summary>
		/// 默认False条件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Expression<Func<T, bool>> False<T>() { return f => false; }

		/// <summary>
		/// 拼接 OR 条件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="exp"></param>
		/// <param name="condition"></param>
		/// <returns></returns>
		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> exp, Expression<Func<T, bool>> condition) {
			ParameterExpression p = exp.Parameters[0];

			SubstExpressionVisitor visitor = new SubstExpressionVisitor();
			visitor.subst[condition.Parameters[0]] = p;

			Expression body = Expression.OrElse(exp.Body, visitor.Visit(condition.Body));
			return Expression.Lambda<Func<T, bool>>(body, p);
		}

		/// <summary>
		/// 拼接And条件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="exp"></param>
		/// <param name="condition"></param>
		/// <returns></returns>
		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exp, Expression<Func<T, bool>> condition) {
			ParameterExpression p = exp.Parameters[0];

			SubstExpressionVisitor visitor = new SubstExpressionVisitor();
			visitor.subst[condition.Parameters[0]] = p;

			Expression body = Expression.AndAlso(exp.Body, visitor.Visit(condition.Body));
			return Expression.Lambda<Func<T, bool>>(body, p);
		}		
	}
	internal class SubstExpressionVisitor : System.Linq.Expressions.ExpressionVisitor {
		public Dictionary<Expression, Expression> subst = new Dictionary<Expression, Expression>();

		protected override Expression VisitParameter(ParameterExpression node) {
			Expression newValue;
			if (subst.TryGetValue(node, out newValue)) {
				return newValue;
			}
			return node;
		}
	}
}