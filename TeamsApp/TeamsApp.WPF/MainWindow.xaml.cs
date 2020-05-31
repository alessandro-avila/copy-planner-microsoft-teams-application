using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TeamsAppLib.Api;
using TeamsAppLib.Factory;
using TeamsAppLib.Helpers;
using TeamsAppLib.Log;
using TeamsAppLib.Models;
using TeamsAppLib.Settings;
using PlannerPlan = TeamsAppLib.Models.PlannerPlan;
using PlannerPlanDetails = TeamsAppLib.Models.PlannerPlanDetails;

namespace TeamsAppWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _connected = false;
        private string token = null;

        private AuthenticationHelper authHelper;
        private string _displayName = null;

        private PlannerPlan sourcePlanner = null;
        private PlannerPlan destinationPlanner = null;

        public MainWindow()
        {
            InitializeComponent();

            // Authentication Helper.
            AuthenticationHelper.Init(
                App.Current.Resources["ida:ClientID"].ToString(),
                App.Current.Resources["ida:ReturnUrl"].ToString()
                );
            authHelper = AuthenticationHelper.Instance;
            if (authHelper == null)
            {
                InfoText.Text = "Error in initializing the Authentication Provider.";
                return;
            }

            this.LoadTeamsButton.IsEnabled = false;
            InfoText.Text = "Press the Connect button to connect to Office 365.";
        }

        private async Task<bool> SignInCurrentUserAsync()
        {
            var authResult = await authHelper.GetAccessTokenAsync();
            if (authResult?.AccessToken != null)
            {
                this.token = authResult.AccessToken;
                this._displayName = authResult.Account.Username;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!Application.Current.Resources.Contains("ida:ClientID"))
            {
                InfoText.Text = Constants.MESSAGE_ERROR_APPNOTREGISTERED;
                ConnectButton.IsEnabled = false;
                return;
            }
            InfoText.Text = Constants.MESSAGE_INFO_PRESSCONNECTBUTTON;
            ConnectButton.IsEnabled = true;
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;

            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (!_connected)
            {
                try
                {
                    if (await SignInCurrentUserAsync())
                    {
                        cs.Debug("SignIn done.");
                        InfoText.Text = $"Hi {_displayName}!\nClick on the \"Load Teams\" button to start.";
                        ConnectButton.Content = "Disconnect";
                        _connected = true;
                        LoadTeamsButton.IsEnabled = true;
                    }
                    else
                    {
                        InfoText.Text = Constants.MESSAGE_ERROR_CANNOTCONNECTO365;
                    }
                }
                catch (Exception ex)
                {
                    cs.Exception(ex);
                    InfoText.Text = Constants.MESSAGE_ERROR_GENERIC + " " + ex.Message;
                }
            }
            else
            {
                try
                {
                    authHelper.SignOut();
                }
                catch (Exception ex)
                {
                    cs.Exception(ex);
                    InfoText.Text = Constants.MESSAGE_ERROR_GENERIC + " " + ex.Message;
                }
                cs.Debug("SignOut done.");

                // Disable / clean controls.
                // Combobox.
                ComboBox_Teams_Source.IsEnabled = false;
                ComboBox_Teams_Dest.IsEnabled = false;
                ComboBox_Teams_Source.ItemsSource = ComboBox_Teams_Dest.ItemsSource = null;
                // Planners.
                PlannerSourceList.IsEnabled = false;
                PlannerDestinationList.IsEnabled = false;
                PlannerSourceList.ItemsSource = PlannerDestinationList.ItemsSource = null;
                // Copy button.
                CopyPlannerButton.IsEnabled = false;

                LoadTeamsButton.IsEnabled = false;

                InfoText.Text = Constants.MESSAGE_INFO_PRESSCONNECTBUTTON;
                ConnectButton.Content = "Connect";

                _connected = false;
            }
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        public async Task LoadTeams()
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            ComboBox_Teams_Source.ItemsSource = ComboBox_Teams_Dest.ItemsSource = null;
            PlannerSourceList.ItemsSource = PlannerDestinationList.ItemsSource = null;

            cs.Debug("Get TeamApi.");
            IEnumerable<Team> teams = null;
            try
            {
                var tApi = TeamsFactory.GetStaticApi("TeamApi", this.token) as TeamApi;
                teams = await tApi.GetJoinedTeams();
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                InfoText.Text = Constants.MESSAGE_ERROR_GENERIC + " " + ex.Message;
            }
            ComboBox_Teams_Source.ItemsSource = ComboBox_Teams_Dest.ItemsSource = teams;
        }

        public async Task LoadChannels(string teamId)
        {
            if (string.IsNullOrWhiteSpace(teamId))
            {
                return;
            }

            ComboBox_Channels.ItemsSource = null;
            var cApi = TeamsFactory.GetStaticApi("ChannelApi", this.token) as ChannelApi;
            var channels = await cApi.GetChannels(teamId);
            ComboBox_Channels.ItemsSource = channels;
        }

        public async Task LoadPlanners(ListView plannerList, string groupId)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (plannerList == null
                || string.IsNullOrWhiteSpace(groupId))
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return;
            }

            plannerList.ItemsSource = null;
            IEnumerable<PlannerPlan> planners;
            try
            {
                var pApi = TeamsFactory.GetStaticApi("PlannerApi", this.token) as PlannerApi;
                planners = await pApi.GetPlanners(groupId);
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                InfoText.Text = Constants.MESSAGE_ERROR_GENERIC + " " + ex.Message;
                throw;
            }
            plannerList.ItemsSource = planners?.ToList();
        }

        private async void LoadTeamsButton_Click(object sender, RoutedEventArgs e)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            LoadTeamsButton.IsEnabled = false;
            ComboBox_Teams_Source.IsEnabled = ComboBox_Teams_Dest.IsEnabled = false;
            ProgressBar.Visibility = Visibility.Visible;
            InfoText.Text = Constants.MESSAGE_INFO_LOADING;

            try
            {
                await LoadTeams();
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                InfoText.Text = Constants.MESSAGE_ERROR_GENERIC + " " + ex.Message;
                return;
            }
            LoadTeamsButton.IsEnabled = true;
            ComboBox_Teams_Source.IsEnabled = ComboBox_Teams_Dest.IsEnabled = true;
            ProgressBar.Visibility = Visibility.Collapsed;
            InfoText.Text = Constants.MESSAGE_TEAMS_LOADED;
        }

        private async void ComboBox_Teams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbx = sender as ComboBox;
            var selectedItem = cbx?.SelectedItem;
            var selectedTeam = selectedItem as Team;

            if (selectedItem == null)
                return;

            LoadTeamsButton.IsEnabled = false;
            ProgressBar.Visibility = Visibility.Visible;
            InfoText.Text = Constants.MESSAGE_INFO_LOADING;

            try
            {

                if (cbx.Name.Equals("ComboBox_Teams_Source"))
                {
                    PlannerSourceList.IsEnabled = true;
                    await LoadPlanners(this.PlannerSourceList, selectedTeam.Id);
                }
                else if (cbx.Name.Equals("ComboBox_Teams_Dest"))
                {
                    PlannerDestinationList.IsEnabled = true;
                    await LoadPlanners(this.PlannerDestinationList, selectedTeam.Id);
                }
                else
                {
                    return;
                }
                CopyPlannerButton.IsEnabled = true;
                InfoText.Text = Constants.MESSAGE_PLANNER_LOADED;
            }
            catch(Exception ex)
            {                 
                InfoText.Text = Constants.MESSAGE_ERROR_GENERIC + " " + ex.Message;
            }
            finally
            {
                LoadTeamsButton.IsEnabled = true;
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void ComboBox_Channels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ComboBox)?.SelectedItem;
            var selectedChannel = selectedItem as Channel;
        }

        private void PlannerSourceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ListView).SelectedItem;
            var selectedPlanner = selectedItem as PlannerPlan;

            this.sourcePlanner = selectedPlanner;
        }

        private void PlannerDestinationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ListView).SelectedItem;
            var selectedPlanner = selectedItem as PlannerPlan;

            this.destinationPlanner = selectedPlanner;
        }

        private async void CopyPlanner_Click(object sender, RoutedEventArgs e)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();
            cs.Debug("Copy Planner.");

            if (this.sourcePlanner == null
                || this.destinationPlanner == null)
            {
                InfoText.Text = Constants.MESSAGE_PLANNER_SELECT;
                cs.Error("Copy Planner failed: planners are null.");
                return;
            }
            cs.Debug($"Source Planner: {this.sourcePlanner.Id} - {this.sourcePlanner.Title}");
            cs.Debug($"Destination Planner: {this.destinationPlanner.Id} - {this.destinationPlanner.Title}");
            string sPlanId = this.sourcePlanner.Id;
            string dPlanId = this.destinationPlanner.Id;

            if (string.IsNullOrWhiteSpace(sPlanId)
                || string.IsNullOrWhiteSpace(dPlanId))
            {
                InfoText.Text = Constants.MESSAGE_PLANNER_SELECT;
                return;
            }

            InfoText.Text = Constants.MESSAGE_INFO_COPY;
            LoadTeamsButton.IsEnabled = false;
            ComboBox_Teams_Source.IsEnabled = ComboBox_Teams_Dest.IsEnabled = false;
            CopyPlannerButton.IsEnabled = false;
            PlannerSourceList.IsEnabled = PlannerDestinationList.IsEnabled = false;
            ProgressBar.Visibility = Visibility.Visible;

            try
            {
                await CopyPlanner(sPlanId, dPlanId);
                InfoText.Text = Constants.MESSAGE_PLANNER_COPIED;
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                InfoText.Text = Constants.MESSAGE_ERROR_GENERIC + " " + ex.Message;
            }
            finally
            {
                LoadTeamsButton.IsEnabled = true;
                ComboBox_Teams_Source.IsEnabled = ComboBox_Teams_Dest.IsEnabled = true;
                CopyPlannerButton.IsEnabled = true;
                PlannerSourceList.IsEnabled = PlannerDestinationList.IsEnabled = true;
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private async Task CopyPlanner(string sPlanId, string dPlanId)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            var pApi = TeamsFactory.GetStaticApi("PlannerApi", this.token) as PlannerApi;

            // Get Source/Dest Planner details.
            PlannerPlanDetails sPlanDetails;
            PlannerPlanDetails dPlanDetails;
            try
            {
                cs.Debug($"Get Planner details for: {sPlanId}.");
                cs.Debug($"Get Planner details for: {dPlanId}.");

                sPlanDetails = await pApi.GetPlannerDetails(sPlanId);
                dPlanDetails = await pApi.GetPlannerDetails(dPlanId);

                // Update Dest Planner details.
                cs.Debug($"Update planner details: {dPlanId}.");
                await pApi.UpdatePlannerDetails(dPlanId, dPlanDetails.ETag, sPlanDetails.CategoryDescriptions, dPlanDetails.SharedWith);
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }

            try
            {
                // Get Source Planner Buckets.
                var sBuckets = await pApi.GetBuckets(sPlanId);
                // Get Source Planner Tasks.
                var sTasks = await pApi.GetTasks(sPlanId);

                // Clear Destination Planner.
                var dBuckets = await pApi.GetBuckets(dPlanId);
                var bApi = TeamsFactory.GetStaticApi("BucketApi", this.token) as BucketApi;
                foreach (var bucket in dBuckets)
                {
                    await bApi.DeleteBucket(bucket.Id, bucket.ETag);
                }

                await pApi.CreateBucketsAndTasks(dPlanId, sBuckets, sTasks);
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }
        }
    }
}
