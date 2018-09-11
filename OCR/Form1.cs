using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using System.Collections;
using System.IO;
using System.Threading;

namespace OCR
{
    public partial class Form1 : Form
    {
        public Form1()
        {            
            InitializeComponent();
            this.Text = "TCKN Parser";
            progressBar1.Visible = false;
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            string PDF_FilesDirPath;
            // dlg.Filter = "PDF Files(*.PDF)|*.PDF|All Fİles(*.*)|*.* ";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Lütfen işlem tamamlanıncaya kadar uygulamayı kapatmayınız !!!");
                progressBar1.Visible = true;
                button2.Enabled = false;
                progressBar1.MarqueeAnimationSpeed = 20;
                PDF_FilesDirPath = dlg.SelectedPath.ToString();
                DirectoryInfo dir = new DirectoryInfo(PDF_FilesDirPath);
                try
                {                   
                    select_Thread_Folder(PDF_FilesDirPath, dir.Parent.FullName);
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Error : " + ex.Message);
                }
            }

            

        }

        static void select_Thread_Folder(string PDF_FilesDirPath, string ParentDirPath)
        {
            string saveDirPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TCKN'li_PDF'ler";
            string saveImageDirPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PdfImages";
            Directory.CreateDirectory(saveDirPath);
            Directory.CreateDirectory(saveImageDirPath);
            DirectoryInfo inf = new DirectoryInfo(saveImageDirPath);
            inf.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            DirectoryInfo dir = new DirectoryInfo(PDF_FilesDirPath);
            FileInfo[] pdf_Files = dir.GetFiles();
            int filesCount = (pdf_Files.Length );
            if(filesCount < 4)
            {
                foreach (FileInfo file in pdf_Files)
                {
                    if (file.Extension == ".PDF" || file.Extension == ".pdf")
                    { Rename_Pdf(file.FullName, saveDirPath, file.Name, saveImageDirPath); }

                }
                Directory.Delete(saveImageDirPath, true); MessageBox.Show("İşlem Başarılı..."); Application.Restart();
            }
            else
            {
                moveFiles(PDF_FilesDirPath, filesCount);
            }
            
        }

        static void startThreading(string saveDirPath1, string saveDirPath2, string saveDirPath3, string saveDirPath4,/* string saveDirPath5, string saveDirPath6,*//* string saveDirPath7, string saveDirPath8,*/ string mainFolder) // mainFolder = pdf files dir path
        {
            try
            {
                
                Thread th1 = new Thread(() => threadingOperations(saveDirPath1,mainFolder));
                th1.Start();
                Thread th2 = new Thread(() => threadingOperations(saveDirPath2,mainFolder));
                th2.Start();
                Thread th3 = new Thread(() => threadingOperations(saveDirPath3,mainFolder));
                th3.Start();
                Thread th4 = new Thread(() => threadingOperations(saveDirPath4,mainFolder));
                th4.Start();
                //Thread th5 = new Thread(() => threadingOperations(saveDirPath5, mainFolder));
                //th5.Start();
                //Thread th6 = new Thread(() => threadingOperations(saveDirPath6, mainFolder));
                //th6.Start();
                //Thread th7 = new Thread(() => threadingOperations(saveDirPath7, mainFolder));
                //th7.Start();
                //Thread th8 = new Thread(() => threadingOperations(saveDirPath8, mainFolder));
                //th8.Start();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        static void moveFiles(string path , int fileCount) // path = Pdf files dir path
        {
            string moveDirPath1 = path + "\\1";
            string moveDirPath2 = path + "\\2";
            string moveDirPath3 = path + "\\3";
            string moveDirPath4 = path + "\\4";
            //string moveDirPath5 = path + "/5";
            //string moveDirPath6 = path + "/6";
            //string moveDirPath7 = path + "/7";
            //string moveDirPath8 = path + "/8";
            int quarterFileCount = fileCount / 4;
            Directory.CreateDirectory(moveDirPath1);
            Directory.CreateDirectory(moveDirPath2);
            Directory.CreateDirectory(moveDirPath3);
            Directory.CreateDirectory(moveDirPath4);
            //Directory.CreateDirectory(moveDirPath5);
            //Directory.CreateDirectory(moveDirPath6);
            //Directory.CreateDirectory(moveDirPath7);
            //Directory.CreateDirectory(moveDirPath8);

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] pdf_Files = dir.GetFiles();
            
            for (int i = 0 ; i < pdf_Files.Length; i++)
            {
                if ( i < quarterFileCount) { System.IO.File.Move( pdf_Files[i].FullName, moveDirPath1 + "\\" + pdf_Files[i].Name ); }
                else if (i < (2 * quarterFileCount) ) { System.IO.File.Move(pdf_Files[i].FullName, moveDirPath2 + "\\" + pdf_Files[i].Name); }
                else if (i < (3 * quarterFileCount)) { System.IO.File.Move(pdf_Files[i].FullName, moveDirPath3 + "\\" + pdf_Files[i].Name); }
                //else if (i < (4 * quarterFileCount)) { System.IO.File.Move(pdf_Files[i].FullName, moveDirPath4 + "/" + pdf_Files[i].Name); }
                //else if (i < (5 * quarterFileCount)) { System.IO.File.Move(pdf_Files[i].FullName, moveDirPath5 + "/" + pdf_Files[i].Name); }
                else  { System.IO.File.Move(pdf_Files[i].FullName, moveDirPath4 + "\\" + pdf_Files[i].Name); }
            }

            startThreading(moveDirPath1, moveDirPath2, moveDirPath3, moveDirPath4 ,/* moveDirPath5, moveDirPath6,*//* moveDirPath7, moveDirPath8,*/ path);

        }

        static void re_MoveFiles(string path , string mainFolder)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] pdf_Files = dir.GetFiles();

