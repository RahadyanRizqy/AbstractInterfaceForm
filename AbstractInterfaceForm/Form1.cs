using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstractInterfaceForm
{
    public partial class Form1 : Form
    {   
        public Form1()
        {
            InitializeComponent();
        }
        private void textBox1_TextChanged(object sender, EventArgs e) { label1.Text = ""; }
        public void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void button1_Click(object sender, EventArgs e)
        {
            NextQuery nq = new NextQuery($"into pembeli(id_pembeli, nama, nomor_telp, alamat) values((select count(id_pembeli) from pembeli)+1, '{textBox1.Text}', '{textBox2.Text}', '{textBox3.Text}');");
            Database db = new SQLDBHelper(nq.insert());
            db.ExecQuery();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            label1.Text = "Nama";
            label2.Text = "Nomor Telp";
            label3.Text = "Alamat";
            button2_Click(sender, e);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            NextQuery nq = new NextQuery("* from pembeli;");
            Database db = new SQLDBHelper(nq.select()) ;
            DataTable dt = new DataTable();
            db.ExecReader(dataGridView1, dt);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            int id_pembeli = Convert.ToInt32(textBox4.Text);
            NextQuery nq = new NextQuery($"from pembeli where id_pembeli = {id_pembeli};");
            Database db = new SQLDBHelper(nq.delete());
            db.ExecQuery();
            button2_Click(sender, e);
            textBox4.Text = "";
        }
        private void Form1_Load(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { label2.Text = ""; }
        private void textBox3_TextChanged(object sender, EventArgs e) { label3.Text = ""; }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
    }
    public interface Select { public string select(); }
    public interface Insert { public string insert(); }
    public interface Delete { public string delete(); }
    public class NextQuery : Select, Insert, Delete
    {
        public string query;
        public NextQuery(string query) { this.query = query; }
        public string select() { return "select " + query; }
        public string insert() { return "insert " + query; }
        public string delete() { return "delete " + query; }
    }
    public abstract class Database
    {
        public NpgsqlConnection conn;
        public string query;
        public string connstr;
        public Database(string query)
        {
            conn = new NpgsqlConnection("Server=localhost;Port=5432;User Id=postgres;Password=radriz;Database=testonly;");
            this.query = query;
        }
        public abstract void ExecQuery();
        public abstract void ExecReader(DataGridView dgv, DataTable dt);
    }
    class SQLDBHelper : Database
    {
        public SQLDBHelper(string query) : base(query) { this.query = query; }
        public override void ExecQuery()
        {
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            cmd.Dispose();
        }
        public override void ExecReader(DataGridView dgv, DataTable dt)
        {
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            NpgsqlDataReader dr = cmd.ExecuteReader();
            dt.Load(dr);
            dgv.DataSource = dt;
            conn.Close();
            cmd.Dispose();

        }
    }
}