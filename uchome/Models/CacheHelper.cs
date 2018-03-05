using System;
using System.Web;

namespace UCHome.Models {
	/// <summary>
	/// 缓存相关的操作类
    /// Copyright (C) Wicresoft
	/// </summary>
	public class CacheHelper
	{
		/// <summary>
		/// 获取当前应用程序指定CacheKey的Cache值
		/// </summary>
		/// <param name="CacheKey"></param>
		/// <returns></returns>
		public static object GetCache(string CacheKey)
		{
			System.Web.Caching.Cache objCache = HttpRuntime.Cache;
			return objCache[CacheKey];
		}

		/// <summary>
		/// 设置当前应用程序指定CacheKey的Cache值
		/// </summary>
		/// <param name="CacheKey"></param>
		/// <param name="objObject"></param>
		public static void SetCache(string CacheKey, object objObject)
		{
			System.Web.Caching.Cache objCache = HttpRuntime.Cache;
			objCache.Insert(CacheKey, objObject);
		}

		/// <summary>
		/// 设置当前应用程序指定CacheKey的Cache值，且设置绝对到期时间（不管有没有操作都将过期的时间）以及弹性过期时间(停止操作后的过期时间)
		/// </summary>
		/// <param name="CacheKey"></param>
		/// <param name="objObject"></param>       
		public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration,TimeSpan slidingExpiration )
		{
			System.Web.Caching.Cache objCache = HttpRuntime.Cache;
			objCache.Insert(CacheKey, objObject,null,absoluteExpiration,slidingExpiration);           
		}
	}
}
