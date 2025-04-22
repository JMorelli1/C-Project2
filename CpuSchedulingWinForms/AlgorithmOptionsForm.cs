using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CpuSchedulingWinForms
{
    public partial class AlgorithmOptionsForm : Form
    {
        private List<TextBox> burstTimeTextBoxes = new List<TextBox>();

        private List<TextBox> arrivalTimeTextBoxes = new List<TextBox>();
        private List<TextBox> priorityTextBoxes = new List<TextBox>();
        private TextBox quantumTimeTextBox = null;
        int numOfProcesses = 0;

        public AlgorithmOptionsForm(object type, int np)
        {
            this.numOfProcesses = np;
            this.Text = "Dynamic Form";
            this.Width = 400;
            this.Height = 800; 
            this.Tag = type;

            CreateDynamicForm();
        }

        private void CreateDynamicForm()
        {
            String selectedType = (String) this.Tag;
            int topMargin = 20;

            for (int i = 0; i < numOfProcesses; i++){
                CreateFormOption("Burst Time P" + i, "burst", topMargin);

                if(selectedType == "PRIORITY"){
                    topMargin += 30;
                    CreateFormOption("Priority of P" + i, "priority", topMargin);
                }

                if(selectedType == "RR"){
                    topMargin += 30;
                    CreateFormOption("Arrival Time P" + i, "arrival", topMargin);
                }

                topMargin += 30;
            }

            if(selectedType == "RR"){
                    CreateFormOption("Quantum Time", "quantum", topMargin);
                    topMargin += 30;
                }

            // Submit Button
            Button submitButton = new Button();
            submitButton.Text = "Submit";
            submitButton.Top = topMargin;
            submitButton.Left = 120;
            submitButton.Click += SubmitButton_Click;
            this.Controls.Add(submitButton);
        }

        private void CreateFormOption(String labelText, String tag, int topMargin) {
                // Label
                Label label = new Label();
                label.Text = labelText;
                label.Top = topMargin;
                label.Left = 10;
                label.Width = 100;
                this.Controls.Add(label);

                // TextBox
                TextBox textBox = new TextBox();
                textBox.Top = topMargin;
                textBox.Left = 120;
                textBox.Width = 200;
                textBox.Tag = tag;
                this.Controls.Add(textBox);

                switch(tag){
                    case "burst":
                        burstTimeTextBoxes.Add(textBox);
                        break;
                    case "arrival":
                        arrivalTimeTextBoxes.Add(textBox);
                        break;
                    case "priority":
                        priorityTextBoxes.Add(textBox);
                        break;
                    case "quantum":
                        quantumTimeTextBox = textBox;
                        break;
                }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            this.Close();

            String selectedType = (String) this.Tag;
            List<ProcessControlBlock> pcbs = new List<ProcessControlBlock>();
            Int32 quantumTime = quantumTimeTextBox != null ? Convert.ToInt32(quantumTimeTextBox.Text) : -1;

            // Read values from dynamically created textboxes
            for(int index = 0; index < burstTimeTextBoxes.Count; index++){
                ProcessControlBlock pcb = new ProcessControlBlock();

                pcb.BurstTime = (index < burstTimeTextBoxes.Count) ? Convert.ToInt32(burstTimeTextBoxes.ElementAt(index).Text) : -1;
                pcb.ArrivalTime = (index < arrivalTimeTextBoxes.Count) ? Convert.ToInt32(arrivalTimeTextBoxes.ElementAt(index).Text) : -1;
                pcb.Priority = (index < priorityTextBoxes.Count) ? Convert.ToInt32(priorityTextBoxes.ElementAt(index).Text) : -1;

                pcbs.Add(pcb);
            }

            // Run selected algorithm
            switch(selectedType){
                case "FCFS":
                    Algorithms.fcfsAlgorithm(pcbs);
                    break;
                case "SJF":
                    Algorithms.sjfAlgorithm(pcbs);
                    break;
                case "PRIORITY":
                    Algorithms.priorityAlgorithm(pcbs);
                    break;
                case "RR":
                    Algorithms.roundRobinAlgorithm(pcbs, quantumTime);
                    break;
            }

        }
    }
}
