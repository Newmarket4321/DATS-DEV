using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
namespace DATS_Timesheets
{
    public partial class ManagePayTypes : Form
    {
        DataTable storage = null;
        public static string Ptype;
        public static int TypeText;
        public static string descreption;
        public static string paytypeActive;
        public static string result;
        DatabaseType type;
        public string Paytype
        {
            get { return Ptype; }
            set { Ptype = value; }
        }

        public int PayTypeText
        {
            get { return TypeText; }
            set { TypeText = value; }
        }

        public string Descreption
        {
            get { return descreption; }
            set { descreption = value; }
        }

        public ManagePayTypes()
        {
            InitializeComponent();
            loaddata();
        }
        public void loaddata()
        {
            SQL sql = new SQL("select PayType, Description, PayTypeActive, TransType, Rate_A, Rate_R, RegYN, StdYN, OTYN, VacYN, Rate_Exp," +
                " AbsYN, Colour, SAL, EXE, FFE, HRLY, FAC, PTC, XGRD, PTH, " +
                "CON, O_SCL, SEIU_C from PayCodes order by Description");

             DataTable dt = sql.Run();
            dataGridView1.DataSource = dt;
            for (int i = 0; i <= dataGridView1.ColumnCount - 1; i++)
            {
                dataGridView1.Columns[i].ReadOnly = true;
            }
            storage = dt;
            Text = "ManagePayTypes";
        }
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //if (dataGridView1.IsCurrentCellDirty)
            //{
            //    dataGridView1.Rows[e.RowIndex].ErrorText = "";
            //    int newInteger;
            //    if (int.TryParse(PayType.ToString(), out newInteger) == true)
            //    {
            //        e.Cancel = true;
            //        dataGridView1.Rows[e.RowIndex].ErrorText = "the value must be a non-negative integer";

            //    }
            //    else
            //    {
            //            MessageBox.Show("Invalid number! The value must be a numeric");
            //    }

