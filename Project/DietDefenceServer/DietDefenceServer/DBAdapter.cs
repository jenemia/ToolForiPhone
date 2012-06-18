using System;
using System.Collections;
using MySql.Data.MySqlClient;


namespace ChattingServer
{
    public class DBAdapter
    {
        MySqlConnection mConn;

        public DBAdapter()
        {
            this.ConnectDatabase();
            try
            {
                mConn.Open();
            }
            catch
            {
                mConn.Open();
            }
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
                this.mConn = new MySqlConnection(_str);
            }
            catch (Exception)
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
            _command.Connection = this.mConn;
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
            else if (table.Equals("userJoin"))
                _result = JoinTable(_reader);

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

        private ArrayList JoinTable(MySqlDataReader reader)
        {
            ArrayList _result = new ArrayList();
            Hashtable _hash;
            try
            {
                while (reader.Read())
                {
                    _hash = new Hashtable();
                    _hash.Add("name", reader["name"]);
                    _hash.Add("passwd", reader["passwd"]);
                    _hash.Add("win", reader["win"]);
                    _hash.Add("lose", reader["lose"]);

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
         * attributes : "id,passwd"
         * values : "soohyun,1234"
         */
        public bool InsertUserJoinTuple(string table, string attributes, string id, string pw)
        {
            string _sql = "insert into " + table + " (" + attributes + ") " +
                "values ('" + id + "', '" + pw + "');";

            if (this.CheckID(id)) // id가 존재 안할 때
                return false;

            MySqlCommand _command;
            _command = new MySqlCommand();
            _command.Connection = this.mConn;
            _command.CommandText = _sql;
            try
            {
                _command.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            return true;
        }

        public bool CheckID(string id)
        {
            string _sql = "select no from joinUser where id = '" + id + "'";

            MySqlCommand _command;
            _command = new MySqlCommand();
            _command.Connection = this.mConn;
            _command.CommandText = _sql;
            MySqlDataReader _reader = _command.ExecuteReader();

            _reader.Read();
            try
            {//id가 존재한다는 것
                int _no = Convert.ToInt32(_reader["no"]);
                _reader.Close();
                return true;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                _reader.Close();
                return false;
            }
        }

        public bool CheckIDandPW(string id, string pw)
        {
            string _sql = "select no from joinUser where id = '" + id +
                                              "' and passwd = '" + pw + "'";

            MySqlCommand _command;
            _command = new MySqlCommand();
            _command.Connection = this.mConn;
            _command.CommandText = _sql;

            MySqlDataReader _reader = _command.ExecuteReader();
            try
            {
                _reader.Read();

                //id, pw가 일치 한 것이 존재한다는 것
                int _no = Convert.ToInt32(_reader["no"]);
                _reader.Close();
                return true;
            }
            catch
            {
                _reader.Close();
                return false;
            }
        }

        public bool recordWinAndLose(string winID, string loseID)
        {
            string _winSql = "update joinUser set win = win+1 where id = '"
                                + winID + "';";
            string _loseSql = "update joinUser set lose = lose+1 where id = '"
                                + loseID + "';";

            MySqlCommand _command;
            _command = new MySqlCommand();
            _command.Connection = this.mConn;

            try
            {
                _command.CommandText = _winSql;
                _command.ExecuteNonQuery();

                _command.CommandText = _loseSql;
                _command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
