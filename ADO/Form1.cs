using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ADO
{
    public partial class Form1 : Form
    {
        //static string connString = "SERVER=127.0.0.1;PORT=3306;DATABASE=Terre;UID=root;PASSWORD=;";
        static string connString = "SERVER=127.0.0.1;PORT=3306;UID=root;PASSWORD=;";
        int id = 0;
     
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox6.Enabled = false;
            textBox7.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            try
            {
                using (MySqlConnection con = new MySqlConnection(connString))
                {
                    con.Open();
                    MessageBox.Show("Connexion à la base réussie");

                    con.Close(); // Optionnel
                }
            }
            catch (MySqlException)
            {
                MessageBox.Show("BD deja crée ou probléme de connexion à la base ");
            }
            button1.Enabled = false;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button3.Enabled = true;
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();
                using (MySqlCommand commandDROP = new MySqlCommand("DROP DATABASE Terre ", con))
                {
                    try { int o = commandDROP.ExecuteNonQuery();
                        MessageBox.Show("La base existe, on l'a supprimé et on va la recrée");
                        using (MySqlCommand commandCREATE = new MySqlCommand("CREATE DATABASE Terre ", con))
                        {
                            commandCREATE.ExecuteNonQuery();



                        }
                    }
                    catch
                    {
                        MessageBox.Show("La base n'existe pas, on va la crée");
                        using (MySqlCommand commandCREATE = new MySqlCommand("CREATE DATABASE Terre ", con))
                        {
                            commandCREATE.ExecuteNonQuery();



                        }
                    }
               


                }

                    con.Close();
                
            }
            button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();
                try
                {
                    using (MySqlCommand command = new MySqlCommand(
                    "CREATE TABLE Terre.Pays (Id SMALLINT NOT NULL AUTO_INCREMENT,CodeNum SMALLINT, Alpha2 VARCHAR(2), Alpha3 VARCHAR(3), NomFR VARCHAR(255), NomEN VARCHAR(255),CapitaleFR VARCHAR(255),CapitaleEN VARCHAR(255), PRIMARY KEY (Id,Alpha2))",
                    con))
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Table creee!!!");
                    }
                }
                catch { MessageBox.Show("Table non creee ou deja crée"); }
                con.Close();
            }
            button3.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
      

            MySqlConnection connection = null;
            try
            {
              
        //Insertion des Pays
                string[] lines = File.ReadAllLines("pays.csv");
                foreach (string line in lines)
                {
                    connection = new MySqlConnection(connString);
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    string[] col = line.Split(';');
                    cmd.CommandText = "INSERT INTO Terre.Pays(CodeNum,Alpha2,Alpha3,NomFR,NomEN) " +
                    "VALUES(@CodeNum,@Alpha2,@Alpha3,@NomFR,@NomEN)";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@CodeNum", col[0].Split(',')[1]);
                    cmd.Parameters.AddWithValue("@Alpha2", col[0].Split(',')[2]);
                    cmd.Parameters.AddWithValue("@Alpha3", col[0].Split(',')[3]);
                    cmd.Parameters.AddWithValue("@NomFR", col[0].Split(',')[4]);
                    cmd.Parameters.AddWithValue("@NomEN", col[0].Split(',')[5]);

                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                //insertion des Capitaux
                string[] lines1 = File.ReadAllLines("Capitales.csv");
                foreach (string line in lines1)
                {
                    connection = new MySqlConnection(connString);
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    string[] col = line.Split(';');

                   
                    String premierePartie="";
                    String deuxiemePartie="";
              
                    if(col[0].Contains(' ')) { 
                       
                    premierePartie =col[0].Split(' ')[0];
                   
                    deuxiemePartie =col[0].Split(' ')[1].ToString();
                    }
                  
             

                    cmd.CommandText = "UPDATE Terre.Pays SET CapitaleFR =@CapitaleFR WHERE NomFR =@NomFR";
                    cmd.Parameters.AddWithValue("@CapitaleFR", col[1]);
                    cmd.Parameters.AddWithValue("@NomFR", premierePartie);
                   
                    cmd.Prepare();
                  
                   
             

                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                MessageBox.Show("Insertion Du fichier vers la BD terminé");
            
               
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!File.Exists("pays.csv")) {

                throw new InvalidOperationException("Le fichier pays n'existe pas");
        
            }
          

            string readText = File.ReadAllText("pays.csv");
            textBox1.Text = readText;

         
        
        }

        private void button6_Click(object sender, EventArgs e)
        {
        
            if (!File.Exists("Capitales.csv"))
            {

                throw new InvalidOperationException("Le fichier Capitales n'existe pas");

            }

            string readText = File.ReadAllText("Capitales.csv");
            textBox2.Text = readText;
        
        }

        private void button7_Click(object sender, EventArgs e)
        {
           
            string query = "select * from Terre.Pays"; 
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dataGridView1.DataSource = ds.Tables[0];
                }
                conn.Close();
            }
        }

       

        private void button9_Click(object sender, EventArgs e)
        {
            // string query = "select Id,CodeNum,Alpha3,NomFR,NomEN,CapitaleFR,CapitaleEN from Terre.Pays" +
            //     "where Alpha2 =="+textBox3.Text;
            try
            {
                MySqlConnection conn = new MySqlConnection(connString);
                conn.Open();
                Console.WriteLine("Connected");

                string query = "select Id,CodeNum,Alpha3,NomFR,NomEN,CapitaleFR,CapitaleEN from Terre.Pays WHERE Alpha2=@Alpha2 ";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Alpha2", textBox3.Text);
                cmd.Prepare();

                MySqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    textBox4.Text = rd["Id"].ToString()+" " + rd["CodeNum"].ToString() + " " + rd["Alpha3"].ToString()
                    + " " + rd["NomFR"].ToString() + " " + rd["NomEN"].ToString() + " " + rd["CapitaleFR"].ToString() + " "
                        + rd["CapitaleEN"].ToString();
     
                }
                rd.Close();

                conn.Close();
                Console.WriteLine("Closed");
            }
            catch (MySqlException e1)
            {
                Console.WriteLine("Error: " + e1.Message);
            }
        
    }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            textBox6.Enabled = true;
            textBox7.Enabled = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                //  connection = new MySqlConnection(connString);
                connection.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "UPDATE Terre.Pays SET CapitaleFR =@CapitaleFR,CapitaleEn=@CapitaleEN WHERE Alpha2 =@Alpha2";
                if (textBox6.Text.Equals("")) {
                    cmd.Parameters.AddWithValue("@CapitaleFR", null);
                }
                else {
                    cmd.Parameters.AddWithValue("@CapitaleFR", textBox6.Text);
                }
                if (textBox7.Text.Equals("")) {
                    cmd.Parameters.AddWithValue("@CapitaleEN", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@CapitaleEN", textBox7.Text);
                }
               
                cmd.Parameters.AddWithValue("@Alpha2", textBox5.Text);

                cmd.Prepare();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

            try
            {
                MySqlConnection conn = new MySqlConnection(connString);
                conn.Open();
                Console.WriteLine("Connected");

                string query = "select * from Terre.Pays WHERE Alpha2=@Alpha2 ";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Alpha2", textBox5.Text);
                cmd.Prepare();

                MySqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    textBox8.Text = rd["Id"].ToString() + " " + rd["CodeNum"].ToString() + " " + rd["Alpha3"].ToString()
                    + " " + rd["NomFR"].ToString() + " " + rd["NomEN"].ToString() + " " + rd["Alpha2"].ToString() + " "+ rd["CapitaleFR"].ToString() + " "
                        + rd["CapitaleEN"].ToString();

                }
                rd.Close();

                conn.Close();
                Console.WriteLine("Closed");
            }
            catch (MySqlException e1)
            {
                Console.WriteLine("Error: " + e1.Message);
            }


        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (!textBox12.Text.Equals(""))
            {
                try
                {
                    MySqlConnection conn = new MySqlConnection(connString);
                    conn.Open();
                    Console.WriteLine("Connected");

                    string query = "select Id,CodeNum,NomFR,NomEN,CapitaleFR,CapitaleEN" +
                        " from Terre.Pays WHERE CodeNum=@CodeNum ";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CodeNum", textBox12.Text);
                    cmd.Prepare();

                    MySqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        textBox13.Text = rd["Id"].ToString() + " " + rd["CodeNum"].ToString() + " "
                        + " " + rd["NomFR"].ToString() + " " + rd["NomEN"].ToString() + " " + " " + rd["CapitaleFR"].ToString() + " "
                            + rd["CapitaleEN"].ToString();

                    }
                    rd.Close();

                    conn.Close();
                    Console.WriteLine("Closed");
                }
                catch (MySqlException e1)
                {
                    Console.WriteLine("Error: " + e1.Message);
                }
            }

            /****************************************/
            if (!textBox11.Text.Equals(""))
            {
                try
                {
                    MySqlConnection conn = new MySqlConnection(connString);
                    conn.Open();
                    Console.WriteLine("Connected");

                    string query = "select NomEN" +
                        " from Terre.Pays WHERE Alpha2=@Alpha2 ";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Alpha2", textBox11.Text);
                    cmd.Prepare();

                    MySqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        textBox13.Text = rd["NomEN"].ToString();

                    }
                    rd.Close();

                    conn.Close();
                    Console.WriteLine("Closed");
                }
                catch (MySqlException e1)
                {
                    Console.WriteLine("Error: " + e1.Message);
                }
            }
            /********************************/
            if (!textBox9.Text.Equals(""))
            {
                try
                {
                    MySqlConnection conn = new MySqlConnection(connString);
                    conn.Open();
                    Console.WriteLine("Connected");
                   String[] separation= textBox9.Text.Split(' ');
                    String nomPaysFR = separation[0];
                    String capitalePaysFR = separation[1];

                    string query = "select CapitaleEN" +
                        " from Terre.Pays WHERE NomFR=@NomFR AND CapitaleFR=@CapitaleFR";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NomFR", nomPaysFR);
                    cmd.Parameters.AddWithValue("@CapitaleFR", capitalePaysFR);
                    cmd.Prepare();

                    MySqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        textBox13.Text = rd["CapitaleEN"].ToString();

                    }
                    rd.Close();

                    conn.Close();
                    Console.WriteLine("Closed");
                }
                catch (MySqlException e1)
                {
                    Console.WriteLine("Error: " + e1.Message);
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
          
             
            //ajouter la liste des images dans la BDD
            MySqlConnection conn1 = new MySqlConnection(connString);
            conn1.Open();
            FileStream fs;
           // MySqlCommand cmd;
            BinaryReader br;
           // conn.Open();
            string query1 = "select Alpha2 from Terre.Pays";
           
            MySqlCommand cmd2 = new MySqlCommand(query1, conn1);
       
           

            MySqlDataReader rd2 = cmd2.ExecuteReader();
            while (rd2.Read())
            {
                MySqlConnection conn3 = new MySqlConnection(connString);
                string query2 = "select NomFR from Terre.Pays WHERE Alpha2=@Alpha2";
                // Console.WriteLine(rd2["Alpha2"].ToString());
                string FileName = "drapeaux/" + rd2["Alpha2"].ToString() + ".png";
                MySqlCommand cmd = new MySqlCommand(query2, conn3);
                cmd.Parameters.AddWithValue("@Alpha2", rd2["Alpha2"].ToString());
                conn3.Open();
                cmd.Prepare();
                MySqlDataReader rd3 = cmd.ExecuteReader();
                bool fileExist = File.Exists(FileName);
                if (fileExist)
                {
                    while (rd3.Read())
                    {
                        byte[] ImageData;
                        fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                        br = new BinaryReader(fs);
                        ImageData = br.ReadBytes((int)fs.Length);
                        br.Close();
                        fs.Close();
                        string CmdString = "UPDATE Terre.Pays SET Drapeau=@Drapeau WHERE NomFR=@NomFR";
                        MySqlConnection conn4 = new MySqlConnection(connString);
                        MySqlCommand cmd1 = new MySqlCommand(CmdString, conn4);
                        cmd1.Parameters.Add("@Drapeau", MySqlDbType.Blob);
                        cmd1.Parameters.Add("@NomFR", MySqlDbType.VarChar, 45);
                        cmd1.Parameters["@Drapeau"].Value = ImageData;
                        cmd1.Parameters["@NomFR"].Value = rd3["NomFR"].ToString();
                        conn4.Open();
                        cmd1.Prepare();
                        int RowsAffected = cmd1.ExecuteNonQuery();

                        conn4.Close();
                    }
                }
                conn3.Close();









            }
            rd2.Close();

            conn1.Close();

            
            
            
           
          
          

        }

        private void button12_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();
            string query = "ALTER TABLE Terre.Pays ADD COLUMN Drapeau BLOB";
            MySqlCommand cmd = new MySqlCommand(query, conn);


            MySqlDataReader rd = cmd.ExecuteReader();
            rd.Close();

            conn.Close();
            Console.WriteLine("Closed");

        }

        private void button13_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();
            /* string CmdString = "INSERT INTO TABLE Terre.Pays WHERE " +
                 " Alpha2=@Alpha2" +
                 " AND NomFR=@NomFR" +
                 " AND CodeNum=@CodeNum" +
                 " AND Alpha3=@Alpha3" +
                 " AND CapitaleFR=@CapitaleFR" +
                 " AND Drapeau=@Drapeau ";*/
            /* cmd.CommandText = "INSERT INTO Terre.Pays(CodeNum,Alpha2,Alpha3,NomFR,NomEN) " +
                    "VALUES(@CodeNum,@Alpha2,@Alpha3,@NomFR,@NomEN)";*/
            string CmdString = "INSERT INTO Terre.Pays(CodeNum,Alpha2,Alpha3,NomFR,CapitaleFR,Drapeau) "
                + "VALUES(@CodeNum,@Alpha2,@Alpha3,@NomFR,@CapitaleFR,@Drapeau)";
            string FileName = "drapeaux/tn.png";
            FileStream fs;
        
            BinaryReader br;
            byte[] ImageData;
            fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fs);
            
            ImageData = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            MySqlConnection conn4 = new MySqlConnection(connString);
            conn4.Open();
            MySqlCommand cmd1 = new MySqlCommand(CmdString, conn4);
            cmd1.Parameters.AddWithValue("@Alpha2", textBox14.Text);
            cmd1.Parameters.AddWithValue("@Alpha3", textBox15.Text);
            cmd1.Parameters.AddWithValue("@CodeNum", textBox10.Text);
            cmd1.Parameters.AddWithValue("@CapitaleFR", textBox16.Text);
            cmd1.Parameters.AddWithValue("@NomFR", textBox17.Text);
            cmd1.Parameters.AddWithValue("@Drapeau", ImageData);
            
            cmd1.Prepare();
            cmd1.ExecuteNonQuery();
            conn4.Close();
           



            //
          


        }
    }
}
