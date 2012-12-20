// Accord.NET Sample Applications
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2012
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Multivariate;

using Accord.Statistics.Models.Fields;
using Accord.Statistics.Models.Fields.Functions;
using Accord.Statistics.Models.Fields.Learning;

using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;

namespace Gestures
{
    public partial class MainForm : Form
    {

        Database database;

        HiddenMarkovClassifier<MultivariateNormalDistribution> hmm;
        HiddenConditionalRandomField<double[]> hcrf;



        public MainForm()
        {
            InitializeComponent();

            gvSamples.AutoGenerateColumns = false;

            database = new Database();
            cbClasses.DataSource = database.Classes;
            gvSamples.DataSource = database.Samples;

            openDataDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Resources");
        }


        private void btnLearnHMM_Click(object sender, EventArgs e)
        {
            if (gvSamples.Rows.Count == 0)
            {
                MessageBox.Show("Please load or insert some data first.");
                return;
            }

            BindingList<Sequence> samples = database.Samples;
            BindingList<String> classes = database.Classes;

            double[][][] inputs = new double[samples.Count][][];
            int[] outputs = new int[samples.Count];

            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = samples[i].Input;
                outputs[i] = samples[i].Output;
            }

            int states = 5;
            int iterations = 0;
            double tolerance = 0.01;
            bool rejection = false;


            hmm = new HiddenMarkovClassifier<MultivariateNormalDistribution>(classes.Count,
                new Forward(states), new MultivariateNormalDistribution(2), classes.ToArray());


            // Create the learning algorithm for the ensemble classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution>(hmm,

                // Train each model using the selected convergence criteria
                i => new BaumWelchLearning<MultivariateNormalDistribution>(hmm.Models[i])
                {
                    Tolerance = tolerance,
                    Iterations = iterations,

                    FittingOptions = new NormalOptions()
                    {
                        Regularization = 1e-5
                    }
                }
            );

            teacher.Empirical = true;
            teacher.Rejection = rejection;


            // Run the learning algorithm
            double error = teacher.Run(inputs, outputs);


            // Classify all training instances
            foreach (var sample in database.Samples)
            {
                sample.RecognizedAs = hmm.Compute(sample.Input);
            }

            foreach (DataGridViewRow row in gvSamples.Rows)
            {
                var sample = row.DataBoundItem as Sequence;
                row.DefaultCellStyle.BackColor = (sample.RecognizedAs == sample.Output) ?
                    Color.LightGreen : Color.White;
            }

            btnLearnHCRF.Enabled = true;
        }

        private void btnLearnHCRF_Click(object sender, EventArgs e)
        {
            if (gvSamples.Rows.Count == 0)
            {
                MessageBox.Show("Please load or insert some data first.");
                return;
            }

            var samples = database.Samples;
            var classes = database.Classes;

            double[][][] inputs = new double[samples.Count][][];
            int[] outputs = new int[samples.Count];

            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = samples[i].Input;
                outputs[i] = samples[i].Output;
            }

            int iterations = 100;
            double tolerance = 0.01;


            hcrf = new HiddenConditionalRandomField<double[]>(
                new MarkovMultivariateFunction(hmm));


            // Create the learning algorithm for the ensemble classifier
            var teacher = new HiddenResilientGradientLearning<double[]>(hcrf)
            {
                Iterations = iterations,
                Tolerance = tolerance
            };


            // Run the learning algorithm
            double error = teacher.Run(inputs, outputs);


            foreach (var sample in database.Samples)
            {
                sample.RecognizedAs = hcrf.Compute(sample.Input);
            }

            foreach (DataGridViewRow row in gvSamples.Rows)
            {
                var sample = row.DataBoundItem as Sequence;
                row.DefaultCellStyle.BackColor = (sample.RecognizedAs == sample.Output) ?
                    Color.LightGreen : Color.White;
            }
        }


        private void openDataStripMenuItem_Click(object sender, EventArgs e)
        {
            openDataDialog.ShowDialog();
        }

        private void saveDataStripMenuItem_Click(object sender, EventArgs e)
        {
            saveDataDialog.ShowDialog();
        }


        private void openDataDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (var stream = openDataDialog.OpenFile())
                database.Load(stream);

            btnLearnHMM.Enabled = true;
        }

        private void saveDataDialog_FileOk(object sender, CancelEventArgs e)
        {
            using (var stream = saveDataDialog.OpenFile())
                database.Save(stream);
        }


        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            database.Clear();
            hmm = null;
            hcrf = null;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }




        private void btnYes_Click(object sender, EventArgs e)
        {
            string selectedItem = cbClasses.SelectedItem as String;
            string classLabel = String.IsNullOrEmpty(selectedItem) ?
                cbClasses.Text : selectedItem;

            if (database.Add(inputCanvas.GetSequence(), classLabel) != null)
            {
                inputCanvas.Clear();
                btnLearnHMM.Enabled = true;
            }
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            showQuestion();
        }




        private void inputCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (hmm == null)
            {
                showQuestion();
            }
            else if (hcrf == null)
            {
                showConfirm(hmm.Compute(Sequence.Preprocess(inputCanvas.GetSequence())));
            }
            else
            {
                showConfirm(hcrf.Compute(Sequence.Preprocess(inputCanvas.GetSequence())));
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            inputCanvas.Clear();
        }



        private void showQuestion()
        {
            lbWhat.Text = "What's this?";
            lbQuestion.Visible = false;
            btnNo.Visible = false;
            btnOK.Text = "OK";
        }

        private void showConfirm(int index)
        {
            if (index < 0) return;

            string label = database.Classes[index];

            lbWhat.Text = "Is this a ";
            cbClasses.Text = label;
            lbQuestion.Visible = true;
            btnOK.Text = "Yes";
            btnNo.Visible = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }



    }
}
