using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TrainWorld.Buildings
{
    public class Inserter : MonoBehaviour
    {
        [SerializeField]
        int InserterSpeed = 1;
        List<Storage> provider;
        List<Storage> receiver;

        Storage agnetStorage;
        List<Storage> connectedStorages;
        DepartureConditionType inserterMode;

        private void Awake()
        {
            provider = new List<Storage>();
            receiver = new List<Storage>();
            connectedStorages = new List<Storage>();
        }

        public void AddProvider(Storage storage)
        {
            provider.Add(storage);
        }


        public void AddProviders(List<Storage> storages)
        {
            provider.AddRange(storages);
        }

        public void RemoveProvider(Storage storage)
        {
            provider.Remove(storage);
        }

        public void AddReceiver(Storage storage)
        {
            receiver.Add(storage);
        }

        public void AddReceivers(List<Storage> storages)
        {
            receiver.AddRange(storages);
        }

        public void SetStorages(Storage agent, List<Storage> storages, DepartureConditionType mode)
        {
            agnetStorage = agent;
            connectedStorages = storages;
            inserterMode = mode;

            if(inserterMode == DepartureConditionType.Load)
            {
                provider = connectedStorages;
                receiver = new List<Storage> { agent };
            }else if(inserterMode == DepartureConditionType.Unload)
            {
                receiver = connectedStorages;
                provider = new List<Storage> { agent };
            }
        }

        public void RemoveReceiver(Storage storage)
        {
            receiver.Remove(storage);
        }

        private Storage PollNextProvider(Storage currentProvider)
        {
            int providerIndex = provider.IndexOf(currentProvider);

            if (currentProvider != provider.Last())
                return provider[providerIndex + 1];
            else
                return  null;
        }

        private Storage PollNextReceiver(Storage currentReceiver)
        {
            int receiverIndex = receiver.IndexOf(currentReceiver);

            if (currentReceiver != receiver.Last())
                return receiver[receiverIndex + 1];
            else
                return null;
        }


        public void StartInserterCoroutine() 
        {
            StartCoroutine("InserterCoroutine");
        }

        public void StopInserterCoroutine()
        {
            StopCoroutine("InserterCoroutine");
        }

        IEnumerator InserterCoroutine()
        {
            while (true)
            {
                InserterLoop();

                yield return new WaitForSeconds(1.0f);
            }
        }

        private void InserterLoop()
        {
            int remainingSpace = CheckReceiverSpace(InserterSpeed); // 여유 공간 확인
            int supply = TryReceiveFromProvider(remainingSpace);
            // generator 인가? 공급자로부터 item을 가져옴 : 아이템을 생성함
            TrySendToReceiver(supply); // 가져온 양만큼 전달
        }

        private int CheckReceiverSpace(int amount)
        {
            int remainSpace = 0;
            if (receiver.Count == 0)
                return 0;

            Storage currentReceiver = receiver.Count != 0 ? receiver.First() : null;
            while (currentReceiver != null)
            {
                remainSpace += currentReceiver.GetRemainingSpace();

                if (remainSpace >= amount)
                {
                    remainSpace = amount;
                    break;
                }

                currentReceiver = PollNextReceiver(currentReceiver);
                if (currentReceiver == null)
                    break;
            }

            return remainSpace;
        }

        private int TrySendToReceiver(int amount)
        {
            int remains = amount;
            if (receiver.Count == 0)
                return remains;

            Storage currentReceiver = receiver.Count != 0 ? receiver.First() : null;
            while (currentReceiver != null)
            {
                if (currentReceiver.IsFull())
                {
                    currentReceiver = PollNextReceiver(currentReceiver);
                    if (currentReceiver == null)
                        continue;
                }

                remains = currentReceiver.GiveItemToStorage(remains);

                if (remains == 0)
                    return 0;
            }

            return remains;
        }

        private int TryReceiveFromProvider(int request)
        {
            int obtained = 0;
            if (provider.Count == 0)
                return 0;

            Storage currentProvider = provider.Count != 0 ? provider.First() : null;
            while (currentProvider != null)
            {
                if (currentProvider.IsEmpty())
                {
                    currentProvider = PollNextProvider(currentProvider);
                    if (currentProvider == null)
                        continue;
                }

                obtained += currentProvider.GetItemFromStorage(request - obtained);

                if (obtained >= request)
                {
                    obtained = request;
                    return obtained;
                }
            }
            return obtained;
        }

        internal void ClearProviderReceiver()
        {
        }
    }
}
