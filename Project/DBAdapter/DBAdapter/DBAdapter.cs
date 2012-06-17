using System;
using System.Collections;
using MySql.Data.MySqlClient;


namespace DBAdapterNamespace
{
    public class DBAdapter
    {
        MySqlConnection mConn;

        public DBAdapter()
        {
            this.ConnectDatabase();
            mConn.Open();
        }

        ~DBAdapter()
        {
            this.mConn.Close();
        }

        private void ConnectDatabase()
        {
            try
            {
                string _str = String.Format(@"server=myungjun.org;user=jenemia;password=soohyun;database=jenemia");
                mConn = new MySqlConnection(_str);
            }
            catch( Exception )
            {
                this.ConnectDatabase();
            }
        }

        public ArrayList SelectTuples(string table, string attributes, string where)
        {
            ArrayList _result = new ArrayList();

            string _sql = "select " + attributes + " from " + table + where + ";";

            MySqlCommand _command;
            _command = new MySqlCommand();
            _command.Connection = mConn;
            _command.CommandText = _sql;
            MySqlDataReader _reader = _command.ExecuteReader();

            if (table.Equals("tower"))
                _result = TowerTable(_reader);
            else if (table.Equals("enemy"))
                _result = EnemyTable(_reader);
            else if (table.Equals("user"))
                _result = UserTable(_reader);
            else if (table.Equals("stage"))
                _result = StageTable(_reader);

            _reader.Close();
            return _result;
        }

        private ArrayList TowerTable(MySqlDataReader reader)
        {
            ArrayList _result = new ArrayList();
            Hashtable _hash;
            try
            {
                while (reader.Read())
                {
                    _hash = new Hashtable();
                    _hash.Add("type", reader["type"]);
                    _hash.Add("x", reader["x"]);
                    _hash.Add("y", reader["y"]);
                    _hash.Add("name", reader["name"]);
                    _hash.Add("level", reader["level"]);
                    _hash.Add("range", reader["range"]);
                    _hash.Add("damage", reader["damage"]);
                    _hash.Add("speed", reader["speed"]);
                    _hash.Add("price", reader["price"]);
                    _hash.Add("remove", reader["remove"]);
                    _hash.Add("increasedamage", reader["increasedamage"]);
                    _hash.Add("increasemax", reader["increasemax"]);
                    _hash.Add("increaseprice", reader["increaseprice"]);

                    _result.Add(_hash);
                }
            }
            catch
            {
            }

            return _result;
        }

        private ArrayList EnemyTable(MySqlDataReader reader)
        {
            ArrayList _result = new ArrayList();
            Hashtable _hash;
            try
            {
                while (reader.Read())
                {
                    _hash = new Hashtable();
                    _hash.Add("type", reader["type"]);
                    _hash.Add("x", reader["x"]);
                    _hash.Add("y", reader["y"]);
                    _hash.Add("name", reader["name"]);
                    _hash.Add("speed", reader["speed"]);
                    _hash.Add("hp", reader["hp"]);
                    _hash.Add("damage", reader["damage"]);
                    _hash.Add("price", reader["price"]);

                    _result.Add(_hash);
                }
            }
            catch
            {
            }

            return _result;
        }

        private ArrayList UserTable(MySqlDataReader reader)
        {
            ArrayList _result = new ArrayList();
            Hashtable _hash;
            try
            {
                while (reader.Read())
                {
                    _hash = new Hashtable();
                    _hash.Add("hp", reader["hp"]);
                    _hash.Add("money", reader["money"]);

                    _result.Add(_hash);
                }
            }
            catch
            {
            }

            return _result;
        }

        private ArrayList StageTable(MySqlDataReader reader)
        {
            ArrayList _result = new ArrayList();
            Hashtable _hash;
            try
            {
                while (reader.Read())
                {
                    _hash = new Hashtable();
                    _hash.Add("stageNo", reader["stageNo"]);
                    _hash.Add("enemyNo", reader["enemyNo"]);
                    _hash.Add("count", reader["count"]);

                    _result.Add(_hash);
                }
            }
            catch
            {
            }

            return _result;
        }
        /*
         * table : "joinUser"
         * attributes : "name,passwd"
         * values : "soohyun,1234"
         */
        public bool InsertTuple( string table, string attributes, string values )
        {
            string _sql = "insert into " + table + "(" + attributes + ")" +
                "values (" + values + ");";

            MySqlCommand _command;
            _command = new MySqlCommand();
            _command.Connection = mConn;
            _command.CommandText = _sql;
            MySqlDataReader _reader = _command.ExecuteReader();
            return true;
        }
    }
}
