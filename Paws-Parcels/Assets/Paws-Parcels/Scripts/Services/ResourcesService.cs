using System;
using System.Collections.Generic;
using ObservableCollections;
using Paws_Parcels.Scripts.Saves.States.GameResources;
using Paws_Parcels.Scripts.Saves.States.GameState;
using R3;
using UnityEngine;

namespace Services
{
    public class ResourcesService
    {
        private readonly GameProxy _gameProxy;
        
        private readonly Dictionary<ResourcesType, ResourceProxy> _resourceProxiesDict = new();
        private readonly Dictionary<ResourcesType, ResourceSubscription> _observeResourceSubscriptionsDict = new();

        public ResourcesService(GameStateProvider gameStateProvider)
        {
            _gameProxy = gameStateProvider.GameProxy;
            
            _gameProxy.Resources.ForEach(res => _resourceProxiesDict.Add(res.ResourcesType, res));
            
            _gameProxy.Resources.ObserveAdd().Subscribe(e =>
            {
                if (_observeResourceSubscriptionsDict.TryGetValue(e.Value.ResourcesType, out var subscription))
                {
                    subscription.Subscription?.Dispose();
                    subscription.Subscription = e.Value.Amount.Subscribe(v => subscription.Amount.Value = v);
                }
                
                _resourceProxiesDict.Add(e.Value.ResourcesType, e.Value);
            });
            
            _gameProxy.Resources.ObserveRemove().Subscribe(e =>
            {
                if (_observeResourceSubscriptionsDict.TryGetValue(e.Value.ResourcesType, out var subscription))
                {
                    subscription.Subscription?.Dispose();
                    subscription.Amount.Value = 0;
                }
                
                _resourceProxiesDict.Remove(e.Value.ResourcesType);
            });
            
            _gameProxy.Resources.ObserveClear().Subscribe(e =>
            {
                foreach (var keyValuePair in _observeResourceSubscriptionsDict)
                {
                    keyValuePair.Value.Subscription?.Dispose();
                    keyValuePair.Value.Amount.Value = 0;
                }

                _resourceProxiesDict.Clear();
            });
        }
        
        public bool TryAddResource(ResourcesType resourcesType, int amount)
        {
            if (amount < 0)
            {
                Debug.LogError("Amount of Resources cannot be negative.");
                return false;
            }
            
            var resourceProxy = GetResourceProxy(resourcesType);

            resourceProxy.Amount.Value += amount;
            
            return true;
        }
        
        public bool TrySpendResource(ResourcesType resourcesType, int amount)
        {
            if (amount < 0)
            {
                Debug.LogError("Amount of Resources cannot be negative.");
                return false;
            }
            
            var resourceProxy = GetResourceProxy(resourcesType);
            
            if (resourceProxy.Amount.Value < amount)
            {
                Debug.LogError($"Resource {resourcesType} is less than {amount}.");
                return false;
            }

            resourceProxy.Amount.Value -= amount;
            
            return true;
        }

        public bool IsEnoughResources(ResourcesType resourcesType, int amount)
        {
            var resourceProxy = GetResourceProxy(resourcesType);
            
            return resourceProxy.Amount.Value >= amount;
        }

        
        
        private class ResourceSubscription
        {
            public ReactiveProperty<int> Amount { get; }
            public IDisposable Subscription { get; set; }

            public ResourceSubscription(ReactiveProperty<int> amount, IDisposable subscription)
            {
                Amount = amount;
                Subscription = subscription;
            }
        }
        
        public ReadOnlyReactiveProperty<int> ObserveResource(ResourcesType resourcesType)
        {
            if (_observeResourceSubscriptionsDict.TryGetValue(resourcesType, out var resourceSubscription))
            {
                return resourceSubscription.Amount;
            }
            
            var thisResourcesAmount = new ReactiveProperty<int>(0);
            
            var resourceProxy = GetResourceProxy(resourcesType);
            var disposable = resourceProxy.Amount.Subscribe(v => thisResourcesAmount.Value = v);
            
            _observeResourceSubscriptionsDict.Add(resourcesType, new ResourceSubscription(thisResourcesAmount, disposable));
            
            return thisResourcesAmount;
        }

        public int GetResourceAmount(ResourcesType resourcesType)
        {
            var resourceProxy = GetResourceProxy(resourcesType);
            
            return resourceProxy.Amount.Value;
        }

        private ResourceProxy GetResourceProxy(ResourcesType resourcesType)
        {
            var getResourceResult = _resourceProxiesDict.TryGetValue(resourcesType, out var resourceProxy);
            
            if (getResourceResult == false)
            {
                resourceProxy = CreateNewResource(resourcesType);
            }
            
            return resourceProxy;
        }
        
        private ResourceProxy CreateNewResource(ResourcesType resourcesType)
        {
            var newResourceData = new ResourceData
            {
                ResourcesType = resourcesType,
                Amount = 0
            };
            
            var newResourceProxy = new ResourceProxy(newResourceData);
            _gameProxy.Resources.Add(newResourceProxy);
            
            return newResourceProxy;
        }
    }
}