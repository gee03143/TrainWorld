using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld
{
    public class Grid<T>
    {
        private int width;
        private int height;

        private T[,] data; 

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public Grid(int w, int h)
        {
            this.width = w;
            this.height = h;
        }

        public T this[int x,int y]
        {
            get
            {
                if (x < 0 || x >= width || y < 0 || y >= height)
                    throw new IndexOutOfRangeException("Index out of range");

                return data[x, y];
            }

            set
            {
                if (x < 0 || x >= width || y < 0 || y >= height)
                    throw new IndexOutOfRangeException("Index out of range");

                data[x, y] = value;
            }
        }
        
        public T GetGridCellAt(Vector3Int position)
        {
            return data[position.x, position.z];
        }

    }

}