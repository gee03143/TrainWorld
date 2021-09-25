using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using TrainWorld.Traffic;

namespace TrainWorld
{
    public class Clipboard
    {
        private static List<(TrainStation, DepartureConditionType)> schedules;

        static Clipboard()
        {
            schedules = new List<(TrainStation, DepartureConditionType)>();
        }

        public static void CopyToClipboard(List<(TrainStation, DepartureConditionType)> original)
        {
            schedules.Clear();

            original.ForEach(x => schedules.Add(x));
        }

        public static void PasteFromClipboard(ref List<(TrainStation, DepartureConditionType)> target)
        {
            target.Clear();
            target = schedules;
        }
    }
}