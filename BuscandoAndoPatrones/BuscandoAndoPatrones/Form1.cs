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
using System.Threading;

namespace BuscandoAndoPatrones
{           
            

    public partial class Form1 : Form
    {
        string pathSource = "";   //Path donde realizará la busqueda
            string[] extensiones = new string[] { "*.txt", "*.", "*.rtf", "*.doc*" }; //tipo de extensiones a buscar
            List<string> filePaths = new List<string>();
            List<string> lToRemove = new List<string>();
            List<File> lContenedor = new List<File>();// Contenedor de objetos tipo File
            BackgroundWorker m_oWorker;
            Boolean n=false;
        public Form1()
        {
            InitializeComponent();
            m_oWorker = new BackgroundWorker();
            m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
            m_oWorker.ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged);
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;
        }

        private void bntBuscar_Click(object sender, EventArgs e)
        {
            

            pathSource = ObtienePathInicial();
            string secuencia = "";
            filePaths.Clear();
            lContenedor.Clear();
            lToRemove.Clear();
            try
            {   grid.DataSource = null;
                secuencia = GetPattern();
                if (!String.IsNullOrEmpty(secuencia)){ DoWork(secuencia);}
                else{MessageBox.Show("Vuelve a intentarlo!!");}
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private string GetPattern()
        {   List<string> palabras = new List<string>();
            string pattern="";
            if ((String.IsNullOrEmpty(textBox1.Text) && String.IsNullOrEmpty(textBox2.Text)))
            {MessageBox.Show("Error!! Captura las 2 palabras, \n Son necesarias para ejecutar el programa ");}
            else if (!String.IsNullOrEmpty(textBox1.Text))
            {   palabras.Add(textBox1.Text);
                palabras.Add(textBox2.Text);
                palabras.Remove("");
                palabras=palabras.Select(x=> x.Replace(x,@"\b"+x+@"\b")).ToList();
                if (rb_or.Checked) // OR
                {pattern = string.Join("|", palabras.ToArray());}
                else if (rb_and.Checked) // AND
                {
                    try
                    {
                        n = true;
                        int xx;
                        bool pp;
                            pp = int.TryParse(textBox3.Text, out xx);
                            if (pp)
                            {pattern = string.Join( @"\s+([a-z|A-Z|0-9|ñ,Ñ]+\s+){" + xx + "}", palabras.ToArray());}
                            else if (textBox3.Text=="")
                            {pattern = string.Join(".*?", palabras.ToArray());}
                            else { MessageBox.Show("Por favor ingresa un número válido o dejalo en blanco"); }  
                    }
                    catch (Exception)
                    {throw;}  
                }
            }
            else{}
            return pattern;
        }
        private void Form1_Load(object sender, EventArgs e)
        {  rb_or.Select();
            bntBuscar.Refresh();
            bntBuscar.Focus();
            bntBuscar.Select();}

        private void rb_and_CheckedChanged(object sender, EventArgs e)
        {label3.Visible = true;
         textBox3.Visible = true;
         label5.Visible = true;}

        private void rb_or_CheckedChanged(object sender, EventArgs e)
        {   label3.Visible = false;
            textBox3.Visible = false;
            label5.Visible = false;
        }

        public void DoWork( string token)
        {
            Thread.Sleep(100);
            m_oWorker.ReportProgress(0);

            string pattern = token;   //patron de busqueda
            DateTime inicio = DateTime.Now;
            foreach (var item in extensiones) { filePaths.AddRange(Directory.GetFiles(pathSource, item.ToString(), SearchOption.AllDirectories).ToList<string>()); }
            
            foreach (var item in filePaths)
            {
                string variable = item.ToString();
                if (Path.GetExtension(item) == ".txt" || Path.GetExtension(item) == ".doc" ||
                    Path.GetExtension(item) == ".docx" || Path.GetExtension(item) == "" || Path.GetExtension(item) == ".rtf")
                { }
                else { lToRemove.Add(item); }
            }
            // Remueve de la lista original todos aquellos files que no sean
            filePaths.RemoveAll(x => lToRemove.Contains(x));
            //creación de objeos File en base a la lista de files encontrados en la carpeta
            int contador=0;

            foreach (var item in filePaths)
            {
                contador++;
                File oFile = new File(Path.GetFileName(item),  item , Path.GetExtension(item), 0, false, Path.GetDirectoryName(item).Replace(pathSource,"") );
                lContenedor.Add(oFile);
                RichTextBox x = oFile.ReadFile(oFile.Path);
                oFile.BuscaPatron(x, pattern);
                Thread.Sleep(10);
                m_oWorker.ReportProgress((contador * 100) / filePaths.Count);
                
            }
            
            int h = lContenedor.Where(n => n.Bandera.Equals(true)).Count();
            

                lContenedor.Sort(delegate(File x, File y) // Ordena el arreglo por el campo coincidencias
                {
                    return y.Coincidencias.CompareTo(x.Coincidencias);
                });

            grid.DataSource = lContenedor.Where(x => x.Bandera.Equals(true)).ToList<File>();
            grid.Columns["Tipo"].Visible = false;
            grid.Columns["Bandera"].Visible = false;
            grid.Columns["Content"].Visible = false;
            grid.Columns["Matriz"].Visible = false;
            grid.Columns["Nombre"].Visible = false;
            grid.Columns["Path"].Visible = false;
            grid.AutoResizeColumns();
            DateTime fin = DateTime.Now;
            var duracion = fin - inicio;
            if (h>0)
            {
                MessageBox.Show("Busqueda Finalizada!!\nTotal de archivos revisados: " + filePaths.Count +"\n Duración: "+duracion.ToString());
            }
            else
            {
                MessageBox.Show("Busqueda Finalizada!! \nNo se encontraron coincidencias\nTotal de archivos revisados: " + filePaths.Count );
            }
            
            
        }

        private void grid_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
                       
            //if(e.RowIndex>=0)
            //{
            //    string nombre = string.Empty;
            //    RichTextBox rtb = new RichTextBox();
            //    rtb=(RichTextBox)grid.Rows[e.RowIndex].Cells["Content"].Value;
            //    nombre = grid.Rows[e.RowIndex].Cells["Nombre"].Value.ToString();
            //    File obj = new File();
            //    obj=  lContenedor.Find(o => o.Nombre.Equals(nombre));
            //    Dictionary<int, string> d = new Dictionary<int, string>();
            //    d=obj.Matriz;
            //    Render re = new Render();
            //    re.R(rtb,d);
            //    re.Show();
            //}
        }

        public string ObtienePathInicial()
        {
            try
            {
                string txtDestino = System.Windows.Forms.Application.StartupPath.ToString();
                DirectoryInfo setting = new DirectoryInfo(txtDestino);
                FileInfo[] settingfile = setting.GetFiles("*.ini");
                StreamReader objReader = new StreamReader(setting + "//" + settingfile[0].Name, System.Text.Encoding.UTF8);
                string linea = "";
                linea = objReader.ReadLine();
                while (linea != null) {return @linea;}
                objReader.Close();
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }


        void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
            ProgressBar1.Value = e.ProgressPercentage;
            //lblStatus.Text = "Procesando......" + ProgressBar1.Value.ToString() + "%";
            lblStatus.Text = e.ProgressPercentage.ToString() + "%";
            
        }

        void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(10);
                m_oWorker.ReportProgress(i);
                if (m_oWorker.CancellationPending)
                {
                    e.Cancel = true;
                    m_oWorker.ReportProgress(0);
                    return;
                }
            }

            m_oWorker.ReportProgress(100);
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string nombre = string.Empty;
                string ruta = string.Empty;
                RichTextBox rtb = new RichTextBox();
                rtb = (RichTextBox)grid.Rows[e.RowIndex].Cells["Content"].Value;
                nombre = grid.Rows[e.RowIndex].Cells["Nombre"].Value.ToString();
                ruta = grid.Rows[e.RowIndex].Cells["Path"].Value.ToString();
                File obj = new File();
                obj = lContenedor.Find(o => o.Nombre.Equals(nombre));
                Dictionary<int, string> d = new Dictionary<int, string>();
                d = obj.Matriz;
                Render re = new Render();
                re.R(rtb, d,n,textBox1.Text,textBox2.Text,ruta);
                re.Show();
            }
        }

       

    }
}