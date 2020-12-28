using System;
using System.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GTA;
using GTA.Native;
using System.Drawing;
using System.Windows.Forms;
using GTAV_PredatorMissile.Missile;

namespace GTAV_PredatorMissile
{
    public class ScriptMain : Script
    {
        bool missileAvailable = false;
        int _KillsRequired = 0;
        List<Ped> killedPeds = new List<Ped>();
        private Keys _activationKey;
        private bool _UseNightvision, _UseHUD, _UseRedboxes, _UseAnnouncer;

        ExternalSound fireSound = new ExternalSound(Properties.Resources.missilefire1);

        private static readonly UnmanagedMemoryStream[] s_predatorUse_SoundList =
            new UnmanagedMemoryStream[] { Properties.Resources.NS_1mc_use_predator_02,
            Properties.Resources.PG_1mc_use_predator_01,
            Properties.Resources.US_1mc_use_predator_01,
            Properties.Resources.US_1mc_use_predator_02,
             Properties.Resources.UK_1mc_use_predator_02
            };

        private static readonly UnmanagedMemoryStream[] s_predatorAchieved_SoundList =
            new UnmanagedMemoryStream[] {
            Properties.Resources.NS_1mc_achieve_predator_01,
            Properties.Resources.PG_1mc_achieve_predator_01,
            Properties.Resources.PG_1mc_achieve_predator_02,
            Properties.Resources.US_1mc_achieve_predator_01,
            Properties.Resources.UK_1mc_achieve_predator_01
            };

        public ScriptMain()
        {
            LoadConfig();
            Tick += OnTick;
            KeyUp += KeyJustUp;
        }

        private void LoadConfig()
        {
            string path = "scripts\\GTAV_PredatorMissile.ini";

            if (!File.Exists(path))
                Utils.CreateConfig(Properties.Resources.GTAV_PredatorMissile);

            _KillsRequired = Configuration.GetConfigSetting<int>("General", "Killstreak");
            _UseHUD = Configuration.GetConfigSetting<bool>("General", "HUD");
            _UseRedboxes = Configuration.GetConfigSetting<bool>("General", "Redboxes");
            _UseNightvision = Configuration.GetConfigSetting<bool>("General", "Nightvision");
            _UseAnnouncer = Configuration.GetConfigSetting<bool>("General", "Announcer");
            _activationKey = Configuration.GetConfigSetting<Keys>("KeyBinds", "Activate");
        }

        private void KeyJustUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == _activationKey)
            {
                BeginMissileSequence();
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            MonitorPlayer();
        }

        private void BeginMissileSequence()
        {
            if (_UseAnnouncer)
            {
                fireSound.Play();
            }

            var origin = Game.Player.Character.Position;

            var spawnOrigin = origin + new GTA.Math.Vector3(0, 0, 500.0f);

            MissileController.SpawnMissile(spawnOrigin, _UseHUD, _UseRedboxes, _UseNightvision);

            UI.Notify("Predator Missile Inbound");
        }

        private void MonitorPlayer()
        {
            Ped player = Game.Player.Character;

            if (killedPeds.Count() >= _KillsRequired && !missileAvailable)
            {
                missileAvailable = true;

                if (_UseAnnouncer)
                {
                    using (var sound = new ExternalSound(s_predatorAchieved_SoundList.GetRandomItem()))
                    {
                        sound.Play();
                    }
                }

                killedPeds.Clear();
            }

            else
            {
                if (!missileAvailable)
                {
                    foreach (Ped ped in World.GetAllPeds())
                    {
                        if (killedPeds.Find(item => item.Handle == ped.Handle) != null) continue;

                        if (ped.IsDead && ped.GetKiller() == player)
                        {
                            if (ped.GetKiller() == player)
                            {
                                killedPeds.Add(ped);
                            }
                        }
                    }
                }
            }
        }
    }
}
