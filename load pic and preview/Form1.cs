using System.IO;
using System.Data.SqlClient;
using System.Linq;
using System.Data;

namespace load_pic_and_preview
{
    public partial class Form1 : Form
    {
        SqlConnection cnn = new SqlConnection(@"Data Source=DESKTOP-PPIILUB;Initial Catalog=pics;Integrated Security=True");
        SqlCommand cmd;
        SqlDataAdapter da;
        DataTable dt;


        

        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opndlg = new OpenFileDialog();
            opndlg.Filter = "All Pictures(*.*)|*.jpg;*.jmp;*.png;*.gif";
            opndlg.ShowDialog();
            if (!String.IsNullOrEmpty(opndlg.FileName))
            {
                cnn.Close();
                cnn.Open();

                byte[] arr = File.ReadAllBytes(opndlg.FileName);
                cmd = new SqlCommand("insert into Images (pic) values(@p)",cnn);
                cmd.Parameters.Add("@p", SqlDbType.VarBinary).Value = arr;
                cmd.ExecuteNonQuery();
                cnn.Close();
                Form1_Load(sender, e);

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cnn.Close();
            cnn.Open();
            da = new SqlDataAdapter("select * from Images", cnn);
            dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            DataGridViewImageColumn img=(DataGridViewImageColumn)dataGridView1.Columns[1];
            img.ImageLayout = DataGridViewImageCellLayout.Stretch;
            cnn.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex != -1 && e.ColumnIndex == 1)
            {
                cnn.Close();
                cnn.Open();
                cmd = new SqlCommand("select pic from Images where id=@i", cnn);
                cmd.Parameters.Add("@i", SqlDbType.Int).Value = (int)dataGridView1.CurrentRow.Cells[0].Value;
                byte[] ar=(byte[])cmd.ExecuteScalar();
                MemoryStream ms = new MemoryStream(ar);
                pictureBox1.Image = Image.FromStream(ms);
                cnn.Close();
            }
        }
    }
}