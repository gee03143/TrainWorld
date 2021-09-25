using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rails;
using System;

namespace TrainWorld.Traffic {
    public class RailBlock
    {
        public bool hasAgent = false;

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
                rails = new HashSet<(Vector3Int, Direction8way)>();
            }
            rails.Add(position);
        }

        public void AddRail(Vector3Int position, Direction8way direction)
        {
            if (rails == null)
            {
                rails = new HashSet<(Vector3Int, Direction8way)>();
            }
            rails.Add((position, direction));
        }

        public void RemoveRail(Vector3Int position, Direction8way direction)
        {
            if (rails.Contains((position, direction)) == false)
            {
                Debug.Log("Cannot find that position at railBlock. Your Input : " + position + " " + direction);
                return;
            }
            rails.Remove((position, direction));
        }

        public (RailBlock, RailBlock) Divide((Vector3Int, Direction8way) startPosition)
        {
            RailBlock groupA = new RailBlock(BFSSearcher.BFSSearch(startPosition));
            HashSet<(Vector3Int, Direction8way)> clone = new HashSet<(Vector3Int, Direction8way)>(rails);
            clone.ExceptWith(groupA.GetRails());
            RailBlock groupB = new RailBlock(clone);


            if (groupB.GetRails().Count == 0)    //  if two are equal = cannot divide with startPosition
            {
                Debug.Log("Cannot Divide");
                return (null, null);
            }
            else
            {
                return (groupA, groupB);
            }
        }

        public void Merge(RailBlock other)
        {
            rails.UnionWith(other.GetRails());
        }

        public void RemoveMyself()
        {
            rails.Clear();
        }

        public void UpdateRailsBlockReference()
        {
            foreach ((Vector3Int, Direction8way) position in rails)
            {
                if(PlacementManager.IsRailAtPosition(position))
                    PlacementManager.GetRailAt(position).myRailblock = this;
            }
        }

        public void ChangeColor(Color color)
        {
            foreach ((Vector3Int, Direction8way) position in rails)
            {
                PlacementManager.GetRailAt(position).GetComponentInChildren<RailColorChanger>().ChangeRailColor(color);
            }
        }

        public void ChangeColorToDefault()
        {
            foreach ((Vector3Int, Direction8way) position in rails)
            {
                PlacementManager.GetRailAt(position).GetComponentInChildren<RailColorChanger>().ChangeRailColorToDefault();
            }
        }
    }
}
