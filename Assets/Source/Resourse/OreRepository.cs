using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Resourse
{
    public class OreRepository : MonoBehaviour
    {
        private readonly List<Ore> _freeResources = new();
        private readonly List<Ore> _busyResources = new();

        public void Pull(Ore ore)
        {
            if (ore == null)
                throw new NullReferenceException();

            if (_freeResources.Contains(ore) || _busyResources.Contains(ore))
                return;

            _freeResources.Add(ore);
        }

        public Ore TakeFree()
        {
            if (_freeResources.Count == 0)
                return null;

            Ore ore = _freeResources[0];
            _freeResources.RemoveAt(0);
            _busyResources.Add(ore);

            return ore;
        }

        public void Remove(Ore ore)
        {
            if (ore == null)
                throw new NullReferenceException();

            _freeResources.Remove(ore);
            _busyResources.Remove(ore);
        }
    }
}