            foreach(FileInfo file in pdf_Files)
            {
                file.MoveTo(mainFolder + "\\" + file.Name);
            }
            dir.Delete();
        }

        public static int count = 0;
        static void threadingOperations(string dirPath,string mainFolder) //mainFolder = pdf files dir path
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(mainFolder);

               // string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string saveDirPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TCKN'li_PDF'ler";
                string saveImageDirPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PdfImages";
                DirectoryInfo dir = new DirectoryInfo(dirPath);
                FileInfo[] pdf_Files = dir.GetFiles();
            
                foreach (FileInfo file in pdf_Files)
                {
                    if(file.Extension == ".PDF" || file.Extension == ".pdf" )
                    { Rename_Pdf(file.FullName, saveDirPath, file.Name, saveImageDirPath); }
                                       
                }
                count++;
                re_MoveFiles(dirPath, mainFolder);
                if (count == 4  ) { Directory.Delete(saveImageDirPath, true); MessageBox.Show("İşlem Başarılı..."); Application.Restart(); }
            }
            catch (Exception ex)
            {
                 
                re_MoveFiles(dirPath, mainFolder);
                if (count == 3) { MessageBox.Show("Error: " + ex); Application.Restart(); } 
                count++;
                
            }

        }


        static void Rename_Pdf(string path, string saveDirPath, string name, string saveImageDirPath)
        {
            var pdfFile = path;
            var pdfToImg = new NReco.PdfRenderer.PdfToImageConverter();
            pdfToImg.ScaleTo = 4000; // fit 200x200 box
            pdfToImg.GenerateImage(pdfFile, 1,
              NReco.PdfRenderer.ImageFormat.Jpeg, saveImageDirPath + "\\" + name + ".jpg");
            using (var img = new Bitmap(saveImageDirPath + "\\" + name + ".jpg"))
            {
                using ( var ocr = new TesseractEngine("./tessdata", "tur", EngineMode.Default))
                {

                    using (var page = ocr.Process(img))
                    { 
                    string text = page.GetText();
                    char[] txtArray = text.ToCharArray();
                    int[] checkingTCKN = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 1 };
                    string TCKN = "";

                    for (int i = 1; i < txtArray.Length; i++)
                    {
                        if (
                            ( txtArray[(i - 1)] != '0' && txtArray[(i - 1)] != '1' && txtArray[(i - 1)] != '2' && txtArray[(i - 1)] != '3' && txtArray[(i - 1)] != '4' && txtArray[(i - 1)] != '5' && txtArray[(i - 1)] != '6' && txtArray[(i - 1)] != '7' && txtArray[(i - 1)] != '8' && txtArray[(i - 1)] != '9')

                            && (txtArray[i] == '1' || txtArray[i] == '2' || txtArray[i] == '3' || txtArray[i] == '4' || txtArray[i] == '5' || txtArray[i] == '6' || txtArray[i] == '7' || txtArray[i] == '8' || txtArray[i] == '9')

                            && (txtArray[(i + 1)] == '0' || txtArray[(i + 1)] == '1' || txtArray[(i + 1)] == '2' || txtArray[(i + 1)] == '3' || txtArray[(i + 1)] == '4' || txtArray[(i + 1)] == '5' || txtArray[(i + 1)] == '6' || txtArray[(i + 1)] == '7' || txtArray[(i + 1)] == '8' || txtArray[(i + 1)] == '9')

                            && (txtArray[(i + 2)] == '0' || txtArray[(i + 2)] == '1' || txtArray[(i + 2)] == '2' || txtArray[(i + 2)] == '3' || txtArray[(i + 2)] == '4' || txtArray[(i + 2)] == '5' || txtArray[(i + 2)] == '6' || txtArray[(i + 2)] == '7' || txtArray[(i + 2)] == '8' || txtArray[(i + 2)] == '9')

                            && (txtArray[(i + 3)] == '0' || txtArray[(i + 3)] == '1' || txtArray[(i + 3)] == '2' || txtArray[(i + 3)] == '3' || txtArray[(i + 3)] == '4' || txtArray[(i + 3)] == '5' || txtArray[(i + 3)] == '6' || txtArray[(i + 3)] == '7' || txtArray[(i + 3)] == '8' || txtArray[(i + 3)] == '9')

                            && (txtArray[(i + 4)] == '0' || txtArray[(i + 4)] == '1' || txtArray[(i + 4)] == '2' || txtArray[(i + 4)] == '3' || txtArray[(i + 4)] == '4' || txtArray[(i + 4)] == '5' || txtArray[(i + 4)] == '6' || txtArray[(i + 4)] == '7' || txtArray[(i + 4)] == '8' || txtArray[(i + 4)] == '9')

                            && (txtArray[(i + 5)] == '0' || txtArray[(i + 5)] == '1' || txtArray[(i + 5)] == '2' || txtArray[(i + 5)] == '3' || txtArray[(i + 5)] == '4' || txtArray[(i + 5)] == '5' || txtArray[(i + 5)] == '6' || txtArray[(i + 5)] == '7' || txtArray[(i + 5)] == '8' || txtArray[(i + 5)] == '9')

                            && (txtArray[(i + 6)] == '0' || txtArray[(i + 6)] == '1' || txtArray[(i + 6)] == '2' || txtArray[(i + 6)] == '3' || txtArray[(i + 6)] == '4' || txtArray[(i + 6)] == '5' || txtArray[(i + 6)] == '6' || txtArray[(i + 6)] == '7' || txtArray[(i + 6)] == '8' || txtArray[(i + 6)] == '9')

                            && (txtArray[(i + 7)] == '0' || txtArray[(i + 7)] == '1' || txtArray[(i + 7)] == '2' || txtArray[(i + 7)] == '3' || txtArray[(i + 7)] == '4' || txtArray[(i + 7)] == '5' || txtArray[(i + 7)] == '6' || txtArray[(i + 7)] == '7' || txtArray[(i + 7)] == '8' || txtArray[(i + 7)] == '9')

                            && (txtArray[(i + 8)] == '0' || txtArray[(i + 8)] == '1' || txtArray[(i + 8)] == '2' || txtArray[(i + 8)] == '3' || txtArray[(i + 8)] == '4' || txtArray[(i + 8)] == '5' || txtArray[(i + 8)] == '6' || txtArray[(i + 8)] == '7' || txtArray[(i + 8)] == '8' || txtArray[(i + 8)] == '9')

                            && (txtArray[(i + 9)] == '0' || txtArray[(i + 9)] == '1' || txtArray[(i + 9)] == '2' || txtArray[(i + 9)] == '3' || txtArray[(i + 9)] == '4' || txtArray[(i + 9)] == '5' || txtArray[(i + 9)] == '6' || txtArray[(i + 9)] == '7' || txtArray[(i + 9)] == '8' || txtArray[(i + 9)] == '9')

                            && (txtArray[(i + 10)] == '0' || txtArray[(i + 10)] == '2' || txtArray[(i + 10)] == '4' || txtArray[(i + 10)] == '6' || txtArray[(i + 10)] == '8')

                            && (txtArray[(i + 11)] != '0' && txtArray[(i + 11)] != '1' && txtArray[(i + 11)] != '2' && txtArray[(i + 11)] != '3' && txtArray[(i + 11)] != '4' && txtArray[(i + 11)] != '5' && txtArray[(i + 11)] != '6' && txtArray[(i + 11)] != '7' && txtArray[(i + 11)] != '8' && txtArray[(i + 11)] != '9')

                          )
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                checkingTCKN[j] = (int)Char.GetNumericValue(txtArray[(i + j)]);
                                TCKN += txtArray[i + j];
                            }

                            // Change pdf name with TCKN
                            if (isIt_TCKN(checkingTCKN) == true)
                            {
                                byte[] array = System.IO.File.ReadAllBytes(path);
                                System.IO.File.WriteAllBytes(saveDirPath + "\\" + TCKN + ".pdf", array);

                                break;
                            }
                            else { continue; }
                        }
                    }
                }
            }
        }



        }

        static bool isIt_TCKN( int[] TCKN)
        {
            int tenthNumber =( ((TCKN[0] + TCKN[2] + TCKN[4] + TCKN[6] + TCKN[8]) * 7 ) - (TCKN[1] + TCKN[3] + TCKN[5] + TCKN[7])) % 10 ;
            int eleventhNumber = (TCKN[0] + TCKN[1] + TCKN[2] + TCKN[3] + TCKN[4] + TCKN[5] + TCKN[6] + TCKN[7] + TCKN[8] + TCKN[9]) % 10;

            if( tenthNumber == TCKN[9] && eleventhNumber == TCKN[10] ) { return true; }

            return false;
        }
       
    }
}
