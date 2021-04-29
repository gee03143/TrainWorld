using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrainWorld.Traffic
{
    public class RailBlockManager : MonoBehaviour
    {
        public List<RailBlock> railBlocks;

        private List<Color> colors;

        private void Awake()
        {
            railBlocks = new List<RailBlock>();
            colors = new List<Color> { Color.red, Color.yellow, Color.green, Color.blue, Color.white, Color.cyan };
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
