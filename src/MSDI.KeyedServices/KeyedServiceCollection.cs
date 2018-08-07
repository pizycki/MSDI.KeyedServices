using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MSDI.KeyedServices
{
    /// <summary>
    /// Set of registered services under the same contract, distinguished by key.
    /// </summary>
    /// <typeparam name="TService">Service contract type</typeparam>
    /// <typeparam name="TKey">Type of key we distinguish implementation types with</typeparam>
    public class KeyedServiceCollection<TService, TKey>
    {
        private readonly Dictionary<TKey, Type> _implementationTypes;

        public KeyedServiceCollection()
        {
            _implementationTypes = new Dictionary<TKey, Type>();
        }

        /// <summary>
        /// Adds or replaces record.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        public void Add(TKey key, Type type)
        {
            _implementationTypes.Add(key, type);
        }

        /// <summary>
        /// Retrieves implementation type by key.
        /// When key cannot be found, throws exception.
        /// </summary>
        public Type this[TKey key] => _implementationTypes[key];

        private static Func<TKey, Type, Type> ReplaceType => (k, t) => t;
    }
}
