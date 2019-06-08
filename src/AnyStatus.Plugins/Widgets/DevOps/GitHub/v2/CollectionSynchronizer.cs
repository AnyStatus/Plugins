//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace AnyStatus.Plugins.Widgets.DevOps.GitHub.v2
//{
//    public class CollectionSynchronizer<TSource, TDestination>
//    {
//        public Func<TSource, TDestination, bool> CompareFunc { get; set; }
//        public Action<TDestination> RemoveAction { get; set; }
//        public Action<TSource> AddAction { get; set; }
//        public Action<TSource, TDestination> UpdateAction { get; set; }

//        public void Synchronize(ICollection<TSource> sourceItems, ICollection<TDestination> destinationItems)
//        {
//            RemoveItems(sourceItems, destinationItems);

//            AddOrUpdateItems(sourceItems, destinationItems);
//        }

//        private void RemoveItems(ICollection<TSource> sourceCollection, ICollection<TDestination> destinationCollection)
//        {
//            foreach (var destinationItem in destinationCollection.ToArray())
//            {
//                var sourceItem = sourceCollection.FirstOrDefault(item => CompareFunc(item, destinationItem));

//                if (sourceItem == null)
//                {
//                    RemoveAction(destinationItem);
//                }
//            }
//        }

//        private void AddOrUpdateItems(ICollection<TSource> sourceCollection, ICollection<TDestination> destinationCollection)
//        {
//            var destinationList = destinationCollection.ToList();
//            foreach (var sourceItem in sourceCollection)
//            {
//                var destinationItem = destinationList.FirstOrDefault(item => CompareFunc(sourceItem, item));

//                if (destinationItem == null)
//                {
//                    AddAction(sourceItem);
//                }
//                else
//                {
//                    UpdateAction(sourceItem, destinationItem);
//                }
//            }
//        }
//    }
//}
