﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using FluentValidation.Results;

namespace Logiwa.ProductManagement
{
    public partial class ProductManagement : Form
    {
        public ProductManagement()
        {
            InitializeComponent();
        }
        SqlConnection connection = new SqlConnection(@"data source=DESKTOP-QAB9N31;initial catalog=Logiwa;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");


        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Product ID should be empty!");
                return;
            }
            LogiwaEntities1 db = new LogiwaEntities1();
            var categoryName = comboBox1.Text;
            var categoryId = db.tblCategory
                .Where(c => c.CATEGORYNAME == categoryName)
                .Select(c => c.CATEGORYID)
                .FirstOrDefault();

            ProductData productData = new ProductData();
            productData.ProductName = txtProductName.Text;
            productData.ProductCategoryId = categoryId;
            productData.InStock = Convert.ToInt32(txtProductStock.Text);

            // geçerlilik kontrolü ve ekleme işlemi
            ProductValid valid = new ProductValid();
            ValidationResult result = valid.Validate(productData);
            IList<ValidationFailure> failures = result.Errors;
            if (!result.IsValid)
            {
                foreach (ValidationFailure failure in failures)
                {
                    MessageBox.Show(failure.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                LogiwaEntities1 logiwa = new LogiwaEntities1();
                var productname = productData.ProductName;
                var product2 = logiwa.tblProduct.FirstOrDefault(p => p.PRODUCTNAME == productname);
                if (product2 != null)
                {
                    MessageBox.Show("This product already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    tblProduct product3 = new tblProduct();
                    product3.PRODUCTNAME = productData.ProductName;
                    product3.PRODUCTCATEGORYID = productData.ProductCategoryId;
                    product3.PRODUCTSTOCK = productData.InStock;
                    logiwa.tblProduct.Add(product3);
                    logiwa.SaveChanges();
                    MessageBox.Show("Product added!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                LogiwaEntities1 db = new LogiwaEntities1();
                ProductData productData = new ProductData();
                productData.ProductId = Convert.ToInt32(textBox1.Text);
                int productid = productData.ProductId;
                var x = db.tblProduct.Find(productid);
                db.tblProduct.Remove(x);
                db.SaveChanges();
                MessageBox.Show("Product Deleted!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ProductManagement_Load(object sender, EventArgs e)
        {//valuemember id display member category namecombobox
            LogiwaEntities1 db = new LogiwaEntities1();
            var categories = db.tblCategory.ToList();
            comboBox1.DataSource = categories;
            comboBox1.ValueMember = "CATEGORYID";
            comboBox1.DisplayMember = "CATEGORYNAME";

        }
    }
}
