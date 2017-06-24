using System.Drawing;
using Akka.Actor;
using Octokit;
using Label = System.Windows.Forms.Label;

namespace GithubActors.Actors
{
    public class GithubAuthenticationActor : ReceiveActor
    {
        public class Authenticate
        {
            public Authenticate(string oAuthToken)
            {
                OAuthToken = oAuthToken;
            }

            public string OAuthToken { get; }
        }

        public class AuthenticationFailed { }

        public class AuthenticationCancelled { }

        public class AuthenticationSuccess { }

        private readonly Label _statusLabel;
        private readonly GithubAuth _form;

        public GithubAuthenticationActor(Label statusLabel, GithubAuth form)
        {
            _statusLabel = statusLabel;
            _form = form;
            Unauthenticated();
        }

        private void Unauthenticated()
        {
            Receive<Authenticate>(auth =>
            {
                //need a client to test our credentials with
                var client = GithubClientFactory.GetUnauthenticatedClient();
                GithubClientFactory.OAuthToken = auth.OAuthToken;
                client.Credentials = new Credentials(auth.OAuthToken);
                BecomeAuthenticating();
                client.User.Current().ContinueWith<object>(tr =>
                {
                    if (tr.IsFaulted)
                        return new AuthenticationFailed();
                    if (tr.IsCanceled)
                        return new AuthenticationCancelled();
                    return new AuthenticationSuccess();
                }).PipeTo(Self);
            });
        }

        private void BecomeAuthenticating()
        {
            _statusLabel.Visible = true;
            SetStatusLabelText("Authenticating...", Color.Yellow);

            Become(Authenticating);
        }

        private void BecomeUnauthenticated(string reason)
        {
            SetStatusLabelText("Authentication failed. Please try again.", Color.Red);
            Become(Unauthenticated);
        }

        private void SetStatusLabelText(string text, Color color)
        {
            _statusLabel.ForeColor = color;
            _statusLabel.Text = text;
        }

        private void Authenticating()
        {
            Receive<AuthenticationFailed>(failed => BecomeUnauthenticated("Authentication failed."));
            Receive<AuthenticationCancelled>(cancelled => BecomeUnauthenticated("Authentication timed out."));
            Receive<AuthenticationSuccess>(success =>
            {
                var launcherForm = new LauncherForm();
                launcherForm.Show();
                _form.Hide();
            });
        }
    }
}
