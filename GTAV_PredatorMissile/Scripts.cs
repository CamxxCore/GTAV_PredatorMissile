using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using GTA.Math;

public static class Scripts
{
    /// <summary>
    /// Fade screen to black
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="wait"></param>
    public static void FadeOutScreen(int duration, int wait)
    {
        Function.Call(Hash.DO_SCREEN_FADE_OUT, duration);
        Script.Wait(wait);
    }

    /// <summary>
    /// Fade screen in
    /// </summary>
    /// <param name="wait"></param>
    /// <param name="duration"></param>
    public static void FadeInScreen(int wait, int duration)
    {
        Script.Wait(wait);
        Function.Call(Hash.DO_SCREEN_FADE_IN, duration);
    }

    public static void GetGroundZfor3DCoord(Vector3 coord, out Vector3 result)
    {
        OutputArgument zcoord = new OutputArgument();
        Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, coord.X, coord.Y, coord.Z, zcoord);
        result = new Vector3(coord.X, coord.Y, zcoord.GetResult<float>());
    }

    public static bool GetControlInput(Resources.ControlInput control)
    {
        return Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, (int)control);
    }

    public static void RequestPTFXAsset(string name)
    {
        if (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, name))
        {
            Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, name);

            while (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, name))
                Script.Wait(0);
        }
    }

}
