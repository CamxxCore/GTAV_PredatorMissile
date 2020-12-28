using System;
using System.Linq;
using GTA;
using GTA.Native;
using GTA.Math;
using System.Drawing;
using System.Collections.Generic;
using GTAV_GamepadHandler;

namespace GTAV_PredatorMissile.Missile
{
    public class Missile : Entity
    {
        public event EventHandler Exploded;

        GamepadHandler gamepadHandler = new GamepadHandler();

        private List<Ped> _foundPeds = new List<Ped>();
        private int aliveTime;
        private int spawnTime;
        private bool active;
        private bool hud;
        private bool boost;
        private bool targetOverlay;
        private bool nightVision;
        private Vector3 target;
        private List<Entity> targetEntities;
        private Camera mainCamera;
        private GameSound soundLoop1;

        #region Public Variables

        public bool Active
        {
            get { return this.active; }
        }

        public bool HUD
        {
            get { return this.hud; }
            set { this.hud = value; }
        }

        public bool TargetOverlay
        {
            get { return this.targetOverlay; }
            set { this.targetOverlay = value; }
        }

        public bool NightVision
        {
            get { return this.nightVision; }
            set { this.nightVision = value; }
        }

        #endregion

        public Missile(Vector3 origin) :
            base(World.CreateProp("w_lr_rpg_rocket", new Vector3(origin.X, origin.Y, origin.Z), false, false).Handle)
        {
            Function.Call(Hash.SET_ENTITY_RECORDS_COLLISIONS, this, true);
            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, this, true);
            Function.Call(Hash.SET_ENTITY_LOD_DIST, this, 1000);
            this.spawnTime = Game.GameTime;
            this.hud = true;
            this.targetOverlay = true;
            this.target = origin;
            this.mainCamera = World.CreateCamera(new Vector3(Position.X, Position.Y, Position.Z), Vector3.Zero, 60);
            this.mainCamera.AttachTo(this, new Vector3(0, 0, 0f));
            this.gamepadHandler.RightStickChanged += GamepadHandler_RightStickChanged;
            this.soundLoop1 = new GameSound("Helicopter_Wind", "BASEJUMPS_SOUNDS");
            this.targetEntities = World.GetNearbyEntities(origin, 1000f)
                .Where(x => Function.Call<int>(Hash.GET_PED_TYPE, x.Handle) != 28 &&
                x.Handle != Game.Player.Character.Handle).ToList();
        }

        Vector3 newRotation;
        int startTime;


