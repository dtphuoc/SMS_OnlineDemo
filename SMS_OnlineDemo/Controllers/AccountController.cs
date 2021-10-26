using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SMS_OnlineDemo.Models;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;

namespace SMS_OnlineDemo.Controllers
{
    
    public class AccountController : Controller
    {

        DemoSMS_OnlienEntities3 db = new DemoSMS_OnlienEntities3();
        String sql_con = @"Data Source=DESKTOP-JINTQRI\SQLEXPRESS;Initial Catalog=DemoSMS_Onlien;Integrated Security=True";
        public ActionResult Index()
        {

            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                ViewBag.error = "Login failed";
                return RedirectToAction("Login");
            }
        }


        public ActionResult Login()
        {
            return View();
        }


        SqlConnection con;
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password ,string status)
        {
            if (ModelState.IsValid)
            {
                var f_password = EncodePassword(password);
                var data = db.Users.Where(s => s.Email.Equals(email) && s.PassWord.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["FullName"] = data.FirstOrDefault().UserName;

                    if (data.FirstOrDefault().Gender == true)
                    {
                        Session["Gender"] = "Male";
                    }
                    else if (data.FirstOrDefault().Gender == false)
                    {
                        Session["Gender"] = "Female";
                    }

                    

                    if (data.FirstOrDefault().WorkStatus == true)
                    {
                        Session["WorkStatus"] = "Students";
                    }
                    else if (data.FirstOrDefault().WorkStatus == false)
                    {
                        Session["WorkStatus"] = "Employees";
                    }

                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["Address"] = data.FirstOrDefault().Address;
                    Session["DOB"] = data.FirstOrDefault().DOB;
                    Session["UserID"] = data.FirstOrDefault().User_Id;
                    Session["UserName"] = data.FirstOrDefault().UserName;


                    con = new SqlConnection(sql_con);
                    String sql = "update Users set Status = @Status where User_Id =" + data.FirstOrDefault().User_Id;
                    SqlCommand command = new SqlCommand(sql, con);

                    command.Parameters.AddWithValue("@Status", "Logged In");

                    con.Open();
                    command.ExecuteNonQuery();
                    con.Close();
                    return RedirectToAction("Index");
                }


                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User _user)
        {
            if (ModelState.IsValid)
            { // check xem email đã tồn tại trên đb chưa
                var checkEmail = db.Users.FirstOrDefault(s => s.Email == _user.Email);
                var checkUserName = db.Users.FirstOrDefault(s => s.UserName == _user.UserName);
                if (checkEmail == null && checkUserName == null)
                {
                    _user.PassWord = EncodePassword(_user.PassWord);
                    //_user.Password = _user.Password;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Users.Add(_user);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return RedirectToAction("Register");
                }
            }
            return View();
        }

        //Logout
        public ActionResult Logout()
        {
            var data = db.Users;

            con = new SqlConnection(sql_con);
            String sql = "update Users set Status = @Status where User_Id =" + data.FirstOrDefault().User_Id;
            SqlCommand command = new SqlCommand(sql, con);

            command.Parameters.AddWithValue("@Status", "Logged Out");

            con.Open();
            command.ExecuteNonQuery();
            con.Close();

            Session.Clear();
            return RedirectToAction("Login");

            //Session.Abandon();
            //return RedirectToAction("Login");
        }

        //create md5 string
        public static string EncodePassword(string originalPassword)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }
    }
}