using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace BuscandoAndoPatrones
{
    public partial class Render : Form
    {
        Dictionary<int, string> match = new Dictionary<int, string>();
        int contador = 0; // contador que se usa en el arreglo
        int i = 1; // contador que se usa en pantalla
        String p1, p2;
        Boolean nbool;
        
        public Render()
        {
            InitializeComponent();
            
        }

        public void R(RichTextBox a, Dictionary<int,string> b, Boolean n,String a1,String a2,string x)
        {  
            //2 3-05-2016 No se mostraban las imagenes que tenía algunos archivos rtfs.
            // variable a que es un richtexbox ya no funciona después del cambio realizado hot 23-05-2016
            // se modificó la función R para que se le pasara el path del archivo y abrir desde acá el file al objeto richtextbox


            match = b;
            p1 = a1;
            p2 = a2;
            nbool = n;
            
            rtb.Clear();
            rtb.LoadFile(x, RichTextBoxStreamType.RichText);
            //rtb.Text = a.Text;
            int cursor = rtb.SelectionStart;
            if (n==false)
            {
                foreach (var item in b)
                {
                    rtb.Select(item.Key, item.Value.Length);
                    rtb.SelectionBackColor = Color.Yellow;
                    rtb.SelectionStart = item.Key + item.Value.Length;
                    rtb.SelectionBackColor = Color.White;   
                }
            }
            else
            {
                foreach (var item in b)
                {
                    //rtb.Select(item.Key, item.Value.Length);
                    rtb.Select(item.Key, a1.Length);
                    rtb.SelectionBackColor = Color.Yellow;
                    rtb.Select((item.Key + item.Value.Length)-(a2.Length), a2.Length);
                    rtb.SelectionBackColor = Color.Yellow;
                    rtb.SelectionStart = item.Key + item.Value.Length;
                    rtb.SelectionBackColor = Color.White;
                }

            }
           
            rtb.SelectionStart = 0;
            rtb.Refresh();
        }

        private void Render_FormClosing(object sender, FormClosingEventArgs e)
        {
            rtb.Clear();
            rtb.Dispose();
        }

        private void Render_Resize(object sender, EventArgs e)
        {
            rtb.Width = this.Width - 50;
            rtb.Height = this.Height - 90;
            rtb.Top = 40;
            rtb.Left = 10;
            
        }

        private void rtb_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (nbool==false)
            {
                if (contador < match.Count)//0,1,2,3 < 4
                {
                    foreach (var item in match)
                    {
                        rtb.Select(item.Key, item.Value.Length);
                        rtb.SelectionBackColor = Color.Yellow;
                        rtb.SelectionStart = item.Key + item.Value.Length;
                        rtb.SelectionBackColor = Color.White;
                    }
                    rtb.Select(match.ElementAt(contador).Key, match.ElementAt(contador).Value.Length);
                    rtb.SelectionBackColor = Color.Tomato;
                    rtb.Refresh();
                    label1.Text = (i).ToString() + " de " + match.Count.ToString();
                    if (contador == match.Count - 1) { MessageBox.Show("Estas en la última coincidencia"); }
                    else { contador++; i++; }
                }
             
            }
            else
            {
                if (contador < match.Count)//0,1,2,3 < 4
                {
                    foreach (var item in match)
                    {
                        rtb.Select(item.Key, p1.Length);
                        rtb.SelectionBackColor = Color.Yellow;
                        rtb.Select((item.Key + item.Value.Length) - (p2.Length), p2.Length);
                        rtb.SelectionBackColor = Color.Yellow;
                        rtb.SelectionStart = item.Key + item.Value.Length;
                        rtb.SelectionBackColor = Color.White;
                    }
                    rtb.Select(match.ElementAt(contador).Key, p1.Length);
                    rtb.SelectionBackColor = Color.Tomato;
                    rtb.Select((match.ElementAt(contador).Key + match.ElementAt(contador).Value.Length) - (p2.Length), p2.Length);
                    rtb.SelectionBackColor = Color.Tomato;
                    rtb.Refresh();
                    label1.Text = (i).ToString() + " de " + match.Count.ToString();
                    if (contador == match.Count - 1) { MessageBox.Show("Estas en la última coincidencia"); }
                    else { contador++; i++; }
                }
            
            }

            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (nbool==false)
            {
                if (contador >= 0 && contador < match.Count)
                {
                    label1.Text = (i).ToString() + " de " + match.Count.ToString();
                    foreach (var item in match)
                    {
                        rtb.Select(item.Key, item.Value.Length);
                        rtb.SelectionBackColor = Color.Yellow;
                        rtb.SelectionStart = item.Key + item.Value.Length;
                        rtb.SelectionBackColor = Color.White;
                    }
                    rtb.Select(match.ElementAt(contador).Key, match.ElementAt(contador).Value.Length);
                    rtb.SelectionBackColor = Color.Tomato;
                    rtb.Refresh();
                    if (contador != 0)
                    { contador--; i--; }
                    else
                    {
                        MessageBox.Show("Estas en la primera coincidencia");
                    }
                }
                
            }
            else
            {
                if (contador >= 0 && contador < match.Count)
                {
                    label1.Text = (i).ToString() + " de " + match.Count.ToString();
                    foreach (var item in match)
                    {
                        rtb.Select(item.Key, p1.Length);
                        rtb.SelectionBackColor = Color.Yellow;
                        rtb.Select((item.Key + item.Value.Length) - (p2.Length), p2.Length);
                        rtb.SelectionBackColor = Color.Yellow;
                        rtb.SelectionStart = item.Key + item.Value.Length;
                        rtb.SelectionBackColor = Color.White;
                    }
                    rtb.Select(match.ElementAt(contador).Key, p1.Length);
                    rtb.SelectionBackColor = Color.Tomato;
                    rtb.Select((match.ElementAt(contador).Key + match.ElementAt(contador).Value.Length) - (p2.Length), p2.Length);
                    rtb.SelectionBackColor = Color.Tomato;
                    rtb.Refresh();
                    if (contador != 0)
                    { contador--; i--; }
                    else
                    {
                        MessageBox.Show("Estas en la primera coincidencia");
                    }
                }

            }
            
             

            
           
        }
        
        


    }
}
