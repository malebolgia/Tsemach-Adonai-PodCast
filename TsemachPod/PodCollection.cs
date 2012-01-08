using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsemachPod
{
    class PodCollection
    {
        ArrayList PodList = new ArrayList();
        ArrayList PodListSettings = new ArrayList();

        public void AddPod(Pod pod)
        {
           PodList.Add(pod);
        }

        public void AddPodSettings(Pod pod)
        {
            PodListSettings.Add(pod);
        }


        public Pod GetPod(int index)
        {
            return (Pod)PodList[index];
        }


        public Pod GetPodSettings(int index)
        {
            return (Pod)PodListSettings[index];
        }

        public int Count
        {
            get
            {
                return PodList.Count;
            }
        }

        public int CountSettings
        {
            get
            {
                return PodListSettings.Count;
            }
        }
    }
}