        private void GamepadHandler_RightStickChanged(object sender, AnalogStickChangedEventArgs e)
        {
           newRotation = Rotation + (new Vector3(Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 221) * -4f, 0,
            Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 220) * -5f) * 4.0f);

            newRotation = Vector3.Clamp(newRotation, new Vector3(-170.0f, newRotation.Y, newRotation.Z), new Vector3(-10.0f, newRotation.Y, newRotation.Z));

            startTime = Game.GameTime;
         /*   if (Rotation.Y >= 60.0)
                Rotation = new Vector3(Rotation.X, 90.0F, Rotation.Z);
            else if (Rotation.Y <= -90.0f)
                Rotation = new Vector3(Rotation.X, -90.0f, Rotation.Z);*/
        }

        protected void OnExploded(object sender, EventArgs e)
        {
            Exploded?.Invoke(sender, e);
        }

        public void StartRoutine()
        {
            if (targetEntities.Count < 0) throw new Exception("Missile::EnterView: No targets.");

            Scripts.FadeOutScreen(200, 200);

            var spawnOrigin = Position - new Vector3(0, 0, 500.0f);

            var direction = spawnOrigin - Position;

            var spawnRotation = Utils.DirectionToRotation(direction);

            Rotation = spawnRotation;

            newRotation = spawnRotation;

            mainCamera.Shake(CameraShake.Vibrate, 4f);

            mainCamera.MotionBlurStrength = 10.0f;

            World.RenderingCamera = mainCamera;

            soundLoop1.Play(this);

            if (nightVision)
                Function.Call(Hash.SET_NIGHTVISION, true);

            active = true;

            Scripts.FadeInScreen(200, 200);
        }

        public void StopRoutine()
        {
            mainCamera.Destroy();
            soundLoop1.Destroy();
            MarkAsNoLongerNeeded();
            Delete();
            active = false;
        }

        private void PlayExplosion(Entity ent)
        {
            Scripts.RequestPTFXAsset("scr_oddjobtraffickingair");
            Function.Call(Hash._SET_PTFX_ASSET_NEXT_CALL, "scr_oddjobtraffickingair");

            if (ent.IsInWater)
                Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, "scr_ojdg4_water_exp", ent.Position.X, ent.Position.Y, ent.Position.Z, 0.0, 0.0, 0.0, 4.0, 0, 0, 0);
            else
                Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, "scr_drug_grd_train_exp", ent.Position.X, ent.Position.Y, ent.Position.Z, 0.0, 0.0, 0.0, 4.0, 0, 0, 0);

            World.AddExplosion(ent.Position, ExplosionType.Train, 1.0f, 1.0f);
            Script.Wait(20);
            World.AddExplosion(ent.Position + ent.Position.LeftVector(Vector3.WorldUp) * 3, (ExplosionType)17, 30f, 1.5f);
            Script.Wait(20);
            World.AddExplosion(ent.Position + ent.Position.RightVector(Vector3.WorldUp) * 3, (ExplosionType)26, 30f, 1.5f);
            Script.Wait(20);
            World.AddExplosion(ent.Position + ent.ForwardVector * 3, (ExplosionType)17, 30f, 1.5f);
            Script.Wait(20);
            World.AddExplosion(ent.Position - ent.ForwardVector * 3, (ExplosionType)26, 30f, 1.5f);

            if (Function.Call<bool>(Hash.REQUEST_SCRIPT_AUDIO_BANK, "FBI_HEIST_ELEVATOR_DEBRIS_01", 0))
            {
                Function.Call(Hash.PLAY_SOUND_FROM_ENTITY, -1, "Explosion_01", Game.Player.Character.Handle, "FBI_HEIST_ELEVATOR_SHAFT_DEBRIS_SOUNDS", 0, 0);
            }

            if (Function.Call<bool>(Hash.REQUEST_SCRIPT_AUDIO_BANK, "FBI_Heist_Finale_Building_Explosion", 0))
            {
                Function.Call(Hash.PLAY_SOUND_FROM_ENTITY, -1, "detonation", Game.Player.Character.Handle, "FBI_HEIST_RAID", 0, 0);
            }
        }

        public static void DrawHUD(Color color)
        {
            var location = new Point(UI.WIDTH / 3, UI.HEIGHT - UI.HEIGHT / 4);
            var rect = new UIRectangle(new Point(location.X - 30, location.Y), new Size(5, 65));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X - 30, location.Y + 60), new Size(65, 5));
            rect.Color = color;
            rect.Draw();
            location = new Point(UI.WIDTH / 3, UI.HEIGHT / 4);
            rect = new UIRectangle(new Point(location.X - 30, location.Y), new Size(5, 67));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X - 30, location.Y), new Size(65, 5));
            rect.Color = color;
            rect.Draw();
            location = new Point(UI.WIDTH - UI.WIDTH / 3, UI.HEIGHT / 4);
            rect = new UIRectangle(new Point(location.X + 30, location.Y), new Size(5, 65));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X - 30, location.Y), new Size(65, 5));
            rect.Color = color;
            rect.Draw();
            location = new Point(UI.WIDTH - UI.WIDTH / 3, UI.HEIGHT - UI.HEIGHT / 4);
            rect = new UIRectangle(new Point(location.X + 30, location.Y), new Size(5, 65));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X - 30, location.Y + 60), new Size(65, 5));
            rect.Color = color;
            rect.Draw();
            location = new Point(UI.WIDTH / 2, UI.HEIGHT / 2);
            rect = new UIRectangle(new Point(location.X - 40, location.Y), new Size(5, 65));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X - 40, location.Y), new Size(80, 5));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X + 40, location.Y), new Size(5, 65));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X - 40, location.Y + 60), new Size(80, 5));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X - 120, location.Y + 30), new Size(80, 2));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X - 120, location.Y + 30), new Size(80, 2));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X + 45, location.Y + 30), new Size(80, 2));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X + 45, location.Y + 30), new Size(80, 2));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X, location.Y + 60), new Size(2, 80));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X, location.Y - 75), new Size(2, 80));
            rect.Color = color;
            rect.Draw();
        }

        public static void DrawHUD(Point location, Color color)
        {
            var rect = new UIRectangle(new Point(location.X - 30, location.Y), new Size(7, 67));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X + 30, location.Y), new Size(7, 67));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X - 30, location.Y + 60), new Size(67, 7));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X - 30, location.Y), new Size(67, 7));
            rect.Color = color;
            rect.Draw();
        }

        private bool GetControlInput(Control control)
        {
            return Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, (int)control);
        }

        private void CheckControlState()
        {
            Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 0);

            if (GetControlInput(Control.VehicleDriveLook) ||
                GetControlInput(Control.ReplayNewmarker))
            {
                boost = true;
            }

            if (GetControlInput(Control.ScriptPadUp) ||
                GetControlInput(Control.ParachutePitchUpOnly) ||
                GetControlInput(Control.ScaledLookUpOnly))
            {
                Position += Vector3.RelativeFront;
            }

            if (GetControlInput(Control.ScriptPadDown) ||
                GetControlInput(Control.VehicleMoveDown) ||
                GetControlInput(Control.VehicleSlowMoUpDown))
            {
                Position += Vector3.RelativeBack;
            }

            if (GetControlInput(Control.ScriptPadLeft) ||
                GetControlInput(Control.ParachuteTurnLeftOnly) ||
                GetControlInput(Control.ScaledLookLeftOnly))
            {
                Position += Vector3.RelativeLeft;
            }

            if (GetControlInput(Control.ScriptPadRight) ||
                GetControlInput(Control.VehicleMoveRight) ||
                GetControlInput(Control.RadioWheelLeftRight))
            {
                Position += Vector3.RelativeRight;
            }
        }
    
        float moveDist = 20.0f;

        public void Update()
        {
            gamepadHandler.Update();


            UI.ShowSubtitle(Rotation.ToString());

            Function.Call(Hash.REQUEST_COLLISION_AT_COORD, Position.X, Position.Y, Position.Z);
            

            if (Function.Call<bool>(Hash.HAS_ENTITY_COLLIDED_WITH_ANYTHING, this))/*World.Raycast(Position, Position + target * 0.2f, IntersectOptions.Everything).DitHitAnything*/
            {
                if (IsInWater)
                    Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, "scr_ojdg4_water_exp", Position.X, Position.Y, Position.Z, 0.0, 0.0, 0.0, 4.0, 0, 0, 0);
                else
                    Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, "scr_drug_grd_train_exp", Position.X, Position.Y, Position.Z, 0.0, 0.0, 0.0, 4.0, 0, 0, 0);

                World.AddExplosion(Position, ExplosionType.Train, 1.0f, 1.0f);
                Script.Wait(20);
                World.AddExplosion(Position + Position.LeftVector(Vector3.WorldUp) * 3, (ExplosionType)17, 30f, 1.5f);
                Script.Wait(20);
                World.AddExplosion(Position + Position.RightVector(Vector3.WorldUp) * 3, (ExplosionType)26, 30f, 1.5f);
                Script.Wait(20);
                World.AddExplosion(Position + ForwardVector * 3, (ExplosionType)17, 30f, 1.5f);
                Script.Wait(20);
                World.AddExplosion(Position - ForwardVector * 3, (ExplosionType)26, 30f, 1.5f);

                StopRoutine();

                OnExploded(this, new EventArgs());

                if (nightVision)
                    Function.Call(Hash.SET_NIGHTVISION, false);
                return;
            }

        //   CheckControlState();

            target = Position + Utils.RotationToDirection(Rotation);

            moveDist += 0.8f * aliveTime;

            if (moveDist >= 100.0f) moveDist = 50.0f;

            ApplyForce(ForwardVector * moveDist);

            Rotation = Vector3.Lerp(Rotation, newRotation, Game.LastFrameTime * 10.0F);

            mainCamera.Rotation = Rotation;

            if (targetOverlay)
            {
                this.targetEntities = World.GetNearbyEntities(Position, 1000f)
              .Where(x => Function.Call<int>(Hash.GET_PED_TYPE, x.Handle) != 28 &&
              x.Handle != Game.Player.Character.Handle).ToList();

                for (int i = 0; i < (targetEntities.Count < 30 ? targetEntities.Count : 30); i++)
                {
                    if ((Function.Call<bool>(Hash.IS_ENTITY_A_VEHICLE, targetEntities[i].Handle) &&
                        !new Vehicle(targetEntities[i].Handle).EngineRunning) ||
                        targetEntities[i].Handle == 0 ||
                        !targetEntities[i].IsAlive)
                        continue;

                    var pos = targetEntities[i].Position;
                    Function.Call(Hash.SET_DRAW_ORIGIN, pos.X, pos.Y, pos.Z, 0);
                    DrawHUD(new Point(), Color.Red);
                    Function.Call(Hash.CLEAR_DRAW_ORIGIN);
                }
            }

            if (hud)
                DrawHUD(Color.White);

            if (boost)
                moveDist = 4.0f;

            aliveTime = Game.GameTime - spawnTime;
        }

    }
}