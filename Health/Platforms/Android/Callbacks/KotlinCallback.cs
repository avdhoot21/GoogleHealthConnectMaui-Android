using Android.Runtime;
using AndroidX.Health.Connect.Client;
using AndroidX.Health.Connect.Client.Aggregate;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Health.MainPage;

namespace Health.Platforms.Android.Callbacks
{
    internal class KotlinCallback
    {
        private IHealthConnectClient healthConnectClient;
        public KotlinCallback(IHealthConnectClient client)
        {
            healthConnectClient = client;
        }

        public enum MyCoroutineSingletons
        {
            COROUTINE_SUSPENDED,
            UNDECIDED,
            RESUMED
        }
        public async Task<List<AggregationResultGroupedByDuration>> AggregateGroupByDuration(global::AndroidX.Health.Connect.Client.Request.AggregateGroupByDurationRequest request)
        {
            var tcs = new TaskCompletionSource<Java.Lang.Object>();
            Java.Lang.Object result = healthConnectClient.AggregateGroupByDuration(request,new Continuation(tcs, default));

            if (result is Java.Lang.Enum CoroutineSingletons)
            {
                MyCoroutineSingletons checkedEnum = (MyCoroutineSingletons)Enum.Parse(typeof(MyCoroutineSingletons), CoroutineSingletons.ToString());
                if (checkedEnum == MyCoroutineSingletons.COROUTINE_SUSPENDED)
                {
                    result = await tcs.Task;
                }
            }
            if (result is Java.Util.AbstractList javaLists)
            {
                List<AggregationResultGroupedByDuration> dotNetList = new List<AggregationResultGroupedByDuration>();
                for (int i = 0; i < javaLists.Size(); i++)
                {
                    if (javaLists.Get(i) is AggregationResultGroupedByDuration item)
                    {
                        dotNetList.Add(item);
                    }
                }
                return dotNetList;
            }
            if (result is JavaList javaList)
            {
                List<AggregationResultGroupedByDuration> dotNetList = new List<AggregationResultGroupedByDuration>();
                for (int i = 0; i < javaList.Size(); i++)
                {
                    if (javaList.Get(i) is AggregationResultGroupedByDuration item)
                    {
                        dotNetList.Add(item);
                    }
                }
                return dotNetList;
            }
            return null;
        }


        public async Task<List<string>> GetGrantedPermissions()
        {
            var tcs = new TaskCompletionSource<Java.Lang.Object>();
            Java.Lang.Object result = healthConnectClient.PermissionController.GetGrantedPermissions(new Continuation(tcs, default));
            
            if (result is Java.Lang.Enum CoroutineSingletons)
            {
                MyCoroutineSingletons checkedEnum = (MyCoroutineSingletons)Enum.Parse(typeof(MyCoroutineSingletons), CoroutineSingletons.ToString());
                if (checkedEnum == MyCoroutineSingletons.COROUTINE_SUSPENDED)
                {
                    result = await tcs.Task;
                   return GetJavaObject(result.ToString());
                }
            }
            
            if (result is Kotlin.Collections.AbstractMutableSet abstractMutableSet)
            {
                Java.Util.ISet javaSet = abstractMutableSet.JavaCast<Java.Util.ISet>();
                return ConvertISetToList(javaSet);
            }
            return null;
        }

        static List<string> GetJavaObject(string result)
        {
            // Your result string (with permissions enclosed in square brackets)
            //  result = "{[android.permission.health.READ_ACTIVE_CALORIES_BURNED, android.permission.health.READ_BASAL_BODY_TEMPERATURE, android.permission.health.READ_BASAL_METABOLIC_RATE, android.permission.health.READ_BLOOD_GLUCOSE, android.permission.health.READ_BLOOD_PRESSURE, android.permission.health.READ_BODY_FAT, android.permission.health.READ_BODY_TEMPERATURE, android.permission.health.READ_BODY_WATER_MASS, android.permission.health.READ_BONE_MASS, android.permission.health.READ_CERVICAL_MUCUS, android.permission.health.READ_EXERCISE, android.permission.health.READ_DISTANCE, android.permission.health.READ_ELEVATION_GAINED, android.permission.health.READ_FLOORS_CLIMBED, android.permission.health.READ_HEART_RATE, android.permission.health.READ_HEART_RATE_VARIABILITY, android.permission.health.READ_HEIGHT, android.permission.health.READ_HYDRATION, android.permission.health.READ_INTERMENSTRUAL_BLEEDING, android.permission.health.READ_LEAN_BODY_MASS, android.permission.health.READ_MENSTRUATION, android.permission.health.READ_NUTRITION, android.permission.health.READ_OVULATION_TEST, android.permission.health.READ_OXYGEN_SATURATION, android.permission.health.READ_POWER, android.permission.health.READ_RESPIRATORY_RATE, android.permission.health.READ_RESTING_HEART_RATE, android.permission.health.READ_SEXUAL_ACTIVITY, android.permission.health.READ_SLEEP, android.permission.health.READ_SPEED, android.permission.health.READ_STEPS, android.permission.health.READ_TOTAL_CALORIES_BURNED, android.permission.health.READ_VO2_MAX, android.permission.health.READ_WEIGHT, android.permission.health.READ_WHEELCHAIR_PUSHES]}";

            // Remove the curly braces and split by comma to extract permissions
            /*  result = result.Trim('{', '[', ']', '}'); // Trim the surrounding brackets and braces
              string[] permissionsArray = result.Split(new string[] { ", " }, StringSplitOptions.None); // Split by comma + space

              // Convert to Java.Lang.Object array (Java's equivalent of string[])
              Java.Lang.Object[] javaPermissionsArray = new Java.Lang.Object[permissionsArray.Length];
              for (int i = 0; i < permissionsArray.Length; i++)
              {
                  javaPermissionsArray[i] = new Java.Lang.String(permissionsArray[i]);
              }

              return javaPermissionsArray; // Return as Java.Lang.Object[]*/
            result = result.Trim('{', '[', ']', '}'); // Trim the surrounding brackets and braces
            string[] permissionsArray = result.Split(new string[] { ", " }, StringSplitOptions.None); // Split by comma + space

            // Convert to List<string>
            List<string> permissionsList = new List<string>(permissionsArray);

            return permissionsList; // Return as List<string>
        }


        public static List<string> ConvertISetToList(ISet javaSet)
        {
            List<string> listOfStrings = new List<string>();
            var iterator = javaSet.Iterator();

            while (iterator.HasNext)
            {
                Java.Lang.Object element = iterator.Next();
                listOfStrings.Add((string)element.JavaCast<Java.Lang.String>());
            }

            return listOfStrings;
        }

    }
}
