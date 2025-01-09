using AndroidX.Health.Connect.Client;
using AndroidX.Health.Connect.Client.Aggregate;
using AndroidX.Health.Connect.Client.Request;
using AndroidX.Health.Connect.Client.Records;
using Health.Platforms.Android.Callbacks;
using AndroidX.Health.Connect.Client.Time;
using AndroidX.Health.Connect.Client.Records.Metadata;
using Java.Time;
using AndroidX.Activity.Result.Contract;
using AndroidX.Activity.Result;
using AndroidX.Health.Connect.Client.Permission;
using Android.Runtime;
using Kotlin.Jvm.Internal;
using Java.Util;
using Health.Platforms.Android.Permissions;
using AndroidX.Health.Connect.Client.Units;
using Android.Health.Connect.DataTypes.Units;
using Android.App;


namespace Health
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private Instant DateTimeToInstant(DateTime date)
        {
            long unixTimestamp = ((DateTimeOffset)date).ToUnixTimeSeconds();
            return Instant.OfEpochSecond(unixTimestamp);
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            DateTime startOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local);
            Instant startTime = DateTimeToInstant(startOfDay);
            Instant endTime = DateTimeToInstant(DateTime.Now);
            WeightRecord weightRecord = new WeightRecord(startTime, ZoneOffset.OfHours(2), AndroidX.Health.Connect.Client.Units.Mass.InvokeKilograms(50), new Metadata());
            StepsRecord stepsRecord = new StepsRecord(startTime, ZoneOffset.OfHours(2), endTime, ZoneOffset.OfHours(1), 1, new Metadata());
            DistanceRecord distanceRecord = new DistanceRecord(startTime, ZoneOffset.OfHours(2), endTime, ZoneOffset.OfHours(1), AndroidX.Health.Connect.Client.Units.Length.InvokeMeters(11), new Metadata());

            List<string> PermissionsToGrant = new List<string>();
            PermissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(weightRecord.Class)));
            PermissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(stepsRecord.Class)));
            PermissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(distanceRecord.Class)));
            string providerPackageName = "com.google.android.apps.healthdata";
            int availabilityStatus = HealthConnectClient.GetSdkStatus(Platform.CurrentActivity, providerPackageName);

            if (availabilityStatus == HealthConnectClient.SdkUnavailable)
            {
                return; // early return as there is no viable integration
            }

            if (availabilityStatus == HealthConnectClient.SdkUnavailableProviderUpdateRequired)
            {
                // Optionally redirect to package installer to find a provider, for example:
                Platform.CurrentActivity.StartActivity(new Android.Content.Intent(Android.Content.Intent.ActionView, Android.Net.Uri.Parse($"market://details?id={providerPackageName}&url=healthconnect%3A%2F%2Fonboarding"))
                    .SetPackage("com.android.vending")
                    .PutExtra("overlay", true)
                    .PutExtra("callerId", Platform.CurrentActivity.PackageName));
                return;
            }


            if (OperatingSystem.IsAndroidVersionAtLeast(26) && (availabilityStatus == HealthConnectClient.SdkAvailable))
            {

                ICollection<AggregateMetric> metrics = new List<AggregateMetric> { StepsRecord.CountTotal, DistanceRecord.DistanceTotal, WeightRecord.WeightMax, WeightRecord.WeightMin };

                ICollection<DataOrigin> dataOrginFilter = new List<DataOrigin>();

                AggregateGroupByDurationRequest request = new AggregateGroupByDurationRequest(metrics, TimeRangeFilter.After(startTime), Duration.OfDays(1), dataOrginFilter);


                try
                {
                    var healthConnectClient = new KotlinCallback(HealthConnectClient.GetOrCreate(Android.App.Application.Context));

                    List<string> GrantedPermissions = await healthConnectClient.GetGrantedPermissions();

                    List<string> MissingPermissions = PermissionsToGrant.Except(GrantedPermissions).ToList();

                    if (MissingPermissions.Count > 0)
                    {
                        GrantedPermissions = await PermissionHandler.Request(new HashSet(PermissionsToGrant));
                    }

                    bool allPermissionsGranted = PermissionsToGrant.All(permission => GrantedPermissions.Contains(permission));
                    if (allPermissionsGranted)
                    {
                        var Result = await healthConnectClient.AggregateGroupByDuration(request);
                        var Weight = Result.FirstOrDefault(x => x.Result.Contains(WeightRecord.WeightMax))?.Result.Get(WeightRecord.WeightMax).JavaCast<AndroidX.Health.Connect.Client.Units.Mass>();

                        var StepCountTotal = Result.FirstOrDefault(x => x.Result.Contains(StepsRecord.CountTotal))?.Result.Get(StepsRecord.CountTotal).JavaCast<Java.Lang.Number>();
                        var DistanceTotal = Result.FirstOrDefault(x => x.Result.Contains(DistanceRecord.DistanceTotal))?.Result.Get(DistanceRecord.DistanceTotal).JavaCast<AndroidX.Health.Connect.Client.Units.Length>();


                        if (StepCountTotal != null)
                        {
                            CounterBtn.Text = "StepsCount:" + StepCountTotal.ToString() + " " + "DistanceTotal" + DistanceTotal.Meters.ToString() + " " + "Weight" + Weight;
                            DisplayAlert("Health Data", "StepsCount:" + StepCountTotal.ToString() + " " + "DistanceTotal" + DistanceTotal.Meters.ToString() + " " + "Weight" + Weight, "OK");
                            SemanticScreenReader.Announce(CounterBtn.Text);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }

        private void ConnectBtn_Clicked(object sender, EventArgs e)
        {

        }
    }

}
