using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace TrainWorld.Traffic
{
    public class RailBlockManager : MonoBehaviour
    {
        private List<RailBlock> railBlocks;

        private List<Color> colors;

        private void Awake()
        {
            railBlocks = new List<RailBlock>();
            colors = new List<Color> { Color.red, Color.yellow, Color.green, Color.blue, Color.white, Color.cyan };
        }

        public List<RailBlock> GetAllAdjascentRailBlocks(Vector3Int position, Direction8way direction)
        {
            List<RailBlock> adjascentRailBlocks = new List<RailBlock>();
            List<(Vector3Int, Direction8way)> adjascentTuples = new List<(Vector3Int, Direction8way)>();

            adjascentTuples.AddRange(PlacementManager.GetRailAt(position, direction).GetNeighbourTuples()
                .Select(x => (x.Item1, x.Item2.Opposite())).ToList());
            adjascentTuples.AddRange(PlacementManager.GetRailAt(position, direction.Opposite()).GetNeighbourTuples()
                .Select(x => (x.Item1, x.Item2.Opposite())).ToList());
            adjascentTuples.AddRange(PlacementManager.GetRailsAtPosition(position).Select(x => (x.Position, x.Direction)).ToList());

            foreach (var tuple in adjascentTuples)
            {
                RailBlock block = PlacementManager.GetRailAt(tuple).myRailblock;
                if (block != null)
                    adjascentRailBlocks.Add(block);
            }

            return adjascentRailBlocks;
        }

        internal void Unite(RailBlock blockA, List<RailBlock> blocks)
        {
            foreach (var railBlock in blocks)
            {
                blockA.Merge(railBlock);
                railBlocks.Remove(railBlock);
            }
        }

        internal void Split(RailBlock original, Vector3Int splitPosition, Direction8way splitDirection)
        {
            RailBlock railBlockA;
            RailBlock railBlockB;
            (railBlockA, railBlockB) = original.Divide((splitPosition, splitDirection));

            if (railBlockA == null && railBlockB == null) // fail to divide railblock
            {
                //do nothing
            }
            else
            {
                railBlocks.Remove(original);
                railBlocks.Add(railBlockA);
                railBlocks.Add(railBlockB);

                original.RemoveMyself();
                railBlockA.UpdateRailsBlockReference();
                railBlockB.UpdateRailsBlockReference();
            }

        }

        public RailBlock MakeNewBlock(Vector3Int position, Direction8way direction)
        {
            RailBlock newBlock = new RailBlock();
            newBlock.AddRail(position, direction);
            newBlock.AddRail(position, direction.Opposite());
            railBlocks.Add(newBlock);
            return newBlock;
        }

        void OnDrawGizmosSelected()
        {
            int i = 0;
            foreach (var railBlock in railBlocks)
            {
                railBlock.ChangeColor(colors[i % 6]);
                i++;
            }
        }
    }
}
