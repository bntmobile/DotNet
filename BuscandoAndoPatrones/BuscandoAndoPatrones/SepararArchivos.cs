using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuscandoAndoPatrones
{
    public partial class SepararArchivos : Form
    {
        List<string> listFiles = new List<string>();    
        List<int> listIniciaRTF= new List<int>();
        public SepararArchivos()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listFiles.Clear();
            listView1.Items.Clear();

            using (FolderBrowserDialog fbd = new FolderBrowserDialog() { Description="Select your path."})
            {
                if (fbd.ShowDialog()==DialogResult.OK)
                {
                    textBox1.Text = fbd.SelectedPath;
                    foreach (string item in Directory.GetFiles(fbd.SelectedPath))
                    {
                        imageList1.Images.Add(System.Drawing.Icon.ExtractAssociatedIcon(item));
                        FileInfo fi = new FileInfo(item);
                        listFiles.Add(fi.FullName);
                        listView1.Items.Add(fi.Name,imageList1.Images.Count-1);
                    }
                    
                }
                
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("Empieza a leer archivos, espera un momento...");
            LeerFile(listFiles[listView1.FocusedItem.Index].ToString());
        }

        public void LeerFile(string Path)
        {
            listIniciaRTF.Clear();

            string rtfText = System.IO.File.ReadAllText(@Path);
            int fin = rtfText.Length;
            var mtchs = Regex.Matches(rtfText, @"\{\\rtf1\\ansi", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if (mtchs.Count==0)
            {
                MessageBox.Show("El archivo no contiene lo necesario para poder ser procesado");
            }
            else
            {
                foreach (Match item in mtchs)
                {
                    listIniciaRTF.Add(item.Index);
                }
                List<string> ArchivosList = new List<string>();
                int y = 1;
                for (int i = 0; i < listIniciaRTF.Count() - 1; i++)
                {
                    ArchivosList.Add(rtfText.Substring(listIniciaRTF[i], listIniciaRTF[y] - listIniciaRTF[i]));
                    y++;
                }
                ArchivosList.Add(rtfText.Substring(listIniciaRTF[listIniciaRTF.Count() - 1], fin - listIniciaRTF[listIniciaRTF.Count() - 1]));


                string ruta = textBox1.Text + @"\output";
                Directory.CreateDirectory(ruta);

                RichTextBox rtb = new RichTextBox();
                rtb.Clear();
                int contador = 1;
                foreach (var item in ArchivosList)
                {
                    rtb.Clear();
                    string rutFin = ruta + @"\" + contador.ToString() + ".rtf";
                    System.IO.File.WriteAllText(rutFin, item);
                    contador++;
                }
                contador--;
                MessageBox.Show("Revisa en la ruta " + ruta + " se han creado un total de " + contador + " archivos");

            }

            
            }
        }



    }






