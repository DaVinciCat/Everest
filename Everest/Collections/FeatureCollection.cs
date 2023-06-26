using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Everest.Collections
{
	public interface IFeatureCollection : IDictionary<Type, object>
	{
	}

	public class FeatureCollection : ConcurrentDictionary<Type, object>, IFeatureCollection
	{

	}

	public static class FeatureCollectionExtensions
	{
		public static T Get<T>(this IFeatureCollection features) =>
			features.TryGetValue(typeof(T), out var value) ? (T)value : default(T);

		public static IFeatureCollection Set<T>(this IFeatureCollection features, T feature)
		{
			features[typeof(T)] = feature;
			return features;
		}
	}
}
