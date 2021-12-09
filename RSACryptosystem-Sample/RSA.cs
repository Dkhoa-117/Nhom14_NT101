﻿using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
//using System.Security;
using System.Xml;
using System.Security.Cryptography; 
//using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Text;

namespace RSA1710900
{
    public partial class RSACryptosystem : Form
    {
        private delegate void btnEncryptDecrypt();

        private RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        private string KeyPath; //Duong dan cho public va private key.
        private bool isFile; //Ma hoa File hay Folder !


        public RSACryptosystem()
        {
            InitializeComponent();
        }

        private void RSACryptoSystem_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Thông báo", MessageBoxButtons.OKCancel)!=System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }    
        }

        private void RSACryptoSystem_FormLoad(object sender, EventArgs e)
        {
            this.cbbKeyLength.Items.Add("512 bits");
            this.cbbKeyLength.Items.Add("1024 bits");
            this.cbbKeyLength.Items.Add("2048 bits");
            this.cbbKeyLength.Items.Add("4096 bits");
            this.cbbKeyLength.Items.Add("8192 bits");

            this.tbPublicKey.ReadOnly = true;
            this.tbPublicKey.BackColor = System.Drawing.SystemColors.Window;
            this.tbPrivateKey.ReadOnly = true;
            this.tbPrivateKey.BackColor = System.Drawing.SystemColors.Window;
            this.cbbKeyLength.Text = "1024 bits";
            Control.CheckForIllegalCrossThreadCalls = false; 
        }

        private void ChangeButtonState(bool isEnable)
        {
            this.btnReset.Enabled = isEnable;
            this.btnOutOpen.Enabled = isEnable;
            this.tbOutput.Enabled = isEnable;
            this.btEncrypt.Enabled = isEnable;
            this.btDecrypt.Enabled = isEnable;
            this.btGenerateKey.Enabled = isEnable;
            this.btOpenFileIn.Enabled = isEnable;
            this.btOpenFileKeys.Enabled = isEnable;
            this.btOpenFolderIn.Enabled = isEnable;
            this.btSelectOutput.Enabled = isEnable;
        }


        private void btGenerate_Key(object sender, EventArgs e)
        {
            
            // Tạo file chứa key
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.DefaultExt = "xml";
            saveFileDialog1.Filter = "Xml File|*.xml";
            saveFileDialog1.Title = "Select File";

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            
            int KeyLength = 0;

            if (this.cbbKeyLength.Text == "1024 bits") KeyLength = 1024;
            else if (this.cbbKeyLength.Text == "512 bits") KeyLength = 512;
            else if (this.cbbKeyLength.Text == "2048 bits") KeyLength = 2048;
            else if (this.cbbKeyLength.Text == "4096 bits") KeyLength = 4096;
            else if (this.cbbKeyLength.Text == "8192 bits") KeyLength = 8192;

            String pathPrivateKey = saveFileDialog1.FileName;         

            //tạo key có độ dài lengthKey
            RSA = new RSACryptoServiceProvider(KeyLength); //tạo public key va private key có độ dài lengtheKey

                             
            File.WriteAllText(pathPrivateKey, RSA.ToXmlString(true));  // Private Key

            KeyPath = pathPrivateKey;
            tbPathKeys.Text = pathPrivateKey; //Hiển thị đường dẫn key

            if (File.Exists(KeyPath))
            {
                if (Path.GetExtension(KeyPath) == ".xml") //kiểm tra định dạng
                {
                    XmlDocument xml = new XmlDocument();
                        xml.LoadXml(File.ReadAllText(KeyPath)); //đọc RSA Key (public && private)

                    try
                    {
                        RSAParameters _publicKey = RSA.ExportParameters(false); //public Key
                        var sw = new StringWriter();
                        var xs = new XmlSerializer(typeof(RSAParameters));
                        xs.Serialize(sw, _publicKey);
                        string XmlFileOfPublicKey = sw.ToString(); // xuat ra file xml vao tbPublicKey 


                        string pathPublicKey = "C:\\Users\\Admin\\Downloads\\pathPublicKey.xml"; //Can than :))))
                        File.WriteAllText(pathPublicKey, XmlFileOfPublicKey);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(File.ReadAllText(pathPublicKey)); //doc public key
                        XmlNode xmlNode_1 = xmlDoc.SelectSingleNode("/RSAParameters/Exponent"); //e 
                        XmlNode xmlNode_2 = xmlDoc.SelectSingleNode("/RSAParameters/Modulus"); //n (n,e)!!
                        string publicKey = xmlNode_1.InnerText + xmlNode_2.InnerText;


                        byte[] bytes = Encoding.Default.GetBytes(publicKey);
                        string hexStringPublicKey = BitConverter.ToString(bytes);
                        hexStringPublicKey = hexStringPublicKey.Replace("-", " ");
                        tbPublicKey.Text = hexStringPublicKey;               

                        XmlDocument xmlDoc_Key = new XmlDocument();
                        xmlDoc_Key.LoadXml(File.ReadAllText(KeyPath));
                        XmlNode xmlNode_3= xmlDoc_Key.SelectSingleNode("/RSAKeyValue/D");
                        bytes = Encoding.Default.GetBytes(xmlNode_3.InnerText);
                        string hexStringPrivateKey = BitConverter.ToString(bytes);
                        hexStringPrivateKey = hexStringPrivateKey.Replace("-", " ");
                        tbPrivateKey.Text = hexStringPrivateKey;

                    }
                    catch (Exception ix)
                    {
                        MessageBox.Show(ix.Message);
                    }
                }
            }
            MessageBox.Show("Tạo key có độ dài " + KeyLength.ToString() + " bits thành công.");

        }

