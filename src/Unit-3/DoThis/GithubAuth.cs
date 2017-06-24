﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using Akka.Actor;
using GithubActors.Actors;

namespace GithubActors
{
    public partial class GithubAuth : Form
    {
        private IActorRef _authActor;

        public GithubAuth()
        {
            InitializeComponent();
        }

        private void GithubAuth_Load(object sender, EventArgs e)
        {
            linkGhLabel.Links.Add(new LinkLabel.Link() { LinkData = "https://help.github.com/articles/creating-an-access-token-for-command-line-use/" });
            _authActor =
                Program.GithubActors.ActorOf(Props.Create(() => new GithubAuthenticationActor(lblAuthStatus, this)), ActorPaths.GithubAuthenticatorActor.Name);
        }

        private void linkGhLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link.LinkData is string link)
            {
                Process.Start(link);
            }
        }

        private void btnAuthenticate_Click(object sender, EventArgs e)
        {
            _authActor.Tell(new GithubAuthenticationActor.Authenticate(tbOAuth.Text));
        }
    }
}
