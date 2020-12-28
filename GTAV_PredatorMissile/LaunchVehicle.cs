using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using GTA.Math;

namespace GTAV_PredatorMissile
{
    public class LaunchVehicle
    {
        private Camera cam;

        private int activeIndex;

        private Ped ped;

        private Vehicle vehicle;

        private List<Vector3> pointRoute;

        public Vehicle Vehicle {  get { return vehicle; } }

        public LaunchVehicle(Vector3 position)
        {
          
            this.activeIndex = 0;
            this.pointRoute = SetupPointRoute(position);
            this.cam = new Camera(Function.Call<int>(Hash.CREATE_CAMERA_WITH_PARAMS, 0x19286a9, pointRoute[activeIndex].X, pointRoute[activeIndex].Y, pointRoute[activeIndex].Z, 0.0f, 0.0f, 0.0f, 65.0f, 0, 2));
            this.cam.IsActive = true;
            this.vehicle = CreateVehicle(pointRoute[activeIndex+ 1]);
            this.ped = CreatePed(PedHash.Pilot01SMY, pointRoute[activeIndex], pointRoute[activeIndex + 1]);

        }

        public LaunchVehicle(Vehicle vehicle)
        {
            this.activeIndex = 0;
            this.vehicle = vehicle;

            this.pointRoute = SetupPointRoute(vehicle.Position);

            this.cam = new Camera(Function.Call<int>(Hash.CREATE_CAMERA_WITH_PARAMS, 0x19286a9, pointRoute[activeIndex].X, pointRoute[activeIndex].Y, pointRoute[activeIndex].Z, 0.0f, 0.0f, 0.0f, 65.0f, 0, 2));
 
            this.cam.AttachTo(vehicle, new Vector3(0, 0, 5f));
            cam.PointAt(vehicle.Position); ;
            this.cam.IsActive = true;
            Function.Call<bool>(Hash.TASK_VEHICLE_DRIVE_TO_COORD, vehicle.GetPedOnSeat(VehicleSeat.Driver).Handle, vehicle.Handle, pointRoute[activeIndex].X, pointRoute[activeIndex].Y, pointRoute[activeIndex].Z, 60F, 1, vehicle.Model.Hash, 262144, 5.0, -1);
        }

        public Vehicle CreateVehicle(Vector3 position)
        {
            return vehicle = World.CreateVehicle(VehicleHash.Titan, pointRoute[activeIndex]);
        }


        private Ped CreatePed(PedHash hash, Vector3 startPos, Vector3 endPos)
        {
            Model model = new Model(hash);
            model.Request(1000);
            ped = Function.Call<Ped>(Hash.CREATE_PED, 26, (int)hash, startPos.X, startPos.Y, startPos.Z, 0, 0, 0);
            Function.Call(Hash.SET_PED_INTO_VEHICLE, ped.Handle, vehicle.Handle, -1);
            Function.Call<bool>(Hash.TASK_VEHICLE_DRIVE_TO_COORD, ped.Handle, vehicle.Handle, endPos.X, endPos.Y, endPos.Z, 60F, 1, vehicle.Model.Hash, 262144, 0.0f, -1);
            return ped;
        }

        public List<Vector3> SetupPointRoute(Vector3 position)
        {
            var list = new List<Vector3>();

            for (int i = 0; i < 3000; i++)
            {
                var vec = Utils.DrawCircle(position, 200f, 3000, i);
                list.Add(vec);
            }

            return list;
        }

        public void Update()
        {
            //   cam.PointAt(pointRoute[activeIndex + 1]);
            vehicle.Rotation = new Vector3(cam.Rotation.X, cam.Rotation.Y - 40F, cam.Rotation.Z - 90F);
          //  var newRot = vehicle.Rotation * q;



       //     vehicle.Rotation = vehicle.ForwardVector * 2;
            vehicle.Position = pointRoute[activeIndex];

            activeIndex++;
            activeIndex = activeIndex % pointRoute.Count;

      //      Script.Wait(1000);
         //   activeIndex

           // if (vehicle.Position.DistanceTo(pointRoute[activeIndex]) < 10f)
         //   {
         //       UI.ShowSubtitle("new coord");
        //        Function.Call<bool>(Hash.TASK_VEHICLE_DRIVE_TO_COORD, ped.Handle, vehicle.Handle, pointRoute[activeIndex + 1].X, pointRoute[activeIndex + 1].Y, pointRoute[activeIndex + 1].Z + 100f, 60F, 1, vehicle.Model.Hash, 786599, 5.0, -1);
        //        activeIndex++;
        //    }
        }
    }
}
