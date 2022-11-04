using ExamenIntra.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace ExamenIntra.Controllers
{
    public class StoredProcController : Controller
    {
        private string connectionString;
        public IConfiguration configuration;

        public StoredProcController(IConfiguration _configuration)
        {
            this.configuration = _configuration;
        }

        // procedure: getListActivities
        // GET: StoredProcController
        public ActionResult Index()
        {
            SqlConnection conn;
            SqlCommand cmd;
            SqlDataReader reader;
            List<Activite> listActivities;

            connectionString = configuration.GetConnectionString("defaultConnection");
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //cmd.CommandText = "getListActivities";
            cmd.CommandText = "getListActivitiesByOrderDesc";
            cmd.Connection = conn;

            conn.Open();
            reader = cmd.ExecuteReader();
            listActivities = new List<Activite>();
            while (reader.Read())
            {
                Activite activity = new Activite();
                activity.Id = reader.GetInt32("id");
                activity.Name = reader.GetString("nom");
                activity.Duree = reader.GetInt32("duree");
                activity.Cout = reader.GetDecimal("cout");
                activity.Vote= reader.GetInt32("vote");
                listActivities.Add(activity);
            }

            return View(listActivities);
        }

        // GET: StoredProcController/Details/5
        public ActionResult Details(int id)
        {
            Activite activity = null;

            SqlConnection conn;
            SqlCommand cmd;
            SqlDataReader reader;

            connectionString = configuration.GetConnectionString("defaultConnection");
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "detailsActivity";
            cmd.Parameters.Add(new SqlParameter("@paramId", id));
            cmd.Connection = conn;

            conn.Open();
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    activity = new Activite();
                    activity.Id = reader.GetInt32("id");
                    activity.Name = reader.GetString("nom");
                    activity.Duree = reader.GetInt32("duree");
                    activity.Cout = reader.GetDecimal("cout");
                    activity.Vote = reader.GetInt32("vote");
                }
                return View(activity);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: StoredProcController/Voter/5
        public ActionResult Voter(int id)
        {
            Activite activity = null;

            SqlConnection conn;
            SqlCommand cmdGetVote, cmdUpdateVote, cmd;
            SqlDataReader reader;

            connectionString = configuration.GetConnectionString("defaultConnection");
            conn = new SqlConnection(connectionString);
            // recuperer data de vote pour une activite
            cmdGetVote = new SqlCommand();
            cmdGetVote.CommandType = System.Data.CommandType.StoredProcedure;
            cmdGetVote.CommandText = "detailsActivity";
            cmdGetVote.Parameters.Add(new SqlParameter("@paramId", id));
            cmdGetVote.Connection = conn;
            conn.Open();
            reader = cmdGetVote.ExecuteReader();
            int vote = 0;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    // lire la valeur de vote
                    vote = reader.GetInt32("vote");
                }
            }
            conn.Close();

            // compter une vote pour cette activite
            vote++;
            // mise a jour la valeur de vote pout cette activite
            cmdUpdateVote = new SqlCommand();
            cmdUpdateVote.CommandType = System.Data.CommandType.StoredProcedure;
            cmdUpdateVote.CommandText = "voterActivity";
            cmdUpdateVote.Parameters.Add(new SqlParameter("@paramId", id));
            cmdUpdateVote.Parameters.Add(new SqlParameter("@paramVote", vote));
            cmdUpdateVote.Connection = conn;

            conn.Open();
            int rowCount = cmdUpdateVote.ExecuteNonQuery();
            conn.Close();

            // mise a jour page web
            cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "detailsActivity";
            cmd.Parameters.Add(new SqlParameter("@paramId", id));
            cmd.Connection = conn;

            conn.Open();
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    activity = new Activite();
                    activity.Id = reader.GetInt32("id");
                    activity.Name = reader.GetString("nom");
                    activity.Duree = reader.GetInt32("duree");
                    activity.Cout = reader.GetDecimal("cout");
                    activity.Vote = reader.GetInt32("vote");
                }
                return View(activity);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: StoredProcController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: StoredProcController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: StoredProcController/Edit/5
        public ActionResult Edit(int id)
        {
            Activite activity = null;

            SqlConnection conn;
            SqlCommand cmd;
            SqlDataReader reader;

            connectionString = configuration.GetConnectionString("defaultConnection");
            conn = new SqlConnection(connectionString);
            cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "detailsActivity";
            cmd.Parameters.Add(new SqlParameter("@paramId", id));
            cmd.Connection = conn;

            conn.Open();
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    activity = new Activite();
                    activity.Id = reader.GetInt32("id");
                    activity.Name = reader.GetString("nom");
                    activity.Duree = reader.GetInt32("duree");
                    activity.Cout = reader.GetDecimal("cout");
                    activity.Vote = reader.GetInt32("vote");
                }
                return View(activity);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: StoredProcController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Activite activite)
        {
            try
            {
                SqlConnection conn;
                SqlCommand cmd;

                connectionString = configuration.GetConnectionString("defaultConnection");
                conn = new SqlConnection(connectionString);
                cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "updateActivity";
                cmd.Parameters.Add(new SqlParameter("@paramId", activite.Id));
                cmd.Parameters.Add(new SqlParameter("@paramDuree", activite.Duree));
                cmd.Parameters.Add(new SqlParameter("@paramCout", activite.Cout));
                cmd.Connection = conn;

                conn.Open();
                int rowCount = cmd.ExecuteNonQuery();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: StoredProcController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: StoredProcController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
