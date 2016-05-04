using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

// Delegates in Action: exemplos de utilização de delegates.
//
//  * Processamento de resultados independente de algoritmo de pesquisa.
//  * Comportamento a executar em fio de execução auxiliar.
//  * Envio de mensagem para execução de código.
//
public class FileSearch : Form
{
    private TextBox text;
    private Button button;
    private TextBox res;

    public FileSearch()
    {
        PrepareGUI();
    }

    private void PrepareGUI()
    {
        this.Name = "FileSearch";
        this.Text = "File Search";
        this.Size = new Size(320, 280);
        this.StartPosition = FormStartPosition.CenterScreen;
        Form.CheckForIllegalCrossThreadCalls = true;
        
        text = new TextBox();
        text.Name = "Pattern";
        text.Text = "*.txt";
        text.Size = new Size(this.ClientRectangle.Width - 32, this.ClientRectangle.Height/4 - 24);
        text.Location = new Point(16, 16);
        
        button = new Button();
        button.Name = "Button";
        button.Text = "Go";
        button.Size = new Size(this.ClientRectangle.Width - 32, this.ClientRectangle.Height/4 - 24);
        button.Location = new Point(16, this.ClientRectangle.Height/4 + 8);

        res = new TextBox();
        res.Name = "Results";
        res.Text = "";
        res.Multiline = true;
        res.ScrollBars = ScrollBars.Vertical;
        res.Size = new Size(this.ClientRectangle.Width - 32, this.ClientRectangle.Height/2 - 24);
        res.Location = new Point(16, this.ClientRectangle.Height/2 + 8);
        
        this.Controls.Add(text);
        this.Controls.Add(button);
        this.Controls.Add(res);
        
        button.Click += new System.EventHandler(this.Button_OnClick);
        
        this.FormClosing += (o,e) =>
        {
            DialogResult res = MessageBox.Show("Really close?", "Confirm", MessageBoxButtons.YesNo);
            e.Cancel = res == DialogResult.No;
        };
    }

    private delegate void UseName(string name);
    
    // São usadas três peças que requerem instâncias de delegates para
    // indicar comportamentos específicos.
    //
    // A utilização de expressões lambda permite que toda a implementação
    // fique concentrada em Button_OnClick, apesar de ser constituída por
    // quatro métodos distintos.
    //
    private void Button_OnClick(object source, EventArgs args)
    {
        Control control = (Control)source;
        String pattern = text.Text;
        Thread searchThread = new Thread(() =>      // Delegate com código a executar em thread auxiliar.
        {
            //FindFiles("C:\\", pattern, name =>      // Delegate com reacção a cada ficheiro encontrado
            //{
            //    control.BeginInvoke((Action)(() =>  // Delegate com código a executar na thread de controlo da GUI
            //    {
            //        res.Text = res.Text + name + Environment.NewLine;
            //    }));
            //});

            FindFiles("c:\\", pattern, name => {
                control.Invoke((Action)(() =>{
                    if (name.Length < 15)
                        res.AppendText(name + Environment.NewLine);
                    res.Update();
                }));
            });


        });
        
        searchThread.IsBackground = true; //stop thread when process is ended
        searchThread.Start();
    }
    
    // Algoritmo de pesquisa recursiva de ficheiros, invoca delegate useName para cada
    // ficheiro encontrado, criando assim independência entre a pesquisa e o processamento
    // dos resultados, sem ter de armazenar os resultados numa colecção intermédia e adiar
    // o seu processamento para o final da pesquisa.
    //
    private static void FindFiles(string folder, string pattern, UseName useName)
    {
        Console.WriteLine("FOLDER: " + folder);
        try
        {
            foreach (string filename in Directory.EnumerateFiles(folder, pattern))
            {
                Console.WriteLine("File: " + filename);
                useName(filename);
            }
            foreach (string subfolder in Directory.EnumerateDirectories(folder))
            {
                FindFiles(subfolder, pattern, useName);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    public static void Main(String[] args)
    {
        Application.Run(new FileSearch());
    }
}
