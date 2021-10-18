using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Buildings
{
    public class ConsumerStorage : Storage
    {
        public override int GiveItemToStorage(int amount)
        {
            OnReceiveItem?.Invoke();
            return 0;
        }

        public override bool IsEmpty()
        {
            return true;
        }

        public override bool IsFull()
        {
            return false;
        }

        public override int GetRemainingSpace()
        {
            return maxStorage;
        }
    }
}
