using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Akka.Actor;

namespace GithubActors.Actors
{
    /// <summary>
    /// Actor responsible for printing the results and progress from a <see cref="GithubCoordinatorActor"/>
    /// onto a <see cref="RepoResultsForm"/> (runs on the UI thread)
    /// </summary>
    public class RepoResultsActor : ReceiveActor
    {
        private readonly DataGridView _grid;
        private readonly ToolStripStatusLabel _statusLabel;
        private readonly ToolStripProgressBar _progressBar;
        private readonly bool _hasSetProgress = false;

        public RepoResultsActor(DataGridView grid, ToolStripStatusLabel statusLabel, ToolStripProgressBar progressBar)
        {
            _grid = grid;
            _statusLabel = statusLabel;
            _progressBar = progressBar;
            InitialReceives();
        }

        private void InitialReceives()
        {
            //progress update
            Receive<GithubProgressStats>(stats =>
            {
                //time to start animating the progress bar
                if (!_hasSetProgress && stats.ExpectedUsers > 0)
                {
                    _progressBar.Minimum = 0;
                    _progressBar.Step = 1;
                    _progressBar.Maximum = stats.ExpectedUsers;
                    _progressBar.Value = stats.UsersThusFar;
                    _progressBar.Visible = true;
                    _statusLabel.Visible = true;
                }

                _statusLabel.Text =
                    $"{stats.UsersThusFar} out of {stats.ExpectedUsers} users ({stats.QueryFailures} failures) [{stats.Elapsed} elapsed]";
                _progressBar.Value = stats.UsersThusFar + stats.QueryFailures;
            });

            //user update
            Receive<IEnumerable<SimilarRepo>>(repos =>
            {
                foreach (var similarRepo in repos)
                {
                    var repo = similarRepo.Repo;
                    var row = new DataGridViewRow();
                    row.CreateCells(_grid);
                    row.Cells[0].Value = repo.Owner.Login;
                    row.Cells[1].Value = repo.Name;
                    row.Cells[2].Value = repo.HtmlUrl;
                    row.Cells[3].Value = similarRepo.SharedStarrers;
                    row.Cells[4].Value = repo.OpenIssuesCount;
                    row.Cells[5].Value = repo.StargazersCount;
                    row.Cells[6].Value = repo.ForksCount;
                    _grid.Rows.Add(row);
                }
            });

            //critical failure, like not being able to connect to Github
            Receive<GithubCoordinatorActor.JobFailed>(failed =>
            {
                _progressBar.Visible = true;
                _progressBar.ForeColor = Color.Red;
                _progressBar.Maximum = 1;
                _progressBar.Value = 1;
                _statusLabel.Visible = true;
                _statusLabel.Text = $"Failed to gather data for Github repository {failed.Repo.Owner} / {failed.Repo.Repo}";
            });
        }
    }
}
