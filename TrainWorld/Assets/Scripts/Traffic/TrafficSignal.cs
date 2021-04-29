using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrainWorld.Rails;

namespace TrainWorld.Traffic
{
    public class TrafficSignal : MonoBehaviour, ISelectableObject
    {
        private Vector3Int position;

        public Vector3Int Position
        {
            get { return position; }
            set { position = value; }
        }

        private Direction8way direction;

        public Direction8way Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        [SerializeField]
        private Rail master;
        [SerializeField]
        private Renderer redLight;
        [SerializeField]
        private Renderer greenLight;

        List<Renderer> renderers;

        private void Awake()
        {
            renderers = new List<Renderer> { redLight, greenLight };
        }

        internal void Init(Vector3Int position, Direction8way direction)
        {
            this.position = position;
            this.direction = direction;
        }

        private void Update()
        {
            if (master.myRailblock == PlacementManager.GetRailAt(master.Position, DirectionHelper.Opposite(master.Direction)).myRailblock) // cannot divide
            {
                ChangeLightToGray();
            }
            else if (master.myRailblock.hasAgent == true)
            {
                ChangeLightColor(true);
            }
            else
            {
                ChangeLightColor(false);
            }
        }

        public void ChangeLightColor(bool toRed)
        {
            foreach (var light in renderers)
            {
                light.material.color = Color.gray;
            }

            if (toRed)
            {
                greenLight.material.color = Color.red;
            }
            else
            {
                redLight.material.color = Color.green;
            }
        }

        public void ChangeLightToGray()
        {
            foreach (var light in renderers)
            {
                light.material.color = Color.gray;
            }
        }

        public SelectableObjectType GetSelectableObjectType()
        {
            return SelectableObjectType.Traffic;
        }

        public override string ToString()
        {
            return position.ToString() + " " + direction.ToString();
        }

        public void ShowMyUI()
        {
            Debug.Log(this.ToString());
        }
    }
}