        private void btnOpenFileKeys_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Xml files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (op.ShowDialog() == DialogResult.OK)
            {
                KeyPath = op.FileName;
                tbPathKeys.Text = op.FileName;
            }

            if (File.Exists(KeyPath))
            {

                if (Path.GetExtension(KeyPath) == ".xml")
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(File.ReadAllText(KeyPath));
                    try
                    {
                        XmlNode xnList = xml.SelectSingleNode("/RSAKeyValue/D");

                        RSAParameters _publicKey = RSA.ExportParameters(false);
                        var sw = new StringWriter();
                        var xs = new XmlSerializer(typeof(RSAParameters));
                        xs.Serialize(sw, _publicKey);
                        string XmlFileOfPublicKey = sw.ToString(); // xuat ra file xml vao tbPublicKey 


                        string pathPublicKey = "C:\\Users\\Admin\\Downloads\\pathPublicKey.xml"; //Can than :))))
                        File.WriteAllText(pathPublicKey, XmlFileOfPublicKey);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(File.ReadAllText(pathPublicKey)); //doc public key
                        XmlNode xmlNode_1 = xmlDoc.SelectSingleNode("/RSAParameters/Exponent"); //e 
                        XmlNode xmlNode_2 = xmlDoc.SelectSingleNode("/RSAParameters/Modulus"); // n 
                        string publicKey = xmlNode_1.InnerText + xmlNode_2.InnerText;


                        byte[] bytes = Encoding.Default.GetBytes(publicKey);
                        string hexStringPublicKey = BitConverter.ToString(bytes);
                        hexStringPublicKey = hexStringPublicKey.Replace("-", " ");


                        //byte[] bytes = Encoding.Default.GetBytes(publicKey_NoHeader);
                        //string hexStringPublicKey = BitConverter.ToString(bytes);
                        //hexStringPublicKey = hexStringPublicKey.Replace("-", " ");

                        tbPublicKey.Text = hexStringPublicKey;


                        bytes = Encoding.Default.GetBytes(xnList.InnerText);
                        string hexStringPrivateKey = BitConverter.ToString(bytes);
                        hexStringPrivateKey = hexStringPrivateKey.Replace("-", " ");
                        tbPrivateKey.Text = hexStringPrivateKey;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed: " + ex.Message);
                    }
                }
            }
        }

        private void btnOpenFileIn_Click(object sender, EventArgs e)
        {           
            isFile = true;
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "All Files (*.*)|*.*";
            if (op.ShowDialog() == DialogResult.OK)
                tbInput.Text = op.FileName;
        }

        private void btnOutOpen_Click(object sender, EventArgs e) //Mở thư mục Output
        {
            if (tbOutput.Text.Length > 0)
            {
                try
                {
                    Process prc = new System.Diagnostics.Process(); 
                    prc.StartInfo.FileName = tbOutput.Text;
                    prc.Start();
                }
                catch (Exception ioex)
                {
                    MessageBox.Show("Failed: " + ioex.Message);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đường dẫn !");
            }
        }

        private void RSA_Algorithm(string inputFile, string outputFile, RSAParameters RSAKeyInfo, bool isEncrypt)
        {
            try
            {
                FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read); //Đọc file input
                FileStream fsCiperText = new FileStream(outputFile, FileMode.Create, FileAccess.Write); //Tạo file output
                //fsCiperText.SetLength(0); 
                byte[] bin, encryptedData;
                long rdlen = 0; //readlength
                long totlen = fsInput.Length; // totallength
                int len;

                this.pgbProcess.Minimum = 0;
                this.pgbProcess.Maximum = 100;

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(); 
                RSA.ImportParameters(RSAKeyInfo); //Nhập thông tin khoá RSA (public va private)

                int maxBytesCanEncrypted;

                
                //RSA chỉ có thể mã hóa các khối dữ liệu ngắn hơn độ dài khóa, chia dữ liệu cho một số khối và sau đó mã hóa từng khối và sau đó hợp nhất chúng
                
                if (isEncrypt) // Ma hoa  //Buffer
                        
                    maxBytesCanEncrypted = RSA.KeySize / 8 - 11; //ma hoa 245 bytes du lieu thanh 256 bytes
                
                else  //Giai ma
                    maxBytesCanEncrypted = RSA.KeySize / 8 ; //256 bytes



                //Read from the input file, then encrypt and write to the output file.
                

                while (rdlen < totlen) // 0  64 128 192 224 
                {
                    if (totlen - rdlen < maxBytesCanEncrypted) 
                        maxBytesCanEncrypted = (int)(totlen - rdlen); //Block cuoi: totlen - rdlen < (RSA.KeySize - 384) / 8) + 37

                    bin = new byte[maxBytesCanEncrypted];
                    len = fsInput.Read(bin, 0, maxBytesCanEncrypted); // o maxbyte -1

                    if (isEncrypt) 
                        encryptedData = RSA.Encrypt(bin, false); //Mã Hoá false = ko dem OAEP
                    else 
                        encryptedData = RSA.Decrypt(bin, false); //Giải mã false = ko dem OAEP
                    //
                    fsCiperText.Write(encryptedData, 0, encryptedData.Length);

                    rdlen = rdlen + len; //trong truong hop block cuoi co the thieu byte! (ko thi + bin.Length !)

                    this.lbProcess.Text = "Tên tệp xử lý: " + Path.GetFileName(inputFile) + " \nThành công: " + ((long)(rdlen * 100) / totlen).ToString() + " %";
                    this.lbProcess.Update();
                    this.lbProcess.Refresh();

                    this.pgbProcess.Value = (int)((rdlen * 100) / totlen);  //thanh tiến trình
                }
                

                fsCiperText.Close(); //save file
                fsInput.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show("Failed: " + ex.Message);
            }

        }


        private void btnEncryptClick()
        {
            ChangeButtonState(false);

            if (this.tbPathKeys.Text.Length == 0)
            {                                              
                MessageBox.Show("Key không hợp lệ!");
                ChangeButtonState(true);
                return;
            }

            try
            {
                if (tbInput.Text.Length != 0 /*&&
                tbPathKeys.Text.Length != 0 &&
                tbN.Text.Length != 0 */ )
                {

                    //Calculator time execution
                    Stopwatch sw = Stopwatch.StartNew();
                    sw.Start();

                    string inputFileName = tbInput.Text, outputFileName = "";
                    

                    if (isFile)
                    {
                        outputFileName = tbOutput.Text +"\\"+ Path.GetFileName(tbInput.Text) + ".nhom11";
                    }


                    //get Keys.
                    RSA = new RSACryptoServiceProvider();
                    RSA.FromXmlString(File.ReadAllText(this.KeyPath));


                    if (isFile)
                    {
                        RSA_Algorithm(inputFileName, outputFileName, RSA.ExportParameters(true), true);
                    }

                    else
                    {

                        string[] filePaths = Directory.GetFiles(inputFileName, "*"); //Lay file trorng folder.

                        if (filePaths.Length == 0 /*|| (filePaths.Length == 1 && (Path.GetFileName(filePaths[0]) == "Thumbs.db"))*/)
                        {
                            MessageBox.Show("Thư mục rỗng!");
                            ChangeButtonState(true);
                            return;
                        }


                        for (int i = 0; i < filePaths.Length; i++) //Ma hoa tung file.
                        {
                            outputFileName = tbOutput.Text + "\\" + Path.GetFileName(filePaths[i]) + ".nhom11";
                            //if (Path.GetFileName(filePaths[i]) != "Thumbs.db")
                            RSA_Algorithm(filePaths[i], outputFileName, RSA.ExportParameters(true), true);
                            //SaveFile(outputFileName, Encrypt(inputFileName));

                        }
                    }


                    ChangeButtonState(true);
                    sw.Stop();
                    double elapsedMs = sw.Elapsed.TotalMilliseconds / 1000;
                    MessageBox.Show("Thời gian thực thi " + elapsedMs.ToString() + "s");


                }
                else
                {
                    ChangeButtonState(true);
                    MessageBox.Show("Dữ liệu không đủ để mã hóa!"); // Ko co du 1 trong 3: N, D, E
                }
            }
            catch (Exception ex)
            {
                ChangeButtonState(true);
                MessageBox.Show("Failed: " + ex.Message);
            }

            ChangeButtonState(true);


        }
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (tbOutput.Text.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn đường dẫn đến thư mục Output");
                return;
            }
            if (tbPathKeys.Text.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn đường dẫn đến Key!");
                return;
            }

            btnEncryptDecrypt s = new btnEncryptDecrypt(btnEncryptClick);
            s.BeginInvoke(null, null); //ko block UI Thread 

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
           
            this.isFile = true;
            this.tbPathKeys.Clear();
            this.tbInput.Clear();
            this.tbPublicKey.Clear();
            this.tbPrivateKey.Clear(); 
            

            this.tbOutput.Clear();
            this.KeyPath = "";
            this.cbbKeyLength.Text = "1024 bits";
            this.lbProcess.Text = "";
            this.lbProcess.Update();
            RSA = new RSACryptoServiceProvider();
            if (this.pgbProcess.Value > 0)
                this.pgbProcess.Value = 0;
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (tbOutput.Text.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn đường dẫn đến thư mục Output");
                return;
            }
            btnEncryptDecrypt s = new btnEncryptDecrypt(btnDecryptClick);
            s.BeginInvoke(null, null); //Ko block UI Thread

        }
        private void btnDecryptClick()
        {
            ChangeButtonState(false);

            try
            {
                if (tbInput.Text.Length != 0 &&
                   tbPathKeys.Text.Length != 0)
                {
                    //Calculator time ex...
                    Stopwatch sw = Stopwatch.StartNew();
                    sw.Start();

                    string inputFileName = tbInput.Text, outputFileName = "";

                    if (isFile && Path.GetExtension(inputFileName) != ".nhom11")
                    {
                        MessageBox.Show("Tệp tin này không được hỗ trợ đển giải mã!");
                        ChangeButtonState(true);
                        return;
                    }

                    if (isFile)
                    {

                        outputFileName = tbOutput.Text + "\\" + Path.GetFileName(inputFileName.Substring(0, inputFileName.Length - 7));


                    }

                    RSA = new RSACryptoServiceProvider();
                    RSA.FromXmlString(File.ReadAllText(this.KeyPath));

                    if (isFile)
                        RSA_Algorithm(inputFileName, outputFileName, RSA.ExportParameters(true), false);
                    else
                    {
                        string[] filePaths = Directory.GetFiles(inputFileName, "*.nhom11", SearchOption.AllDirectories);
                        if (filePaths.Length == 0 || (filePaths.Length == 1))
                        {
                            MessageBox.Show("Thư mục rỗng!");
                            ChangeButtonState(true);
                            return;
                        }

                        for (int i = 0; i < filePaths.Length; i++)
                        { 
                            outputFileName = tbOutput.Text + "\\" + Path.GetFileName(filePaths[i].Substring(0, filePaths[i].Length - 7));
                            RSA_Algorithm(filePaths[i], outputFileName, RSA.ExportParameters(true), false);
                        }

                    }
                    ChangeButtonState(true);
                    sw.Stop();
                    double elapsedMs = sw.Elapsed.TotalMilliseconds / 1000;
                    MessageBox.Show("Tổng thời gian thực thi: " + elapsedMs.ToString() + "s");
                }
                else
                {
                    MessageBox.Show("Không đủ điều kiện để giải mã !");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed: " + ex.Message);
            }
            ChangeButtonState(true);
        }

  
        private void btnOpenFolderIn_Click(object sender, EventArgs e)
        {
            isFile = false; //Xac dinh file hay folder
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                this.tbInput.Text = folderBrowserDialog1.SelectedPath;
        }

      
        

       
    
        private void selectOutput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f1 = new FolderBrowserDialog();
            if (f1.ShowDialog() == DialogResult.OK)
            {
                this.tbOutput.Text = f1.SelectedPath;
            }    
        }



        private void btnCheckFile_Click(object sender, EventArgs e)
        {
            var KiemTraFile = new KiemTraFile();
            KiemTraFile.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
    
}