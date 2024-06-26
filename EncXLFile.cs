﻿using Syncfusion.XlsIO;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PGENV3
{
    public partial class EncXLFile : Form
    {
        public EncXLFile()
        {
            InitializeComponent();
        }

        GetPathOrExtention gte = new GetPathOrExtention();
        private void EncryptXLFile(string fileName)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;

                FileStream inputStream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite);

                IWorkbook workbook = application.Workbooks.Open(inputStream);
                IWorksheet worksheet = workbook.Worksheets[0];

                workbook.PasswordToOpen = TbEncPwd2.Text;
              
                FileStream outputStream = new FileStream(gte.GetDirPath(fileName) + "\\Encrypted-" + gte.GetfileName(fileName), FileMode.Create, FileAccess.Write);
                
                workbook.SaveAs(outputStream);
                outputStream.Close();
                inputStream.Close();
                workbook.Close();
            }
        }

        private async Task EncryptXLFile()
        {
            if (TbEncPwd1.Text != TbEncPwd2.Text)
            {
                MessageBox.Show("Please insert the same password!", "P-GEN", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (TbEncPwd1.Text == "" && TbEncPwd2.Text == "")
            {
                MessageBox.Show("Please enter a password!", "P-GEN", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (TbEncPwd1.Text.Length < 8 && TbEncPwd2.Text.Length < 8)
            {
                MessageBox.Show("Minimum password length is 8 characters!", "P-GEN", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            ofd.Multiselect = false;

            if (TbEncPwd1.Text == TbEncPwd2.Text && ofd.ShowDialog() == DialogResult.OK)
            {
                if (gte.GetFileExtension(ofd.FileName) != ".xlsx")
                {
                    LblProceeding.ResetText();
                    MessageBox.Show("File version is not supported!" + '\n' + '\n' + "Only Excel version 2019 and above are supported!", "P-GEN", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (gte.GetFileExtension(ofd.FileName) == ".xlsx")
                {
                    LblProceeding.Text = "Proceeding...Do not exit the software!";
                    var task = Task.Run(() => EncryptXLFile(ofd.FileName)).ContinueWith(t => { File.Delete(ofd.FileName); t.Wait(); });
                    try
                    {
                        await task;
                        LblProceeding.Text = "Done!";
                        MessageBox.Show("File Successfully Encrypted!", "P-GEN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LblProceeding.ResetText();
                    }

                    catch (AggregateException)
                    {

                        MessageBox.Show("Ensure the password is correct!" + '\n' + '\n' + "Ensure the files you want to encrypt is not opened in another software!", "P-GEN", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LblProceeding.ResetText();
                        return;
                    }

                    catch (Exception)
                    {
                        LblProceeding.ResetText();
                        MessageBox.Show("Ensure the password is correct!" + '\n' + '\n' + "Ensure the file you want to encrypt is not opened in another software!", "P-GEN", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                                     
            }
    }
        private void BtnXLFileEnc_Click(object sender, EventArgs e)
        {
            _ = EncryptXLFile();
        }
        private void BtnShowPWD1_Click_1(object sender, EventArgs e)
        {
            BtnShowPWD1.Visible = false;
            BtnHidePWD1.Visible = true;
            TbEncPwd1.PasswordChar = '\0';
        }
        private void BtnShowPWD2_Click_1(object sender, EventArgs e)
        {
            BtnHidePWD2.Visible = true;
            BtnShowPWD2.Visible = false;
            TbEncPwd2.PasswordChar = '\0';
        }
        private void BtnHidePWD1_Click_1(object sender, EventArgs e)
        {
            BtnShowPWD1.Visible = true;
            BtnHidePWD1.Visible = false;
            TbEncPwd1.PasswordChar = '*';
        }
        private void BtnHidePWD2_Click_1(object sender, EventArgs e)
        {
            BtnHidePWD2.Visible = false;
            BtnShowPWD2.Visible = true;
            TbEncPwd2.PasswordChar = '*';
        }
        private void BtnExit_Click_1(object sender, EventArgs e)
        {
            Close();
        }
        private void TbEncPwd1_MouseDown(object sender, MouseEventArgs e)
        {
            toolTip1.Show("Minimum password length is 8 characters!", this.TbEncPwd1);
        }
        private void TbEncPwd2_MouseDown(object sender, MouseEventArgs e)
        {
            toolTip1.Show("Minimum password length is 8 characters!", this.TbEncPwd2);
        }
    }
}
