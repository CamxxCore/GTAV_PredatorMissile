using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;

    public class iFruit : List<iFruitContact>
    {
        private bool _shouldDraw;

        public void AddContact(iFruitContact contact)
        {
            base.Add(contact);
        }

        public void Update()
        {
            int mHash = Function.Call<int>(Hash.GET_HASH_KEY, "appcontacts");
            if (Function.Call<int>(Hash._GET_NUMBER_OF_INSTANCES_OF_STREAMED_SCRIPT, mHash) > 0 && _shouldDraw)
            {
                base.ForEach(x => x.Draw());
                _shouldDraw = false;
            }

            else _shouldDraw = true;
        }
    }