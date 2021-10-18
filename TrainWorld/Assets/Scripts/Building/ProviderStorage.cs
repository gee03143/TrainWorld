using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Buildings
{
    public class ProviderStorage : Storage
    {

        public override int GetItemFromStorage(int amount)
        {
            //아이템을 창고에서 꺼내감
            OnSendItem?.Invoke();
            return amount;
        }

        public override bool IsEmpty()
        {
            return false;
        }

        public override bool IsFull()
        {
            return true;
        }

        public override int GetRemainingSpace()
        {
            return 0;
        }
    }
}
