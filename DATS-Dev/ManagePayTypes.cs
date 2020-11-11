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
            SQL sql = new SQL("select * from PayCodes order by Description");

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
                if (Edit.Name == "Delete" || Edit.Name == "Cancel" || dataGridView1.Rows[e.RowIndex].Cells[1].Value != null)
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
                                    VacYN=@VACYN, Rate_Exp=@RATE_EXP, AbsYN=@ABSYN, Colour=@COLOUR, ALL_Salary=@ALLSALARY, 
                                    ALL_PTC=@ALLPTC, ALL_PTH=@ALLPTH, ALL_SEIU_Contract=@ALLSEIUCONTRACT, 
                                    OpsOffice_HRLY=@OPSOFFICEHRLY, Roads_HRLY=@ROADSHRLY, Fleet_HRLY=@FLEETHRLY, Water_HRLY=@WATERHRLY,
                                    Parks_HRLY=@PARKSHRLY, F_Maintanence_Operations=@FMAINTANENCEOPERATIONS
                                    where PayType in (" + dataGridView1.Rows[e.RowIndex].Cells["PayType"].Value.ToString() + @")");

                            //sql.AddParameter("@PTYPE", dataGridView1.Rows[e.RowIndex].Cells["PayType"].Value.ToString());
                            sql.AddParameter("@PayType", dataGridView1.Rows[e.RowIndex].Cells["PayType"].Value.ToString());
                            sql.AddParameter("@DESCRIPTION", dataGridView1.Rows[e.RowIndex].Cells["Description"].Value.ToString());
                            sql.AddParameter("@PAYTYPEACTIVE", dataGridView1.Rows[e.RowIndex].Cells["PayTypeActive"].Value.ToString());
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
                            sql.AddParameter("@ALLSALARY", dataGridView1.Rows[e.RowIndex].Cells["ALL_Salary"].Value.ToString());
                            sql.AddParameter("@ALLPTC", dataGridView1.Rows[e.RowIndex].Cells["ALL_PTC"].Value.ToString());
                            sql.AddParameter("@ALLPTH", dataGridView1.Rows[e.RowIndex].Cells["ALL_PTH"].Value.ToString());
                            sql.AddParameter("@ALLSEIUCONTRACT", dataGridView1.Rows[e.RowIndex].Cells["ALL_SEIU_Contract"].Value.ToString());
                            sql.AddParameter("@OPSOFFICEHRLY", dataGridView1.Rows[e.RowIndex].Cells["OpsOffice_HRLY"].Value.ToString());
                            sql.AddParameter("@ROADSHRLY", dataGridView1.Rows[e.RowIndex].Cells["Roads_HRLY"].Value.ToString());
                            sql.AddParameter("@FLEETHRLY", dataGridView1.Rows[e.RowIndex].Cells["Fleet_HRLY"].Value.ToString());
                            sql.AddParameter("@WATERHRLY", dataGridView1.Rows[e.RowIndex].Cells["Water_HRLY"].Value.ToString());
                            sql.AddParameter("@PARKSHRLY", dataGridView1.Rows[e.RowIndex].Cells["Parks_HRLY"].Value.ToString());
                            sql.AddParameter("@FMAINTANENCEOPERATIONS", dataGridView1.Rows[e.RowIndex].Cells["F_Maintanence_Operations"].Value.ToString());

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
                                  @RegYN, @StdYN, @OTYN ,@VacYN, @Rate_Exp, @AbsYN, @Colour, @ALL_Salary, @ALL_PTC, @ALL_PTH,
                                  @ALL_SEIU_Contract, @OpsOffice_HRLY,@Roads_HRLY, @Fleet_HRLY, @Water_HRLY, @Parks_HRLY,
                                  @F_Maintanence_Operations)");

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
                    sql.AddParameter("@ALL_Salary", "");
                    sql.AddParameter("@ALL_PTC", "");
                    sql.AddParameter("@ALL_PTH", "");
                    sql.AddParameter("@ALL_SEIU_Contract", "");
                    sql.AddParameter("@OpsOffice_HRLY", "");
                    sql.AddParameter("@Roads_HRLY", "");
                    sql.AddParameter("@Fleet_HRLY", "");
                    sql.AddParameter("@Water_HRLY", "");
                    sql.AddParameter("@Parks_HRLY", "");
                    sql.AddParameter("@F_Maintanence_Operations", "");

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
