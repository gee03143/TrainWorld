using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TrainWorld.Buildings
{
    public class Storage : MonoBehaviour
    {
        public Action OnReceiveItem;
        public Action OnSendItem;
        public Action OnDataChanged;
        public Action OnStorageFull;
        public Action OnStorageEmpty;

        [SerializeField]
        private Image storageUIBar; 

        [SerializeField]
        private int currentStorage = 0;

        public int CurrentStorage
        {
            get { return currentStorage; }
            private set {
                currentStorage = Mathf.Min(value, maxStorage);
                ChangeStorageUIBar();
                if (currentStorage == maxStorage)
                {
                    OnStorageFull?.Invoke();
                }
                else if (currentStorage == 0)
                {
                    OnStorageEmpty?.Invoke();
                }
            }
        }

        [SerializeField]
        public int maxStorage = 100;

        public virtual int GetItemFromStorage(int amount)
        {
            //아이템을 창고에서 꺼내감
            int supply = amount;

            if (CurrentStorage >= amount)
                CurrentStorage -= amount;
            else
            {
                supply = CurrentStorage;
                CurrentStorage = 0;
            }
            OnSendItem?.Invoke();
            return supply;
        }

        private void ChangeStorageUIBar()
        {
            if(storageUIBar == null)
            {
                Debug.Log("storage ui bar is null");
                return;
            }
            storageUIBar.fillAmount = (float)CurrentStorage / (float)maxStorage;
        }

        public virtual int GiveItemToStorage(int amount)
        {
            //창고에 아이템 적재, remains는 창고에 넣고 남은 양
            int remains = 0;
            CurrentStorage += amount;

            if (CurrentStorage > maxStorage)
            {
                remains = CurrentStorage - maxStorage;
                CurrentStorage = maxStorage;
            }
            OnReceiveItem?.Invoke();
            return remains;
        }

        public void ClearAllActions()
        {
            OnReceiveItem = OnSendItem = OnDataChanged = OnStorageFull = null;
        }

        public virtual bool IsEmpty()
        {
            return CurrentStorage == 0;
        }

        public virtual bool IsFull()
        {
            return CurrentStorage == maxStorage;
        }

        public virtual int GetRemainingSpace()
        {
            return maxStorage - CurrentStorage;
        }
    }
}
