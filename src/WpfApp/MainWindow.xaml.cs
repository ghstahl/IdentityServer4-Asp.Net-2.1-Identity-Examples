using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IdentityModel.OidcClient;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string clientID = "native.code";
        private const string Authority = "https://localhost:44317/";

        public MainWindow()
        {
            InitializeComponent();

            // Creates a redirect URI using an available port on the loopback address.
            var unusedPort = GetRandomUnusedPort();
            var browser = new SystemBrowser(unusedPort);
            RedirectUri = $"http://{IPAddress.Loopback}:{unusedPort}/";
            var authority = Authority;

            Options = new OidcClientOptions()
            {
                Authority = authority,
                ClientId = clientID,
                Scope = "openid profile",
                RedirectUri = RedirectUri,
                Browser = browser,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect

            };
            OidcClient = new OidcClient(Options);
            OidcClient.Options.Policy.Discovery.ValidateEndpoints = false;
        }

        public OidcClientOptions Options { get; set; }
        public OidcClient OidcClient { get; private set; }
        public string RedirectUri { get; set; }

        // ref http://stackoverflow.com/a/3978040
        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private async void buttonNeo4j_Click(object sender, RoutedEventArgs e)
        {

            output("redirect URI: " + RedirectUri);

            // create an HttpListener to listen for requests on that redirect URI.

            var result = await OidcClient.LoginAsync();

            

            if (result.IsError)
            {
                output($"\n\nError:\n{result.Error}");
            }
            else
            {
                output("\n\nClaims:");
                foreach (var claim in result.User.Claims)
                {
                    output($"{claim.Type}: {claim.Value}");
                }

                output("");
                output($"Access token:\n{result.AccessToken}");

                if (!string.IsNullOrWhiteSpace(result.RefreshToken))
                {
                    output($"Refresh token:\n{result.RefreshToken}");
                }
            }
        }
        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        /// <param name="output">string to be appended</param>
        public void output(string output)
        {
            textBoxOutput.Text = textBoxOutput.Text + output + Environment.NewLine;
            Console.WriteLine(output);
        }
    }
}