            //}

          

        }
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {
                if ((e.Exception) is System.FormatException)
                {
                    //MessageBox.Show("Please enter valid values");
                    MessageBox.Show(e.Exception.Message, "Error FormatException", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //Suppress a ConstraintException
                    e.ThrowException = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR: dataGridView1_DataError",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DialogResult result;
            if (e.ColumnIndex == 1)
            {
               // if(dataGridView1.Rows[e.RowIndex].Cells[3].Value != null)
                    Ptype = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["PayType"].Value);
                Delete.Name = Paytype + "_Delete";
                string Task = "";
                if (Delete.Name == "Delete" || Delete.Name == "Cancel" || dataGridView1.Rows[e.RowIndex].Cells[1].Value != null)
                    Task = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                else
                    return;
                
                if (Task == "Delete")
                {

                    SQL sql = new SQL("Select count(*) from Timesheets where PayType = @PayType");
                    sql.AddParameter("@PayType", Paytype);
                    if (int.Parse(sql.Run().Rows[0][0].ToString()) >= 1)
                    {
                        result = MessageBox.Show("This Pay Type is in use! You Can not delete this.");
                    }
                    else
                    {
                        result = MessageBox.Show("Are You sure want to Delete?", "Confirmation", MessageBoxButtons.YesNo);
                        if (result == System.Windows.Forms.DialogResult.Yes &&
                            dataGridView1.CurrentCell.OwningColumn.Name == Paytype + "_Delete")
                        {
                            sql = new SQL("delete from PayCodes where PayType = @PayType");
                            sql.AddParameter("@PayType", Paytype);

                            sql.Run();
                            MessageBox.Show("Record deleted !");
                            loaddata();
                            dataGridView1.Update();
                            dataGridView1.Refresh();
                        }
                        else
                        {
                            MessageBox.Show("Record not deleted !");
                        }
                    }
                }
                else if(Task == "Cancel")
                {

                    dataGridView1.Rows[e.RowIndex].Cells[0].Value = "Edit";
                    dataGridView1.Rows[e.RowIndex].Cells[1].Value = "Delete";
                    Edit.UseColumnTextForLinkValue = true;
                    Edit.Name = Paytype + "_Edit";
                    if (dataGridView1.CurrentCell.OwningColumn.Name != Paytype + "_Edit" || dataGridView1.CurrentCell.OwningColumn.Name != Paytype + "_Save")
                    {

                        dataGridView1.CurrentCell.ReadOnly = true;
                    }
                    Delete.UseColumnTextForLinkValue = true;
                    Delete.Name = Paytype + "_Delete";
                    loaddata();
                    dataGridView1.Update();
                    dataGridView1.Refresh();
                }
            }
            if(e.ColumnIndex == 0)
            {
                Ptype = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["PayType"].Value);
                string Task = "";
                if (Edit.Name == "Edit" || Edit.Name == "Save" || dataGridView1.Rows[e.RowIndex].Cells[0].Value != null)
                    Task = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                else
                    return;
                if (Task == "Edit")
                {
                    result = MessageBox.Show("Are You sure want to Edit this record?", "Confirmation", MessageBoxButtons.YesNo);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        Edit.Name = Paytype + "_Edit";
                        if (dataGridView1.Rows[e.RowIndex].Cells["PayType"].Value.ToString() == Ptype &&
                            dataGridView1.CurrentCell.OwningColumn.Name == Paytype + "_Edit" &&
                            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != "Save")
                        {
                            //MessageBox.Show(Task + "   " + Edit.Name);
                            for (int i = 0; i <= dataGridView1.Rows[e.RowIndex].Cells.Count - 1; i++)
                            {
                                if(i != 1 || i != 0)
                                {
                                    dataGridView1.Rows[e.RowIndex].Cells[i].ReadOnly = false;
                                    
                                }
                            }
                            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "Save";
                            Edit.Name = Paytype + "_Save";
                            Edit.UseColumnTextForLinkValue = false;
                            dataGridView1.Rows[e.RowIndex].Cells[1].Value = "Cancel";
                            Delete.Name = Paytype + "_Cancel";
                            Delete.UseColumnTextForLinkValue = false;
                            Delete.ReadOnly = true;
                            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = true;
                        }
                    }
                }
                else if(Task == "Save")
                {
                    if (Regex.IsMatch(@"^-?\d*\.{0,-1}\d+$", dataGridView1.Rows[e.RowIndex].Cells["RegYN"].Value.ToString()) &&
                        Regex.IsMatch(@"^-?\d*\.{0,-1}\d+$", dataGridView1.Rows[e.RowIndex].Cells["StdYN"].Value.ToString()) &&
                         Regex.IsMatch(@"^-?\d*\.{0,-1}\d+$", dataGridView1.Rows[e.RowIndex].Cells["OTYN"].Value.ToString()) &&
                         Regex.IsMatch(@"^-?\d*\.{0,-1}\d+$", dataGridView1.Rows[e.RowIndex].Cells["VacYN"].Value.ToString()) &&
                         Regex.IsMatch(@"^-?\d*\.{0,-1}\d+$", dataGridView1.Rows[e.RowIndex].Cells["AbsYN"].Value.ToString()))
                    {
                        result = MessageBox.Show("Are You sure want to Save this record?", "Confirmation", MessageBoxButtons.YesNo);

                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        if (dataGridView1.Rows[e.RowIndex].Cells["PayType"].Value.ToString() == Ptype &&
                            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != "Edit" &&
                            dataGridView1.CurrentCell.OwningColumn.Name == Paytype + "_Save")
                        {
                            if(dataGridView1.CurrentCell.OwningColumn.Name != Paytype + "_Edit" || dataGridView1.CurrentCell.OwningColumn.Name != Paytype + "_Save")
                            {

                                for (int i = 0; i <= dataGridView1.ColumnCount - 1; i++)
                                {
                                    if (dataGridView1.CurrentCell.ReadOnly == dataGridView1.Columns[i].ReadOnly)
                                        dataGridView1.Columns[i].ReadOnly = false;
                                    else
                                        dataGridView1.Columns[i].ReadOnly = true;
                                }
                            }

                            SQL sql = new SQL(@"UPDATE PayCodes SET PayType=@PayType, Description=@DESCRIPTION, PayTypeActive=@PAYTYPEACTIVE,
                                    TransType=@TRANSTYPE, Rate_A=@RATE_A, Rate_R=@RATE_R, RegYN=@REGYN, StdYN=@STDYN, OTYN=@OTYN,
                                    VacYN=@VACYN, Rate_Exp=@RATE_EXP, AbsYN=@ABSYN, Colour=@COLOUR, SAL=@SAL, EXE=@EXE, FFE=@FFE, 
                                    HRLY=@HRLY, FAC=@FAC, PTC=@PTC, XGRD=@XGRD, PTH=@PTH, CON=@CON, O_SCL=@O_SCL, SEIU_C = @SEIU_C
                                    where PayType in (" + dataGridView1.Rows[e.RowIndex].Cells["PayType"].Value.ToString() + @")");
                                
                            sql.AddParameter("@PayType", dataGridView1.Rows[e.RowIndex].Cells["PayType"].Value.ToString());
                            sql.AddParameter("@DESCRIPTION", dataGridView1.Rows[e.RowIndex].Cells["Description"].Value.ToString());
                            sql.AddParameter("@PAYTYPEACTIVE", dataGridView1.Rows[e.RowIndex].Cells["PTypeActive"].Value.ToString());
                            sql.AddParameter("@TRANSTYPE", dataGridView1.Rows[e.RowIndex].Cells["TransType"].Value.ToString());
                            sql.AddParameter("@RATE_A", dataGridView1.Rows[e.RowIndex].Cells["Rate_A"].Value.ToString());
                            sql.AddParameter("@RATE_R", dataGridView1.Rows[e.RowIndex].Cells["Rate_R"].Value.ToString());
                            sql.AddParameter("@REGYN", dataGridView1.Rows[e.RowIndex].Cells["RegYN"].Value.ToString());
                            sql.AddParameter("@STDYN", dataGridView1.Rows[e.RowIndex].Cells["StdYN"].Value.ToString());
                            sql.AddParameter("@OTYN", dataGridView1.Rows[e.RowIndex].Cells["OTYN"].Value.ToString());
                            sql.AddParameter("@VACYN", dataGridView1.Rows[e.RowIndex].Cells["VacYN"].Value.ToString());
                            sql.AddParameter("@RATE_EXP", dataGridView1.Rows[e.RowIndex].Cells["Rate_Exp"].Value.ToString());
                            sql.AddParameter("@ABSYN", dataGridView1.Rows[e.RowIndex].Cells["AbsYN"].Value.ToString());
                            sql.AddParameter("@COLOUR", dataGridView1.Rows[e.RowIndex].Cells["Colour"].Value.ToString());
                            sql.AddParameter("@SAL", dataGridView1.Rows[e.RowIndex].Cells["SAL"].Value.ToString());
                            sql.AddParameter("@EXE", dataGridView1.Rows[e.RowIndex].Cells["EXE"].Value.ToString());
                            sql.AddParameter("@FFE", dataGridView1.Rows[e.RowIndex].Cells["FFE"].Value.ToString());
                            sql.AddParameter("@HRLY", dataGridView1.Rows[e.RowIndex].Cells["HRLY"].Value.ToString());
                            sql.AddParameter("@FAC", dataGridView1.Rows[e.RowIndex].Cells["FAC"].Value.ToString());
                            sql.AddParameter("@PTC", dataGridView1.Rows[e.RowIndex].Cells["PTC"].Value.ToString());
                            sql.AddParameter("@XGRD", dataGridView1.Rows[e.RowIndex].Cells["XGRD"].Value.ToString());
                            sql.AddParameter("@PTH", dataGridView1.Rows[e.RowIndex].Cells["PTH"].Value.ToString());
                            sql.AddParameter("@CON", dataGridView1.Rows[e.RowIndex].Cells["CON"].Value.ToString());
                            sql.AddParameter("@O_SCL", dataGridView1.Rows[e.RowIndex].Cells["O_SCL"].Value.ToString());
                            sql.AddParameter("@SEIU_C", dataGridView1.Rows[e.RowIndex].Cells["SEIU_C"].Value.ToString());

                            sql.Run();

                            MessageBox.Show("Record Saved successfully!");
                            Edit.UseColumnTextForLinkValue = true;
                            Edit.Name = Paytype + "_Edit";
                            Delete.UseColumnTextForLinkValue = true;
                            Delete.Name = Paytype + "_Delete";
                            loaddata();
                            dataGridView1.Update();
                            dataGridView1.Refresh();
                        }
                    }
                    }
                    else
                    {
                        MessageBox.Show("Invalid number!The value must be a 0 or -1");

                    }
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            int output;
            if (int.TryParse(PayType_Txt.Text, out output) == true)
            {
                //MessageBox.Show("output " + TypeText.ToString());  //Print 123434
                TypeText = int.Parse(PayType_Txt.Text);
                SQL sql = new SQL("Select count(*) from PayCodes where PayType = @PayType");
                sql.AddParameter("@PayType", PayTypeText);

                if (int.Parse(sql.Run().Rows[0][0].ToString()) >= 1)
                {
                    MessageBox.Show("That PayType is already associated with a DATS account.");
                }
                else
                {
                    if (PayType_Txt.Text != "" && Description_txt.Text != "")
                    {

                    descreption = Description_txt.Text;

                    sql = new SQL(@"insert into PayCodes values (@PayType, @Description, @PayTypeActive, @TransType, @Rate_A, @Rate_R, 
                                  @RegYN, @StdYN, @OTYN ,@VacYN, @Rate_Exp, @AbsYN, @Colour, @SAL, @EXE, @FFE,
                                  @HRLY, @FAC, @PTC, @XGRD, @PTH, @CON,@O_SCL, @SEIU_C)");

                    sql.AddParameter("@PayType", PayTypeText);
                    sql.AddParameter("@Description", Description_txt.Text);
                    sql.AddParameter("@PayTypeActive", TypeActive.Checked);
                    sql.AddParameter("@TransType", "");
                    sql.AddParameter("@Rate_A", "");
                    sql.AddParameter("@Rate_R", "");
                    sql.AddParameter("@RegYN", "");
                    sql.AddParameter("@StdYN", "");
                    sql.AddParameter("@OTYN", "");
                    sql.AddParameter("@VacYN", "");
                    sql.AddParameter("@Rate_Exp", "");
                    sql.AddParameter("@AbsYN", "");
                    sql.AddParameter("@Colour", "");
                    sql.AddParameter("@SAL", "");
                    sql.AddParameter("@EXE", "");
                    sql.AddParameter("@FFE", "");
                    sql.AddParameter("@HRLY", "");
                    sql.AddParameter("@FAC", "");
                    sql.AddParameter("@PTC", "");
                    sql.AddParameter("@XGRD", "");
                    sql.AddParameter("@PTH", "");
                    sql.AddParameter("@CON", "");
                    sql.AddParameter("@O_SCL", "");
                    sql.AddParameter("@SEIU_C", "");

                    sql.Run();

                    MessageBox.Show("That PayType is  successfuly Saved.");
                    loaddata();
                    dataGridView1.Update();
                    dataGridView1.Refresh();
                    PayType_Txt.Text = "";
                    Description_txt.Text = "";
                    TypeActive.Checked = false;
                    }
                    else
                    {
                        MessageBox.Show("Pay Type and Description is required ! Please fill it with valid values");
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid number! Plese Enter valid Paytype value.");
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Core.export(Text, storage);
        }
    }
}
