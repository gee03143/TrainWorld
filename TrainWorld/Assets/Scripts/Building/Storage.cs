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

        public int GetItemFromStorage(int amount)
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

        public int GiveItemToStorage(int amount)
        {
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

        internal bool IsEmpty()
        {
            return CurrentStorage == 0;
        }

        internal bool IsFull()
        {
            return CurrentStorage == maxStorage;
        }

        internal int GetRemainingSpace()
        {
            return maxStorage - CurrentStorage;
        }
    }
}
