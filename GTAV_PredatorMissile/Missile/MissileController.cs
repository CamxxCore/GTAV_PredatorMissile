using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;

namespace GTAV_PredatorMissile.Missile
{
    public sealed class MissileController : Script
    {
        private static Missile activeMissile;

        public static bool MissileActive { get { return activeMissile != null ? activeMissile.Active : false;  } }

        public MissileController()
        {
            Tick += OnTick;
        }

        public static void SpawnMissile(Vector3 origin, bool hud, bool targetOverlay, bool nightVision)
        {
            if (activeMissile != null) return;

            activeMissile = new Missile(origin)
            {
                HUD = hud,
                TargetOverlay = targetOverlay,
                NightVision = nightVision
            };

            activeMissile.Exploded += ActiveMissile_Exploded;

            activeMissile.StartRoutine();

            Game.Player.Character.SetDamageProofs(true, true, true, true, true, false);

            UI.Notify("Predator Missile Inbound");
        }
    
        private void OnTick(object sender, EventArgs e)
        {
            if (activeMissile != null)
            {
                activeMissile.Update();
            }
        }

        private static void ActiveMissile_Exploded(object sender, EventArgs e)
        {
            UI.Notify("Predator Missile Done");

            Game.Player.Character.SetDamageProofs(false, true, true, false, false, false);

            World.RenderingCamera = null;

            activeMissile.Exploded -= ActiveMissile_Exploded;

            activeMissile = null;
        }

        protected override void Dispose(bool A_0)
        {
            if (activeMissile != null && activeMissile.Active)
                activeMissile.StopRoutine();
        }
    }
}
