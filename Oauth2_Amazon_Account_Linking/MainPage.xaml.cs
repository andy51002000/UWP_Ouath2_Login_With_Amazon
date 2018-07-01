using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Oauth2_Amazon_Account_Linking
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string _token;
        public MainPage()
        {
            this.InitializeComponent();
        }
        private async void Button_ShowAsync(object sender, RoutedEventArgs e)
        {
            using (HttpClient httpclient = new HttpClient())
            {
                var response = await httpclient.GetStringAsync(new Uri("https://api.amazon.com/user/profile?access_token=" + _token));
                var obj = new JsonObject();

                if (JsonObject.TryParse(response, out obj))
                {
       
                    MessageDialog dialog = new MessageDialog($"UserName:{obj.GetNamedString("name")}, UserEmail:{obj.GetNamedString("email")}", "User Information");
                    await dialog.ShowAsync();
                }

            }

        }
        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                String clientID = "YourClientId";

                string redirectUrl = "https://localhost";
                string baseUrl =
                    "https://amazon.com/ap/oa?" +
                    "client_id=" + clientID +
                    "&response_type=token" +
                    "&scope=profile" +
                    "&redirect_uri=" + redirectUrl;

                var uri = new Uri(baseUrl);
                var redirectUri = new Uri(redirectUrl);
                WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, uri, redirectUri);

                if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    //Get & Store Access Token first
                    string webAuthResultResponseData = webAuthenticationResult.ResponseData.ToString();
                    _token = webAuthResultResponseData
                                .Split('&')
                                .FirstOrDefault( s=> s.Contains("token="))
                                .Split('=')[1];


                }
                else
                {
                    MessageDialog dialog = new MessageDialog("Error message by AuthenticateAsync(): " + webAuthenticationResult.ResponseStatus.ToString());
                    await dialog.ShowAsync();
                }

            }
            catch (Exception Error)
            {
                MessageDialog dialog = new MessageDialog("Error message by exception: " + Error.Message);
                await dialog.ShowAsync();
            }
        }
    }
}
