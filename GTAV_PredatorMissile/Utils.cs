using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Media;
using System.Reflection;
using GTA;
using GTA.Math;
using System.Threading.Tasks;


public static class Utils
{
    /// <summary>
    /// Returns a 3D coordinate on a circle on the given point with the specified center, radius and total amount of points
    /// </summary>
    /// <param name="center">Center of the circle</param>
    /// <param name="radius">Total radius of the circle</param>
    /// <param name="totalPoints">Total points around circumference</param>
    /// <param name="currentPoint">The point on the circle for which to return a coordinate</param>
    /// <returns></returns>
    public static Vector3 DrawCircle(Vector3 center, float radius, float totalPoints, float currentPoint)
    {
        float ptRatio = currentPoint / totalPoints;
        float pointX = center.X + (float)(Math.Cos(ptRatio * 2 * Math.PI)) * radius;
        float pointY = center.Y + (float)(Math.Sin(ptRatio * 2 * Math.PI)) * radius;
        Vector3 panelCenter = new Vector3(pointX, pointY, center.Z);
        return panelCenter;
    }

    /// <summary>
    /// Set damage proofs for an entity
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="bulletProof"></param>
    /// <param name="fireProof"></param>
    /// <param name="explosionProof"></param>
    /// <param name="collisionProof"></param>
    /// <param name="meleeProof"></param>
    /// <param name="drownProof"></param>
    public static void SetDamageProofs(this Entity entity, bool bulletProof, bool fireProof, bool explosionProof, bool collisionProof, bool meleeProof, bool drownProof)
    {
        var proofs = new DamageProofs(entity, bulletProof, fireProof, explosionProof, collisionProof, meleeProof, false, false, drownProof);
        proofs.SetAll();
    }

    /// <summary>
    /// Extension for getting a random item from a list
    /// </summary>
    /// <typeparam name="T">Type of list</typeparam>
    /// <param name="items">The list</param>
    /// <returns></returns>
    public static T GetRandomItem<T>(this IEnumerable<T> items)
    {
        var random = new Random();
        return (T)(object)items.ToList<T>()[random.Next(0, items.Count())];
    }


    public static Vector3 RotationToDirection(Vector3 rotation)
    {
        double retZ = rotation.Z * 0.01745329f;
        double retX = rotation.X * 0.01745329f;
        double absX = Math.Abs(Math.Cos(retX));
        return new Vector3((float)-(Math.Sin(retZ) * absX), (float)(Math.Cos(retZ) * absX), (float)Math.Sin(retX));
    }

    public static Vector3 DirectionToRotation(GTA.Math.Vector3 direction)
    {
        direction.Normalize();

        var x = Math.Atan2(direction.Z, Math.Sqrt(direction.Y * direction.Y + direction.X * direction.X));
        var y = 0;
        var z = -Math.Atan2(direction.X, direction.Y);

        return new Vector3
        {
            X = (float)RadToDeg(x),
            Y = (float)RadToDeg(y),
            Z = (float)RadToDeg(z)
        };
    }


    public static double Hermite(double value1, double tangent1, double value2, double tangent2, double amount)
    {
        // All transformed to double not to lose precission
        // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
        double v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
        double sCubed = s * s * s;
        double sSquared = s * s;

        if (amount == 0f)
            result = value1;
        else if (amount == 1f)
            result = value2;
        else
            result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                t1 * s +
                v1;
        return (double)result;
    }

    public static GTA.Math.Vector3 SmoothStep(GTA.Math.Vector3 start, GTA.Math.Vector3 end, float amount)
    {
        GTA.Math.Vector3 vector;

        amount = (amount > 1.0f) ? 1.0f : ((amount < 0.0f) ? 0.0f : amount);
        amount = (amount * amount) * (3.0f - (2.0f * amount));

        vector.X = start.X + ((end.X - start.X) * amount);
        vector.Y = start.Y + ((end.Y - start.Y) * amount);
        vector.Z = start.Z + ((end.Z - start.Z) * amount);

        return vector;
    }

    public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
    {
        Vector3 a = target - current;
        float magnitude = a.Length();
        if (magnitude <= maxDistanceDelta || magnitude == 0f)
        {
            return target;
        }

        return current + a / magnitude * maxDistanceDelta;
    }

    public static double RadToDeg(double rad)
    {
        return rad * 180.0 / Math.PI;
    }

    public static Vector3 RightVector(this Vector3 position, Vector3 up)
    {
        position.Normalize();
        up.Normalize();
        return Vector3.Cross(position, up);
    }

    public static Vector3 LeftVector(this Vector3 position, Vector3 up)
    {
        position.Normalize();
        up.Normalize();
        return -(Vector3.Cross(position, up));
    }

    /// <summary>
    /// Creates a new configuration file. If one exists, it will be deleted
    /// </summary>
    public static void CreateConfig(string configData)
    {

        Logger.Log("Creating configuration file...");

        string path = "scripts\\GTAV_PredatorMissile.ini";
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
            }

            catch (UnauthorizedAccessException)
            {
                Logger.Log("Failed to create config.");
                return;

            }

            catch (IOException)
            {
                Logger.Log("Failed to create config.");
                return;

            }

            catch (Exception)
            {
                Logger.Log("Failed to create config.");
                return;

            }
        }

        IList<string> list = PopulateItemListFromEmbedded(configData);
        WriteListToFile(list, path);
    }

    /// <summary>
    /// Inverts a config setting and returns a bool
    /// indicating the updated setting
    /// </summary>
    /// <param name="section">The section of the config file</param>
    /// <param name="key">The config setting</param>
    /// <returns></returns>


    /// <summary>
    /// Gets a config setting
    /// </summary>
    /// <param name="section">The section of the config file</param>
    /// <param name="key">The config setting</param>
    /// <returns></returns>
  

    public static IList<string> PopulateItemListFromEmbedded(string resource)
    {
        string[] text = resource.GetLines();
        return new List<string>(text);
    }

    /// <summary>
    /// Concatenates an array of strings with each member on a new line
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string[] GetLines(this string s)
    {
        return s.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
    }

    /// <summary>
    /// Writes a list of strings to a file at the specified path. If one exists, it will be deleted
    /// </summary>
    /// <param name="list">The list to write</param>
    /// <param name="filepath">The specified path</param>
    public static void WriteListToFile(IList<string> list, string filepath)
    {
        if (File.Exists(filepath)) File.Delete(filepath);
        using (StreamWriter stream = new StreamWriter(filepath))
        {
            foreach (string line in list)
            {
                stream.WriteLine(line);
            }
        }
    }

    /// <summary>
    /// Return a random item from a collection
    /// </summary>
    /// <typeparam name="T">The type of the collection</typeparam>
    /// <param name="list">The target collection</param>
    /// <returns></returns>
    public static T GetRandomItem<T>(IList<T> list)
    {
        Type type = typeof(T);
        var rdm = new Random(Guid.NewGuid().GetHashCode()).Next(0, list.Count);
        return (T)(object)list[rdm];
    }


    public static SoundPlayer ReadResourceToSound(string soundName)
    {
        Assembly a = Assembly.GetExecutingAssembly();
        Stream s = a.GetManifestResourceStream(string.Format("{0}.{1}", a.FullName, soundName));
        return new SoundPlayer(s);
    }

}