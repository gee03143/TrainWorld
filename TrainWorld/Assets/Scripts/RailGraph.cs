using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{ 
    public class RailNode
    {
        public Vector3Int position;
        public Rail rail;

        public Dictionary<Direction, RailNode> connectedRails;

        public RailNode(Vector3Int position, Direction direction)
        {
            this.position = position;
            connectedRails = new Dictionary<Direction, RailNode>();
            this.connectedRails.Add(direction, this);
            this.connectedRails.Add(DirectionHelper.Opposite(direction), this);
        }

        public Direction GetClosestExtendDirection(Vector3 mousePosition)
        {
            double minDist = double.PositiveInfinity;
            Direction returnValue = Direction.N;

            foreach (var direction in connectedRails.Keys)
            {
                double distance = Vector3.Distance(mousePosition, position + DirectionHelper.ToDirectionVector(direction));
                if (minDist > distance)
                {
                    minDist = distance;
                    returnValue = direction;
                }
            }

            return returnValue;
        }
    }

    public class RailGraph : MonoBehaviour
    {
        private Dictionary<Vector3Int, RailNode> railGraph;

        private void Awake()
        {
            railGraph = new Dictionary<Vector3Int, RailNode>();
        }

        public void AddNode(Vector3Int position, Direction direction)
        {
            if(railGraph.ContainsKey(position) == false)
            {
                railGraph.Add(position, new RailNode(position, direction));
            }
            else
            {
                RailNode selectedNode = GetNodeAt(position);
                if(selectedNode.connectedRails.ContainsKey(direction) == false)
                {
                    selectedNode.connectedRails.Add(direction, selectedNode);
                }

                if (selectedNode.connectedRails.ContainsKey(DirectionHelper.Opposite(direction)) == false)
                {
                    selectedNode.connectedRails.Add(DirectionHelper.Opposite(direction), selectedNode);
                }
            }

            ConnectWithAdjescentNodes(position);
        }

        private void ConnectWithAdjescentNodes(Vector3Int position)
        {
            RailNode startNode = GetNodeAt(position);
            List<Direction> directions = new List<Direction>(startNode.connectedRails.Keys);

            foreach (var direction in directions)
            {
                RailNode adjescentNode = GetNodeAt(position + DirectionHelper.ToDirectionVector(direction));
                if (adjescentNode == null)
                    continue;
                if (adjescentNode.connectedRails.ContainsKey(DirectionHelper.Opposite(direction)))  //이웃 노드가 연결될 수 있다면
                {
                    AddEdge(position, adjescentNode.position, direction, DirectionHelper.Opposite(direction));
                }
            }
        }

        public RailNode GetNodeAt(Vector3Int position)
        {
            if (railGraph.ContainsKey(position) == true)
                return railGraph[position];

            return null;
        }

        public void AddEdge(Vector3Int position1, Vector3Int position2, Direction direction1, Direction direction2)
        {
            if(railGraph.ContainsKey(position1) == false)
            {
                AddNode(position1, direction1);
            }
            if (railGraph.ContainsKey(position2) == false)
            {
                AddNode(position2, direction2);
            }

            railGraph[position1].connectedRails[direction1] = railGraph[position2];
            railGraph[position2].connectedRails[direction2] = railGraph[position1];
        }

        private void OnDrawGizmos()
        {
            if (railGraph == null)
                return;

            Gizmos.color = Color.red;

            foreach (var nodePosition in railGraph.Keys)
            {
                foreach (var item in GetNodeAt(nodePosition).connectedRails.Keys)
                {
                    Gizmos.DrawLine(nodePosition, GetNodeAt(nodePosition).connectedRails[item].position);
                }
            }
        }
    }
}