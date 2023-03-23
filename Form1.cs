using RestSharp;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleHTTPClient
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
        }

        private string chosenFileName;
        private string URL = "http://127.0.0.1:8000/classify";

        async private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(chosenFileName))
                {
                    if (File.Exists(chosenFileName))
                    {
                        await Task.Run(() => UploadFileWithRestSharpAsync());

                    }
                    else
                    {
                        MessageBox.Show("File is deleted or moved");
                    }
                }
                else
                {
                    MessageBox.Show("Choose file");
                }
            }
            catch (IOException)
            {
                MessageBox.Show("File is opened and couldn't be proceeded");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ToDefault();
            }

        }

        private void ToDefault()
        {
            label1.Text = String.Empty;
            chosenFileName = String.Empty;
            buttonSend.Enabled = false;
        }

        private async Task UploadFileWithRestSharpAsync()
        {
            var client = new RestClient("http://localhost:8000");

            var request = new RestRequest("/classify")
                .AddFile("file", chosenFileName, "multipart/form-data");


            var response = await client.PostAsync(request);

            var json = JsonDocument.Parse(response.Content);

            MessageBox.Show(json.RootElement.GetProperty("txt").GetString().Substring(0, 50));
            MessageBox.Show(json.RootElement.GetProperty("file-name").GetString().Substring(0, 50));
        }

        private void buttonPick_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Microsoft Word (*.docx)|*.docx";
            openFileDialog.Title = "Choose Files";
            openFileDialog.ShowDialog();

            if (!String.IsNullOrEmpty(openFileDialog.FileName))
            {
                label1.Text = openFileDialog.FileName;
                chosenFileName = openFileDialog.FileName;
                buttonSend.Enabled = true;
            }

        }
    }
}
