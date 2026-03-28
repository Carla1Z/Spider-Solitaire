// Extensiones generales para colecciones.
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpiderSolitaire.Extensions
{
    /// <summary>
    /// Extensiones de utilidad para colecciones usadas en ViewModels.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Reemplaza todos los elementos de un ObservableCollection
        /// sin crear una nueva instancia (preserva los bindings de UI).
        /// </summary>
        public static void ReplaceAll<T>(
            this ObservableCollection<T> collection,
            IEnumerable<T> newItems)
        {
            collection.Clear();
            foreach (var item in newItems)
                collection.Add(item);
        }

        /// <summary>
        /// Convierte una lista a ObservableCollection.
        /// </summary>
        public static ObservableCollection<T> ToObservable<T>(
            this IEnumerable<T> source)
            => new ObservableCollection<T>(source);

        /// <summary>
        /// Toma los últimos N elementos de una lista.
        /// </summary>
        public static List<T> TakeLast<T>(this List<T> list, int count)
        {
            int start = System.Math.Max(0, list.Count - count);
            return list.GetRange(start, list.Count - start);
        }
    }
}