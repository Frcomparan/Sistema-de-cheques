﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_de_Cheques
{
    public partial class SearchChecks : Form
    {

        Beneficiary beneficiarySQL = new Beneficiary();
        Check checkSQL = new Check();

        public SearchChecks()
        {
            InitializeComponent();
            InitCBBeneficiaries();
            InitDatePicker();
        }


        private void InitCBBeneficiaries()
        {
            cbBeneficiaries.Items.Clear();

            foreach (Beneficiary beneficiary in beneficiarySQL.GetBeneficiariesSLQ())
            {
                int row = cbBeneficiaries.Items.Add(beneficiary.Name);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!checkValidations()) return;
            List<string> filters = new List<string>();
            string beneficiary = "";
            string[] mounts = new string[2];
            DateTime[] dates = new DateTime[2];
            string[] invoices = new string[2];

            if (cbBeneficiaries.SelectedIndex != -1)
            {
                beneficiary = (cbBeneficiaries.SelectedIndex + 1).ToString();
                filters.Add("beneficiary");
            }

            if (txtInitialInvoice.Text != "" || txtLastInvoice.Text != "")
            {
                invoices[0] = txtInitialInvoice.Text;
                invoices[1] = txtLastInvoice.Text; 
                filters.Add("invoice");
            }

            if (txtInitialMount.Text != "" || txtLastMount.Text != "")
            {
                mounts[0] = txtInitialMount.Text;
                mounts[1] = txtLastMount.Text;
                filters.Add("mount");
            }

            if (txtInitialDate.Value < DateTime.Today.AddDays(1) || txtLastDate.Value < DateTime.Today)
            {
                dates[0] = txtInitialDate.Value;
                dates[1] = txtLastDate.Value;
                filters.Add("date");
            }

            List<Check> checks = checkSQL.GetChecksByValuesSQL(filters, beneficiary, mounts, dates, invoices);
            InitChecksTable(checks);
            InitTextBoxes();
        }

        private void InitChecksTable(List<Check> checks)
        {
            checksTable.Rows.Clear();
            foreach (Check check in checks)
            {
                int fila = checksTable.Rows.Add();
                checksTable.Rows[fila].Cells[0].Value = check.Invoice;
                checksTable.Rows[fila].Cells[1].Value = beneficiarySQL.GetBeneficiarySQL(check.Beneficiary).Name;
                checksTable.Rows[fila].Cells[2].Value = check.Mount;
                checksTable.Rows[fila].Cells[3].Value = check.Date.ToShortDateString();
            }
        }

        private void InitDatePicker()
        {
            txtInitialDate.Value = DateTime.Today.AddDays(1);
            txtLastDate.Value = DateTime.Today;
        }

        private bool checkValidations()
        {
            //if (txtInitialDate.Value != DateTime.Today.AddDays(1) && txtLastDate.Value == DateTime.Today.AddDays(1))
            //{
            //    MessageBox.Show(
            //    "Si quiere buscar por fecha debe usar los dos campos",
            //    "Problema con la busqueda",
            //    MessageBoxButtons.OK,
            //    MessageBoxIcon.Error);
            //    return false;
            //}

            if (txtInitialMount.Text != "" && txtLastMount.Text == "")
            {
                MessageBox.Show(
                "Si quiere buscar por monto debe usar los dos campos",
                "Problema con la busqueda",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return false;
            }

            if (txtInitialInvoice.Text != "" && txtLastInvoice.Text == "")
            {
                MessageBox.Show(
                "Si quiere buscar por folio debe usar los dos campos",
                "Problema con la busqueda",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return false;
            }

            if (txtInitialDate.Value > DateTime.Today.AddDays(1) && txtLastDate.Value > DateTime.Today)
            {
                MessageBox.Show(
                "No puede filtrar por fechas que estan en el futuro",
                "Problema con la busqueda",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return false;
            }

            if (txtInitialDate.Value > txtLastDate.Value.AddDays(1))
            {
                MessageBox.Show(
                "La fecha inicial no puede ser mayor a la fecha final",
                "Problema con la busqueda",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return false;
            }

            if (!HelperMethods.IsNumeric(txtInitialInvoice.Text) || !HelperMethods.IsNumeric(txtLastInvoice.Text))
            {
                MessageBox.Show(
                    "Los folios deben tener formato numerico",
                    "Problema con la busqueda",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            if (txtInitialInvoice.Text != "" || txtLastInvoice.Text != "")
            {
                if (int.Parse(txtInitialInvoice.Text) > int.Parse(txtLastInvoice.Text))
                {
                    MessageBox.Show(
                    "El folio inicial no puede ser mayor al folio final",
                    "Problema con la busqueda",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    return false;
                }
            }

            if (txtInitialMount.Text != "" || txtLastMount.Text != "")
            {
                if (!HelperMethods.IsMoney(txtInitialMount.Text) || !HelperMethods.IsMoney(txtLastMount.Text))
                {
                    MessageBox.Show(
                        "Los montos deben tener formato numerico",
                        "Problema con la busqueda",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }

                if (decimal.Parse(txtInitialMount.Text) > decimal.Parse(txtLastMount.Text))
                {
                    MessageBox.Show(
                    "El monto inicial no puede ser mayor al monto final",
                    "Problema con la busqueda",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        private void InitTextBoxes()
        {
            InitDatePicker();
            txtInitialInvoice.Text = "";
            txtInitialMount.Text = "";
            txtLastInvoice.Text = "";
            txtLastMount.Text = "";
            cbBeneficiaries.SelectedIndex = -1;
        }
    }
}
