﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace autoDATA
{    
    public partial class Admin : Form
    {
        String connectionstring;
        MySqlConnection con;
        public string query;

        public Admin()
        {
            InitializeComponent();
        }

        //Form LOAD esemény:
        private void Admin_Load(object sender, EventArgs e)
        {
            lbRegBy.Visible = false;

            //AUTÓK kiválasztható üzemanyagainak feltöltése:
            //string ARRAY:           
            string[] fuels = new string[] { "válasszon", "benzin", "dízel", "LPG", "CNG", "hibrid", "elektromos", "üzemanyagcella" };
            cbAdminCarsFuel.DataSource = fuels;

            //AUTÓK lökettérfogatának feltöltése:
            string[] disparray = new string[] {
                "válasszon","0,1","0,2","0,3","0,4","0,5","0,6","0,7","0,8","0,9",
                "1,0","1,1","1,2","1,3","1,4","1,5","1,6","1,7","1,8","1,9",
                "2,0","2,1","2,2","2,3","2,4","2,5","2,6","2,7","2,8","2,9",
                "3,0","3,1","3,2","3,3","3,4","3,5","3,6","3,7","3,8","3,9",
                "4,0","4,1","4,2","4,3","4,4","4,5","4,6","4,7","4,8","4,9",
                "5,0","5,1","5,2","5,3","5,4","5,5","5,6","5,7","5,8","5,9",
                "6,0","6,1","6,2","6,3","6,4","6,5","6,6","6,7","6,8","6,9",
                "7,0","7,1","7,2","7,3","7,4","7,5","7,6","7,7","7,8","7,9",
                "8,0","n/a"
            };
            cbAdminCarsDisp.DataSource = disparray;

            //AUTÓK gyártási évei:
            //gyártás kezdete:
            string[] prodyearsarray = new string[] {
                "1900","1901","1902","1903","1904","1905","1906","1907","1908","1909","1910",
                "1911","1912","1913","1914","1915","1916","1917","1918","1919","1920",
                "1921","1922","1923","1924","1925","1926","1927","1928","1929","1930",
                "1931","1932","1933","1934","1935","1936","1937","1938","1939","1940",
                "1941","1942","1943","1944","1945","1946","1947","1948","1949","1950",
                "1951","1952","1953","1954","1955","1956","1957","1958","1959","1960",
                "1961","1962","1963","1964","1965","1966","1967","1968","1969","1970",
                "1971","1972","1973","1974","1975","1976","1977","1978","1979","1980",
                "1981","1982","1983","1984","1985","1986","1987","1988","1989","1990",
                "1991","1992","1993","1994","1995","1996","1997","1998","1999","2000",
                "2001","2002","2003","2004","2005","2006","2007","2008","2009","2010",
                "2011","2012","2013","2014","2015","2016","2017","2018","2019","2020",
                "válasszon"
            };
            Array.Reverse(prodyearsarray); //az egyszerűség kedvéért csökkenő a sorrend
            cbAdminCarsProdStart.DataSource = prodyearsarray;

            //gyártás vége:
            string[] prodyearsarray2 = new string[] {
               "1900","1901","1902","1903","1904","1905","1906","1907","1908","1909","1910",
                "1911","1912","1913","1914","1915","1916","1917","1918","1919","1920",
                "1921","1922","1923","1924","1925","1926","1927","1928","1929","1930",
                "1931","1932","1933","1934","1935","1936","1937","1938","1939","1940",
                "1941","1942","1943","1944","1945","1946","1947","1948","1949","1950",
                "1951","1952","1953","1954","1955","1956","1957","1958","1959","1960",
                "1961","1962","1963","1964","1965","1966","1967","1968","1969","1970",
                "1971","1972","1973","1974","1975","1976","1977","1978","1979","1980",
                "1981","1982","1983","1984","1985","1986","1987","1988","1989","1990",
                "1991","1992","1993","1994","1995","1996","1997","1998","1999","2000",
                "2001","2002","2003","2004","2005","2006","2007","2008","2009","2010",
                "2011","2012","2013","2014","2015","2016","2017","2018","2019","2020",
                "még gyártásban", "válasszon"
            };
            Array.Reverse(prodyearsarray2);
            cbAdminCarsProdEnd.DataSource = prodyearsarray2;

            //AUTÓK hengerszámának feltöltése:
            string[] cylinderarray = new string[] { "válasszon", "nincs", "1", "2", "3", "4", "5", "6", "8", "10", "12", "16" };
            cbAdminCarsCyl.DataSource = cylinderarray;

            //AUTÓK karosszériatípusainak feltöltése:
            //string LIST:            
            List<string> bodylist = new List<string>();
            bodylist.Add("válasszon");
            bodylist.Add("ferdehátú");
            bodylist.Add("szedán");
            bodylist.Add("kombi");
            bodylist.Add("kabrió");
            bodylist.Add("kupé");
            bodylist.Add("egyterű");
            bodylist.Add("busz");
            bodylist.Add("pickup");
            bodylist.Add("terepjáró");
            bodylist.Add("sport");
            cbAdminCarsBody.DataSource = bodylist;

            //FELHASZNÁLÓK munkakörei string ARRAY :
            string[] positions = new string[]
                { "válasszon", "adminisztrátor", "újságíró", "szerkesztő", "főszerkesztő", "fotós", "vágó"};
            cbAdminUsersPosition.DataSource = positions;

            databaseConnect(); //adatbázis kapcsolódás metódusának a meghívása

            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (con.State == ConnectionState.Open)
            {
                lbAdminConnection.Text = "Kapcsolódva";
                lbAdminConnection.ForeColor = Color.Green;
                loadCarCategoriesForAdmin(); //autókategóriák betöltése
                loadCarMakesForAdminSearch(); //autómárkák betöltése
                loadGerarbox();
            }
            else
            {
                lbAdminConnection.Text = "Nincs kapcsolat";
                lbAdminConnection.ForeColor = Color.Red;
            }           
        }

        //AUTÓK VÁLTÓFAJTÁK betöltése ADATBÁZISBÓL:
        private void loadGerarbox()
        {     
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                query = "SELECT * FROM gears";

                DataTable mytable = new DataTable();
                MySqlCommand search = new MySqlCommand(query, con);
                MySqlDataReader open = search.ExecuteReader();
                mytable.Load(open);
                cbAdminCarsGearbox.DisplayMember = "gear_type";
                cbAdminCarsGearbox.DataSource = mytable;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            };
        }

        //ADATBÁZIS KAPCSOLAT metódus:
        private void databaseConnect()
        {
            try
            {
                //connectionstring = "datasource = localhost; DataBase = auto_data; username = root; password = ; charset = utf8";

                connectionstring = "datasource = 94.76.215.115; DataBase = petersze_autodata; username = petersze_petersze; password = Rmbg5780Ar; charset = utf8";

                using (con = new MySqlConnection(connectionstring))
                {
                    con.Open();                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                lbAdminConnection.Text = "Nincs kapcsolat";
            };
        }

        //AUTÓKATEGÓRIÁK betöltése ADATBÁZISBÓL:      
        public void loadCarCategoriesForAdmin()
        {
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                query = "SELECT * FROM carcategories";

                DataTable mytable = new DataTable();
                MySqlCommand search = new MySqlCommand(query, con);
                MySqlDataReader open = search.ExecuteReader();
                mytable.Load(open);
                cbAdminCarsCategory.DisplayMember = "categories";
                cbAdminCarsCategory.DataSource = mytable;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            };
        }

        //AUTÓMÁRKÁK betöltése adatbázisból:
        private void loadCarMakesForAdminSearch()
        {   
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                query = "SELECT * FROM carmakes";

                DataTable mytable = new DataTable();
                MySqlCommand search = new MySqlCommand(query, con);
                MySqlDataReader open = search.ExecuteReader();
                mytable.Load(open);
                cbAdminCarsMakeSearch.DisplayMember = "makes";
                cbAdminCarsMakeSearch.DataSource = mytable;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            };
        }       

        //AUTÓMODELLEK betöltése adatbázisból:
        private void loadCarmodels(string make)            
        { 
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                query = "SELECT model FROM " +@make;

                MySqlCommand cmd = new MySqlCommand(query, con);                
                cmd.Parameters.Add(new MySqlParameter("@make", make));             
                MySqlDataReader open = cmd.ExecuteReader();
                DataTable mytable = new DataTable();
                mytable.Load(open);
                cbAdminCarsModelSearch.DisplayMember = "model";
                cbAdminCarsModelSearch.DataSource = mytable;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            };
        }

        //Ha az AUTÓ KERESŐBEN ki lett választva egy MÁRKA akkor annak megfelelően töltse be a modelleket:
        private void cbAdminCarsMakeSearch_TextChanged(object sender, EventArgs e)
        {
            switch (cbAdminCarsMakeSearch.Text)
            {
                case "Alfa Romeo": //tehát ha pl az Alfa romeo van kiválasztva, akkor csak az Alfra Romeo modelljeit mutassa
                    loadCarmodels("alfaromeo");
                    break;
                case "Alpina":
                    loadCarmodels("alpina");
                    break;
                case "Aston Martin":
                    loadCarmodels("astonmartin");
                    break;                
                case "Audi":
                    loadCarmodels("audi");
                    break;
                case "Bentley":
                    loadCarmodels("bentley");
                    break;
                case "BMW":
                    loadCarmodels("bmw");
                    break;
                case "Bugatti":
                    loadCarmodels("bugatti");
                    break;
                case "Citroen":
                    loadCarmodels("citroen");
                    break;
                case "Dacia":
                    loadCarmodels("dacia");
                    break;
                case "Ferrari":
                    loadCarmodels("ferrari+");
                    break;
                case "Fiat":
                    loadCarmodels("fiat");
                    break;
                case "Ford":
                    loadCarmodels("ford");
                    break;
                case "Honda":
                    loadCarmodels("honda");
                    break;
                case "Hyundai":
                    loadCarmodels("hyundai");
                    break;
                case "Jaguar":
                    loadCarmodels("jaguar");
                    break;
                case "Jeep":
                    loadCarmodels("jeep");
                    break;
                case "Kia":
                    loadCarmodels("kia");
                    break;
                case "Lada":
                    loadCarmodels("lada");
                    break;
                case "Lamborghini":
                    loadCarmodels("lamborghini");
                    break;
                case "Lancia":
                    loadCarmodels("lancia");
                    break;
                case "Land Rover":
                    loadCarmodels("landrover");
                    break;
                case "Lexus":
                    loadCarmodels("lexus");
                    break;
                case "Mazda":
                    loadCarmodels("mazda");
                    break;
                case "Mercedes-Benz":
                    loadCarmodels("mercedesbenz");
                    break;
                case "Mercedes-AMG":
                    loadCarmodels("mercedesamg");
                    break;
                case "Mini":
                    loadCarmodels("mini");
                    break;
                case "Mitsubishi":
                    loadCarmodels("mitsubishi");
                    break;
                case "Nissan":
                    loadCarmodels("nissan");
                    break;
                case "Opel":
                    loadCarmodels("opel");
                    break;
                case "Peugeot":
                    loadCarmodels("peugeot");
                    break;
                case "Porsche":
                    loadCarmodels("porsche");
                    break;
                case "Renault":
                    loadCarmodels("renault");
                    break;
                case "Rolls-Royce":
                    loadCarmodels("rollsroyce");
                    break;
                case "Saab":
                    loadCarmodels("saab");
                    break;
                case "SEAT":
                    loadCarmodels("seat");
                    break;
                case "Skoda":
                    loadCarmodels("skoda");
                    break;
                case "Smart":
                    loadCarmodels("smart");
                    break;
                case "Ssangyong":
                    loadCarmodels("ssangyong");
                    break;
                case "Subaru":
                    loadCarmodels("subaru");
                    break;
                case "Suzuki":
                    loadCarmodels("suzuki");
                    break;
                case "Tesla":
                    loadCarmodels("tesla");
                    break;
                case "Toyota":
                    loadCarmodels("toyota");
                    break;
                case "Volkswagen":
                    loadCarmodels("volkswagen");
                    break;
                case "Volvo":
                    loadCarmodels("volvo");
                    break;
            }
        }       

        //AUTÓ KERESÉS gomb klikk esemény:
        private void bnAdminCarsSearch_Click(object sender, EventArgs e)
        {
            //mezők kiürítése:
            tbAdminCarsID.Clear();
            cbAdminCarsCategory.Text = "válasszon";
            tbAdminCarsMake.Clear();
            tbAdminCarsModel.Clear();
            tbAdminCarsCode.Clear();
            cbAdminCarsBody.Text = "válasszon";
            cbAdminCarsFuel.Text = "válasszon";
            cbAdminCarsCyl.Text = "válasszon";
            cbAdminCarsCylArr.Text = "válasszon";
            cbAdminCarsAsp.Text = "válasszon";
            nudAdminCarsPower.Value = 0;
            nudAdminCarsTorque.Value = 0;
            cbAdminCarsDisp.Text = "válasszon";
            cbAdminCarsGearbox.Text = "válasszon";
            cbAdminCarsGears.Text = "válasszon";
            cbAdminCarsDrivetrain.Text = "válasszon";
            nudAdminCarsAcc100.Value = 0;
            nudAdminCarsAcc200.Value = 0;
            nudAdminCarsVmax.Value = 0;
            nudAdminCarsConsmp.Value = 0;
            cbAdminCarsProdStart.Text = "válasszon";
            cbAdminCarsProdEnd.Text = "válasszon";
            nudAdminCarsBatCap.Value = 0;
            nudAdminCarsRange.Value = 0;

            //datagridview kiürítése:
            dgvAdminCars.DataSource = null;           

            //az autókeresőben annak megfelelően kell beállítani a lekérdezést, hogy mit választott ki a felhasználó:
            string searchquery = "";

            //ha egy mező sincs kiválasztva:
            if (cbAdminCarsMakeSearch.Text == "válasszon" && cbAdminCarsModelSearch.Text == "")
            {
                searchquery = "SELECT cars.id AS 'ID', category AS 'KATEGÓRIA', make AS 'MÁRKA', model AS 'MODELL', code AS 'GYÁRI KÓD', body AS 'KAROSSZÉRIA', fuel_type AS 'ÜZEMANYAG', cylinder_number AS 'HENGERSZÁM', cylinder_arrangement AS 'HENGERELRENDEZÉS', aspiration AS 'FELTÖLTÉS', power AS 'TELJESÍTMÉNY', torque AS 'NYOMATÉK', displacement AS 'HENGERŰRTARTALOM', gearbox_type AS 'SEBESSÉGVÁLTÓ', gears AS 'FOKOZATOK', powertrain AS 'HAJTÁS', acceleration100 AS '0-100', acceleration200 AS '0-200', vmax AS 'VÉGSEBESSÉG', consumption AS 'FOGYASZTÁS', production_start AS 'GYÁRTÁS KEZDETE', production_end AS 'GYÁRTÁS VÉGE', bat_capacity AS 'AKKU', fuel_range AS 'HATÓTÁV', CONCAT(last_name,' ',first_name) AS 'REGISZTRÁLTA' FROM cars LEFT JOIN users ON cars.registered_by = users.id";

                loadCarDataToTable(searchquery);
            }

            if (cbAdminCarsMakeSearch.Text == "válasszon" && cbAdminCarsModelSearch.Text == "válasszon")
            {
                searchquery = "SELECT cars.id AS 'ID', category AS 'KATEGÓRIA', make AS 'MÁRKA', model AS 'MODELL', code AS 'GYÁRI KÓD', body AS 'KAROSSZÉRIA', fuel_type AS 'ÜZEMANYAG', cylinder_number AS 'HENGERSZÁM', cylinder_arrangement AS 'HENGERELRENDEZÉS', aspiration AS 'FELTÖLTÉS', power AS 'TELJESÍTMÉNY', torque AS 'NYOMATÉK', displacement AS 'HENGERŰRTARTALOM', gearbox_type AS 'SEBESSÉGVÁLTÓ', gears AS 'FOKOZATOK', powertrain AS 'HAJTÁS', acceleration100 AS '0-100', acceleration200 AS '0-200', vmax AS 'VÉGSEBESSÉG', consumption AS 'FOGYASZTÁS', production_start AS 'GYÁRTÁS KEZDETE', production_end AS 'GYÁRTÁS VÉGE', bat_capacity AS 'AKKU', fuel_range AS 'HATÓTÁV', CONCAT(last_name,' ',first_name) AS 'REGISZTRÁLTA' FROM cars LEFT JOIN users ON cars.registered_by = users.id";

                loadCarDataToTable(searchquery);
            }

            //ha csak a márka van kiválasztva:
            else if (cbAdminCarsMakeSearch.Text != "válasszon" && cbAdminCarsModelSearch.Text == "")
            {
                searchquery = "SELECT cars.id AS 'ID', category AS 'KATEGÓRIA', make AS 'MÁRKA', model AS 'MODELL', code AS 'GYÁRI KÓD', body AS 'KAROSSZÉRIA', fuel_type AS 'ÜZEMANYAG', cylinder_number AS 'HENGERSZÁM', cylinder_arrangement AS 'HENGERELRENDEZÉS', aspiration AS 'FELTÖLTÉS', power AS 'TELJESÍTMÉNY', torque AS 'NYOMATÉK', displacement AS 'HENGERŰRTARTALOM', gearbox_type AS 'SEBESSÉGVÁLTÓ', gears AS 'FOKOZATOK', powertrain AS 'HAJTÁS', acceleration100 AS '0-100', acceleration200 AS '0-200', vmax AS 'VÉGSEBESSÉG', consumption AS 'FOGYASZTÁS', production_start AS 'GYÁRTÁS KEZDETE', production_end AS 'GYÁRTÁS VÉGE', bat_capacity AS 'AKKU', fuel_range AS 'HATÓTÁV', CONCAT(last_name,' ',first_name) AS 'REGISZTRÁLTA' FROM cars LEFT JOIN users ON cars.registered_by = users.id WHERE make = '" + cbAdminCarsMakeSearch.Text + "'";

                loadCarDataToTable(searchquery);
            }

            else if (cbAdminCarsMakeSearch.Text != "válasszon" && cbAdminCarsModelSearch.Text == "válasszon")
            {
                searchquery = "SELECT cars.id AS 'ID', category AS 'KATEGÓRIA', make AS 'MÁRKA', model AS 'MODELL', code AS 'GYÁRI KÓD', body AS 'KAROSSZÉRIA', fuel_type AS 'ÜZEMANYAG', cylinder_number AS 'HENGERSZÁM', cylinder_arrangement AS 'HENGERELRENDEZÉS', aspiration AS 'FELTÖLTÉS', power AS 'TELJESÍTMÉNY', torque AS 'NYOMATÉK', displacement AS 'HENGERŰRTARTALOM', gearbox_type AS 'SEBESSÉGVÁLTÓ', gears AS 'FOKOZATOK', powertrain AS 'HAJTÁS', acceleration100 AS '0-100', acceleration200 AS '0-200', vmax AS 'VÉGSEBESSÉG', consumption AS 'FOGYASZTÁS', production_start AS 'GYÁRTÁS KEZDETE', production_end AS 'GYÁRTÁS VÉGE', bat_capacity AS 'AKKU', fuel_range AS 'HATÓTÁV', CONCAT(last_name,' ',first_name) AS 'REGISZTRÁLTA' FROM cars LEFT JOIN users ON cars.registered_by = users.id WHERE make = '" + cbAdminCarsMakeSearch.Text + "'";

                loadCarDataToTable(searchquery);
            }

            //ha a márka és modell is ki van válaztva
            else if (cbAdminCarsMakeSearch.Text != "válasszon" && cbAdminCarsModelSearch.Text != "")
            {
                searchquery = "SELECT cars.id AS 'ID', category AS 'KATEGÓRIA', make AS 'MÁRKA', model AS 'MODELL', code AS 'GYÁRI KÓD', body AS 'KAROSSZÉRIA', fuel_type AS 'ÜZEMANYAG', cylinder_number AS 'HENGERSZÁM', cylinder_arrangement AS 'HENGERELRENDEZÉS', aspiration AS 'FELTÖLTÉS', power AS 'TELJESÍTMÉNY', torque AS 'NYOMATÉK', displacement AS 'HENGERŰRTARTALOM', gearbox_type AS 'SEBESSÉGVÁLTÓ', gears AS 'FOKOZATOK', powertrain AS 'HAJTÁS', acceleration100 AS '0-100', acceleration200 AS '0-200', vmax AS 'VÉGSEBESSÉG', consumption AS 'FOGYASZTÁS', production_start AS 'GYÁRTÁS KEZDETE', production_end AS 'GYÁRTÁS VÉGE', bat_capacity AS 'AKKU', fuel_range AS 'HATÓTÁV', CONCAT(last_name,' ',first_name) AS 'REGISZTRÁLTA' FROM cars LEFT JOIN users ON cars.registered_by = users.id WHERE make = '" + cbAdminCarsMakeSearch.Text + "' AND model = '" + cbAdminCarsModelSearch.Text + "'";

                loadCarDataToTable(searchquery);
            }

            else if (cbAdminCarsMakeSearch.Text != "válasszon" && cbAdminCarsModelSearch.Text != "válasszon")
            {
                searchquery = "SELECT cars.id AS 'ID', category AS 'KATEGÓRIA', make AS 'MÁRKA', model AS 'MODELL', code AS 'GYÁRI KÓD', body AS 'KAROSSZÉRIA', fuel_type AS 'ÜZEMANYAG', cylinder_number AS 'HENGERSZÁM', cylinder_arrangement AS 'HENGERELRENDEZÉS', aspiration AS 'FELTÖLTÉS', power AS 'TELJESÍTMÉNY', torque AS 'NYOMATÉK', displacement AS 'HENGERŰRTARTALOM', gearbox_type AS 'SEBESSÉGVÁLTÓ', gears AS 'FOKOZATOK', powertrain AS 'HAJTÁS', acceleration100 AS '0-100', acceleration200 AS '0-200', vmax AS 'VÉGSEBESSÉG', consumption AS 'FOGYASZTÁS', production_start AS 'GYÁRTÁS KEZDETE', production_end AS 'GYÁRTÁS VÉGE', bat_capacity AS 'AKKU', fuel_range AS 'HATÓTÁV', CONCAT(last_name,' ',first_name) AS 'REGISZTRÁLTA' FROM cars LEFT JOIN users ON cars.registered_by = users.id WHERE make = '" + cbAdminCarsMakeSearch.Text + "' AND model = '" + cbAdminCarsModelSearch.Text + "'";

                loadCarDataToTable(searchquery);
            }                              
        }

        //AUTÓ ADATOK BETÖLTÉSE TÁBLÁBA metódus:
        private void loadCarDataToTable(string query)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                DataTable mytable = new DataTable();
                MySqlCommand search = new MySqlCommand(query, con);
                MySqlDataReader open = search.ExecuteReader();
                mytable.Load(open);
                if (mytable.Rows.Count > 0)
                {
                    dgvAdminCars.DataSource = mytable;
                }
                else
                {
                    dgvAdminCars.DataSource = mytable;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            };
        }

        //AUTÓ MÓDOSÍTÁS gomb klikk esemény:
        private void bnAdminCarsMod_Click(object sender, EventArgs e)
        {
            if (tbAdminCarsID.Text == "")
            {
                MessageBox.Show("Először jelöljön ki egy autót!"); //csak akkor lehet autót módosítani, ha ki van választva egy konkrét modell
            }
            else if (cbAdminCarsCategory.Text == "válasszon"
            || tbAdminCarsMake.Text == "válasszon" || tbAdminCarsMake.Text == ""
            || tbAdminCarsModel.Text == "válasszon" || tbAdminCarsModel.Text == ""
            || cbAdminCarsFuel.Text == "válasszon" || cbAdminCarsFuel.Text == ""
            || cbAdminCarsBody.Text == "válasszon" || cbAdminCarsBody.Text == ""
            || cbAdminCarsCyl.Text == "válasszon" || cbAdminCarsCyl.Text == ""
            || cbAdminCarsCylArr.Text == "válasszon" || cbAdminCarsCylArr.Text == ""
            || cbAdminCarsAsp.Text == "válasszon" || cbAdminCarsAsp.Text == ""
            || nudAdminCarsPower.Value == 0
            || nudAdminCarsTorque.Value == 0
            || cbAdminCarsDisp.Text == "válasszon" || cbAdminCarsDisp.Text == ""
            || cbAdminCarsGearbox.Text == "válasszon" || cbAdminCarsGearbox.Text == ""
            || cbAdminCarsGears.Text == "válasszon" || cbAdminCarsGears.Text == ""
            || cbAdminCarsDrivetrain.Text == "válasszon" || cbAdminCarsDrivetrain.Text == ""
            || nudAdminCarsVmax.Value == 0
            || nudAdminCarsConsmp.Value == 0
            || cbAdminCarsProdStart.Text == "válasszon" || cbAdminCarsProdStart.Text == ""
            || cbAdminCarsProdEnd.Text == "válasszon" || cbAdminCarsProdEnd.Text == ""
            )
            {
                MessageBox.Show("A csillaggal jelölt mezők kitöltése kötelező!"); //csak úgy lehet elmenteni a módosítást, ha minden kötelező mező ki van töltve
            }
            else
            {
                string modquery = "UPDATE cars SET " +
                    "category = '" + cbAdminCarsCategory.Text + "'," +
                    "make = '" + tbAdminCarsMake.Text + "' ," +
                    "model = '" + tbAdminCarsModel.Text + "'," +
                    "code = '" + tbAdminCarsCode.Text + "'," +
                    "body = '" + cbAdminCarsBody.Text + "'," +
                    "fuel_type = '" + cbAdminCarsFuel.Text + "'," +
                    "cylinder_number = '" + cbAdminCarsCyl.Text + "'," +
                    "cylinder_arrangement = '" + cbAdminCarsCylArr.Text + "'," +
                    "aspiration = '" + cbAdminCarsAsp.Text + "'," +
                    "power = '" + nudAdminCarsPower.Value + "'," +
                    "torque = '" + nudAdminCarsTorque.Value + "'," +
                    "displacement = '" + cbAdminCarsDisp.Text + "'," +
                    "gearbox_type = '" + cbAdminCarsGearbox.Text + "'," +
                    "gears = '" + cbAdminCarsGears.Text + "'," +
                    "powertrain = '" + cbAdminCarsDrivetrain.Text + "'," +
                    "acceleration100 = '" + nudAdminCarsAcc100.Value + "'," +
                    "acceleration200 = '" + nudAdminCarsAcc200.Value + "'," +
                    "vmax = '" + nudAdminCarsVmax.Value + "'," +
                    "consumption = '" + nudAdminCarsConsmp.Value + "'," +
                    "production_start = '" + cbAdminCarsProdStart.Text + "'," +
                    "production_end = '" + cbAdminCarsProdEnd.Text + "'," +
                    "bat_capacity = '" + nudAdminCarsBatCap.Value + "'," +
                    "fuel_range = '" + nudAdminCarsRange.Value + "'" +
                    "WHERE id = '" + tbAdminCarsID.Text + "'";          

                //megerősítés kérése a felhasználótól:
                DialogResult dr = new DialogResult();
                Confirm a = new Confirm("Biztosan módosítani kívánja a kijelölt autót?");
                dr = a.ShowDialog();
                if (dr == DialogResult.Yes)
                {
                    MySqlCommand modify = new MySqlCommand(modquery, con); 
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        if (modify.ExecuteNonQuery() == 1) //módosítás végrehajtása
                        {
                            //minden autóadat betöltéséhez kell egy minden adatot tartalmazó lekérdezés:
                            query = "SELECT cars.id AS 'ID', category AS 'KATEGÓRIA', make AS 'MÁRKA', model AS 'MODELL', code AS 'GYÁRI KÓD', body AS 'KAROSSZÉRIA', fuel_type AS 'ÜZEMANYAG', cylinder_number AS 'HENGERSZÁM', cylinder_arrangement AS 'HENGERELRENDEZÉS', aspiration AS 'FELTÖLTÉS', power AS 'TELJESÍTMÉNY', torque AS 'NYOMATÉK', displacement AS 'HENGERŰRTARTALOM', gearbox_type AS 'SEBESSÉGVÁLTÓ', gears AS 'FOKOZATOK', powertrain AS 'HAJTÁS', acceleration100 AS '0-100', acceleration200 AS '0-200', vmax AS 'VÉGSEBESSÉG', consumption AS 'FOGYASZTÁS', production_start AS 'GYÁRTÁS KEZDETE', production_end AS 'GYÁRTÁS VÉGE', bat_capacity AS 'AKKU', fuel_range AS 'HATÓTÁV', CONCAT(last_name,' ',first_name) AS 'REGISZTRÁLTA' FROM cars LEFT JOIN users ON cars.registered_by = users.id";

                            delCarFields(); //mezők kiürítése
                            loadCarDataToTable(query); //adatok betöltése táblába
                            MessageBox.Show("Gépjármű sikeresen módosítva!");
                        }
                        else
                        {
                            MessageBox.Show("Módosítás sikertelen!");
                        }
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (dr == DialogResult.No)
                {
                    a.Dispose();
                }
            }
        }

        //AUTÓ TÖRLÉS gomb klikk esemény:
        private void bnAdminCarsDel_Click(object sender, EventArgs e)
        {
            if (tbAdminCarsID.Text == "")
            {
                MessageBox.Show("Először jelöljön ki egy autót!"); //csak akkor lehetséges a törlés, ha ki van választva egy konkrét modell
            }
            else
            {
                string deletequery = "DELETE FROM cars WHERE id = '" + tbAdminCarsID.Text + "'";

                //megerősítés kérése felhasználótól:
                DialogResult dr = new DialogResult();
                Confirm a = new Confirm("Biztosan törölni szeretné a kijelölt autót?"); 
                dr = a.ShowDialog();
                if (dr == DialogResult.Yes)
                {
                    MySqlCommand delete = new MySqlCommand(deletequery, con); 
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        if (delete.ExecuteNonQuery() == 1) //törlés végrehajtása
                        {
                            query = "SELECT cars.id AS 'ID', category AS 'KATEGÓRIA', make AS 'MÁRKA', model AS 'MODELL', code AS 'GYÁRI KÓD', body AS 'KAROSSZÉRIA', fuel_type AS 'ÜZEMANYAG', cylinder_number AS 'HENGERSZÁM', cylinder_arrangement AS 'HENGERELRENDEZÉS', aspiration AS 'FELTÖLTÉS', power AS 'TELJESÍTMÉNY', torque AS 'NYOMATÉK', displacement AS 'HENGERŰRTARTALOM', gearbox_type AS 'SEBESSÉGVÁLTÓ', gears AS 'FOKOZATOK', powertrain AS 'HAJTÁS', acceleration100 AS '0-100', acceleration200 AS '0-200', vmax AS 'VÉGSEBESSÉG', consumption AS 'FOGYASZTÁS', production_start AS 'GYÁRTÁS KEZDETE', production_end AS 'GYÁRTÁS VÉGE', bat_capacity AS 'AKKU', fuel_range AS 'HATÓTÁV', CONCAT(last_name,' ',first_name) AS 'REGISZTRÁLTA' FROM cars LEFT JOIN users ON cars.registered_by = users.id";

                            delCarFields();
                            loadCarDataToTable(query);
                            MessageBox.Show("Gépjármű sikeresen törölve!");
                        }
                        else
                        {
                            MessageBox.Show("Törlés sikertelen!");
                        }
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (dr == DialogResult.No)
                {
                    a.Dispose();
                }
            }
        }

        //AUTÓ datagridview kattintás:
        private void dgvAdminCars_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            lbRegBy.Visible = true; //látszódjon, hogy ki regésztrálta, melyik felhasználó

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dgvAdminCars.Rows[e.RowIndex];

                tbAdminCarsID.Text = row.Cells[0].Value.ToString();
                cbAdminCarsCategory.Text = row.Cells[1].Value.ToString();
                tbAdminCarsMake.Text = row.Cells[2].Value.ToString();
                tbAdminCarsModel.Text = row.Cells[3].Value.ToString();
                tbAdminCarsCode.Text = row.Cells[4].Value.ToString();
                cbAdminCarsBody.Text = row.Cells[5].Value.ToString();
                cbAdminCarsFuel.Text = row.Cells[6].Value.ToString();
                cbAdminCarsCyl.Text = row.Cells[7].Value.ToString();
                cbAdminCarsCylArr.Text = row.Cells[8].Value.ToString();
                cbAdminCarsAsp.Text = row.Cells[9].Value.ToString();
                nudAdminCarsPower.Value = Convert.ToInt32(row.Cells[10].Value.ToString());
                nudAdminCarsTorque.Value = Convert.ToInt32(row.Cells[11].Value.ToString());
                cbAdminCarsDisp.Text = row.Cells[12].Value.ToString();
                cbAdminCarsGearbox.Text = row.Cells[13].Value.ToString();
                cbAdminCarsGears.Text = row.Cells[14].Value.ToString();
                cbAdminCarsDrivetrain.Text = row.Cells[15].Value.ToString();
                nudAdminCarsAcc100.Value = Convert.ToDecimal(row.Cells[16].Value.ToString());
                nudAdminCarsAcc200.Value = Convert.ToDecimal(row.Cells[17].Value.ToString());
                nudAdminCarsVmax.Value = Convert.ToInt32(row.Cells[18].Value.ToString());
                nudAdminCarsConsmp.Value = Convert.ToDecimal(row.Cells[19].Value.ToString());
                cbAdminCarsProdStart.Text = row.Cells[20].Value.ToString();
                cbAdminCarsProdEnd.Text = row.Cells[21].Value.ToString();
                nudAdminCarsBatCap.Value = Convert.ToDecimal(row.Cells[22].Value.ToString());
                nudAdminCarsRange.Value = Convert.ToInt32(row.Cells[23].Value.ToString());

                //ki regisztrálta az adott autót:
                string registeredby = row.Cells[24].Value.ToString();
                if (registeredby == "")
                {
                    lbRegBy.Text = "törölt felhasználó";
                }
                else
                {
                    lbRegBy.Text = registeredby;
                }                
            }
        }

        //ÚJ MODELL hozzáadása gomb klikk esemény:
        private void btnAdminCarsAddModel_Click(object sender, EventArgs e)
        {
            if ((cbAdminCarsMakeSearch.Text == "") || (cbAdminCarsMakeSearch.Text == "válasszon"))
            {
                MessageBox.Show("Először válasszon ki egy márkát!");
            }
            else if (cbAdminCarsModelSearch.Text == "válasszon")
            {
                MessageBox.Show("Írja be az új modellt!");
            }
            else
            {
                switch (cbAdminCarsMakeSearch.Text)
                {
                    case "Alfa Romeo":
                        addNewModel("alfaromeo");
                        break;
                    case "Alpina":
                        addNewModel("alpina");
                        break;
                    case "Aston Martin":
                        addNewModel("astonmartin");
                        break;
                    case "Audi":
                        addNewModel("audi");
                        break;
                    case "Bentley":
                        addNewModel("bentley");
                        break;
                    case "BMW":
                        addNewModel("bmw");
                        break;
                    case "Bugatti":
                        addNewModel("bugatti");
                        break;
                    case "Citroen":
                        addNewModel("citroen");
                        break;
                    case "Dacia":
                        addNewModel("dacia");
                        break;
                    case "Ferrari":
                        addNewModel("ferrari+");
                        break;
                    case "Fiat":
                        addNewModel("fiat");
                        break;
                    case "Ford":
                        addNewModel("ford");
                        break;
                    case "Honda":
                        addNewModel("honda");
                        break;
                    case "Hyundai":
                        addNewModel("hyundai");
                        break;
                    case "Jaguar":
                        addNewModel("jaguar");
                        break;
                    case "Jeep":
                        addNewModel("jeep");
                        break;
                    case "Kia":
                        addNewModel("kia");
                        break;
                    case "Lada":
                        addNewModel("lada");
                        break;
                    case "Lamborghini":
                        addNewModel("lamborghini");
                        break;
                    case "Lancia":
                        addNewModel("lancia");
                        break;
                    case "Land Rover":
                        addNewModel("landrover");
                        break;
                    case "Lexus":
                        addNewModel("lexus");
                        break;
                    case "Mazda":
                        addNewModel("mazda");
                        break;
                    case "Mercedes-Benz":
                        addNewModel("mercedesbenz");
                        break;
                    case "Mercedes-AMG":
                        addNewModel("mercedesamg");
                        break;
                    case "Mini":
                        addNewModel("mini");
                        break;
                    case "Mitsubishi":
                        addNewModel("mitsubishi");
                        break;
                    case "Nissan":
                        addNewModel("nissan");
                        break;
                    case "Opel":
                        addNewModel("opel");
                        break;
                    case "Peugeot":
                        addNewModel("peugeot");
                        break;
                    case "Porsche":
                        addNewModel("porsche");
                        break;
                    case "Renault":
                        addNewModel("renault");
                        break;
                    case "Rolls-Royce":
                        addNewModel("rollsroyce");
                        break;
                    case "Saab":
                        addNewModel("saab");
                        break;
                    case "SEAT":
                        addNewModel("seat");
                        break;
                    case "Skoda":
                        addNewModel("skoda");
                        break;
                    case "Smart":
                        addNewModel("smart");
                        break;
                    case "Ssangyong":
                        addNewModel("ssangyong");
                        break;
                    case "Subaru":
                        addNewModel("subaru");
                        break;
                    case "Suzuki":
                        addNewModel("suzuki");
                        break;
                    case "Tesla":
                        addNewModel("tesla");
                        break;
                    case "Toyota":
                        addNewModel("toyota");
                        break;
                    case "Volkswagen":
                        addNewModel("volkswagen");
                        break;
                    case "Volvo":
                        addNewModel("volvo");
                        break;
                }
            }
        }

        //ÚJ MODEL HOZZÁADÁSA metódus:
        private void addNewModel(string make)
        {      
            //megerősíéts kérése:
            DialogResult dr = new DialogResult();
            Confirm a = new Confirm("Biztos benne, hogy hozzá szeretné adni az új modellt?");
            dr = a.ShowDialog();
            if (dr == DialogResult.Yes)
            {   
                try
                {

                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }

                    string model = cbAdminCarsModelSearch.Text;
                    //biztonságos, paraméteres mysql query:
                    string insertquery = "INSERT INTO " + @make + "(model) VALUES ('" + @model + "')";

                    MySqlCommand insert = new MySqlCommand(insertquery, con);
                    insert.Parameters.Add(new MySqlParameter("@make", make));
                    insert.Parameters.Add(new MySqlParameter("@model", model));

                    if (insert.ExecuteNonQuery() == 1) //végrehajtás
                    {                       
                        MessageBox.Show("Új modell hozzáadva");
                    }
                    else
                    {
                        MessageBox.Show("Hozzáadás sikertelen");
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (dr == DialogResult.No)
            {
                a.Dispose();
            }
        }

        //AUTÓMEZŐK KIÜRÍTÉSE gomb klikk esemény:
        private void bnAdminCarsClearFields_Click(object sender, EventArgs e)
        {
            delCarFields();
        }

        //AUTÓMEZŐK KIÜRÍTÉSE metódus:
        private void delCarFields()
        {
            tbAdminCarsID.Clear();
            cbAdminCarsCategory.Text = "válasszon";
            cbAdminCarsMakeSearch.Text = "válasszon";
            cbAdminCarsModelSearch.DataSource = null;
            tbAdminCarsCode.Clear();
            tbAdminCarsMake.Clear();
            tbAdminCarsModel.Clear();
            cbAdminCarsBody.Text = "válasszon";
            cbAdminCarsFuel.Text = "válasszon";
            cbAdminCarsCyl.Text = "válasszon";
            cbAdminCarsCylArr.Text = "válasszon";
            cbAdminCarsAsp.Text = "válasszon";
            nudAdminCarsPower.Value = 0;
            nudAdminCarsTorque.Value = 0;
            cbAdminCarsDisp.Text = "válasszon";
            cbAdminCarsGearbox.Text = "válasszon";
            cbAdminCarsGears.Text = "válasszon";
            cbAdminCarsDrivetrain.Text = "válasszon";
            nudAdminCarsAcc100.Value = 0;
            nudAdminCarsAcc200.Value = 0;
            nudAdminCarsVmax.Value = 0;
            nudAdminCarsConsmp.Value = 0;
            cbAdminCarsProdStart.Text = "válasszon";
            cbAdminCarsProdEnd.Text = "válasszon";
            nudAdminCarsBatCap.Value = 0;
            nudAdminCarsRange.Value = 0;
            lbRegBy.Text = "";
        }

        //Admin form BEZÁRÁS gomb klikk esemény:
        private void bnCloseWindow_Click(object sender, EventArgs e)
        {              
            DialogResult dr = new DialogResult();
            Confirm a = new Confirm("Biztosan be akarja zárni az Adminisztrációs ablakot?");
            dr = a.ShowDialog();
            if (dr == DialogResult.Yes)
            {                
                this.Close();
            }
            else if (dr == DialogResult.No)
            {
                a.Dispose();
            }
        }

        //FELHASZNÁLÓK KERESÉSE gomb klikk esemény:
        private void bnAdminUsersSearch_Click(object sender, EventArgs e)
        {
            //mezők kiürítése kattintáskor:
            tbAdminUsersID.Clear();            
            cbAdminUsersPosition.Text = "válasszon";
            dtpAdminUsersBirthdate.Value = DateTime.Now;
            tbAdminEmail.Clear();            

            //datagridview kiürítése:
            dataGridUsers.DataSource = null;                     

            //a lekérdezést az határozza meg, hogy mit választott ki a felhasználó:
            if (tbAdminUsersLastName.Text == "" && tbAdminUsersFirstName.Text == "")
            {
                query = "SELECT users.id AS ID, last_name AS 'VEZETÉKNÉV', first_name AS 'KERESZTNÉV', position AS 'POZÍCIÓ', birthdate AS 'SZÜLETÉSI DÁTUM', email AS 'EMAILCÍM', username AS 'FELHASZNÁLÓNÉV', COUNT(cars.id) AS 'AUTÓREGISZTRÁCIÓK SZÁMA' FROM users LEFT JOIN cars ON cars.registered_by = users.id GROUP BY users.id";

                loadUserDataToTable(query);
            }
            else if (tbAdminUsersLastName.Text != "" && tbAdminUsersFirstName.Text == "")
            {
                query = "SELECT users.id AS ID, last_name AS 'VEZETÉKNÉV', first_name AS 'KERESZTNÉV', position AS 'POZÍCIÓ', birthdate AS 'SZÜLETÉSI DÁTUM', email AS 'EMAILCÍM', username AS 'FELHASZNÁLÓNÉV', COUNT(cars.id) AS 'AUTÓREGISZTRÁCIÓK SZÁMA' FROM users LEFT JOIN cars ON cars.registered_by = users.id WHERE last_name = '" + tbAdminUsersLastName.Text + "'GROUP BY users.id ";

                loadUserDataToTable(query);
            }
            else if (tbAdminUsersLastName.Text == "" && tbAdminUsersFirstName.Text != "")
            {
                query = "SELECT users.id AS ID, last_name AS 'VEZETÉKNÉV', first_name AS 'KERESZTNÉV', position AS 'POZÍCIÓ', birthdate AS 'SZÜLETÉSI DÁTUM', email AS 'EMAILCÍM', username AS 'FELHASZNÁLÓNÉV', COUNT(cars.id) AS 'AUTÓREGISZTRÁCIÓK SZÁMA' FROM users LEFT JOIN cars ON cars.registered_by = users.id WHERE first_name = '" + tbAdminUsersFirstName.Text + "'GROUP BY users.id";

                loadUserDataToTable(query);
            }
            else if (tbAdminUsersLastName.Text != "" && tbAdminUsersFirstName.Text != "")
            {
                query = "SELECT users.id AS ID, last_name AS 'VEZETÉKNÉV', first_name AS 'KERESZTNÉV', position AS 'POZÍCIÓ', birthdate AS 'SZÜLETÉSI DÁTUM', email AS 'EMAILCÍM', username AS 'FELHASZNÁLÓNÉV', COUNT(cars.id) AS 'AUTÓREGISZTRÁCIÓK SZÁMA' FROM users LEFT JOIN cars ON cars.registered_by = users.id WHERE first_name = '" + tbAdminUsersFirstName.Text + "' AND last_name ='" + tbAdminUsersLastName.Text + "'GROUP BY users.id";

                loadUserDataToTable(query);
            }           
        }

        //FELHASZNÁLÓI ADATOK BETÖLTÉSE TÁBLÁBA metódus:
        private void loadUserDataToTable(string query)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                DataTable mytable = new DataTable();
                MySqlCommand search = new MySqlCommand(query, con);
                MySqlDataReader open = search.ExecuteReader();
                mytable.Load(open);
                if (mytable.Rows.Count > 0)
                {
                    dataGridUsers.DataSource = mytable;
                }
                else
                {
                    dataGridUsers.DataSource = mytable;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            };
        }

        //FELHASZNÁLÓK MÓDOSÍTÁSA gomb klikk esemény:
        private void bnAdminUsersMod_Click(object sender, EventArgs e)
        {
            string email = tbAdminEmail.Text;
            //email cím helyességének ellenőrzés REGEX:
            Regex regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");

            if (tbAdminUsersID.Text == "")
            {
                MessageBox.Show("Először jelöljön ki egy felhasználót");
            }
            else if (tbAdminUsername.Text == "admin")
            {
                MessageBox.Show("Az adminisztrátor nem módosítható!");
            }
            else if (tbAdminUsersLastName.Text == ""
                || tbAdminUsersFirstName.Text == ""
                || tbAdminUsername.Text == ""
                || cbAdminUsersPosition.Text == "válasszon"
                || dtpAdminUsersBirthdate.Value == DateTime.Now
                || tbAdminEmail.Text == ""
                )
            {
                MessageBox.Show("Minden mező kitöltése kötelező!");
            }
            else if (!regex.IsMatch(email)) //email cím ellenőrzése
            {
                MessageBox.Show("Email cím nem megfelelő");
            }
            else
            {
                string updatequery = "UPDATE users SET last_name = '" + tbAdminUsersLastName.Text + "', first_name = '" + tbAdminUsersFirstName.Text + "', username = '" + tbAdminUsername.Text + "', position = '" + cbAdminUsersPosition.Text + "', birthdate = '" + dtpAdminUsersBirthdate.Value.ToString("yyyy/MM/dd") + "', email = '" + tbAdminEmail.Text + "' WHERE id = '" + tbAdminUsersID.Text + "'";

                query = "SELECT users.id AS ID, last_name AS 'VEZETÉKNÉV', first_name AS 'KERESZTNÉV', position AS 'POZÍCIÓ', birthdate AS 'SZÜLETÉSI DÁTUM', email AS 'EMAILCÍM', username AS 'FELHASZNÁLÓNÉV', COUNT(cars.id) AS 'AUTÓREGISZTRÁCIÓK SZÁMA' FROM users LEFT JOIN cars ON cars.registered_by = users.id GROUP BY users.id";            

                DialogResult dr = new DialogResult();
                Confirm a = new Confirm("Biztosan módosítani szeretné a felhasználót?");
                dr = a.ShowDialog();
                if (dr == DialogResult.Yes)
                {
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        MySqlCommand insert = new MySqlCommand(updatequery, con);
                        if (insert.ExecuteNonQuery() == 1)
                        {
                            loadUserDataToTable(query);
                            MessageBox.Show("Felhasználó sikeresen módosítva");
                        }
                        else
                        {
                            MessageBox.Show("Módosítás sikertelen");
                        }
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (dr == DialogResult.No)
                {
                    a.Dispose();
                }
            }
        }

        //FELHASZNÁLÓK TÖRLÉSE gomb klikk esemény:
        private void bnAdminUsersDel_Click(object sender, EventArgs e)
        {
            if (tbAdminUsersID.Text == "")
            {
                MessageBox.Show("Először jelöljön ki egy felhasználót!");
            }

            else if (tbAdminUsername.Text == "admin")
            {
                MessageBox.Show("Az adminisztrátort nem lehet kitörölni!");
            }
            else
            {
                string deletequery = "DELETE FROM users WHERE id = '" + tbAdminUsersID.Text + "'";       

                DialogResult dr = new DialogResult();
                Confirm a = new Confirm("Biztosan törölni szeretné a felhasználót?");
                dr = a.ShowDialog();
                if (dr == DialogResult.Yes)
                {
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }

                        MySqlCommand insert = new MySqlCommand(deletequery, con);
                        if (insert.ExecuteNonQuery() == 1)
                        {
                            query = "SELECT users.id AS ID, last_name AS 'VEZETÉKNÉV', first_name AS 'KERESZTNÉV', position AS 'POZÍCIÓ', birthdate AS 'SZÜLETÉSI DÁTUM', email AS 'EMAILCÍM', username AS 'FELHASZNÁLÓNÉV', COUNT(cars.id) AS 'AUTÓREGISZTRÁCIÓK SZÁMA' FROM users LEFT JOIN cars ON cars.registered_by = users.id GROUP BY users.id";

                            delUserFields();
                            loadUserDataToTable(query);
                            MessageBox.Show("Felhasználó sikeresen törölve");
                        }
                        else
                        {
                            MessageBox.Show("Törlés sikertelen");
                        }
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (dr == DialogResult.No)
                {
                    a.Dispose();
                }
            }
        }

        //FELHASZNÁLÓMEZŐK TÖRLÉSE gomb klikk esemény:
        private void bnAdminUsersClearFields_Click(object sender, EventArgs e)
        {
            delUserFields();
        }

        //FELHASZNÁLÓMEZŐK TÖRLÉSE metódus:
        private void delUserFields()
        {
            tbAdminUsersID.Clear();
            tbAdminUsersLastName.Clear();
            tbAdminUsersFirstName.Clear();
            tbAdminUsername.Clear();
            cbAdminUsersPosition.Text = "válasszon";
            dtpAdminUsersBirthdate.Value = DateTime.Now;
            tbAdminEmail.Clear();
            lbCarRegNumber.Text = "";
        }

        //FELHASZNÁLÓ datagridview kattintás:
        private void dataGridUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
                if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridUsers.Rows[e.RowIndex];

                tbAdminUsersID.Text = row.Cells[0].Value.ToString();
                tbAdminUsersLastName.Text = row.Cells[1].Value.ToString();
                tbAdminUsersFirstName.Text = row.Cells[2].Value.ToString();
                cbAdminUsersPosition.Text = row.Cells[3].Value.ToString();
                dtpAdminUsersBirthdate.Value = Convert.ToDateTime(row.Cells[4].Value);
                tbAdminEmail.Text = row.Cells[5].Value.ToString();
                tbAdminUsername.Text = row.Cells[6].Value.ToString();
                lbCarRegNumber.Text = row.Cells[7].Value.ToString();
            }
        }

        //ÚJ JELSZÓ adás gomb klikk esemény:
        private void bnAdminUsersNewPw_Click(object sender, EventArgs e)
        {
            if (tbAdminUsersID.Text == "")
            {
                MessageBox.Show("Először jelöljön ki egy felhasználót!");
            }
            else
            {
                //objektum meghívása paraméterrel. A paraméter az a felhasználó, akinek a jelszava módosítva lesz:
                NewPw newpassword = new NewPw (tbAdminUsername.Text);

                bool IsOpen = false;
                foreach (Form f in Application.OpenForms)
                {
                    if (f.Text == "Új jelszó")
                    {
                        IsOpen = true;
                        newpassword.BringToFront();
                        newpassword.Activate();
                    }
                }
                if (IsOpen == false)
                {
                    newpassword = new NewPw(tbAdminUsername.Text);
                    newpassword.Show();
                    newpassword.BringToFront();
                    newpassword.Activate();
                }
            }
        }
    }
}
