using System;
using System.Drawing;
using System.Windows.Forms;

// Delegates in Action: exemplos de utilização de delegates.
//
// Definição de handlers de eventos em aplicações com interface gráfica.
//
public class SimpleForm : Form
{
    private TextBox text;
    private Button button;

    public SimpleForm()
    {
        PrepareGUI();
    }

    private void PrepareGUI()
    {
        this.Name = "SimpleForm";
        this.Text = "A Simple Form for AVE";
        this.Size = new Size(320, 140);
        this.StartPosition = FormStartPosition.CenterScreen;

        text = new TextBox();
        text.Name = "Text";
        text.Text = "AVE";
        text.Size = new Size(this.ClientRectangle.Width - 32, this.ClientRectangle.Height/2 - 24);
        text.Location = new Point(16, 16);
        
        button = new Button();
        button.Name = "Button";
        button.Text = "Go";
        button.Size = new Size(this.ClientRectangle.Width - 32, this.ClientRectangle.Height/2 - 24);
        button.Location = new Point(16, this.ClientRectangle.Height/2 + 8);
        
        this.Controls.Add(text);
        this.Controls.Add(button);

        // Registo de handler no evento Click, para executar Button_OnClick sempre que o botão é pressionado 
        button.Click += new System.EventHandler(this.Button_OnClick);
        
        // Registo de mais um handler no evento Click, configurando uma acção adicional. 
        button.Click += (o,e) => { Console.WriteLine("Text: " + text.Text); };
        
        // Registo de handler para pedir confirmação quando se tenta fechar a janela.
        this.FormClosing += (o,e) =>
        {
            DialogResult res = MessageBox.Show("Really close?", "Confirm", MessageBoxButtons.YesNo);
            e.Cancel = res == DialogResult.No;
        };
    }

    private void Button_OnClick(object source, EventArgs args)
    {
        MessageBox.Show("Text: " + text.Text, "ISEL");
    }
    
    public static void Main(String[] args)
    {
        Application.Run(new SimpleForm());
    }
}
