using System.Windows.Forms;
using System.IO;
namespace ConversorDrawind
{
    public partial class Form_1_Senha : Form
    {
        private string senhaCP = string.Empty;

        public Form_1_Senha()
        {
            InitializeComponent();
            //Fingerprint security = new Fingerprint();
            //senhaCP = security.Value();
        }

        public bool ok = false;
        private string local = Path.Combine(Application.UserAppDataPath, "access.lic");
        private string localLegacy = Path.Combine(Application.UserAppDataPath, "Acess.dll");
        public bool CheckSerial()
        {
            if (Directory.Exists(@"\\192.168.7.10\xdraw$\Aplicativos\ConversorDrawind"))
                return true;
             return false;
        }

        private void ativar_Click(object sender, System.EventArgs e)
        {
            if (tBNumberSerie.Text == senhaCP)
            {
                string tc = "";
                try
                {
                    string readPath = File.Exists(local) ? local : localLegacy;
                    if (File.Exists(readPath))
                    {
                        using (StreamReader sr = new StreamReader(readPath))
                        {
                            sr.ReadLine();
                            tc = sr.ReadLine();
                        }
                    }
                }
                catch (System.Exception)
                {

                }

                using (StreamWriter sw = new StreamWriter(local))
                {
                    sw.WriteLine(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(tBNumberSerie.Text)));
                    sw.WriteLine(tc);
                }

                if (CheckSerial())
                {
                    ok = true;
                    this.Close();
                    MessageBox.Show("Ativação realizada com sucesso!",
                  "Atenção",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Information,
                  MessageBoxDefaultButton.Button1);
                }
                else
                {
                    MessageBox.Show("Senha inválida!",
              "Atenção",
              MessageBoxButtons.OK,
              MessageBoxIcon.Exclamation,
              MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                MessageBox.Show("Senha inválida!",
                              "Atenção",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Exclamation,
                              MessageBoxDefaultButton.Button1);
            }
        }

        private void Senha_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!ok)
                Application.Exit();
        }

        private void cancelar_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
