using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrainWorld.Traffic
{
    public class RailBlockManager : MonoBehaviour
    {
        private Dictionary<int, RailBlock> railBlocks;

        private void Awake()
        {
            railBlocks = new Dictionary<int, RailBlock>();
        }

        void AddRailBlock(RailBlock newBlock)
        {
            railBlocks.Add(railBlocks.Count, newBlock);
        }

        RailBlock GetRailBlockOfID(int id)
        {
            if (railBlocks.ContainsKey(id) == false)
                return null;

            return railBlocks[id];
        }

        void RemoveRailBlockOfID(int id)
        {
            if (railBlocks.ContainsKey(id) == false)
                return;

            railBlocks[id].RemoveMyself();
            railBlocks.Remove(id);
        }
    }
}
