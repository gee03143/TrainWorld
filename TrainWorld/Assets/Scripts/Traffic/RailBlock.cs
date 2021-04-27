using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rails;

namespace TrainWorld.Traffic {
    public class RailBlock
    {
        private HashSet<(Vector3Int, Direction8way)> rails;

        public HashSet<(Vector3Int, Direction8way)> GetRails()
        {
            return rails;
        }

        public RailBlock(HashSet<(Vector3Int, Direction8way)> rails = null)
        {
            if (rails != null)
                this.rails = rails;
            else
                rails = new HashSet<(Vector3Int, Direction8way)>();
        }

        public void AddRail((Vector3Int, Direction8way) position)
        {
            if (rails == null)
            {
                Debug.Log("rails was null");
                rails = new HashSet<(Vector3Int, Direction8way)>();
            }
            rails.Add(position);
        }

        public (RailBlock, RailBlock) Divide((Vector3Int, Direction8way) startPosition)
        {
            RailBlock groupA = new RailBlock(BFSSearcher.BFSSearch(startPosition));
            HashSet<(Vector3Int, Direction8way)> clone = new HashSet<(Vector3Int, Direction8way)>(rails);
            clone.ExceptWith(groupA.GetRails());
            RailBlock groupB = new RailBlock(clone);


            if (groupB.GetRails().Count == 0)    //  if two are equal = cannot divide with startPosition
            {
                Debug.Log(rails.Count);
                Debug.Log(groupA.GetRails().Count);
                Debug.Log(groupB.GetRails().Count);
                Debug.Log("Cannot Divide");
                return (null, null);
            }
            else
            {
                Debug.Log(rails.Count);
                Debug.Log(groupA.GetRails().Count);
                Debug.Log(groupB.GetRails().Count);
                return (groupA, groupB);
            }
        }

        public void Merge(RailBlock other)
        {
            rails.UnionWith(other.GetRails());
        }

        public bool CompareBlock(RailBlock other)
        {
            return rails.SetEquals(other.GetRails());
        }

        public void RemoveMyself()
        {
            rails.Clear();
        }

        public void UpdateRailsBlockReference()
        {
            foreach ((Vector3Int, Direction8way) position in rails)
            {
                PlacementManager.GetRailAt(position).myRailblock = this;
            }
        }
    }
}
