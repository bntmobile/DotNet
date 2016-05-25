using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace BuscandoAndoPatrones
{
    // Clase creada para crear objectos de acuerdo a los files que se van a leer y se haran busquedas en estos objetos
    class File
    {
        #region Attributos

        private string nombre;
        private string path;
        private string tipo;
        private int coincidencias;
        private bool bandera;
        private RichTextBox content;
        private Dictionary<int, string> matriz;
        private string titulo;
        private string carpeta;

       
        public string Titulo
        {
            get { return titulo; }
            set { titulo = value; }
        }

        public Dictionary<int, string> Matriz
        {
            get { return matriz; }
            set { matriz = value; }
        }

        public RichTextBox Content
        {
            get { return content; }
            set { content = value; }
        }
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        public string Path
        {
            get { return path; }
            set { path = value; }
        }
        public string Carpeta
        {
            get { return carpeta; }
            set { carpeta = value; }
        }

        public int Coincidencias
        {
            get { return coincidencias; }
            set { coincidencias = value; }
        }
        public bool Bandera
        {
            get { return bandera; }
            set { bandera = value; }
        }
        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }
        
        
        #endregion

        #region  Constructores y Métodos
        //constructor con 3 parámetros de entrada
        public File(String _Nombre, string _Path, string _Tipo, int _Coinidencias, bool _Bandera, string _Carpeta)
        {
            if (_Carpeta=="")
            {
                _Carpeta=@"\";
            }

            Nombre = _Nombre;
            Path = _Path;
            Tipo = _Tipo;
            Coincidencias = _Coinidencias;
            Bandera = _Bandera;
            Carpeta = _Carpeta;
        }

        public void BuscaPatron(RichTextBox inputRtxt, string patron)
        {
            RichTextBox rtfContent = inputRtxt;
            var mtchs = Regex.Matches(rtfContent.Text, patron, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //var mtchsTitulo = Regex.Matches(rtfContent.Text, @"^\s*\w.+\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var mtchsTitulo = Regex.Matches(rtfContent.Text, @"(.*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            List<string> z = new List<string>();

            foreach (Match item in mtchsTitulo) // én éste se elimina los parrafos con  espacios para solo colocar como titulo el primer  parrafo que no este en blanco
            {
                if (string.IsNullOrEmpty(item.Value.Trim()))
                {}
                else if (item.Length>5)
                {z.Add(item.Value.Trim());}
                else {  }  
            }

            if (mtchs.Count>0)
            {
                this.Bandera = true;
                this.Coincidencias = mtchs.Count;
                this.Content = new RichTextBox();
                this.Content.Text=rtfContent.Text;
                this.matriz = new Dictionary<int, string>();

                if (mtchsTitulo.Count>0)
                { this.Titulo = z[0].Trim().ToString(); }
                else { this.Titulo = "";  }
                
               foreach (Match txt in mtchs)
                {matriz.Add(txt.Index,txt.Value);}
            }
            rtfContent.Clear();
            rtfContent.Dispose();
        }

        public  File()
        {
        }


        public RichTextBox ReadFile(string ruta)
        {
            RichTextBox r = new RichTextBox();
            if (this.Tipo == ".txt" ) {r.LoadFile(this.Path, RichTextBoxStreamType.PlainText);}
            else if (this.Tipo == ".rtf" || this.Tipo == "") { r.LoadFile(this.Path); }
            //else if (this.Tipo == ".rtf" || this.Tipo == "") { r.LoadFile(this.Path, RichTextBoxStreamType.RichText); }
            else if (this.Tipo == ".doc" || this.Tipo == ".docx")
            {   Word.Application wordApp = new Word.Application(); ;
                Word.Document aDoc = new Word.Document();
                object missing = Missing.Value;
                object filename = this.Path;
                object readOnly = true;
                object isVisible = false;
                wordApp.Visible = false;
                aDoc = wordApp.Documents.Open(ref filename, ref missing,
                ref readOnly, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing,
                ref missing, ref isVisible, ref missing, ref missing,
                ref missing, ref missing);
                aDoc.Activate();
                r.Text = aDoc.Content.Text;
                aDoc.Close(ref missing, ref missing, ref missing);
                wordApp.Quit(ref missing, ref missing, ref missing);
            }
            return r;
        }

        #endregion
    }
}
